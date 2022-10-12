using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WireGuard.Core.Messages;
using WireGuard.Core;
using WireGuard.Core.ViewModels;
using WireGuard.GUI.Windows;
using WireGuard.GUI.Classes;
using WireGuard.Core.Classes;

namespace WireGuard.GUI.ViewModels
{
    /// <summary>
    /// ViewModel for the log files
    /// </summary>
    internal class LogViewModel : BaseViewModel
    {
        string _clientLog;
        string _serverLog;
        string _wgLog;

        Client _client;

        /// <summary>
        /// Constructor
        /// </summary>
        public LogViewModel(Client client)
        {
            _client = client;

            RefreshLog = new RelayCommand(RefreshLogMethod);
            ExportLog = new RelayCommand(ExportLogMethod);
            ShowWindow = new RelayCommand(ShowWindowMethod);
        }

        #region Commands

        /// <summary>
        /// Command to refresh the log views
        /// </summary>
        public RelayCommand RefreshLog { get; }

        /// <summary>
        /// Method to call when the RefreshLog command is used
        /// </summary>
        /// <param name="parameter"></param>
        private async void RefreshLogMethod(object parameter)
        {
            if (System.IO.File.Exists(Path.USER_LOG.Replace("<USER>", Environment.UserName)))
                ClientLog = System.IO.File.ReadAllText(Path.USER_LOG.Replace("<USER>", Environment.UserName));
            else
            {
                ClientLog = "No user log found";
                System.IO.File.Create(Path.USER_LOG.Replace("<USER>", Environment.UserName));
            }
                

            if (System.IO.File.Exists(Path.SERVER_LOG))
                ServerLog = System.IO.File.ReadAllText(Path.SERVER_LOG);
            else
                ServerLog = "No server log found";

            ResultMessage rm = await Task.Run(() => {
                _client.Send(new LogContentMessage());
                return (ResultMessage)_client.Recive();
            });

            if (rm.Error != 0)
            {
                RaiseErrorEvent(this, null, rm.ErrorMsg);
                return;
            }

            WireguardLog = rm.ErrorMsg;
        }

        /// <summary>
        /// Command to export the Logfiles to compressed archive
        /// </summary>
        public RelayCommand ExportLog { get; }

        /// <summary>
        /// Method to call when the ExportLog command is used
        /// </summary>
        /// <param name="parameter"></param>
        private async void ExportLogMethod(object parameter)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
                sfd.Filter = "ZIP-Archive|*.zip";
                sfd.FileName = $"WG_Log_{DateTime.Now.ToString("yyyy-MM-dd")}_{Environment.UserName}.zip";

                if (sfd.ShowDialog().Value)
                {
                    ResultMessage rm = await Task.Run(() => {
                        _client.Send(new ExportMessage() { FilePath = sfd.FileName, Username = Environment.UserName });
                        return (ResultMessage)_client.Recive();
                    });


                    if (rm.Error == 0)
                    {
                        MessageBox.Show(Res.GetStr("LBL_EXPORT_SUCCESS"), Res.GetStr("LBL_DB_SUCCESS"), MessageBoxButton.OK);
                        System.Diagnostics.Process.Start("explorer.exe", sfd.FileName.Remove(sfd.FileName.LastIndexOf("\\")));
                    }
                    else
                    {
                        RaiseErrorEvent(this, null, Res.GetStr("ERR_GENERIC"));
                        LogManager.Error($"Unable to export log files [{rm.Error}].\n{rm.ErrorMsg}");
                    }
                }
            }
            catch (Exception ex)
            {
                RaiseErrorEvent(this, ex, "!!ERR_GENERIC");
                LogManager.Error("Error exporting logs");
                LogManager.Error(ex.Message);
            }
        }

        /// <summary>
        /// Command to show the log window
        /// </summary>
        public RelayCommand ShowWindow { get; }

        /// <summary>
        /// Method to handel the ShowWindow command
        /// </summary>
        /// <param name="parameter">Not used</param>
        private void ShowWindowMethod(object parameter)
        {
            LogWindow lw = new LogWindow();
            lw.DataContext = this;
            lw.Show();
            RefreshLogMethod(null);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the text form the client log
        /// </summary>
        public string ClientLog { get => _clientLog; private set { _clientLog = value; NotifyPropertyChanged(nameof(ClientLog)); } }

        /// <summary>
        /// Gets the text from the server log
        /// </summary>
        public string ServerLog { get => _serverLog; private set { _serverLog = value; NotifyPropertyChanged(nameof(ServerLog)); } }

        /// <summary>
        /// Gets the text from the wireguard log
        /// </summary>
        public string WireguardLog { get => _wgLog; private set { _wgLog = value; NotifyPropertyChanged(nameof(WireguardLog)); } }

        #endregion
    }
}
