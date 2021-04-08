using ControlsLibrary.Model;
using System;

namespace ControlsLibrary.Controls.Scene
{
    public class NodeSelectedEventArgs : EventArgs
    {
        public NodeViewModel Node { get; set; }
    }
}
