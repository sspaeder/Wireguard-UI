using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireGuard.Core;

namespace WireGuard.WireGuardUIService.Handler
{
    /// <summary>
    /// Interface for handling messages
    /// </summary>
    internal abstract class MessageHandler
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MessageHandler(Context context)
        {
            Context = context;
        }

        /// <summary>
        /// Handels the message passed to the object
        /// </summary>
        /// <param name="server">Server to respond to the client</param>
        /// <param name="message">Message to process</param>
        public abstract void Handel(Server server, Message message);

        /// <summary>
        /// Type of message the object handels
        /// </summary>
        public abstract Type Type { get; }

        /// <summary>
        /// Gets the context of the service
        /// </summary>
        public Context Context { get; }
    }
}
