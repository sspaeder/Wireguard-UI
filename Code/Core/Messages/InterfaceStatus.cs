using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireGuard.Core.Messages
{
    /// <summary>
    /// Message for the interface status
    /// </summary>
    public sealed class InterfaceStatus : Message
    {
        /// <summary>
        /// Constructor 
        /// </summary>
        public InterfaceStatus() : base("istatus")
        {
        }

        /// <summary>
        /// Converts the message to an JSON object
        /// </summary>
        /// <returns></returns>
        public override string ToJSON() => System.Text.Json.JsonSerializer.Serialize(this);

        /// <summary>
        /// Interface for which this status is for
        /// </summary>
        public string Interface { get; set; }

        /// <summary>
        /// Publickey of this interface
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// Port on that the interface is listening
        /// </summary>
        public string ListeningPort { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Peer { get; set; }

        /// <summary>
        /// Endpoint of this interface
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// IPs allowed in this interface
        /// </summary>
        public string AllowedIPs { get; set; }

        /// <summary>
        /// When the latest handshake was done
        /// </summary>
        public string LatestHandshake { get; set; }

        /// <summary>
        /// The transferd data on this interface
        /// </summary>
        public string Transfer { get; set; }
        
    }
}
