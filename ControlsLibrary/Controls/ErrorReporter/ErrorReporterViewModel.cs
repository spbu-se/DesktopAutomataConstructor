using QuickGraph;
using ControlsLibrary.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System;

namespace ControlsLibrary.Controls.ErrorReporter
{
    public class ErrorReporterViewModel : INotifyPropertyChanged
    {
        private BidirectionalGraph<NodeViewModel, EdgeViewModel> graph = new BidirectionalGraph<NodeViewModel, EdgeViewModel>();
        public BidirectionalGraph<NodeViewModel, EdgeViewModel> Graph
        {
            get => graph;
            set
            {
                graph = value;
                OnPropertyChanged();
            }
        }

        public void GraphEdited(object sender, EventArgs e)
        {
            OnPropertyChanged("HasError");
        }


        public bool HasError
        {
            get
            {
                return Graph.VertexCount % 2 == 1;
            }
        }

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
