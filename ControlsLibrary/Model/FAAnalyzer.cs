using QuickGraph;
using System.Collections.Generic;
using System.Linq;
using ControlsLibrary.Properties.Langs;
using ControlsLibrary.ViewModel;

namespace ControlsLibrary.Model
{
    internal static class FAAnalyzer
    {
        public static ICollection<string> GetErrors(BidirectionalGraph<NodeViewModel, EdgeViewModel> graph)
        {
            var result = new List<string>();

            if (!graph.Vertices.Any(v => v.IsInitial == true) && graph.VertexCount != 0)
            {
                result.Add(Lang.Errors_SetInitial);
            }

            if (!graph.Vertices.Any(v => v.IsFinal == true) && graph.VertexCount != 0)
            {
                result.Add(Lang.Errors_SetAccepting);
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

            foreach (var edge1 in graph.Edges)
            {
                foreach (var edge2 in graph.Edges)
                {
                    if (edge1 != edge2 && edge1.Source == edge2.Source && edge1.TransitionTokens.Intersect(edge2.TransitionTokens).Count() > 0)
                    {
                        return FATypeEnum.NFA;
                    }
                }
            }

            if (graph.Vertices.Where(v => v.IsInitial).Count() > 1)
            {
                return FATypeEnum.NFA;
            }

            return FATypeEnum.DFA;
        }
    }
}
