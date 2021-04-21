using ControlsLibrary.Infrastructure.Command;
using ControlsLibrary.ViewModel.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ControlsLibrary.Model;
using QuickGraph;

namespace ControlsLibrary.Controls.TestPanel
{
    public class TestPanelViewModel : BaseViewModel
    {
        public TestPanelViewModel()
        {
            AddTestCommand = new RelayCommand(OnAddTestCommandExecuted, CanAddTestCommandExecute);
            HideCommand = new RelayCommand(OnHideCommandExecuted, CanHideCommandExecute);
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

        public ICommand HideCommand { get; }

        private void OnHideCommandExecuted(object p) => IsHidden = !IsHidden;

        private bool CanHideCommandExecute(object p) => true;

        private bool isHidden = false;

        public bool IsHidden { get => isHidden; set => Set(ref isHidden, value); }

        public ObservableCollection<TestViewModel> Tests { get; set; }

        public ICommand AddTestCommand { get; set; }

        private bool CanAddTestCommandExecute(object p) => true;

        private void OnAddTestCommandExecuted(object p) => Tests.Add(new TestViewModel(Tests, graph) { Result = ResultEnum.NotRunned });
    }
}
