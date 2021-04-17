using ControlsLibrary.Infrastructure.Command;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ControlsLibrary.Controls.TestPanel
{
    internal class TestPanelViewModel
    {
        public TestPanelViewModel()
        {
            AddTestCommand = new RelayCommand(OnAddTestCommandExecuted, CanAddTestCommandExecute);
            Tests = new ObservableCollection<TestViewModel>();
        }

        public ObservableCollection<TestViewModel> Tests { get; set; }

        public ICommand AddTestCommand { get; set; }

        private bool CanAddTestCommandExecute(object p) => true;

        private void OnAddTestCommandExecuted(object p) => Tests.Add(new TestViewModel(Tests) { Result = Tests.Count % 2 == 0 ? TestResultEnum.NotRunned : TestResultEnum.Failed });
    }
}
