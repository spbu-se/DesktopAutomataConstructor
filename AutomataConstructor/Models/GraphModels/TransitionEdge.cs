using System.Collections.Generic;
using GraphX.Common.Models;

namespace AutomataConstructor.Models.GraphModels
{
    /// <summary>
    /// The model of transitions in the graph
    /// </summary>
    internal class TransitionEdge : EdgeBase<StateVertex>
    {
        /// <summary>
        /// Constructor which gets two vertices and symbols of transition
        /// </summary>
        /// <param name="source">Source vertex</param>
        /// <param name="target">Target vertex</param>
        /// <param name="availableSymbols">Symbols of transition</param>
        public TransitionEdge(StateVertex source, StateVertex target, string name)
            : base(source, target, 1)
        {
            Name = name;
        }

        public string Name { get; set; }

        /// <summary>
        /// Overriding of the base method
        /// </summary>
        public override string ToString() => Name;
    }
}
