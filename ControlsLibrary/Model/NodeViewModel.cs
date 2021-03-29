using ControlsLibrary.ViewModel;
using GraphX.Common.Models;
using System.Collections.Generic;
using System.ComponentModel;

namespace ControlsLibrary.Model
{
    public class NodeViewModel : VertexBase
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
