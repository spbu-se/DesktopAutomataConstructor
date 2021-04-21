using ControlsLibrary.Infrastructure.Command;
using ControlsLibrary.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ControlsLibrary.Controls.Executor;
using QuickGraph;

namespace ControlsLibrary.Controls.TestPanel
{
    public class TestViewModel : INotifyPropertyChanged
    {
        private ICollection<TestViewModel> storage;

        private BidirectionalGraph<NodeViewModel, EdgeViewModel> graph;
        public BidirectionalGraph<NodeViewModel, EdgeViewModel> Graph
        {
            get => graph;
            set
            {
                graph = value;
            }
        }

        public TestViewModel(ICollection<TestViewModel> storage, BidirectionalGraph<NodeViewModel, EdgeViewModel> graph)
        {
            this.storage = storage;
            RemoveFromStorageCommand = new RelayCommand(OnRemoveFromStorageCommandExecuted, CanRemoveFromStorageCommandExecute);
            ExecuteCommand = new RelayCommand(OnExecuteCommandExecuted, CanExecuteCommandExecute);
            Graph = graph;
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

        private void OnRemoveFromStorageCommandExecuted(object p) => storage.Remove(this);

        private bool CanRemoveFromStorageCommandExecute(object p) => storage.Contains(this);

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
