using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Security.AccessControl;
using System.ServiceProcess;
using WireGuard.Core;
using WireGuard.Core.Classes;
using WireGuard.Core.Messages;
using WireGuard.WireGuardUIService.Classes;
using Path = WireGuard.Core.Classes.Path;

namespace WireGuard.WireGuardUIService
{
    /// <summary>
    /// Class to contain all operations
    /// </summary>
    public static class Operations
    {
        #region Variables

        /// <summary>
        /// IPC Interface for the Wireguard tunnel
        /// </summary>
        internal static IWgIpcApi wgIpcApi;

        /// <summary>
        /// Name of the tunnel who should start at boot
        /// </summary>
        internal static string bootUpTunnel;

        #endregion

        #region Helper fcuntions

        /// <summary>
        /// Gets the name of the interface form a file name
        /// </summary>
        /// <param name="file">file name to extract interface name</param>
        /// <returns></returns>
        public static string GetInterfaceName(string file) => file.Remove(file.IndexOf('.')).Trim();

        /// <summary>
        /// Method checks if a folder exists and create a new one if not
        /// </summary>
        public static void CheckAndCreateWGConfigFolder()
        {
            // If the data folder dose not exist, create one 
            if (!Directory.Exists(Path.WIREGUARD_DATA))
                Directory.CreateDirectory(Path.WIREGUARD_DATA);

            // Create new folder security
            DirectorySecurity security = new DirectorySecurity();

            // Sets the local admin as owner of this object
            security.SetOwner(UAC.LocalAdmin);

            // Ignores inheritance from the folder
            security.SetAccessRuleProtection(true, false);

            // Sets the system user to full control
            security.AddAccessRule(new FileSystemAccessRule(UAC.SystemUser, FileSystemRights.FullControl, InheritanceFlags.ContainerInherit| InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));

            // Sets the administrator to full control
            security.AddAccessRule(new FileSystemAccessRule(UAC.LocalAdmin, FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));

            // Set the security settings for the data folder
            DirectoryInfo di = new DirectoryInfo(Path.WIREGUARD_DATA);
            di.SetAccessControl(security);

            // If the config folder dose not exist, create one
            if (!Directory.Exists(Path.WIREGUARD_CONFIG))
                Directory.CreateDirectory(Path.WIREGUARD_CONFIG);

            DirectorySecurity confSec = new DirectorySecurity();

            // Applies the rules of the Data folder
            confSec.SetAccessRuleProtection(false, true);
            
            // Applies the security settings to the folder
            DirectoryInfo confDI = new DirectoryInfo(Path.WIREGUARD_CONFIG);
            confDI.SetAccessControl(confSec);
        }

        #endregion

        #region Message functions

        /// <summary>
        /// Method to start a tunnel service
        /// </summary>
        /// <param name="tunnel">Name of the tunnel to start</param>
        /// <returns></returns>
        public static Message StartTunnel(string tunnel)
        {
            string tunnelName = GetInterfaceName(tunnel);

            // Clear all tunnels before starting a new one
            // This must be done because there is a way to start many tunnels
            // with the start on boot or start on logon option
            ClearTunnel(null);

            //Start the process
            ProcessStartInfo processStart = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = Path.WIREGUARD_EXE,
                Arguments = $"/installtunnelservice \"{Path.WIREGUARD_CONFIG}\\{tunnel}\"",
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            Process p = Process.Start(processStart);

            //Check if process could be created
            if (p == null)
                return new ResultMessage() { Error = -1, ErrorMsg = $"Unable to install VPN-tunnel or to start wireguard.exe." };

            //Wait for exit and send result
            int exitCode = 0;

            if (p.WaitForExit(15 * 1000))
                exitCode = p.ExitCode;
            else
            {
                exitCode = -1;
                p.Kill();
            }

            if (p.ExitCode != 0)
                return new ResultMessage() { Error = -1, ErrorMsg = $"wireguard.exe returned {p.ExitCode}" };

            LogManager.Debug($"Read wireguard.exe std output: {p.StandardOutput.ReadToEnd()}", nameof(Operations));
            LogManager.Debug($"Read wireguard.exe err output: {p.StandardError.ReadToEnd()}", nameof(Operations));

            //Get the Service to check if its running
            LogManager.Debug($"Tunnelname: WireGuardTunnel${tunnel} / Start args: {p.StartInfo.Arguments}", nameof(Operations));

            ServiceController controller = ServiceController.GetServices().Where(x => x.ServiceName == $"WireGuardTunnel${tunnelName}").FirstOrDefault();

            if (controller == null)
            {
                LogManager.Error("Error starting the VPN-tunnel");
                return new ResultMessage() { Error = -1, ErrorMsg = $"Could not find service called: WireGuardTunnel${tunnelName}" };
            }

            controller.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));

            if (controller.Status != ServiceControllerStatus.Running)
                return new ResultMessage() { Error = -1, ErrorMsg = $"WireGuardTunnel${tunnelName} service is not running" };

            return TunnelStatus(tunnelName);
        }

        /// <summary>
        /// Method to stop and deinstall a tunnel service
        /// </summary>
        /// <param name="tunnel">Name of the tunnel to stop</param>
        /// <returns></returns>
        public static ResultMessage StopTunnel(string tunnel)
        {
            //Start the process
            ProcessStartInfo processStart = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = Path.WIREGUARD_EXE,
                Arguments = $"/uninstalltunnelservice \"{tunnel}\""
            };

            LogManager.Debug($"Start wireguard.exe: {processStart.Arguments}", nameof(Operations));
            Process p = Process.Start(processStart);

            //Check if process could be created
            if (p == null)
            {
                LogManager.Error("Unable to deinstall VPN-tunnel or to start wireguard.exe.");

                return new ResultMessage() { Error = -1, ErrorMsg = $"Unable to deinstall VPN-tunnel or to start wireguard.exe." };
            }

            //Wait for exit
            p.WaitForExit(10 * 1000);

            if (p.ExitCode != 0)
            {
                LogManager.Error($"wireguard.exe returned {p.ExitCode}");

                return new ResultMessage() { Error = -1, ErrorMsg = $"wireguard.exe returned {p.ExitCode}" };
            }
            else
                return new ResultMessage() { Error = 0, ErrorMsg = "" };
        }

        /// <summary>
        /// Removes a configuration form the config storage
        /// </summary>
        /// <param name="tunnel">Tunnelname to be removed</param>
        /// <returns></returns>
        public static ResultMessage RemoveConfiguration(string tunnel)
        {
            try
            {
                System.IO.File.Delete($@"{Path.WIREGUARD_CONFIG}\{tunnel}");

                //Return success message
                return new ResultMessage() { Error = 0 };
            }
            catch (Exception ex)
            {
                LogManager.Error("Error removing configuration");
                LogManager.Error(ex);

                return new ResultMessage() { Error = -1, ErrorMsg = ex.Message };
            }
        }

        /// <summary>
        /// Imports a configuration to the storage
        /// </summary>
        /// <param name="file">File to be imported</param>
        /// <returns></returns>
        public static ResultMessage ImportConfiguration(string file)
        {
            try
            {
                FileInfo fi = new FileInfo(file);

                // Must be done to fix the issue No. 11
                // <see href="https://gitlab.rhrk.uni-kl.de/sccm/intern/wireguard/-/issues/11"/>
                string fileName = fi.Name.Replace(' ', '_').
                                          Replace('(', '_').
                                          Replace(')', '_').
                                          Replace('[', '_').
                                          Replace(']', '_');

                File.Move(file, $@"{Path.WIREGUARD_CONFIG}\{fileName}");

                //Get new FileInfo object to set the file premissions
                fi = new FileInfo($@"{Path.WIREGUARD_CONFIG}\{fileName}");

                LogManager.Debug($"New file location: {fi.FullName}", nameof(Operations));
                LogManager.Debug("Creating new access rule", nameof(Operations));

                // Start a new security descriptor for the file
                FileSecurity security = new FileSecurity();

                // Sets the owner to the local administrator
                security.SetOwner(UAC.LocalAdmin);

                // Ignores inheritance from the folder
                security.SetAccessRuleProtection(true, false);

                // Sets the system user to full control
                security.AddAccessRule(new FileSystemAccessRule(UAC.SystemUser, FileSystemRights.FullControl, InheritanceFlags.None, PropagationFlags.None, AccessControlType.Allow));

                // Sets the administrator to full control
                security.AddAccessRule(new FileSystemAccessRule(UAC.LocalAdmin, FileSystemRights.FullControl, InheritanceFlags.None, PropagationFlags.None, AccessControlType.Allow));

                LogManager.Debug($"Security descriptor as string: {security}", nameof(Operations));

                // Sets the new seurity description of the file
                fi.SetAccessControl(security);

                LogManager.Debug($"Access control set!", nameof(Operations));

                //DPAPI.EncryptFile(fi, WIREGUARD_DATA);

                //Return success message
                return new ResultMessage() { Error = 0, ErrorMsg=fileName };
            }
            catch (Exception ex)
            {
                LogManager.Error("Error importing configuration");
                LogManager.Error(ex);

                return new ResultMessage() { Error = -1, ErrorMsg = ex.Message };
            }
        }

        /// <summary>
        /// Retrievs a message of the tunnel status
        /// </summary>
        /// <param name="tunnelName">Name of the tunnel to retrive status for</param>
        /// <returns></returns>
        public static Message TunnelStatus(string tunnelName)
        {
            return wgIpcApi.GetTunnelStatus(tunnelName);
        }

        #endregion

        #region Misc

        /// <summary>
        /// Method to check if the WireGuard Manager Service is running and
        /// to disabled it if it does
        /// </summary>
        public static void CheckWGManagerService()
        {
            ServiceController service = ServiceController.GetServices().Where(x => x.ServiceName == Path.WIREGUARD_SERVICE_NAME).FirstOrDefault();

            if (service == null) return;

            if (service.Status != ServiceControllerStatus.Running)
                return;

            service.Stop();
            Helper.ServiceHelper.ChangeStartMode(service, ServiceStartMode.Disabled);

            service.Close();
        }

        /// <summary>
        /// Removes all tunnels form the service except the sparetunnel
        /// </summary>
        /// <param name="spareTunnel">Spare tunnel interface name to skip</param>
        public static void ClearTunnel(string spareTunnel = "")
        {
            IEnumerable<ServiceController> controller = ServiceController.GetServices().Where(x => x.ServiceName.StartsWith("WireGuardTunnel$"));

            foreach (ServiceController sc in controller)
            {
                string tunnel = sc.ServiceName.Replace("WireGuardTunnel$", "");

                if(tunnel != spareTunnel)
                    Operations.StopTunnel(tunnel);
            }
        }

        /// <summary>
        /// Method to connect to the change of the internet connection to start a tunnel
        /// </summary>
        /// <param name="tunnelName">Name of the tunnel who will be startet on boot</param>
        public static void ConnectToNetworkChange(string tunnelName)
        {
            bootUpTunnel = tunnelName;

            if(NetworkInterface.GetIsNetworkAvailable())
            {
                LogManager.Information($"Network is avaiable. Start tunnel {bootUpTunnel}");
                StartTunnel(bootUpTunnel);
            }
            else
                NetworkChange.NetworkAvailabilityChanged += NetworkChanged;

            LogManager.Information($"StartOnBoot is enabled for tunnel {tunnelName}");
            LogManager.Debug("Connected to NetworkAvailabilityChanged event", nameof(Operations));
        }

        /// <summary>
        /// Hanlder method for when the internet connection is up
        /// </summary>
        private static void NetworkChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            if (e.IsAvailable)
            {
                LogManager.Information($"Network is avaiable. Start tunnel {bootUpTunnel}");
                StartTunnel(bootUpTunnel);
                System.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged -= NetworkChanged;
            }
        }

        #endregion
    }
}
