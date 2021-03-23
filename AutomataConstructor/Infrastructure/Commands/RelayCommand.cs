using AutomataConstructor.Infrastructure.Commands.Base;
using System;

namespace AutomataConstructor.Infrastructure.Commands
{
    /// <summary>
    /// A command that can be instantiated passing the actions to be executed 
    /// </summary>
    internal class RelayCommand : Command
    {
        private readonly Action<object> execute;
        private readonly Func<object, bool> canExecute;

        /// <summary>
        /// Basic constructor provides to set an action to execute by command and function that tells if action can be executed
        /// </summary>
        /// <param name="execute">Action to be executed</param>
        /// <param name="canExecute">Function that tells if action can be executed</param>
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }


        public override bool CanExecute(object parameter) => canExecute?.Invoke(parameter) ?? true;

        public override void Execute(object parameter) => execute(parameter);
    }
}
