using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireGuard.Core;
using WireGuard.Core.Messages;

namespace WireGuard.WireGuardUIService.Handler
{
    internal class InterfaceStatusMessageHandler : MessageHandler
    {
        public InterfaceStatusMessageHandler(Context context) : base(context)
        {

        }

        public override Type Type => typeof(InterfaceStatus);

        public override void Handel(Server server, Message message)
        {
            InterfaceStatus status = (InterfaceStatus)message;
            string tunnelName = Operations.GetInterfaceName(status.Interface);

            try
            {
                server.Send(Operations.TunnelStatus(tunnelName));
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
