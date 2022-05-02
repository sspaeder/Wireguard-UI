using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireGuard.Core.PlugIn
{
    /// <summary>
    /// Interface for the plug in context
    /// </summary>
    public interface IContext
    {
        /// <summary>
        /// Stores a variable
        /// </summary>
        /// <param name="key">Key of the variable</param>
        /// <param name="value">Object to store</param>
        void Set(string key, object value);

        /// <summary>
        /// Gets a variable from the store
        /// </summary>
        /// <typeparam name="T">Type of the variable</typeparam>
        /// <param name="key">Key of the variable to retrieve</param>
        /// <returns>Object of type T</returns>
        T Get<T>(string key);

        /// <summary>
        /// Clears the store
        /// </summary>
        void Clear();

        /// <summary>
        /// Goes to the specified page
        /// </summary>
        /// <param name="page"><see cref="BasePage"/> to navigate to</param>
        void GoToPage(BasePage page);

        /// <summary>
        /// Loads an XAML ressource dictonary
        /// </summary>
        /// <param name="ressource">Path to the XAML ressource dictonary</param>
        void LoadRessource(string ressource);

        /// <summary>
        /// Requests to exit the window
        /// </summary>
        void RequestExit();

        /// <summary>
        /// Imports a config file to the config store
        /// </summary>
        /// <param name="file">Filepath to import</param>
        void ImportConfig(string file);

        /// <summary>
        /// Delets a config file from the store
        /// </summary>
        /// <param name="file">File to delete</param>
        void DeleteConfig(string file);

        /// <summary>
        /// Gets the client to communicate with the server
        /// </summary>
        Client Client { get; }
    }
}
