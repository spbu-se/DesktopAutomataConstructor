using ControlsLibrary.Infrastructure.Command;
using ControlsLibrary.ViewModel.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ControlsLibrary.Model;

namespace ControlsLibrary.Controls.TestPanel
{
    internal class TestPanelViewModel : BaseViewModel
    {
        public TestPanelViewModel()
        {
            AddTestCommand = new RelayCommand(OnAddTestCommandExecuted, CanAddTestCommandExecute);
            HideCommand = new RelayCommand(OnHideCommandExecuted, CanHideCommandExecute);
            Tests = new ObservableCollection<TestViewModel>();
        }

        public ICommand HideCommand { get; }

        private void OnHideCommandExecuted(object p) => isHidden = !isHidden;

        private bool CanHideCommandExecute(object p) => true;

        private bool isHidden = true;

        public bool IsHidden { get => isHidden; set => Set(ref isHidden, value); }

        public ObservableCollection<TestViewModel> Tests { get; set; }

        public ICommand AddTestCommand { get; set; }

        private bool CanAddTestCommandExecute(object p) => true;

        private void OnAddTestCommandExecuted(object p) => Tests.Add(new TestViewModel(Tests) { Result = Tests.Count % 2 == 0 ? ResultEnum.NotRunned : ResultEnum.Failed });
    }
}
