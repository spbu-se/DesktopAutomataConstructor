using ControlsLibrary.FileSerialization;
using ControlsLibrary.Infrastructure.Command;
using ControlsLibrary.Model;
using ControlsLibrary.ViewModel.Base;
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
        private FAExecutor executor;

        /// <summary>
        /// Sets FA executor model to execute tests on it
        /// </summary>
        public FAExecutor Executor
        {
            private get => executor;
            set
            {
                executor = value;
                foreach (var test in Tests)
                {
                    test.Executor = executor;
                }
            }
        }

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
        public async void Save(string path)
        {
            var data = new List<TestSerializationData>();
            foreach (var test in Tests)
            {
                data.Add(new TestSerializationData() { Result = test.Result, ShouldReject = test.ShouldReject, TestString = test.TestString });
            }
            await FileServiceProviderWpf.SerializeDataToFile(path, data);
        }

        /// <summary>
        /// Deserializes tests data from the file by the given path
        /// </summary>
        /// <param name="path">Path of the file to open</param>
        public void Open(string path)
        {
            Tests.Clear();
            using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var textReader = new StreamReader(stream);
            var deserializer = new YAXSerializer(typeof(List<TestSerializationData>));
            foreach (var test in (List<TestSerializationData>)deserializer.Deserialize(textReader))
            {
                AddTest(test.Result, test.TestString, test.ShouldReject);
            }
        }

        /// <summary>
        /// Number of tests with positive actual result
        /// </summary>
        public int NumberOfPassedTests => Tests.Count(test => test.Result == ResultEnum.Passed);

        /// <summary>
        /// Number of tests with negative actual result
        /// </summary>
        public int NumberOfFailedTests => Tests.Count(test => test.Result == ResultEnum.Failed);

        /// <summary>
        /// Number of tests which was not runned
        /// </summary>
        public int NumberOfNotRunnedTests => Tests.Count(test => test.Result == ResultEnum.NotRunned);

        /// <summary>
        /// Hides test panel data
        /// </summary>
        public ICommand HideCommand { get; }

        private void OnHideCommandExecuted(object p) => IsHidden = !IsHidden;

        private bool CanHideCommandExecute(object p) => true;

        private bool isHidden = true;

        /// <summary>
        /// Gets or sets hiding mode
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
            => Tests != null && Executor != null;

        /// <summary>
        /// Collection of tests data
        /// </summary>
        public ObservableCollection<TestViewModel> Tests { get; set; }

        /// <summary>
        /// Adds new tests to the collection
        /// </summary>
        public ICommand AddTestCommand { get; set; }

        private bool CanAddTestCommandExecute(object p) => true;

        private void NotifyTestsResultsChanged()
        {
            OnPropertyChanged(nameof(NumberOfPassedTests));
            OnPropertyChanged(nameof(NumberOfFailedTests));
            OnPropertyChanged(nameof(NumberOfNotRunnedTests));
        }

        /// <summary>
        /// Handles changing of result in some test view model
        /// </summary>
        private void UpdateStorage(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TestViewModel.Result))
            {
                NotifyTestsResultsChanged();
            }
        }

        private void OnAddTestCommandExecuted(object p)
        {
            var newTest = new TestViewModel() { Result = ResultEnum.NotRunned, Executor = this.Executor, Storage = Tests };
            newTest.PropertyChanged += UpdateStorage;
            Tests.Add(newTest);
            NotifyTestsResultsChanged();
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

            NotifyTestsResultsChanged();
        }
    }
}