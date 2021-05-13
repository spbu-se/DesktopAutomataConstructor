using ControlsLibrary.Model;
using ControlsLibrary.Properties.Langs;
using ControlsLibrary.ViewModel;
using QuickGraph;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ControlsLibrary.Controls.TypeAnalyzer
{
    /// <summary>
    /// Contains FA type analyzer data and logic to interact with it
    /// </summary>
    public class TypeAnalyzerViewModel : INotifyPropertyChanged
    {
        private BidirectionalGraph<NodeViewModel, EdgeViewModel> graph = new BidirectionalGraph<NodeViewModel, EdgeViewModel>();

        /// <summary>
        /// Sets FA graph to analyze its type
        /// </summary>
        public BidirectionalGraph<NodeViewModel, EdgeViewModel> Graph
        {
            get => graph;
            set
            {
                graph = value;
                OnPropertyChanged();
                GraphEdited(graph, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handles graph data changing
        /// </summary>
        public void GraphEdited(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(StringType));
        }

        /// <summary>
        /// Returns current FA type converted into the string
        /// </summary>
        public string StringType
        {
            get
            {
                switch (FAAnalyzer.GetType(Graph))
                {
                    case (FATypeEnum.DFA):
                        {
                            return Lang.DFA;
                        }
                    case (FATypeEnum.NFA):
                        {
                            return Lang.NFA;
                        }
                    case (FATypeEnum.EpsilonNFA):
                        {
                            return Lang.EpsilonNFA;
                        }
                }
                return Lang.Types_FSM;
            }
        }

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
