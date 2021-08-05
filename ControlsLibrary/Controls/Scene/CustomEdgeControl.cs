using GraphX.Controls;
using System;

namespace ControlsLibrary.Controls.Scene
{
    class CustomEdgeControl: EdgeControl
    {
        //standart constructor is to be override?
        public CustomEdgeControl(VertexControl source, VertexControl target, object edge, bool showArrows = true)
            :base(source, target, edge, showArrows)
        {

        }
        public bool IsSelected { get; set; }
    }
}
