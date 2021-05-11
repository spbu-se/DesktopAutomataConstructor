using ControlsLibrary.Model;
using QuickGraph;
using ControlsLibrary.ViewModel;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ControlsLibrary.Properties.Langs;

namespace ControlsLibrary.Controls.TypeAnalyzer
{
    public class TypeAnalyzerViewModel : INotifyPropertyChanged
    {
        private BidirectionalGraph<NodeViewModel, EdgeViewModel> graph = new BidirectionalGraph<NodeViewModel, EdgeViewModel>();
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

        public void GraphEdited(object sender, EventArgs e)
        {
            type = FAAnalyzer.GetType(Graph);
            OnPropertyChanged("StringType");
        }

        private FATypeEnum type = FATypeEnum.DFA;

        public string StringType
        {
            get
            {
                switch (type)
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
                return "Finite state machine";
            }
        }

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
