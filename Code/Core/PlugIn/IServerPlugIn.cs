using WireGuard.Core;
using WireGuard.Core.Messages;

namespace WireGuard.Core.PlugIn
{
    /// <summary>
    /// Interface class for the server
    /// </summary>
    public interface IServerPlugIn
    {

        /// <summary>
        /// Method to handel messages directed to a plugin
        /// </summary>
        /// <param name="server">Server object to communicate whith the server</param>
        /// <param name="msg"></param>
        void HandelMessage(Server server, PlugInMessage msg);

        /// <summary>
        /// Gets the name of the plugin to load
        /// </summary>
        string Name { get; }
    }
}
