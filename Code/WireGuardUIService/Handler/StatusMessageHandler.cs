using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using WireGuard.Core;
using WireGuard.Core.Messages;

namespace WireGuard.WireGuardUIService.Handler
{
    internal class StatusMessageHandler : MessageHandler
    {
        public StatusMessageHandler(Context context) : base(context)
        {

        }

        public override Type Type => typeof(StatusMessage); 

        public override void Handel(Server server, Message message)
        {
            StatusMessage status = new StatusMessage();

            //Detect wireguard
            status.WireguradDetected = File.Exists(Core.Classes.Path.WIREGUARD_EXE);

            //Get the config files but check if the directory exists
            /// See Issue <see href="https://gitlab.rhrk.uni-kl.de/sccm/ou_admins/wireguard/-/issues/5"/>
            Operations.CheckAndCreateWGConfigFolder();

            string[] files = Directory.GetFiles(Core.Classes.Path.WIREGUARD_CONFIG);

            for (int i = 0; i < files.Length; i++)
                files[i] = files[i].Split('\\').Last();

            status.AviableConfigs = files;

            //Check if ther is a running config
            ServiceController controller = ServiceController.GetServices().Where(x => x.ServiceName.StartsWith("WireGuardTunnel$")).FirstOrDefault();

            if (controller != null)
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
                        FileName = Core.Classes.Path.WIREGUARD_EXE,
                        Arguments = $"/uninstalltunnelservice \"{tunnelName}\""
                    };

                    LogManager.Debug($"Start wireguard.exe: {processStart.Arguments}", nameof(StatusMessageHandler));
                    Process p = Process.Start(processStart);
                }
            }

            server.Send(status);
        }
    }
}
