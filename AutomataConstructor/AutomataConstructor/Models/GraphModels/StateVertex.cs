using GraphX.Common.Models;

namespace AutomataConstructor.Models.GraphModels
{
    /// <summary>
    /// The model of vertices in the DFA graph
    /// </summary>
    internal class StateVertex : VertexBase
    {
        /// <summary>
        /// Name of the state
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Is state initial
        /// </summary>
        public bool IsInitial { get; set; }

        /// <summary>
        /// Is state final
        /// </summary>
        public bool IsFinal { get; set; }

        /// <summary>
        /// Overriding of the base method
        /// </summary>
        public override string ToString() => Name;
    }
}
