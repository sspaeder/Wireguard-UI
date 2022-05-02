using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireGuard.Core.ViewModels;

namespace WireGuard.WireGuardUIService
{
    /// <summary>
    /// Class for the configurations of the service
    /// </summary>
    static class Config
    {
        /// <summary>
        /// Initalizes the configuration
        /// </summary>
        public static void Init()
        {
            try
            {
                if (File.Exists(Core.Path.SETTINGS))
                    Settings = System.Text.Json.JsonSerializer.Deserialize<SettingsViewModel>(File.ReadAllText(Core.Path.SETTINGS));
                else
                    Settings = new SettingsViewModel();
            }
            catch(Exception ex)
            {
                Core.LogManager.Error(ex);
                Settings = new SettingsViewModel();
            }
        }

        /// <summary>
        /// Gets the settings loaded from the settings file
        /// </summary>
        public static SettingsViewModel Settings { get; private set; }
    }
}
