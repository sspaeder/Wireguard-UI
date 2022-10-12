using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Interop;
using WireGuard.Core;
using WireGuard.Core.Classes;
using WireGuard.Core.Messages;
using WireGuard.Core.PlugIn;
using WireGuard.WireGuardUIService.Classes;
using WireGuard.WireGuardUIService.Handler;

namespace WireGuard.WireGuardUIService
{
    /// <summary>
    /// Class to do the work. This is the service
    /// </summary>
    public class Worker : BackgroundService
    {
        /// <summary>
        /// The server that handels the requests
        /// </summary>
        private Server server;

        /// <summary>
        /// Variable for the server to shutdown properly
        /// </summary>
        private bool shutdown = false;

        /// <summary>
        /// Context of the service
        /// </summary>
        Context context;

        #region BackgroundService Methods

        /// <summary>
        /// Construcotr
        /// </summary>
        /// <param name="logger">logging interface</param>
        public Worker(ILogger<Worker> logger)
        {
            // Close all running sessions from the UI
            foreach (System.Diagnostics.Process p in System.Diagnostics.Process.GetProcessesByName("WireGuardGUI"))
                p.Close();

            // Clear the lock file
            if(System.IO.File.Exists(Path.LOCK_FILE))
                System.IO.File.Delete(Path.LOCK_FILE);

            //Initalize the configuration of the service
            context = new Context();

            // Initilaize the logmanager
            LogManager.Init();
            LogManager.Register(new Log2File("Log.txt", System.AppContext.BaseDirectory));
            LogManager.Register(new ServiceLogger(logger));

            if (context.Settings == null)
            {
                string err = $"settings.json not found on path: {Core.Classes.Path.SETTINGS}";
                LogManager.Error("Error in WireGuardUIService");
                LogManager.Error(err);
                throw new Exception();
            }

            LogManager.SetLogLvlFromString(context.Settings.LogLevel);

            // Check loglevel
            if (context.Settings.LogLevel == "Debug")
                LogManager.Information("LogLevel is set to debug!");
        }

        /// <summary>
        /// Method to start the service
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="=Task"/></returns>
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                server = new Server();

                LogManager.Information("WireGuardUIService startet");

                Operations.wgIpcApi = WgIpcFactory.Create(context.Settings.WgIpcApi);

                //Restores the session
                //Prevents service clearing
                if (!context.Settings.RestoreSession)
                    Operations.ClearTunnel();

                //Starts the service on boot
                if (context.Settings.StartOnBoot)
                    Operations.ConnectToNetworkChange(context.Settings.StartConfigName);
            }
            catch (Exception ex)
            {
                LogManager.Error("Error in WireGuardUIService");
                LogManager.Error(ex);
            }
            
            return base.StartAsync(cancellationToken);
        }

        /// <summary>
        /// Method that does the work
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns><see cref="=Task"/></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            LogManager.Debug("Entering execution routine", nameof(Worker));
            
            await Task.Run(() => 
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    ServerLoop();
                }
            });
        }

        /// <summary>
        /// Method to end the service
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="=Task"/></returns>
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() => {
                shutdown = true;
                
                if(server != null)
                    server.Close();
            });
        }

        /// <summary>
        /// Closes the server
        /// </summary>
        public override void Dispose()
        {
            server?.Dispose();
        }

        #endregion

        #region Worker Mehtods

        /// <summary>
        /// Method to recive and process client messages
        /// Only one client is allowed to connect to the server!
        /// </summary>
        private void ServerLoop()
        {
            try
            {
                LogManager.Debug("Waiting for connection", nameof(Worker));

                server.Start();

                LogManager.Debug("Client connected", nameof(Worker));

                LogManager.Debug("Status message send", nameof(Worker));

                //Message loop
                while (true)
                {
                    // Check if wg service is running or not
                    Operations.CheckWGManagerService();

                    Message msg = server.Recive();

                    if (msg == null)
                        break;

                    LogManager.Debug($"Message of type {msg.Type} recieved", nameof(Worker), nameof(ServerLoop));

                    MessageHandler handler = context.Handler.FirstOrDefault(x => x.Type == msg.GetType());

                    if (handler != null)
                        handler.Handel(server, msg);
                    else
                        HandelUnkownMessage(msg);
                }
            }
            catch(Exception ex)
            {
                if (shutdown == false)
                {
                    LogManager.Error("Error in WireGuardUIService");
                    LogManager.Error(ex);
                }
                    
            }
            finally
            {
                if (server != null)
                    server.Disconnect();
            }
        }

        private void HandelUnkownMessage(Message msg)
        {
            string err = $"No matching MessageHandler for message of type {msg.GetType().Name}";
            LogManager.Error(err);
            server.Send(new ResultMessage() { Error = -1, ErrorMsg = err });

        }

        #endregion
    }
}
