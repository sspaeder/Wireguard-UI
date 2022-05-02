using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WireGuard.GUI.ViewModels;

namespace WireGuard.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel mvm;

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            Application.Current.DispatcherUnhandledException += DispatchUnhandledException;

            InitializeComponent();

            mvm = new MainViewModel(this);
            mvm.ErrorOccured += ErrorOccured;

            DataContext = mvm;

            Loaded += WindowLoaded;
            Closing += WindowClosing;
            StateChanged += WindowStateChanged;
        }

        /// <summary>
        /// Handler when the window state changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void WindowStateChanged(object sender, EventArgs e)
        {
            if(WindowState != WindowState.Minimized)
                ShowInTaskbar = true;
        }

        /// <summary>
        /// Checks if the requierments are fullfiled to run this application
        /// </summary>
        /// <returns></returns>
        private bool SanityCheck()
        {
#if DEBUG
            return true;
#endif

            ServiceController services = ServiceController.GetServices().FirstOrDefault(x => x.ServiceName == Core.Path.WIREGUARD_GUI_SERVICE);

            if(services == null)
                return false;

            return services.Status == ServiceControllerStatus.Running;
        }

        #region Event Handling

        /// <summary>
        /// Handler mehtod to get run if the window is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            if (SanityCheck())
                mvm.ConnectAsync();
            else
            {
                Core.LogManager.Error($"Service {Core.Path.WIREGUARD_GUI_SERVICE} not runnig or not installed.");
                MessageBox.Show(Res.GetStr("LBL_SERVICE_ERROR"));
            }
        }

        /// <summary>
        /// Handler mehto to disconnect befor closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(mvm.Collection.Configs.Any(x => x.Status == Core.ConfigClientStatus.Running))
            {
                WindowState = WindowState.Minimized;
                ShowInTaskbar = false;
                string displayMsg = Res.GetStr("INFO_VPN_STILL_RUNNING");

                ///<remarks>
                /// BalloonTip will not show up if the windows configuration 
                /// "Get notifications from apps and other senders" is deaktivated. 
                /// This prevents that in the TrayBar infos are popping up.
                ///</remarks>
                ///<see href="https://stackoverflow.com/questions/42444541/showballoontip-not-working"/>
                myNotifyIcon.ShowBalloonTip("WireGuard", displayMsg, Hardcodet.Wpf.TaskbarNotification.BalloonIcon.None);

                e.Cancel = true;
            }
            else
            {
                mvm.Disconnect();
            }
        }

        /// <summary>
        /// Handler for errors in the collction
        /// </summary>
        /// <param name="sender">object who called the event</param>
        /// <param name="ex">Exception that got raised</param>
        /// <param name="text">Error message</param>
        private void ErrorOccured(Core.ViewModels.BaseViewModel sender, Exception ex, string text)
        {
            if (text.StartsWith("!!"))
            {
                string msg = (string)App.Current.Resources[text.Substring(2)];

                if (msg == null)
                {
                    Core.LogManager.Information($"Unkown label: {text.Substring(2)}");
                    msg = Res.GetStr("ERR_GENERIC");
                }

                if(ex is not System.TimeoutException)
                {
                    Core.LogManager.Debug($"Error msg key: {msg} / Submitted key: {text.Substring(2)}", nameof(MainWindow));
                    Core.LogManager.Error(ex);
                }
                
                MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                if (ex is System.TimeoutException)
                    Close();
            }
            else
                MessageBox.Show(text, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Function to dispatch an unhadeld exception that occures in the code
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DispatchUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Core.LogManager.Error("An unhadeld exception has occured.");
            Core.LogManager.Error(e.Exception);

            MessageBox.Show(Res.GetStr("ERR_GENERIC"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        #endregion
    }
}
