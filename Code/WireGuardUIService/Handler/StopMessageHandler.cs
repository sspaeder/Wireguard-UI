using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireGuard.Core;
using WireGuard.Core.Messages;

namespace WireGuard.WireGuardUIService.Handler
{
    internal class StopMessageHandler : MessageHandler
    {
        public StopMessageHandler(Context context) : base(context)
        {

        }

        public override Type Type  => typeof(StopMessage);

        public override void Handel(Server server, Message message)
        {
            StopMessage stop = (StopMessage)message;

            try
            {
                string tunnelName = Operations.GetInterfaceName(stop.Name);

                LogManager.Debug($"Beginn stopping: {stop.ToJSON()}", nameof(StopMessageHandler));

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
    }
}
