using GraphX.Controls;
using System;

namespace ControlsLibrary.Controls.Scene
{
    public class CustomVertexControl: VertexControl
    {
        public CustomVertexControl(object vertexData, bool tracePositionChange = true, bool bindToDataObject = true)
            : base(vertexData, tracePositionChange, bindToDataObject)
        {
            IsSelected = false;
        }
        public bool IsSelected { get; set; }
    }
}
