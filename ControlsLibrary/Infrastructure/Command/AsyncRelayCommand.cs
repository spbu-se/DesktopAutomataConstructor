using System;
using System.Threading.Tasks;

namespace ControlsLibrary.Infrastructure.Command
{
    public class AsyncRelayCommand : AsyncBaseCommand
    {
        private readonly Func<object, Task> execute;
        private readonly Func<object, bool> canExecute;
        
        private readonly Action<Exception> onException;

        private bool isExecuting;
        public bool IsExecuting
        {
            get => isExecuting;
            set => isExecuting = value;
        }

        public AsyncRelayCommand(Func<object, Task> execute, Action<Exception> onException, Func<object, bool> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
            this.onException = onException;
        }

        public override bool CanExecute(object parameter) => (canExecute?.Invoke(parameter) ?? true) && !IsExecuting;

        public override async void Execute(object parameter)
        {
            IsExecuting = true;
            try
            {
                await ExecuteAsync(parameter);
            }
            catch (Exception ex)
            {
                onException?.Invoke(ex);
            }
            finally
            {
                IsExecuting = false;
            }
        }

        protected override Task ExecuteAsync(object parameter) => execute(parameter);
        
        protected override Task UndoCommand(object parameter) => execute(parameter);
    }
}