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
    }
}
