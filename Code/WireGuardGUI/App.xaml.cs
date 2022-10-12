using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WireGuard.Core;
using WireGuard.Core.Messages;
using WireGuard.Core.ViewModels;
using WireGuard.GUI.Classes;

namespace WireGuard.GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// ID of the application
        /// </summary>
        const string APP_ID = "Local\\AD8F76C3-F273-4CB5-A697-3377AACB103E";

        /// <summary>
        /// Mutex to prevent second session to start
        /// </summary>
        Mutex mutex;

        /// <summary>
        /// Settings for the application
        /// </summary>
        SettingsViewModel settings = null;

        /// <summary>
        /// Client to connect to the service
        /// </summary>
        Client client;

        /// <summary>
        /// Method gets called before the MainWindow is shown
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            try 
            {
                client = new Client();
                client.Connect();

                client.Send(new SettingsMessage());
                ResultMessage rm = (ResultMessage)client.Recive();

                if(rm.Error == 0)
                {
                    settings = System.Text.Json.JsonSerializer.Deserialize<SettingsViewModel>(rm.ErrorMsg, new System.Text.Json.JsonSerializerOptions() { ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip });
                }
                else
                {
                    throw new Exception($"Error recieving the settings.\n{rm.ErrorMsg}");
                }

                Core.LogManager.Init();
                Core.LogManager.SetLogLvlFromString(settings.LogLevel);
                Core.LogManager.Register(new Core.Log2File("Log.txt", $@"C:\Users\{Environment.UserName}\AppData\Local\Wireguard GUI"));

                mutex = new Mutex(false, APP_ID);

                //Check if the mutex is in use
                if (!mutex.WaitOne(0))
                {
                    ShowApplication();
                    Environment.Exit(0);
                }

                //base.OnStartup(e);

                StartApplication();
            }
            catch (Exception ex)
            {
                Core.LogManager.Error("Error starting the application!");
                Core.LogManager.Error(ex);

                MessageBox.Show("Error starting the application!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Mehtod gets called when the application is closing
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            mutex.Dispose();
        }

        /// <summary>
        /// Method to start the application. 
        /// It will create a lock file in ProgramData wich a second instance can read.
        /// In this file will be the name of the owner of the application and
        /// a handel to the anonymous pipe to communicate with it.
        /// </summary>
        private void StartApplication()
        {
            Core.LogManager.Debug($"Setting-Debug-Level: {settings.LogLevel}", nameof(App));
            Core.LogManager.Debug($"Applied-Debug-Level: {Enum.GetName(typeof(Core.LogLevel), Core.LogManager.LogLevel)}", nameof(App));

            string user = Environment.UserName;

            Directory.CreateDirectory(Core.Classes.Path.DATA_FOLDER);
            File.WriteAllText(Core.Classes.Path.LOCK_FILE, $"{user}");

            MainWindow mw = new MainWindow(client, settings);
            mw.Show();
        }

        /// <summary>
        /// Method to bring the application in to the foreground
        /// </summary>
        private void ShowApplication()
        {
            if(!File.Exists(Core.Classes.Path.LOCK_FILE))
            {
                MessageBox.Show(Res.GetStr("ERR_CLIENT_RUNNING"));
                Environment.Exit(0);
            }
            
            string[] data = File.ReadAllLines(Core.Classes.Path.LOCK_FILE);
            string user = data[0];

            if(user != Environment.UserName)
            {
                MessageBox.Show(Res.GetStr("ERR_CLIENT_IN_USE").Replace("::USER::", user));
                Environment.Exit(0);
            }

            IntPtr ptr = User32.FindWindowByCaption(IntPtr.Zero, "RHRK Wireguard Client");
            bool result = User32.ShowWindow(ptr, User32.WindowState.SHOW_NORMAL);
            User32.SetForegroundWindow(ptr);

            Environment.Exit(0);
        }

        /// <summary>
        /// Handler for unexpected crashes of the application
        /// </summary>
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Core.LogManager.Error("Unhandeld exception!");
            Core.LogManager.Error(e.Exception);

            MessageBox.Show(Res.GetStr("ERR_UNHANDELD_EX"), Res.GetStr("ERR_MB_UNHANDELD"), MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
