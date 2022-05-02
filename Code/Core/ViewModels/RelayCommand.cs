using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace WireGuard.Core.ViewModels
{
    /// <summary>
    /// Relaycommand class for easy handel
    /// </summary>
    public class RelayCommand : ICommand
    {
        readonly Action<object> execute;
        readonly Predicate<object> predicate;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="action">Aktion to be executed</param>
        /// <param name="predicate">Condition if the action can be performed</param>
        public RelayCommand(Action<object> action, Predicate<object> predicate)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            this.execute = action;
            this.predicate = predicate;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="action">Aktion to be executed</param>
        public RelayCommand(Action<object> action) : this(action, null)
        {
        }

        /// <summary>
        /// Event gets called when something changes
        /// Gets called automaticlay in WPF all other technologies have to use RaiseCanExecuteChanged
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Checks if the command can run or not
        /// </summary>
        /// <param name="parameter">Argument to check</param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            if (predicate == null)
                return true;

            return predicate.Invoke(parameter);
        }

        /// <summary>
        /// Executes the command method that it encapsules
        /// </summary>
        /// <param name="parameter">Additional parameter to be considerd</param>
        public void Execute(object parameter) =>
            execute.Invoke(parameter);
    }
}
