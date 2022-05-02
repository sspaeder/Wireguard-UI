using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using WireGuard.Core.Classes;
using WireGuard.Core.Messages;

namespace WireGuard.Core.ViewModels
{
    /// <summary>
    /// Class for representation of an existing config
    /// </summary>
    public class ConfigViewModel : BaseViewModel
    {
        #region Events & Delegates

        /// <summary>
        /// Method handler if a status changed occuerd
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="status">New status of the config</param>
        public delegate void StatusChangedEventHandler(ConfigViewModel sender, ConfigClientStatus status);

        /// <summary>
        /// Status changed event gets raised when the status changed
        /// </summary>
        public event StatusChangedEventHandler StatusChanged;

        #endregion

        #region Variables

        /// <summary>
        /// Config collection the object is associated with
        /// </summary>
        ConfigCollectionViewModel ccvm;

        /// <summary>
        /// Variable to store a client object
        /// </summary>
        Client client;

        /// <summary>
        /// Variable for the status of the client
        /// </summary>
        ConfigClientStatus status = ConfigClientStatus.Stopped;

        /// <summary>
        /// Timer for refreshing the status
        /// </summary>
        Timer timer;

        /// <summary>
        /// Variable if the timer is currently processing
        /// </summary>
        bool timerIsProcessing = false;

        /// <summary>
        /// Intervall time in milliseconds on which the timer event ocuers
        /// </summary>
        double timerInterval = 5 * 1000;

        #endregion

        #region Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="model">Colletction of configs</param>
        /// <param name="client">client who is connected to the server</param>
        /// <param name="name">Name of the configuration</param>
        public ConfigViewModel(ConfigCollectionViewModel model, Client client, string name)
        {
            //Initialize data
            ccvm = model;
            this.client = client;
            Name = name;
            DisplayName = name.Remove(name.IndexOf('.'));

            timer = new Timer(timerInterval);
            timer.Elapsed += RefreshStatus;
            timer.AutoReset = true;

            //Initialize commands
            StartStopCmd = new RelayCommand(StartStopMethodAsync, StartStopPredicate);
        }

        /// <summary>
        /// Handler for an Timer elapsed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RefreshStatus(object sender, ElapsedEventArgs e)
        {
            if (timerIsProcessing || Status == ConfigClientStatus.Stopped)
                return;

            timerIsProcessing = true;

            try
            {
                //Get the new status information of the current connection
                Message msg = await Task.Run(() => 
                {
                    client.Send(new InterfaceStatus() { Interface = Name });
                    return client.Recive();
                });

                if (msg is InterfaceStatus) //Expected message type
                {
                    Statistics.Apply((InterfaceStatus)msg);
                    NotifyPropertyChanged("Statistics");
                }
                else if (msg is ResultMessage) //Comes up when an error occured
                {
                    ResultMessage rm = (ResultMessage)msg;

                    if (rm.Error != 0 && Status == ConfigClientStatus.Running)
                    {
                        RaiseErrorEvent(this, null, $"[{rm.Error}] {rm.ErrorMsg}");
                        timer.Stop();
                    }
                }
                else
                {
                    LogManager.Error($"Unexpexted message type: {msg.Type}");
                    RaiseErrorEvent(this, null, "!!ERR_GENERIC");
                }
            }
            catch(Exception ex)
            {
                timer.Stop();
                LogManager.Error("Exception in RemoveConfigMethod");
                LogManager.Error(ex);
                RaiseErrorEvent(this, ex, "!!ERR_GENERIC");
            }
            finally
            {
                timerIsProcessing = false;
            }
        }

        /// <summary>
        /// Updates the status of the connection
        /// </summary>
        public void UpdateStatus() => RefreshStatus(null, null);

        /// <summary>
        /// Returns a representation string
        /// </summary>
        /// <returns></returns>
        public override string ToString() => DisplayName;

        #endregion

        #region Command Methods

        /// <summary>
        /// Command to start the vpn tunnel
        /// </summary>
        public RelayCommand StartStopCmd { get; }

        /// <summary>
        /// Method who gets called when StartCmd is used
        /// </summary>
        /// <param name="parameter"></param>
        private async void StartStopMethodAsync(object parameter)
        {
            if (status == ConfigClientStatus.Pending)
                return;

            ConfigClientStatus saveValue = Status;

            try
            {
                if (Status == ConfigClientStatus.Running) //If service is running then stop
                {
                    Status = ConfigClientStatus.Pending;

                    ResultMessage rm = await Task.Run(() => {
                        client.Send(new StopMessage() { Name = Name });
                        return (ResultMessage)client.Recive();
                    });

                    if (rm.Error != 0)
                        RaiseErrorEvent(this, null, $"[{rm.Error}] {rm.ErrorMsg}");

                    saveValue = ConfigClientStatus.Stopped;
                    Statistics.Clear();
                    NotifyPropertyChanged("Statistics");
                }
                else //If service is not running then start
                {
                    Status = ConfigClientStatus.Pending;

                    Message m = await Task.Run(() => {
                        client.Send(new StartMessage() { File = Name });
                        return client.Recive();
                    });

                    if (m is ResultMessage) // Result message will only occure on failure
                    {
                        ResultMessage rm = (ResultMessage)m;

                        LogManager.Error($"[{rm.Error}] {rm.ErrorMsg}");
                        RaiseErrorEvent(this, null, "!!ERR_GENERIC");
                        saveValue = ConfigClientStatus.Stopped;
                    }
                    else if(m is InterfaceStatus) // On success the status will be displayed
                    {
                        saveValue = ConfigClientStatus.Running;

                        Statistics.Apply((InterfaceStatus)m);
                        NotifyPropertyChanged("Statistics");
                    }
                }
            }
            catch(Exception ex)
            {
                LogManager.Error("Exception in StartStopMethodAsync");
                LogManager.Error(ex);
                RaiseErrorEvent(this, null, "!!ERR_GENERIC");

                saveValue = ConfigClientStatus.Stopped;
            }
            finally
            {
                Status = saveValue;
            }
        }

        /// <summary>
        /// Method to check if the StartStpoCmd can be run
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private bool StartStopPredicate(object parameter) => !ccvm.Configs.Any(x => x.Status == ConfigClientStatus.Running && x.Name != Name);

        #endregion

        #region Propertys
        
        /// <summary>
        /// Gets the name of the config
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the display name of the config
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Gets the value if the vpn tunnel is running or not
        /// </summary>
        public ConfigClientStatus Status 
        { 
            get => status; 
            set 
            { 
                status = value; 
                NotifyPropertyChanged("Status");

                StatusChanged?.Invoke(this, status);

                if (status == ConfigClientStatus.Running && timerInterval > 0)
                    timer.Start();
                else if (status == ConfigClientStatus.Stopped)
                    timer.Stop();
            } 
        }

        /// <summary>
        /// Tunnel statistics
        /// </summary>
        public Statistic Statistics { get; private set; } = new Statistic();

        /// <summary>
        /// Gets or sets the timer intervall
        /// </summary>
        public double TimerInterval
        {
            get => timerInterval;
            set
            {
                timerInterval = value;

                if (timerInterval > 0)
                    timer.Interval = timerInterval;
                else
                    timer.Stop();
            }
        }

        #endregion
    }
}
