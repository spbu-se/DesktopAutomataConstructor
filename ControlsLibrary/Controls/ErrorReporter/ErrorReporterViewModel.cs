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

        private string errorMessage;

        public string ErrorMessage
        {
            get => errorMessage;
            set
            {
                errorMessage = value;
                OnPropertyChanged();
            }
        }


        public bool HasError
        {
            get
            {
                if (!Graph.Vertices.Any(v => v.IsInitial == true))
                {
                    ErrorMessage = "Set initial state";
                    return true;
                }
                var initials = Graph.Vertices.Where(v => v.IsInitial == true);
                var initialsCount = initials.Count();
                if (initialsCount > 1)
                {
                    ErrorMessage = "Initial state should be only one";
                    return true;
                }
                ErrorMessage = "No issues found";
                return false;
            }
        }

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
