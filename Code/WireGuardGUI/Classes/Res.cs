using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireGuard.GUI
{
    /// <summary>
    /// Class for easy handel of ressources
    /// </summary>
    static class Res
    {
        /// <summary>
        /// Gets a string out of the ressource dictonary
        /// </summary>
        /// <param name="key">Key of the string</param>
        /// <returns>Value of key or in case the key is not existend the key itself</returns>
        public static string GetStr(string key) => 
            App.Current.Resources.Contains(key) ? (string)App.Current.Resources[key] : key;
    }
}
