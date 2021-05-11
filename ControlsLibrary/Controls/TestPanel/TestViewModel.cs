using ControlsLibrary.Controls.Executor;
using ControlsLibrary.Infrastructure.Command;
using ControlsLibrary.Model;
using ControlsLibrary.ViewModel;
using QuickGraph;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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

        private BidirectionalGraph<NodeViewModel, EdgeViewModel> graph;

        /// <summary>
        /// 
        /// </summary>
        public BidirectionalGraph<NodeViewModel, EdgeViewModel> Graph
        {
            get => graph;
            set
            {
                graph = value;
                OnPropertyChanged();
            }
        }

        public TestViewModel()
        {
            RemoveFromStorageCommand = new RelayCommand(OnRemoveFromStorageCommandExecuted, CanRemoveFromStorageCommandExecute);
            ExecuteCommand = new RelayCommand(OnExecuteCommandExecuted, CanExecuteCommandExecute);
        }

        public ICommand ExecuteCommand { get; set; }

        private void OnExecuteCommandExecuted(object p)
        {
            var executor = new ExecutorViewModel();
            executor.InputString = TestString;
            executor.Graph = Graph;
            executor.RunCommand.Execute(new object());
            Result = executor.Result;
            if (shouldReject && Result == ResultEnum.Passed)
            {
                Result = ResultEnum.Failed;
            }
            else if (shouldReject && Result == ResultEnum.Failed)
            {
                Result = ResultEnum.Passed;
            }
        }

        private bool CanExecuteCommandExecute(object p) => true;

        public ICommand RemoveFromStorageCommand { get; set; }

        private void OnRemoveFromStorageCommandExecuted(object p)
        {
            storage.Remove(this);
            OnPropertyChanged("Result");
        }

        private bool CanRemoveFromStorageCommandExecute(object p) => storage != null && storage.Contains(this);

        private string testString;

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

        public ResultEnum Result
        {
            get => result;
            set
            {
                result = value;
                OnPropertyChanged();
                OnPropertyChanged(StringResult);
            }
        }

        public string StringResult { get => ResultPrinter.PrintResult(Result); }

        private bool shouldReject;

        public event PropertyChangedEventHandler PropertyChanged;

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
