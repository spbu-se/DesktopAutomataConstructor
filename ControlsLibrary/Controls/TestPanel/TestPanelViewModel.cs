using ControlsLibrary.FileSerialization;
using ControlsLibrary.Infrastructure.Command;
using ControlsLibrary.Model;
using ControlsLibrary.ViewModel;
using ControlsLibrary.ViewModel.Base;
using QuickGraph;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using YAXLib;

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

        public void Save(string path)
        {
            var data = new List<TestSerializationData>();
            foreach (var test in Tests)
            {
                data.Add(new TestSerializationData() { Result = test.Result, ShouldReject = test.ShouldReject, TestString = test.TestString });
            }
            FileServiceProviderWpf.SerializeDataToFile(path, data);
        }

        public void Open(string path)
        {
            Tests.Clear();
            using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var deserializer = new YAXSerializer(typeof(List<TestSerializationData>));
                using (var textReader = new StreamReader(stream))
                {
                    var datas = (List<TestSerializationData>)deserializer.Deserialize(textReader);
                    foreach (var test in datas)
                    {
                        AddTest(test.Result, test.TestString, test.ShouldReject);
                    }
                }
            }
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
