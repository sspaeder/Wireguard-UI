using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireGuard.Core.ViewModels;

namespace WireGuard.Core.Messages
{
    /// <summary>
    /// Set setting message from client to server
    /// </summary>
    public sealed class SettingsMessage : Message
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SettingsMessage() : base("settings")
        { }

        /// <summary>
        /// Converts the message to an JSON object
        /// </summary>
        /// <returns></returns>
        public override string ToJSON() => System.Text.Json.JsonSerializer.Serialize(this);

        /// <summary>
        /// Settings to be transmitted
        /// </summary>
        public SettingsViewModel Settings { get; set; }
    }
}
