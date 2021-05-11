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
    /// <summary>
    /// Contains test panel data and provides non-visual interaction logic
    /// </summary>
    public class TestPanelViewModel : BaseViewModel
    {
        public FAExecutor Executor { private get; set; }

        public TestPanelViewModel()
        {
            AddTestCommand = new RelayCommand(OnAddTestCommandExecuted, CanAddTestCommandExecute);
            HideCommand = new RelayCommand(OnHideCommandExecuted, CanHideCommandExecute);
            RunAllTestsCommand = new RelayCommand(OnRunAllTestsCommandExecuted, CanRunAllTestsCommandExecute);
            Tests = new ObservableCollection<TestViewModel>();
        }

        /// <summary>
        /// Saves tests into the file by the given path
        /// </summary>
        /// <param name="path">Path of the file to save</param>
        public void Save(string path)
        {
            var data = new List<TestSerializationData>();
            foreach (var test in Tests)
            {
                data.Add(new TestSerializationData() { Result = test.Result, ShouldReject = test.ShouldReject, TestString = test.TestString });
            }
            FileServiceProviderWpf.SerializeDataToFile(path, data);
        }

        /// <summary>
        /// Deserializes tests data fromthe file by the given path
        /// </summary>
        /// <param name="path">Path of the file to open</param>
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

        /// <summary>
        /// Number of tests with positive actual result
        /// </summary>
        public int NumberOfPassedTests { get => Tests.Count(test => test.Result == ResultEnum.Passed); }

        /// <summary>
        /// Number of tests with negative actual result
        /// </summary>
        public int NumberOfFailedTests { get => Tests.Count(test => test.Result == ResultEnum.Failed); }

        /// <summary>
        /// Number of tests which was not runned
        /// </summary>
        public int NumberOfNotRunnedTests { get => Tests.Count(test => test.Result == ResultEnum.NotRunned); }

        /// <summary>
        /// Hides test panel data
        /// </summary>
        public ICommand HideCommand { get; }

        private void OnHideCommandExecuted(object p) => IsHidden = !IsHidden;

        private bool CanHideCommandExecute(object p) => true;

        private bool isHidden = true;

        /// <summary>
        /// Gets or sets hidding mode
        /// </summary>
        public bool IsHidden { get => isHidden; set => Set(ref isHidden, value); }

        /// <summary>
        /// Executes all tests
        /// </summary>
        public ICommand RunAllTestsCommand { get; set; }

        private void OnRunAllTestsCommandExecuted(object p)
        {
            Parallel.ForEach(Tests, (test) => test.ExecuteCommand.Execute(this));
        }

        private bool CanRunAllTestsCommandExecute(object p)
            => Tests != null;

        /// <summary>
        /// Collection of tests data
        /// </summary>
        public ObservableCollection<TestViewModel> Tests { get; set; }

        /// <summary>
        /// Adds new tests to the collection
        /// </summary>
        public ICommand AddTestCommand { get; set; }

        private bool CanAddTestCommandExecute(object p) => true;

        /// <summary>
        /// Handles changing of result in some test view nodel
        /// </summary>
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
            var newTest = new TestViewModel() { Result = ResultEnum.NotRunned, Executor = this.Executor, Storage = Tests };
            newTest.PropertyChanged += UpdateStorage;
            Tests.Add(newTest);
            OnPropertyChanged("NumberOfPassedTests");
            OnPropertyChanged("NumberOfFailedTests");
            OnPropertyChanged("NumberOfNotRunnedTests");
        }

        private void AddTest(ResultEnum result, string testString, bool shouldReject)
        {
            Tests.Add(new TestViewModel()
            {
                Result = result,
                TestString = testString,
                ShouldReject = shouldReject,
                Storage = Tests,
                Executor = this.Executor
            });

            OnPropertyChanged("NumberOfPassedTests");
            OnPropertyChanged("NumberOfFailedTests");
            OnPropertyChanged("NumberOfNotRunnedTests");
        }
    }
}
