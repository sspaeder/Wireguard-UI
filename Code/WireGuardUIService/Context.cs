using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireGuard.Core.Classes;
using WireGuard.Core.PlugIn;
using WireGuard.Core.ViewModels;
using WireGuard.WireGuardUIService.Handler;

namespace WireGuard.WireGuardUIService
{
    /// <summary>
    /// Class for internal running context
    /// </summary>
    internal class Context
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Context()
        {
            try
            {
                Handler = new PlugInController<MessageHandler>();
                Handler.Load(args: new[] { this });

                if (System.IO.Directory.Exists(Path.PLUGIN_FOLDER))
                {
                    foreach (string dll in System.IO.Directory.GetFiles(Path.PLUGIN_FOLDER, "*.dll"))
                        Handler.Load(dll);
                }
                else
                    System.IO.Directory.CreateDirectory(Path.PLUGIN_FOLDER);

                if (System.IO.File.Exists(Path.SETTINGS))
                    Settings = System.Text.Json.JsonSerializer.Deserialize<SettingsViewModel>(System.IO.File.ReadAllText(Path.SETTINGS));
                else
                    Settings = null;
            }
            catch (Exception ex)
            {
                Core.LogManager.Error(ex);
                Settings = new SettingsViewModel();
            }
        }

        /// <summary>
        /// Handler for messages
        /// </summary>
        public PlugInController<MessageHandler> Handler { get; }

        /// <summary>
        /// Gets the settings loaded from the settings file
        /// </summary>
        public SettingsViewModel Settings { get; }

    }
}
