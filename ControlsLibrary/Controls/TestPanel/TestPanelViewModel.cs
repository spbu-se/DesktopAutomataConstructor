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
        }

        public ObservableCollection<TestViewModel> Tests { get; set; }

        public ICommand AddTestCommand;

        private bool CanAddTestCommandExecute(object p) => true;

        private void OnAddTestCommandExecuted(object p) => Tests.Add(new TestViewModel());
    }
}
