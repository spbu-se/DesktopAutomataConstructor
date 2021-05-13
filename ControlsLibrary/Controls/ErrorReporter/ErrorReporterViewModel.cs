using ControlsLibrary.Model;
using ControlsLibrary.Properties.Langs;
using ControlsLibrary.ViewModel;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ControlsLibrary.Controls.ErrorReporter
{
    /// <summary>
    /// Contains data and provides interaction with error reporter logic in FAAnalyzer
    /// </summary>
    public class ErrorReporterViewModel : INotifyPropertyChanged
    {
        private BidirectionalGraph<NodeViewModel, EdgeViewModel> graph = new BidirectionalGraph<NodeViewModel, EdgeViewModel>();

        /// <summary>
        /// Gets or sets graph to analyze
        /// </summary>
        public BidirectionalGraph<NodeViewModel, EdgeViewModel> Graph
        {
            get => graph;
            set
            {
                graph = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Handles graph data changind events
        /// </summary>
        public void GraphEdited(object sender, EventArgs e)
        {
            errors = FAAnalyzer.GetErrors(Graph);

            OnPropertyChanged(nameof(Errors));
            OnPropertyChanged(nameof(HasError));
            OnPropertyChanged(nameof(ErrorMessage));
        }

        /// <summary>
        /// Gets a short message about presence of errors in the model
        /// </summary>
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

        /// <summary>
        /// Gets a list of errors in the model
        /// </summary>
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
