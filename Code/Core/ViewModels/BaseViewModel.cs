using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace WireGuard.Core.ViewModels
{
    /// <summary>
    /// Baseclass for a viewmodel
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Delegate for error handling
        /// </summary>
        /// <param name="sender">Object in which the error occured</param>
        /// <param name="ex">Exception we run into</param>
        /// <param name="text">Error text</param>
        public delegate void ErrorOccuredEventHandler(BaseViewModel sender, Exception ex, string text);

        /// <summary>
        /// Event to be fired if an error occured
        /// </summary>
        public event ErrorOccuredEventHandler ErrorOccured;

        /// <summary>
        /// Ereigniss das ausgelöst wird, wenn sich eine Eigenschaft ändert
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Methode zum vereinfachten aufrufen der Eigenschaftsänderung
        /// </summary>
        /// <param name="propertyName">Name der Eigenschaft die sich geändert hat</param>
        protected void NotifyPropertyChanged(string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// Raises the error event
        /// </summary>
        /// <param name="sender">Object in which the error occured</param>
        /// <param name="ex">Exception we run into</param>
        /// <param name="text">Error text</param>
        protected void RaiseErrorEvent(BaseViewModel sender, Exception ex, string text) => ErrorOccured?.Invoke(sender, ex, text);
    }
}
