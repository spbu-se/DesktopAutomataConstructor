using GraphX.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ControlsLibrary.Model
{
    public class EdgeViewModel : EdgeBase<NodeViewModel>
    {
        /// <summary>
        /// Constructor which gets two vertices and symbols of transition
        /// </summary>
        /// <param name="source">Source vertex</param>
        /// <param name="target">Target vertex</param>
        /// <param name="availableSymbols">Symbols of transition</param>
        public EdgeViewModel(NodeViewModel source, NodeViewModel target, string name, List<char> transitionTokens)
            : base(source, target, 1)
        {
            Name = name;
            if (transitionTokens == null)
            {
                throw new ArgumentNullException(nameof(transitionTokens));
            }
            TransitionTokens = transitionTokens;
        }

        public List<char> TransitionTokens { get; }

        public string Name { get; set; }

        /// <summary>
        /// Overriding of the base method
        /// </summary>
        public override string ToString() => TransitionTokens.Aggregate("", (str, acc) => acc + str);
    }
}
