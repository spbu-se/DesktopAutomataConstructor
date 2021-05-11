using ControlsLibrary.ViewModel;
using System;

namespace ControlsLibrary.Controls.Scene
{
    /// <summary>
    /// Argumments for the node selection event
    /// </summary>
    public class NodeSelectedEventArgs : EventArgs
    {
        public NodeViewModel Node { get; set; }
    }
}
