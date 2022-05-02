using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireGuard.Core.ViewModels
{
    /// <summary>
    /// Class for the settings aviable
    /// </summary>
    public class SettingsViewModel : BaseViewModel
    {
        #region Variables

        /// <summary>
        /// 
        /// </summary>
        static SettingsViewModel current;

        /// <summary>
        /// Variable for the start on boot setting
        /// </summary>
        bool startOnBoot;

        /// <summary>
        /// Variabel for the start on logon setting
        /// </summary>
        bool startOnLogon;

        #endregion

        #region Static Methods

        /// <summary>
        /// Method to load the settings
        /// </summary>
        /// <returns></returns>
        public static SettingsViewModel LoadSettings() => System.Text.Json.JsonSerializer.Deserialize<SettingsViewModel>(System.IO.File.ReadAllText(Path.SETTINGS));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        public static void SetCurrentSettings(SettingsViewModel settings)
        {
            if (current == null)
                current = settings;
        }

        /// <summary>
        /// Method returns the current <see cref="SettingsViewModel"/>
        /// </summary>
        /// <returns><see cref="SettingsViewModel"/></returns>
        public static SettingsViewModel GetCurrent() => current;

        #endregion

        #region Methods

        /// <summary>
        /// Creates a copy of the settings
        /// </summary>
        /// <returns></returns>
        public SettingsViewModel Copy() => new SettingsViewModel()
        {
            StartOnBoot = this.StartOnBoot,
            StartConfigName = this.StartConfigName,
            DefaultConfig = this.DefaultConfig,
            RestoreSession = this.RestoreSession
        };

        /// <summary>
        /// Applys the settings
        /// </summary>
        /// <param name="svm">Settigns to apply</param>
        public void Apply(SettingsViewModel svm)
        {
            //StartOnBoot
            StartOnBoot = svm.StartOnBoot;

            if (StartOnBoot)
                StartConfigName = svm.StartConfigName;
            else
                StartConfigName = null;

            //DefualtConfig
            DefaultConfig = svm.DefaultConfig;

            //RestoreSession
            RestoreSession = svm.RestoreSession;
        }

        #endregion

        #region Propertys

        //Userspace are all config items that the user can change
        //They will be displayed in the settings editor
        #region Userspace

        /// <summary>
        /// Gets or sets the value if the VPN-Tunnel should be established druing startup
        /// </summary>
        public bool StartOnBoot { get => startOnBoot; set { startOnBoot = value; NotifyPropertyChanged(); } }

        /// <summary>
        /// Gets or sets the name of the config-file that should started during startup
        /// </summary>
        public string StartConfigName { get; set; }

        /// <summary>
        /// Gets or sets the defualt config to be selected/started
        /// </summary>
        public string DefaultConfig { get; set; }

        /// <summary>
        /// Gets or sets a value if the session should be restored after shutdown/unexpecting evets...
        /// </summary>
        public bool RestoreSession { get; set; }

        #endregion

        //Adminspace config items will only be avaiable to the administrator
        //They will not show in the settings editor and must be enterd by hand in the config file
        #region Adminspace

        /// <summary>
        /// Returns a value of the heigth of the log
        /// </summary>
        /// <remarks>Valid values are: Debug, Info, Warning, Error</remarks>
        public string LogLevel { get; set; } = "Info";

        /// <summary>
        /// Gets or sets the timer interval in milliseconds for refreshing the status
        /// </summary>
        public double TimerInterval { get; set; } = 5 * 1000;

        /// <summary>
        /// Gets or sets a value if the GUI should run in kiosk mode or not
        /// </summary>
        public bool RunInKioskMode { get; set; }

        /// <summary>
        /// Gets or sets a value for user aviable settings
        /// </summary>
        /// <remarks>
        /// * | NONE | RunInKisokMode | DefaultConfig ....
        /// </remarks>
        public string[] UserAvailableSettings { get; set; }

        /// <summary>
        /// Gets or sets the prefeerd wireguard ipc api for tunnel informations
        /// </summary>
        /// <remarks>
        /// If the value is NULL the default IPC API will be choosen
        /// </remarks>
        public string WgIpcApi { get; set; }

        #endregion

        #endregion
    }
}
