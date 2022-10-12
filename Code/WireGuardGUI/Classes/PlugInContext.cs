using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using WireGuard.Core;
using WireGuard.Core.PlugIn;

namespace WireGuard.GUI.Classes
{
    /// <summary>
    /// Class for the Plugin context
    /// </summary>
    class PlugInContext : IContext
    {
        #region Public Events

        /// <summary>
        /// Hanlder for page changes
        /// </summary>
        /// <param name="page">page to change to</param>
        public delegate void GoToPageEventHandler(BasePage page);

        /// <summary>
        /// Event which gets called if a page change is requested
        /// </summary>
        public event GoToPageEventHandler GoToPageEvent;

        /// <summary>
        /// Handler for requesting the window to close without exit message
        /// </summary>
        public delegate void RequestExitEventHandler();

        /// <summary>
        /// Event for requesting to close the window without exit message
        /// </summary>
        public event RequestExitEventHandler RequestExitEvent;

        /// <summary>
        /// Handler for config add or delete event
        /// </summary>
        /// <param name="file">File that has been changed</param>
        /// <param name="delete">should the file be deleted</param>
        public delegate void ConfigChangedEventHandler(string file, bool delete = false);

        /// <summary>
        /// Event for the changing of a cofing file
        /// </summary>
        public event ConfigChangedEventHandler ConfigChanged;

        #endregion

        #region Variables

        /// <summary>
        /// Data dictonary for temporary data saving
        /// </summary>
        Dictionary<string, object> dictData;

        #endregion

        #region Public Methods

        /// <summary>
        /// Method to change the page
        /// </summary>
        /// <param name="page">page to navigate to</param>
        public void GoToPage(BasePage page) => GoToPageEvent?.Invoke(page);

        /// <summary>
        /// Sets a data to the data storage
        /// </summary>
        /// <param name="key">Key of the data</param>
        /// <param name="value">Value of the data</param>
        public void Set(string key, object value)
        {
            if (dictData.ContainsKey(key))
                dictData[key] = value;
            else
                dictData.Add(key, value);
        }

        /// <summary>
        /// Gets the data of the storage
        /// </summary>
        /// <typeparam name="T">Type of the data to get</typeparam>
        /// <param name="key">Key of the data</param>
        /// <returns></returns>
        public T Get<T>(string key) => (T)dictData[key];

        /// <summary>
        /// Method to clear the data of the context
        /// </summary>
        public void Clear() => dictData.Clear();

        /// <summary>
        /// Method to load the resources form the plugin
        /// </summary>
        /// <param name="data">Path to the resource XAML file to load</param>
        public void LoadRessource(string data)
        {
            string assemblyName = Assembly.GetCallingAssembly().GetName().Name;
            string uriStr = $"pack://application:,,,/{assemblyName};component/{data}";

            ResourceDictionary dictionary = new ResourceDictionary()
            {
                Source = new System.Uri(uriStr)
            };

            Application.Current.Resources.MergedDictionaries.Add(dictionary);
        }

        /// <summary>
        /// Requests to close the window without an exit message
        /// </summary>
        public void RequestExit() => RequestExitEvent?.Invoke();

        /// <summary>
        /// Requests the addition of a new config file
        /// </summary>
        /// <param name="file">File that was imported</param>
        public void ImportConfig(string file) => ConfigChanged?.Invoke(file);

        /// <summary>
        /// Requests the delete of a new config file
        /// </summary>
        /// <param name="file">File that should be removed</param>
        public void DeleteConfig(string file) => ConfigChanged?.Invoke(file, true);

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="client"></param>
        public PlugInContext(Client client)
        {
            dictData = new Dictionary<string, object>();
            Client = client;
        }

        #endregion

        #region Propertys

        /// <summary>
        /// Gets the client to use to send data to the server
        /// </summary>
        public Client Client { get; init; }

        #endregion
    }
}
