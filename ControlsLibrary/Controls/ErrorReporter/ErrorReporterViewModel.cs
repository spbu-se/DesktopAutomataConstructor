using QuickGraph;
using ControlsLibrary.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using ControlsLibrary.Properties.Langs;
using ControlsLibrary.ViewModel;

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
            errors = FAAnalyzer.GetErrors(Graph);

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
                    return Lang.noIssuesFound;
                }
                return Errors.Count.ToString();
            }
        }

        private ICollection<string> errors = new ObservableCollection<string>();

        public ObservableCollection<string> Errors { get => new ObservableCollection<string>(errors); }

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
