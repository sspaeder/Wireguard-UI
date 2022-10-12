using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireGuard.Core;
using WireGuard.Core.Messages;
using WireGuard.WireGuardUIService.Classes;

namespace WireGuard.WireGuardUIService.Handler
{
    internal class LogContentMessageHandler : MessageHandler
    {
        public LogContentMessageHandler(Context context) : base(context)
        {

        }

        public override Type Type => typeof(LogContentMessage);

        public override void Handel(Server server, Message message)
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
