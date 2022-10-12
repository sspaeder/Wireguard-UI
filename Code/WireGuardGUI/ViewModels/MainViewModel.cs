using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WireGuard.Core;
using WireGuard.Core.Classes;
using WireGuard.Core.Messages;
using WireGuard.Core.ViewModels;
using WireGuard.GUI.Classes;
using WireGuard.GUI.Windows;

namespace WireGuard.GUI.ViewModels
{
    /// <summary>
    /// Class for the representation of the main actions and data
    /// </summary>
    internal class MainViewModel : BaseViewModel
    {
        #region Variables

        /// <summary>
        /// Variable to store a refrence to the main window
        /// </summary>
        MainWindow window;

        /// <summary>
        /// Variable for the selected config
        /// </summary>
        ConfigViewModel selected;

        /// <summary>
        /// Client to connecto to the service
        /// </summary>
        Client client;

        /// <summary>
        /// Settings for the application
        /// </summary>
        SettingsViewModel settings;

        /// <summary>
        /// Viewmodel for the import of config files
        /// </summary>
        ImportViewModel import;

        /// <summary>
        /// Viewmodel for the log files
        /// </summary>
        LogViewModel log;

        /// <summary>
        /// Variable to check if the client is currently connecting
        /// </summary>
        bool isConnecting = false;

        /// <summary>
        /// Gets the current running config
        /// </summary>
        ConfigViewModel runnig;

        #endregion

        #region Generell Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public MainViewModel(MainWindow mainWindow, Client client, SettingsViewModel settings)
        {
            //Initialize data
            window = mainWindow;

            this.client = client;
            Settings = settings;

            Collection = new ConfigCollectionViewModel(client, settings);
            Collection.ErrorOccured += Collection_ErrorOccured;
            Collection.ConfigStatusChanged += Collection_ConfigStatusChanged;
            Collection.InvalidCharacters += Collection_InvalidCharacters;

            import = new ImportViewModel(client, Collection)
            {
                FileImportAction = new Action(() => { ImportConfig(); })
            };

            log = new LogViewModel(client);

            //Initialize commands
            CloseCmd = new RelayCommand(CloseCmdMethod);
            ShowWindowCmd = new RelayCommand(ShowWindowMethod);
            AddConfigCmd = new RelayCommand(AddConfigMehtod);
            RemoveConfigCmd = new RelayCommand(RemoveConfigMethod, RemoveConfigPredicate);
            ShowSettings = new RelayCommand(ShowSettingsMethod);
        }

        /// <summary>
        /// Handler for when the imported file contains invalid characters
        /// </summary>
        /// <returns></returns>
        private bool Collection_InvalidCharacters() =>
            MessageBox.Show(Res.GetStr("LBL_INF_UNSUPPORTED_CHARS"), Res.GetStr("LBL_INF_UNSUPPORTED_CHARS_CAPTION"), MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;

        /// <summary>
        /// Handler for errors in the collction
        /// </summary>
        /// <param name="sender">object who called the event</param>
        /// <param name="ex">Exception that got raised</param>
        /// <param name="text">Error message</param>
        private void Collection_ErrorOccured(BaseViewModel sender, Exception ex, string text) => RaiseErrorEvent(sender, ex, text);

        /// <summary>
        /// Handler for status changing of a config element
        /// </summary>
        /// <param name="sender">Config which has changed</param>
        /// <param name="status">The new status of the element</param>
        private void Collection_ConfigStatusChanged(ConfigViewModel sender, ConfigClientStatus status)
        {
            if(status == ConfigClientStatus.Running)
            {
                RunningConfig = sender;
                IsRunning = true;

                if(!ProcessingSettings)
                {
                    window.WindowState = WindowState.Minimized;
                    window.ShowInTaskbar = false;
                }
            }
            else if(status == ConfigClientStatus.Stopped)
            {
                RunningConfig = null;
                IsRunning = false;
            }
                

            NotifyPropertyChanged("IsRunning");
        }

        /// <summary>
        /// Method to connect to the server
        /// </summary>
        public async void ConnectAsync()
        {
            ProcessingSettings = true;

            try
            {
                //Connect to client
                IsConnecting = true;

                client.Send(new StatusMessage());

                //Connect and recive status message
                Message msg = await Task.Run(() => {
                    return client.Recive();
                });

                //Process initial message
                if (msg is StatusMessage)
                {
                    StatusMessage sm = (StatusMessage)msg;

                    //Is wireguard installed
                    if (!sm.WireguradDetected)
                        RaiseErrorEvent(this, null, "!!ERR_NO_WIREGUARD");

                    //Check if configs are avaiable
                    if (sm.AviableConfigs.Length > 0)
                    {
                        foreach (string config in sm.AviableConfigs)
                            Collection.Add(config);

                        //Check if a conifg is currently running
                        if (String.IsNullOrEmpty(sm.RunningConfig))
                            Selected = Collection.Configs.First();
                        else
                        {
                            Selected = Collection.Configs.First(x => x.Name.StartsWith(sm.RunningConfig));
                            Selected.Status = ConfigClientStatus.Running;
                            Selected.UpdateStatus();
                        }
                    }
                }
                else if (msg is ResultMessage) //Handel if an error occured
                {
                    ResultMessage rm = (ResultMessage)msg;
                    LogManager.Error($"[{rm.Error}] {rm.ErrorMsg}");

                    RaiseErrorEvent(this, null, "!!ERR_GENERIC");
                }
                else
                {
                    LogManager.Error($"Unexpexted message type: {msg.Type}");
                    RaiseErrorEvent(this, null, "!!ERR_NO_CONNECTION");
                }
            }
            catch(TimeoutException tex)
            {
                LogManager.Error("Unable to connect to service within 15 seconds");
                RaiseErrorEvent(this, tex, "!!ERR_TIMEOUT");
            }
            catch(Exception ex)
            {
                LogManager.Error("Exception in ConnectAsync");
                LogManager.Error(ex);
                RaiseErrorEvent(this, ex, "!!ERR_GENERIC");
            }
            finally
            {
                //Regardless what happens, the system is not trying to connect anymore
                IsConnecting = false;

                //Notify that the settings has been changed
                NotifyPropertyChanged("Settings");

                //Applys the settings to the application
                ApplySettings();

                ProcessingSettings = false;
            }
        }

        /// <summary>
        /// Disconnects the client
        /// </summary>
        public void Disconnect() => client?.Disconnect();

        /// <summary>
        /// Applys the settings to the application
        /// </summary>
        private void ApplySettings()
        {
            #region Usersettings

            // DefaultConfig
            if(!String.IsNullOrEmpty(settings.DefaultConfig))
            {
                ConfigViewModel cvm = Collection.Configs.FirstOrDefault(x => x.Name == settings.DefaultConfig);

                if (cvm == null)
                    LogManager.Warning($"The default config \"{settings.DefaultConfig}\" could not be applied because it dosent exist.");
                else
                    Selected = cvm;
            }

            #endregion

            #region Adminsettings

            // TimerInterval
            // => Is handeld already at this point

            // RunInKioskMode
            // => Handeld in XAML / uses defaultconfig or first

            if (settings.RunInKioskMode)
            {
                if (Collection.Configs.Count > 0)
                {
                    if (!string.IsNullOrEmpty(settings.DefaultConfig))
                    {
                        ConfigViewModel cvm = Collection.Configs.FirstOrDefault(x => x.Name == settings.DefaultConfig);

                        if (cvm == null)
                        {
                            selected = Collection.Configs.First();
                            LogManager.Warning($"Could not find \"{settings.DefaultConfig}\", loaded \"{selected.Name}\" instead.");
                        }
                        else
                            selected = cvm;
                    }
                    else
                        selected = Collection.Configs[0];
                }
                else
                {
                    LogManager.Error("Could not apply RunInKioskMode because there is no Config to load");
                    RaiseErrorEvent(this, null, "!!ERR_NO_KIOSK_MODE");

                    //Return to normal mode when kioskmode is not working
                    settings.RunInKioskMode = false;
                    NotifyPropertyChanged("Settings");
                }
            }
            #endregion
        }

        /// <summary>
        /// Imports a config through file selection
        /// </summary>
        private void ImportConfig()
        {
            //Open a file dialog window
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Config|*.conf",
                Multiselect = false,
                ValidateNames = true,
                CheckFileExists = true
            };

            if (ofd.ShowDialog().Value)
            {
                if (System.IO.File.Exists(ofd.FileName))
                {
                    //Check if the selected file exists
                    System.IO.FileInfo info = new System.IO.FileInfo(ofd.FileName);

                    // Move the file first to ProgramData, because it is possible that the user
                    // wants to import from a fileshare
                    // Fixxing the issue #13
                    LogManager.Debug($@"Moving file from: '{info.FullName}' to '{Path.DATA_FOLDER}\{info.Name}'", nameof(MainViewModel));

                    if (!System.IO.Directory.Exists(Path.DATA_FOLDER))
                        System.IO.Directory.CreateDirectory(Path.DATA_FOLDER);

                    System.IO.File.Move(info.FullName, $@"{Path.DATA_FOLDER}\{info.Name}");

                    Collection.AddFile($@"{Path.DATA_FOLDER}\{info.Name}");         
                }
                else
                    RaiseErrorEvent(this, null, $"Selected file does not exist!\n{ofd.FileName}");
            }
        }

        #endregion

        #region Command Methods
        
        /// <summary>
        /// Command to close the window
        /// </summary>
        public RelayCommand CloseCmd { get; }

        /// <summary>
        /// Method to handel the command close action
        /// </summary>
        /// <param name="parameter"></param>
        private void CloseCmdMethod(object parameter)
        {
            Disconnect();
            window.Close();
        }

        /// <summary>
        /// Command to display the window again
        /// </summary>
        public RelayCommand ShowWindowCmd { get; }

        /// <summary>
        /// Metho which is called when ShowWindowCmd is used
        /// </summary>
        /// <param name="parameter"></param>
        public void ShowWindowMethod(object parameter)
        {
            window.WindowState = WindowState.Normal;
            window.ShowInTaskbar = true;
        }

        /// <summary>
        /// Command to add a config file
        /// </summary>
        public RelayCommand AddConfigCmd { get; }

        /// <summary>
        /// Method which is called when AddConfigCmd is used
        /// </summary>
        /// <param name="parameter"></param>
        private void AddConfigMehtod(object parameter)
        {
            try
            {
                if (import.Count == 0)
                    ImportConfig();
                else
                {
                    ImportWindow iw = new ImportWindow();
                    iw.DataContext = import;
                    iw.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("Exception in AddConfigMehtod");
                LogManager.Error(ex);
                RaiseErrorEvent(this, ex, "!!ERR_GENERIC"); 
            }
        }

        /// <summary>
        /// Command to remove the current selected config
        /// </summary>
        public RelayCommand RemoveConfigCmd { get; }

        /// <summary>
        /// Method which gets called when RemoveConfigCmd is used
        /// </summary>
        /// <param name="parameter"></param>
        private async void RemoveConfigMethod(object parameter)
        {
            try
            {
                if (MessageBox.Show(Res.GetStr("QST_DELETE_CONFIG"), Res.GetStr("LBL_DELETE_WINDOW"), MessageBoxButton.YesNo) == MessageBoxResult.No)
                    return;

                Message msg = await Task.Run(() => {
                    client.Send(new RemoveMessage() { Name = Selected.Name });
                    return client.Recive();
                });

                if(msg is ResultMessage)
                {
                    ResultMessage rm = (ResultMessage)msg;

                    if(rm.Error == 0)
                    {
                        Collection.Remove(selected);
                        Selected = Collection.Configs.FirstOrDefault();
                    }
                    else
                        RaiseErrorEvent(this, null, $"[{rm.Error}] {rm.ErrorMsg}");
                }
                else
                {
                    LogManager.Error($"Unexpexted message type: {msg.Type}");
                    RaiseErrorEvent(this, null, "!!ERR_GENERIC");
                }
            }
            catch(Exception ex)
            {
                LogManager.Error("Exception in RemoveConfigMethod");
                LogManager.Error(ex);
                RaiseErrorEvent(this, ex, "!!ERR_GENERIC");
            }
        }

        /// <summary>
        /// Method to check if RemoveConfigCmd can be called
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private bool RemoveConfigPredicate(object parameter)
        {
            LogManager.Debug($"Selected Config: {(Selected != null ? Selected : "<NA>") } // Status: {Selected?.Status}", nameof(MainViewModel));

            if (Selected == null)
                return false;

            return Selected.Status == ConfigClientStatus.Stopped;
        }

        /// <summary>
        /// Command to show the settings window
        /// </summary>
        public RelayCommand ShowSettings { get; }

        /// <summary>
        /// Method to call when the show settings command is used
        /// </summary>
        /// <param name="parameter"></param>
        private async void ShowSettingsMethod(object parameter)
        {
            try
            {
                SettingsWindow sw = new SettingsWindow();
                sw.DataContext = new SettingsWindowViewModel()
                {
                    Settings = Settings.Copy(),
                    Collection = Collection
                };

                if (sw.ShowDialog().Value)
                {
                    //Apply the settings
                    Settings.Apply(((SettingsWindowViewModel)sw.DataContext).Settings);

                    //Send the settings to the server
                    Message msg = await Task.Run(() =>
                    {
                        client.Send(new SettingsMessage() { Settings = Settings });
                        return client.Recive();
                    });

                    if (msg is ResultMessage)
                    {
                        ResultMessage rm = (ResultMessage)msg;

                        //Check result
                        if (rm.Error != 0)
                        {
                            LogManager.Error($"[{rm.Error}] {rm.ErrorMsg}");
                            RaiseErrorEvent(this, null, "!!ERR_GENERIC");
                        }  
                    }
                    else
                    {
                        LogManager.Error($"Unexpexted message type: {msg.Type}");
                        RaiseErrorEvent(this, null, "!!ERR_GENERIC");
                    }
                }
            }
            catch(Exception ex)
            {
                LogManager.Error("Exception in ShowSettingsMethod");
                LogManager.Error(ex);
                RaiseErrorEvent(this, ex, "!!ERR_GENERIC");
            }
        }

        #endregion

        #region Propertys

        /// <summary>
        /// Gets the current running config
        /// </summary>
        public ConfigViewModel RunningConfig { get => runnig; private set { runnig = value; NotifyPropertyChanged("RunningConfig"); } }

        /// <summary>
        /// Gets or sets a value if a config is running
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Property to set or get the current selected ConfigViewModel
        /// </summary>
        public ConfigViewModel Selected 
        { 
            get => selected; 
            set 
            { selected = value; NotifyPropertyChanged("Selected"); } 
        }

        /// <summary>
        /// Gets an sorted list of all loaded configurations
        /// </summary>
        public ConfigCollectionViewModel Collection { get; }

        /// <summary>
        /// Gets or sets the value if the client is currently connecting
        /// </summary>
        public bool IsConnecting
        { 
            get => isConnecting; 
            private set { isConnecting = value; NotifyPropertyChanged("IsConnecting"); } 
        }

        /// <summary>
        /// Gets the current settings of the application
        /// </summary>
        public SettingsViewModel Settings { get => settings; private set { settings = value; NotifyPropertyChanged("Settings"); } }

        /// <summary>
        /// Gets the log
        /// </summary>
        public LogViewModel Log { get => log; }

        /// <summary>
        /// Gets the version of the current assembly
        /// </summary>
        public string Version 
        {
            get
            {
                Version v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

                if (v == null)
                    return "<NA>";

                return $"v{v.Major}.{v.Minor}.{v.Build}";
            }
        }

        /// <summary>
        /// Gets a value if the system is currently processing the settings
        /// </summary>
        public bool ProcessingSettings { get; private set; } = false;

        #endregion
    }
}
