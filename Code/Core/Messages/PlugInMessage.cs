using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireGuard.Core.Messages
{
    /// <summary>
    /// Base class for messages directed to a plugin
    /// </summary>
    public class PlugInMessage : Message
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="target">Target of the plugin message</param>
        public PlugInMessage(string target) : base("plugin")
        {
            Target = target;
        }

        /// <summary>
        /// Converts the object to a json string
        /// </summary>
        /// <returns></returns>
        public override string ToJSON() => System.Text.Json.JsonSerializer.Serialize(this);

        /// <summary>
        /// Gets the type of message for the plugin
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// The vlaues that will be submited to the plugin
        /// </summary>
        public Dictionary<string, string> Values { get; set; }
    }
}
