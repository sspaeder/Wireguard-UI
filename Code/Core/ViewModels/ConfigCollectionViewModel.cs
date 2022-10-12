using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireGuard.Core.Messages;

namespace WireGuard.Core.ViewModels
{
    /// <summary>
    /// Class that represents a collection of configurations
    /// </summary>
    public class ConfigCollectionViewModel : BaseViewModel
    {
        /// <summary>
        /// Event handler for if there are invalid characters
        /// </summary>
        /// <returns></returns>
        public delegate bool InvalidCharactersEventHandler();

        /// <summary>
        /// Event gets raised when invalid characters where found
        /// </summary>
        public event InvalidCharactersEventHandler InvalidCharacters;

        /// <summary>
        /// Event gets raised when a config changes its status
        /// </summary>
        public event ConfigViewModel.StatusChangedEventHandler ConfigStatusChanged;

        /// <summary>
        /// Client to communicate with the service
        /// </summary>
        Client client;

        /// <summary>
        /// Settings to apply
        /// </summary>
        SettingsViewModel settings;

        /// <summary>
        /// Construcotr
        /// </summary>
        /// <param name="client">client to communicate with ther service</param>
        public ConfigCollectionViewModel(Client client, SettingsViewModel settings)
        {
            //Initialize data
            Configs = new ObservableCollection<ConfigViewModel>();
            this.client = client;
            this.settings = settings;
            settings.PropertyChanged += SettingsChanged;
        }

        /// <summary>
        /// Adds a config to the collection
        /// </summary>
        /// <param name="file">Name of the config file</param>
        public async void AddFile(string file)
        {
            if (!File.Exists(file))
                RaiseErrorEvent(this, new FileNotFoundException(file), null);

            FileInfo fi = new FileInfo(file);

            if (fi.Name.Any(x => x == ' ' || x == '(' || x == ')' || x == '[' || x == ']'))
                if (!(InvalidCharacters?.Invoke()).Value)
                    return;

            Message msg = await Task.Run(() =>
            {
                client.Send(new ImportMessage() { File = file });
                return client.Recive();
            });

            if (msg is ResultMessage)
            {
                ResultMessage rm = (ResultMessage)msg;

                if (rm.Error != 0)
                    RaiseErrorEvent(this, null, rm.ErrorMsg);
            }
            else
            {
                RaiseErrorEvent(this, null, "UNKOWN_PACKAGE");
            }

            Add(fi.Name);
        }

        /// <summary>
        /// Adds a config to the collection
        /// </summary>
        /// <param name="name">Name of the configuration to be added</param>
        public void Add(string name)
        {
            ConfigViewModel cvm = new ConfigViewModel(this, client, name) { TimerInterval = settings.TimerInterval };
            cvm.ErrorOccured += Config_ErrorOccured;
            cvm.StatusChanged += Config_StatusChanged;
            Configs.Add(cvm);

            HasElements = true;
            NotifyPropertyChanged("HasElements");
        }

        /// <summary>
        /// Removes a config from the collection
        /// </summary>
        /// <param name="model">model to remove</param>
        public void Remove(ConfigViewModel model)
        {
            Configs.Remove(model);

            if(Configs.Count == 0)
            {
                HasElements = false;
                NotifyPropertyChanged("HasElements");
            }
        }

        /// <summary>
        /// Handler for the error events in a config file
        /// </summary>
        /// <param name="sender">object who called the event</param>
        /// <param name="ex">Exception that got raised</param>
        /// <param name="text">Error message</param>
        private void Config_ErrorOccured(BaseViewModel sender, Exception ex, string text) => RaiseErrorEvent(sender, ex, text);

        /// <summary>
        /// Handler for status changed events from a config
        /// </summary>
        /// <param name="sender">Config which has changed</param>
        /// <param name="status">The new status of the config</param>
        private void Config_StatusChanged(ConfigViewModel sender, ConfigClientStatus status) => ConfigStatusChanged?.Invoke(sender, status);

        /// <summary>
        /// Method to observ if a settings value has changed
        /// </summary>
        private void SettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(settings.TimerInterval))
            {
                foreach (ConfigViewModel cvm in Configs)
                    cvm.TimerInterval = settings.TimerInterval;
            }
        }

        /// <summary>
        /// Gets an sorted list of all loaded configurations
        /// </summary>
        public ObservableCollection<ConfigViewModel> Configs { get; }

        /// <summary>
        /// Gets or sets the value if configuration files are aviable
        /// </summary>
        public bool HasElements { get; private set; } = false;
    }
}
