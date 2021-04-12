using QuickGraph;
using ControlsLibrary.ViewModel.Base;
using ControlsLibrary.Model;
using System.ComponentModel;

namespace ControlsLibrary.Controls.ErrorReporter
{
    public class ErrorReporterViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private BidirectionalGraph<NodeViewModel, EdgeViewModel> graph = new BidirectionalGraph<NodeViewModel, EdgeViewModel>();
        public BidirectionalGraph<NodeViewModel, EdgeViewModel> Graph
        {
            get => graph;
            set
            {
                Set(ref graph, value);
            }
        }
    }
}
