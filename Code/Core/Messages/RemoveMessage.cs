using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireGuard.Core.Messages
{
    /// <summary>
    /// Message to remove a configuration
    /// </summary>
    public sealed class RemoveMessage : Message
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public RemoveMessage() : base("remove")
        { 
        }

        /// <summary>
        /// Converts the message to an JSON object
        /// </summary>
        /// <returns></returns>
        public override string ToJSON() => System.Text.Json.JsonSerializer.Serialize(this);

        /// <summary>
        /// The name of the configuration to remove
        /// </summary>
        public string Name { get; set; }
    }
}
