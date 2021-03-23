using GraphX.Logic.Models;
using QuickGraph;

namespace AutomataConstructor.Models.GraphModels
{
    /// <summary>
    /// The model of graph logic
    /// </summary>
    internal class DFAGraphLogic : GXLogicCore<StateVertex, TransitionEdge, BidirectionalGraph<StateVertex, TransitionEdge>>
    {
    }
}
