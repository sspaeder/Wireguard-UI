using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireGuard.Core.Messages
{
    /// <summary>
    /// Message for the import of an new config
    /// </summary>
    public sealed class ImportMessage : Message
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ImportMessage() : base("import")
        {
        }

        /// <summary>
        /// Converts the message to an JSON object
        /// </summary>
        /// <returns></returns>
        public override string ToJSON() => System.Text.Json.JsonSerializer.Serialize(this);
    
        /// <summary>
        /// File to be imported
        /// </summary>
        public string File { get; set; }

    }
}
