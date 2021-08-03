using GraphX.Controls;
using System;

namespace ControlsLibrary.Controls.Scene
{
    public class VertexRemovedEventArgs : EventArgs
    {
        public VertexControl VertexControl { get; set; }
        public CompositeCommand RemoveCommand { get; set; }
    }
}
