using QuickGraph;
using System.Collections.Generic;
using System.Linq;

namespace ControlsLibrary.Model
{
    internal static class FAAnalyzer
    {
        public static ICollection<string> GetErrors(BidirectionalGraph<NodeViewModel, EdgeViewModel> graph)
        {
            var result = new List<string>();

            if (!graph.Vertices.Any(v => v.IsInitial == true) && graph.VertexCount != 0)
            {
                result.Add("Set initial state");
            }

            if (!graph.Vertices.Any(v => v.IsFinal == true) && graph.VertexCount != 0)
            {
                result.Add("Set accepting state");
            }

            return result;
        }

        public static FATypeEnum GetType(BidirectionalGraph<NodeViewModel, EdgeViewModel> graph)
        {
            foreach (var edge in graph.Edges)
            {
                if (edge.IsEpsilon)
                {
                    return FATypeEnum.EpsilonNFA;
                }
            }

            var result = FATypeEnum.DFA;
            foreach (var edge1 in graph.Edges)
            {
                foreach (var edge2 in graph.Edges)
                {
                    if (edge1 != edge2 && edge1.Source == edge2.Source && edge1.TransitionTokens.Intersect(edge2.TransitionTokens).Count() > 0)
                    {
                        result = FATypeEnum.NFA;
                    }
                }
            }

            return result;
        }
    }
}
