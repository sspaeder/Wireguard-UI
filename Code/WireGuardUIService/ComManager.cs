using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.ServiceProcess;
using System.Text.Json;
using WireGuard.Core;
using WireGuard.Core.Messages;
using WireGuard.Core.PlugIn;
using WireGuard.Core.ViewModels;
using WireGuard.WireGuardUIService.Classes;

namespace WireGuard.WireGuardUIService
{
    /// <summary>
    /// Class with all methods to handel the communication
    /// </summary>
    static class ComManager
    {
        /// <summary>
        /// Starts the VPN tunnel with the given config file
        /// </summary>
        /// <param name="server">server to communicate with the client</param>
        /// <param name="start">start message with the informations</param>
        public static void Start(Server server, StartMessage start)
        {
            try
            {
                server.Send(Operations.StartTunnel(start.File));
            }
            catch(Exception ex)
            {
                LogManager.Error("Error starting the VPN-tunnel");
                LogManager.Error(ex);

                server.Send(new ResultMessage() { Error = -1, ErrorMsg = ex.Message });
            }
        }

        /// <summary>
        /// Stops the vpn tunnel with the given config name
        /// </summary>
        /// <param name="server">server to communicate with the client</param>
        /// <param name="stop">stop message with the informations</param>
        public static void Stop(Server server, StopMessage stop)
        {
            try
            {
                string tunnelName = Operations.GetInterfaceName(stop.Name);

                LogManager.Debug($"Beginn stopping: {stop.ToJSON()}", nameof(ComManager));

                ResultMessage rmsg = Operations.StopTunnel(tunnelName);
                server.Send(rmsg);
            }
            catch (Exception ex)
            {
                LogManager.Error("Error stopping the VPN-tunnel");
                LogManager.Error(ex);

                server.Send(new ResultMessage() { Error = -1, ErrorMsg = ex.Message });
            }
        }
        
        /// <summary>
        /// Starts the VPN tunnel with the given config file
        /// </summary>
        /// <param name="server">server to communicate with the client</param>
        /// <param name="start">import message with the informations</param>
        public static void Import(Server server, ImportMessage im)
        {
            server.Send(Operations.ImportConfiguration(im.File));
        }

        /// <summary>
        /// Removes an configuration
        /// </summary>
        /// <param name="server">server to communicate with the client</param>
        /// <param name="rm">remove message with the informations</param>
        public static void Remove(Server server, RemoveMessage rm)
        {
            server.Send(Operations.RemoveConfiguration(rm.Name));
        }

        /// <summary>
        /// Reports the initial status of the service
        /// </summary>
        /// <param name="server">Server to communicate with the client</param>
        public static void ServiceStatus(Server server)
        {
            StatusMessage status = new StatusMessage();

            //Detect wireguard
            status.WireguradDetected = File.Exists(Core.Path.WIREGUARD_EXE);

            //Get the config files but check if the directory exists
            /// See Issue <see href="https://gitlab.rhrk.uni-kl.de/sccm/ou_admins/wireguard/-/issues/5"/>
            Operations.CheckAndCreateWGConfigFolder();

            string[] files = Directory.GetFiles(Core.Path.WIREGUARD_CONFIG);

            for (int i = 0; i < files.Length; i++)
                files[i] = files[i].Split('\\').Last();
                
            status.AviableConfigs = files;

            //Check if ther is a running config
            ServiceController controller = ServiceController.GetServices().Where(x => x.ServiceName.StartsWith("WireGuardTunnel$")).FirstOrDefault();

            if(controller != null)
            {
                string tunnelName = controller.ServiceName.Replace("WireGuardTunnel$", "");

                //Add service as running
                if (controller.Status == ServiceControllerStatus.Running)
                    status.RunningConfig = tunnelName;
                else //Deinstall VPN-tunnel
                {
                    ProcessStartInfo processStart = new ProcessStartInfo()
                    {
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        FileName = Core.Path.WIREGUARD_EXE,
                        Arguments = $"/uninstalltunnelservice \"{tunnelName}\""
                    };

                    LogManager.Debug($"Start wireguard.exe: {processStart.Arguments}", nameof(ComManager));
                    Process p = Process.Start(processStart);
                }
            }

            server.Send(status);
        }

        /// <summary>
        /// Sets settings for the application
        /// </summary>
        /// <param name="server">server to communicate with the client</param>
        /// <param name="sm">settings message with the informations</param>
        public static void SetSettings(Server server, SettingsMessage sm)
        {
            try
            {
                Config.Settings.Apply(sm.Settings);

                string json = JsonSerializer.Serialize(Config.Settings, 
                                                       typeof(SettingsViewModel),
                                                       new JsonSerializerOptions() { WriteIndented = true }
                                                       );

                File.WriteAllText(Core.Path.SETTINGS, json);

                server.Send(new ResultMessage() { Error = 0 });
            }
            catch(Exception ex)
            {
                LogManager.Error("Error by applying settings");
                LogManager.Error(ex);

                server.Send(new ResultMessage() { Error = -1, ErrorMsg = ex.Message });
            }
        }

        /// <summary>
        /// Reports the current status of an specified interface
        /// </summary>
        /// <param name="server">server to communicate with the client</param>
        /// <param name="status">status message with the informations to be filled</param>
        public static void InterfaceStatus(Server server, InterfaceStatus status)
        {
            string tunnelName = Operations.GetInterfaceName(status.Interface);

            try
            {
                server.Send(Operations.TunnelStatus(tunnelName));
            }
            catch(Exception ex)
            {
                LogManager.Error("Error starting the VPN-tunnel");
                LogManager.Error(ex);

                server.Send(new ResultMessage() { Error = -1, ErrorMsg = ex.Message });
            }
        }

        /// <summary>
        /// Method to get the wireguard log content
        /// </summary>
        /// <param name="server">server to communicate with the client</param>
        /// <param name="lcm">message that was recived from the client</param>
        internal static void GetWgLogContent(Server server, LogContentMessage lcm)
        {
            try
            {
                server.Send(new ResultMessage() { Error = 0, ErrorMsg = WgLogReader.Read() }); 
            }
            catch (Exception ex)
            {
                LogManager.Error("Error reading wg logfile");
                LogManager.Error(ex);

                server.Send(new ResultMessage() { Error = -1, ErrorMsg = ex.Message });
            }
        }
    }
}