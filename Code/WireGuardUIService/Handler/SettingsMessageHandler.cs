using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WireGuard.Core;
using WireGuard.Core.Classes;
using WireGuard.Core.Messages;
using WireGuard.Core.ViewModels;

namespace WireGuard.WireGuardUIService.Handler
{
    internal class SettingsMessageHandler : MessageHandler
    {
        public SettingsMessageHandler(Context context) : base(context)
        {

        }

        public override Type Type => typeof(SettingsMessage);

        public override void Handel(Server server, Message message)
        {
            SettingsMessage sm = (SettingsMessage)message;

            try
            {
                string json;

                if (sm.Settings == null)
                {
                    json = System.IO.File.ReadAllText(Path.SETTINGS);
                    server.Send(new ResultMessage() { Error = 0, ErrorMsg = json });

                    return;
                }

                Context.Settings.Apply(sm.Settings);

                json = JsonSerializer.Serialize(Context.Settings,
                                                       typeof(SettingsViewModel),
                                                       new JsonSerializerOptions() { WriteIndented = true }
                                                       );

                System.IO.File.WriteAllText(Path.SETTINGS, json);

                server.Send(new ResultMessage() { Error = 0 });
            }
            catch (Exception ex)
            {
                LogManager.Error("Error by applying settings");
                LogManager.Error(ex);

                server.Send(new ResultMessage() { Error = -1, ErrorMsg = ex.Message });
            }
        }
    }
}
