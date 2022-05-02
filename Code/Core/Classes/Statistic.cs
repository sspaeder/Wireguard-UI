using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireGuard.Core.Messages;

namespace WireGuard.Core.Classes
{
    /// <summary>
    /// Statistic of an interface
    /// </summary>
    public class Statistic
    {
        /// <summary>
        /// Clears the vlaues of the statistic
        /// </summary>
        public void Clear()
        {
            PublicKey = "";
            ListeningPort = "";
            Peer = "";
            Endpoint = "";
            AllowedIPs = "";
            LatestHandshake = "";
            Transfer = "";
        }

        /// <summary>
        /// Applys the new information to the object
        /// </summary>
        /// <param name="status"><see cref="InterfaceStatus"/> message with the informations</param>
        public void Apply(InterfaceStatus status)
        {
            PublicKey = status.PublicKey;
            ListeningPort = status.ListeningPort;
            Peer = status.Peer;
            Endpoint = status.Endpoint;
            AllowedIPs = status.AllowedIPs;
            LatestHandshake = status.LatestHandshake;
            Transfer = status.Transfer;
        }

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
