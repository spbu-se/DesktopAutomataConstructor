using System;
using System.Windows.Input;

namespace AutomataConstructor.Infrastructure.Commands.Base
{
    /// <summary>
    /// The base class of command
    /// </summary>
    internal abstract class Command : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public abstract bool CanExecute(object parameter);

        public abstract void Execute(object parameter);
    }
}
