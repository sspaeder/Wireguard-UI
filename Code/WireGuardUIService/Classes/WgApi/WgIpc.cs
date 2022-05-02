using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireGuard.Core;
using WireGuard.Core.Messages;

namespace WireGuard.WireGuardUIService.Classes.WgApi
{
    /// <summary>
    /// Class for the communication with the tunnel via wg.exe
    /// </summary>
    class WgIpc : IWgIpcApi
    {

        /// <summary>
        /// Extracts an data from a string
        /// </summary>
        /// <param name="data">Data to extract</param>
        /// <returns></returns>
        private static string ExtractValue(string data) => data.Substring(data.IndexOf(':') + 1).Trim();

        /// <summary>
        /// Method tho get the tunnel informations via wg.exe
        /// </summary>
        /// <param name="tunnelName">Name of the tunnel to retirve the inforamtions</param>
        /// <returns></returns>
        public Message GetTunnelStatus(string tunnelName)
        {
            //Start the process
            ProcessStartInfo processStart = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = Path.WIREGUARD_WG_EXE,
                Arguments = $"show {tunnelName}",
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            LogManager.Debug($"Starting of wg.exe with: {processStart.Arguments}", nameof(WgIpc));

            Process p = new Process();
            p.StartInfo = processStart;

            p.Start();

            //Read content of the stream
            string result = p.StandardOutput.ReadToEnd();

            //Wait for exit and send result
            p.WaitForExit();

            LogManager.Debug($"Output of WG is:\n{result}", nameof(WgIpc));

            if (p.ExitCode != 0)
            {
                LogManager.Error($"wg.exe returned {p.ExitCode}");
                return new ResultMessage() { Error = -1, ErrorMsg = $"wg.exe returned {p.ExitCode}" };
            }

            //Process result
            string[] data = result.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            InterfaceStatus status = new InterfaceStatus();

            foreach (string str in data)
            {
                if (str.StartsWith("public key"))
                    status.PublicKey = ExtractValue(str);
                else if (str.StartsWith("listening port"))
                    status.ListeningPort = ExtractValue(str);
                else if (str.StartsWith("peer"))
                    status.Peer = ExtractValue(str);
                else if (str.StartsWith("endpoint"))
                    status.Endpoint = ExtractValue(str);
                else if (str.StartsWith("allowed ips"))
                    status.AllowedIPs = ExtractValue(str);
                else if (str.StartsWith("latest handshake"))
                    status.LatestHandshake = ExtractValue(str);
                else if (str.StartsWith("transfer"))
                    status.Transfer = ExtractValue(str);
            }

            return status;
        }
    }
}
