using ControlsLibrary.Model;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

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
                            return "Deterministic finite automaton";
                        }
                    case (FATypeEnum.NFA):
                        {
                            return "Non-deterministic finite automaton";
                        }
                    case (FATypeEnum.EpsilonNFA):
                        {
                            return "Non-deterministic finite automaton\n with epsilon-transitions";
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
