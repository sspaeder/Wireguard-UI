using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireGuard.Core;
using WireGuard.Core.Messages;

namespace WireGuard.WireGuardUIService.Handler
{
    internal class RemoveMessageHandler : MessageHandler
    {
        public RemoveMessageHandler(Context context) : base(context)
        {

        }

        public override Type Type => typeof(RemoveMessageHandler);

        public override void Handel(Server server, Message message)
        {
            server.Send(Operations.RemoveConfiguration(((RemoveMessage)message).Name));
        }
    }
}
