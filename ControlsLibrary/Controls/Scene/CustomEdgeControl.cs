using GraphX.Controls;
using System;

namespace ControlsLibrary.Controls.Scene
{
    class CustomEdgeControl: EdgeControl
    {
        public CustomEdgeControl(VertexControl source, VertexControl target, object edge, bool showArrows = true)
            :base(source, target, edge, showArrows)
        {
            IsSelected = false;
        }
        public bool IsSelected { get; set; }
    }
}
