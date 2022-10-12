using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireGuard.Core;
using WireGuard.Core.Messages;

namespace WireGuard.WireGuardUIService.Handler
{
    /// <summary>
    /// Class to handel a start message
    /// </summary>
    internal class StartMessageHandler : MessageHandler
    {
        public StartMessageHandler(Context context) : base(context)
        {

        }

        public override Type Type => typeof(StartMessage);

        public override void Handel(Server server, Message message)
        {
            StartMessage start = (StartMessage)message;

            try
            {
                server.Send(Operations.StartTunnel(start.File));
            }
            catch (Exception ex)
            {
                LogManager.Error("Error starting the VPN-tunnel");
                LogManager.Error(ex);

                server.Send(new ResultMessage() { Error = -1, ErrorMsg = ex.Message });
            }
        }
    }
}
