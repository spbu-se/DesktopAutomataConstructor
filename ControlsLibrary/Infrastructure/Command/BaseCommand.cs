using System;
using System.Windows.Input;

namespace ControlsLibrary.Infrastructure.Command
{
    /// <summary>
    /// The base class of command
    /// </summary>
    public abstract class BaseCommand : ICommand
    {
        /// <summary>
        /// Invokes if can execute was changed
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        /// <summary>
        /// Returns if command can be executed
        /// </summary>
        public abstract bool CanExecute(object parameter);

        /// <summary>
        /// Invokes execution action
        /// </summary>
        public abstract void Execute(object parameter);
    }
}
