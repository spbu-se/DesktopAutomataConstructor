using ControlsLibrary.Properties.Langs;
using ControlsLibrary.ViewModel;
using QuickGraph;
using System.Collections.Generic;
using System.Linq;

namespace ControlsLibrary.Model
{
    /// <summary>
    /// Provides analyze of a FA graph data
    /// </summary>
    public static class FAAnalyzer
    {
        /// <summary>
        /// Returns collection of errors in the FA
        /// </summary>
        public static ICollection<string> GetErrors(BidirectionalGraph<NodeViewModel, EdgeViewModel> graph)
        {
            var result = new List<string>();

            if (graph.Vertices.All(v => !v.IsInitial) && graph.VertexCount != 0)
            {
                result.Add(Lang.Errors_SetInitial);
            }

            if (graph.Vertices.All(v => !v.IsFinal) && graph.VertexCount != 0)
            {
                result.Add(Lang.Errors_SetAccepting);
            }

            return result;
        }

        /// <summary>
        /// Returns type of the FA
        /// </summary>
        public static FATypeEnum GetType(BidirectionalGraph<NodeViewModel, EdgeViewModel> graph)
        {

            foreach (var edge in graph.Edges)
            {
                if (edge.IsEpsilon)
                {
                    return FATypeEnum.EpsilonNFA;
                }
            }

            foreach (var edge1 in graph.Edges)
            {
                foreach (var edge2 in graph.Edges)
                {
                    if (edge1 != edge2 && edge1.Source == edge2.Source && edge1.TransitionTokens.Intersect(edge2.TransitionTokens).Any())
                    {
                        return FATypeEnum.NFA;
                    }
                }
            }

            if (graph.Vertices.Count(v => v.IsInitial) > 1)
            {
                return FATypeEnum.NFA;
            }

            return FATypeEnum.DFA;
        }
    }
}