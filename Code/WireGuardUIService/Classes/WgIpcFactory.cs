using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireGuard.WireGuardUIService.Classes.WgApi;

namespace WireGuard.WireGuardUIService.Classes
{
    /// <summary>
    /// Class for the creation of the Wireguard interface api
    /// </summary>
    static class WgIpcFactory
    {
        /// <summary>
        /// Creates a object that implements the IWgIpcApi
        /// </summary>
        /// <param name="preferredApi">Type of the prefferd api type</param>
        /// <returns>Object that implements the IWgIpcApi</returns>
        public static IWgIpcApi Create(string preferredApi)
        {
            if (preferredApi == null)
                return new WgIpc();

            switch(preferredApi)
            {
                case "WgIpc":
                    return new WgIpc();

                default:
                    return new WgIpc();
            }
        }
    }
}
