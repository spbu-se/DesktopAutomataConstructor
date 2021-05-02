using ControlsLibrary.Infrastructure.Command;
using ControlsLibrary.ViewModel.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ControlsLibrary.Model;
using QuickGraph;
using System.Threading.Tasks;
using System.Linq;
using System.ComponentModel;

namespace ControlsLibrary.Controls.TestPanel
{
    public class TestPanelViewModel : BaseViewModel
    {
        public TestPanelViewModel()
        {
            AddTestCommand = new RelayCommand(OnAddTestCommandExecuted, CanAddTestCommandExecute);
            HideCommand = new RelayCommand(OnHideCommandExecuted, CanHideCommandExecute);
            RunAllTestsCommand = new RelayCommand(OnRunAllTestsCommandExecuted, CanRunAllTestsCommandExecute);
            Tests = new ObservableCollection<TestViewModel>();
        }

        private BidirectionalGraph<NodeViewModel, EdgeViewModel> graph;

        public BidirectionalGraph<NodeViewModel, EdgeViewModel> Graph
        {
            get => graph;
            set
            {
                Set(ref graph, value);
                foreach (var test in Tests)
                {
                    test.Graph = graph;
                }
            }
        }

        public int NumberOfPassedTests { get => Tests.Count(test => test.Result == ResultEnum.Passed); }

        public int NumberOfFailedTests { get => Tests.Count(test => test.Result == ResultEnum.Failed); }

        public int NumberOfNotRunnedTests { get => Tests.Count(test => test.Result == ResultEnum.NotRunned); }

        public ICommand HideCommand { get; }

        private void OnHideCommandExecuted(object p) => IsHidden = !IsHidden;

        private bool CanHideCommandExecute(object p) => true;

        private bool isHidden = true;

        public bool IsHidden { get => isHidden; set => Set(ref isHidden, value); }

        public ICommand RunAllTestsCommand { get; set; }

        private void OnRunAllTestsCommandExecuted(object p)
        {
            Parallel.ForEach(Tests, (test) => test.ExecuteCommand.Execute(this));
        }

        private bool CanRunAllTestsCommandExecute(object p)
            => Tests != null;

        public ObservableCollection<TestViewModel> Tests { get; set; }

        public ICommand AddTestCommand { get; set; }

        private bool CanAddTestCommandExecute(object p) => true;

        private void UpdateStorage(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Result")
            {
                OnPropertyChanged("NumberOfPassedTests");
                OnPropertyChanged("NumberOfFailedTests");
                OnPropertyChanged("NumberOfNotRunnedTests");
            }
        }

        private void OnAddTestCommandExecuted(object p)
        {
            var newTest = new TestViewModel() { Result = ResultEnum.NotRunned, Graph = graph, Storage = Tests };
            newTest.PropertyChanged += UpdateStorage;
            Tests.Add(newTest);
            OnPropertyChanged("NumberOfPassedTests");
            OnPropertyChanged("NumberOfFailedTests");
            OnPropertyChanged("NumberOfNotRunnedTests");
        }

        public void AddTest(ResultEnum result, string testString, bool shouldReject)
        {
            Tests.Add(new TestViewModel()
            {
                Result = result,
                TestString = testString,
                ShouldReject = shouldReject,
                Storage = Tests,
                Graph = graph
            });

            OnPropertyChanged("NumberOfPassedTests");
            OnPropertyChanged("NumberOfFailedTests");
            OnPropertyChanged("NumberOfNotRunnedTests");
        }
    }
}
