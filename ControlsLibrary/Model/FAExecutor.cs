using ControlsLibrary.ViewModel;
using QuickGraph;
using System;
using System.Linq;

namespace ControlsLibrary.Model
{
    public class FAExecutor
    {
        /// <summary>
        /// Gets or sets FA graph to execute
        /// </summary>
        public BidirectionalGraph<NodeViewModel, EdgeViewModel> Graph { private get; set; }

        /// <summary>
        /// Basic constructor 
        /// </summary>
        /// <param name="graph"></param>
        public FAExecutor(BidirectionalGraph<NodeViewModel, EdgeViewModel> graph)
        {
            Graph = graph;
        }

        /// <summary>
        /// Gets result of execuction of the FA on the string
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>Result of exection</returns>
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

        /// <summary>
        /// Gets FA model which executes input string step by step
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>FA with specified graph and string</returns>
        public FiniteAutomata StartDebug(string input)
        {
            var errors = FAAnalyzer.GetErrors(Graph);
            if (errors.Count > 0)
            {
                throw new InvalidOperationException(errors.Aggregate("", (folder, error) => folder + error + "\n"));
            }
            var FA = FiniteAutomata.ConvertGraphToAutomata(Graph.Edges.ToList(), Graph.Vertices.ToList());
            FA.SetString(input);
            return FA;
        }
    }
}
