using ControlsLibrary.ViewModel;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ControlsLibrary.Model
{
    public class FAExecutor
    {
        /// <summary>
        /// Gets or sets FA graph to execute
        /// </summary>
        public BidirectionalGraph<NodeViewModel, EdgeViewModel> Graph { private get; set; }

        public FAExecutor(BidirectionalGraph<NodeViewModel, EdgeViewModel> graph)
        {
            Graph = graph;
        }

        public ResultEnum Execute(string input)
        {
            var errors = FAAnalyzer.GetErrors(Graph);
            if (errors.Count > 0)
            {
                throw new InvalidOperationException(errors.Aggregate("", (folder, error) => folder + error + "\n"));
            }
            var FA = FiniteAutomata.ConvertGraphToAutomata(Graph.Edges.ToList(), Graph.Vertices.ToList());
            return FA.DoAllTransitions(input) ? ResultEnum.Passed : ResultEnum.Failed;
        }

        public FiniteAutomata StartDebug(string input)
        {
            var FA = FiniteAutomata.ConvertGraphToAutomata(Graph.Edges.ToList(), Graph.Vertices.ToList());
            FA.SetString(input);
            return FA;
        }
    }
}
