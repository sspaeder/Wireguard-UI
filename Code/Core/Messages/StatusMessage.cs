using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireGuard.Core.ViewModels;

namespace WireGuard.Core.Messages
{
    /// <summary>
    /// Status message from the server
    /// </summary>
    public sealed class StatusMessage : Message
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public StatusMessage() : base("status")
        {
        }

        /// <summary>
        /// Converts the message to an JSON object
        /// </summary>
        /// <returns></returns>
        public override string ToJSON() => System.Text.Json.JsonSerializer.Serialize(this);

        /// <summary>
        /// List of aviable configs to start
        /// </summary>
        public string[] AviableConfigs { get; set; } = new string[0];

        /// <summary>
        /// Returns if a tunnel is open or not
        /// </summary>
        public bool TunnelIsOpen { get; set; } = false;

        /// <summary>
        /// Returns the config name of the current running config
        /// </summary>
        public string RunningConfig { get; set; } = "";

        /// <summary>
        /// Returns a value if wireguard is installed
        /// </summary>
        public bool WireguradDetected { get; set; } = false;
    }
}
