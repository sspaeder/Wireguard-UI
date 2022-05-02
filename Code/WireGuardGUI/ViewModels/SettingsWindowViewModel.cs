using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireGuard.Core.ViewModels;

namespace WireGuard.GUI.ViewModels
{
    /// <summary>
    /// Class for the representation of the settingswindow
    /// </summary>
    class SettingsWindowViewModel
    {
        /// <summary>
        /// Settings to representate
        /// </summary>
        public SettingsViewModel Settings { get; set; }

        /// <summary>
        /// List of avaiable configs
        /// </summary>
        public ConfigCollectionViewModel Collection { get; set; }
    }
}
