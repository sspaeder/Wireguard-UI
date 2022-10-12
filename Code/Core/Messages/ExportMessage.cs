using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireGuard.Core.Messages
{
    /// <summary>
    /// Message to export the log files
    /// </summary>
    public class ExportMessage : Message
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ExportMessage() : base("export")
        {
        }

        /// <summary>
        /// Converts the message to an JSON object
        /// </summary>
        /// <returns></returns>
        public override string ToJSON() => System.Text.Json.JsonSerializer.Serialize(this);

        /// <summary>
        /// Filename to export to
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// User who requested the operation
        /// </summary>
        public string Username { get; set; }
    }
}
