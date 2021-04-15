using QuickGraph;
using ControlsLibrary.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System;
using System.Collections.ObjectModel;

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
            Errors.Clear();
            if (!Graph.Vertices.Any(v => v.IsInitial == true) && Graph.VertexCount != 0)
            {
                Errors.Add("Set initial state");
            }

            if (Graph.Vertices.Where(v => v.IsInitial == true).Count() > 1)
            {
                Errors.Add("Initital state should be only one");
            }

            OnPropertyChanged("Errors");
            OnPropertyChanged("HasError");
            OnPropertyChanged("ErrorMessage");
        }

        public string ErrorMessage
        {
            get
            {
                if (!HasError)
                {
                    return "No issues found";
                }
                return Errors.Count.ToString();
            }
        }

        public ObservableCollection<string> Errors { get; } = new ObservableCollection<string>();

        public bool HasError
        {
            get
            {
                return Errors.Count != 0;
            }
        }

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
