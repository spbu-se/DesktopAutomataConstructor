using ControlsLibrary.Infrastructure.Command;
using ControlsLibrary.Model;
using ControlsLibrary.Properties.Langs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace ControlsLibrary.Controls.TestPanel
{
    /// <summary>
    /// Contains test data and provides interaction logic
    /// </summary>
    public class TestViewModel : INotifyPropertyChanged
    {
        private ICollection<TestViewModel> storage;

        /// <summary>
        /// Storage to remove test from
        /// </summary>
        public ICollection<TestViewModel> Storage
        {
            get => storage;
            set
            {
                storage = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Sets FA executor model to execute strings on it
        /// </summary>
        public FAExecutor Executor { private get; set; }

        public TestViewModel()
        {
            RemoveFromStorageCommand = new RelayCommand(OnRemoveFromStorageCommandExecuted, CanRemoveFromStorageCommandExecute);
            ExecuteCommand = new RelayCommand(OnExecuteCommandExecuted, CanExecuteCommandExecute);
        }

        public ICommand ExecuteCommand { get; private set; }

        private void OnExecuteCommandExecuted(object p)
        {
            try
            {
                Result = Executor.Execute(TestString);
                if (shouldReject && Result == ResultEnum.Passed)
                {
                    Result = ResultEnum.Failed;
                }
                else if (shouldReject && Result == ResultEnum.Failed)
                {
                    Result = ResultEnum.Passed;
                }
            }
            catch (InvalidOperationException e)
            {
                MessageBox.Show(e.Message, Lang.Errors_InvalidAutomaton, MessageBoxButton.OK);
            }
        }

        private bool CanExecuteCommandExecute(object p) => Executor != null;

        /// <summary>
        /// Removes test from the storage
        /// </summary>
        public ICommand RemoveFromStorageCommand { get; private set; }

        private void OnRemoveFromStorageCommandExecuted(object p)
        {
            storage.Remove(this);
            OnPropertyChanged(nameof(Result));
        }

        private bool CanRemoveFromStorageCommandExecute(object p) => storage != null && storage.Contains(this);

        private string testString;

        /// <summary>
        /// String to be executed by the test
        /// </summary>
        public string TestString
        {
            get => testString;
            set
            {
                testString = value;
                OnPropertyChanged();
            }
        }

        private ResultEnum result;

        /// <summary>
        /// Actual result of the execution
        /// </summary>
        public ResultEnum Result
        {
            get => result;
            set
            {
                result = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StringResult));
            }
        }

        /// <summary>
        /// Actual result of the execution converted to the string type
        /// </summary>
        public string StringResult { get => ResultPrinter.PrintResult(Result); }

        private bool shouldReject;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Sets should test reject string or not
        /// </summary>
        public bool ShouldReject
        {
            get => shouldReject;
            set
            {
                shouldReject = value;
                OnPropertyChanged();
            }
        }

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}