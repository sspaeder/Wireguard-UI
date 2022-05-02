using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using WireGuard.Core;
using WireGuard.Core.Messages;
using WireGuard.Core.PlugIn;
using WireGuard.WireGuardUIService.Classes;

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
        /// Controller for the plugins
        /// </summary>
        PlugInController<IServerPlugIn> plugIns;

        #region BackgroundService Methods

        /// <summary>
        /// Construcotr
        /// </summary>
        /// <param name="logger">logging interface</param>
        public Worker(ILogger<Worker> logger)
        {
            //Initalize the configuration of the service
            Config.Init();

            LogManager.Init();
            LogManager.SetLogLvlFromString(Config.Settings.LogLevel);
            LogManager.Register(new Log2File("Log.txt", System.Reflection.Assembly.GetExecutingAssembly().Location.Replace(@"\WireGuardUIService.dll", "")));
            LogManager.Register(new ServiceLogger(logger));

            if (Config.Settings.LogLevel == "Debug")
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
                plugIns = new PlugInController<IServerPlugIn>();

                if (System.IO.Directory.Exists(Path.PLUGIN_FOLDER))
                {
                    foreach (string dll in System.IO.Directory.GetFiles(Path.PLUGIN_FOLDER, "*.dll"))
                        plugIns.Load(dll);
                }
                else
                    System.IO.Directory.CreateDirectory(Path.PLUGIN_FOLDER);

                server = new Server();

                LogManager.Information("WireGuardUIService startet");

                Operations.wgIpcApi = WgIpcFactory.Create(Config.Settings.WgIpcApi);

                //Restores the session
                //Prevents service clearing
                if (!Config.Settings.RestoreSession)
                    Operations.ClearTunnel();

                //Starts the service on boot
                if (Config.Settings.StartOnBoot)
                    Operations.ConnectToNetworkChange(Config.Settings.StartConfigName);
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

                //Sends the status of wireguard to the client
                ComManager.ServiceStatus(server);

                LogManager.Debug("Status message send", nameof(Worker));

                //Message loop
                while (true)
                {
                    // Check if wg service is running or not
                    Operations.CheckWGManagerService();

                    Message msg = server.Recive();

                    if (msg == null)
                        break;

                    //Switch to the message tpyes
                    switch(msg)
                    {
                        case StartMessage sm when msg is StartMessage:
                            ComManager.Start(server, sm);
                            break;

                        case StopMessage sm when msg is StopMessage:
                            ComManager.Stop(server, sm);
                            break;

                        case InterfaceStatus istatus when msg is InterfaceStatus:
                            ComManager.InterfaceStatus(server, istatus);
                            break;

                        case ImportMessage im when msg is ImportMessage:
                            ComManager.Import(server, im);
                            break;

                        case RemoveMessage rm when msg is RemoveMessage:
                            ComManager.Remove(server, rm);
                            break;

                        case SettingsMessage sm when msg is SettingsMessage:
                            ComManager.SetSettings(server, sm);
                            break;

                        case LogContentMessage lcm when msg is LogContentMessage:
                            ComManager.GetWgLogContent(server, lcm);
                            break;

                        case PlugInMessage pMsg when msg is PlugInMessage:
                            IServerPlugIn plugIn = plugIns.Find(x => x.Name == pMsg.Target);
                            plugIn?.HandelMessage(server, pMsg);
                            break;

                        //No matching message type is found (should not appear)
                        default:
                            LogManager.Error($"No matching message method for message of type {msg.Type}");
                            break;
                    }
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

        #endregion
    }
}
