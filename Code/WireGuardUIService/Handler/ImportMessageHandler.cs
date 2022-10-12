using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireGuard.Core;
using WireGuard.Core.Messages;

namespace WireGuard.WireGuardUIService.Handler
{
    internal class ImportMessageHandler : MessageHandler
    {
        public ImportMessageHandler(Context context) : base(context)
        {

        }

        public override Type Type => typeof(ImportMessageHandler);

        public override void Handel(Server server, Message message)
        {
            server.Send(Operations.ImportConfiguration(((ImportMessage)message).File));
        }
    }
}
