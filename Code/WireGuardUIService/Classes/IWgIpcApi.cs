using WireGuard.Core.Messages;

namespace WireGuard.WireGuardUIService.Classes
{
    /// <summary>
    /// Interface for the Wireguard IPC Api for getting the tunnel status
    /// </summary>
    interface IWgIpcApi
    {
        /// <summary>
        /// Method to get the tunnel status
        /// </summary>
        /// <param name="tunnelName">Name of the tunnel to retirve the informations</param>
        /// <returns>Message object. On success InterfaceStatus; on failure ResultMessage</returns>
        Message GetTunnelStatus(string tunnelName);
    }
}
