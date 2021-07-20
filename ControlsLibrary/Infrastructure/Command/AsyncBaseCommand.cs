using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ControlsLibrary.Infrastructure.Command
{
    public abstract class AsyncBaseCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
        public abstract bool CanExecute(object parameter);
        
        public abstract void Execute(object parameter);
        
        protected abstract Task ExecuteAsync(object parameter);
    }
}