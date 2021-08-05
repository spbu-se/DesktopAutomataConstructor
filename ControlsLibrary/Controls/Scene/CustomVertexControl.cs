using GraphX.Controls;
using System.Windows;

namespace ControlsLibrary.Controls.Scene
{
    public class CustomVertexControl: VertexControl
    {
        public CustomVertexControl(object vertexData, bool tracePositionChange = true, bool bindToDataObject = true)
            : base(vertexData, tracePositionChange, bindToDataObject)
        {
            IsSelected = false;
        }

        public static readonly DependencyProperty IsSelectedProperty;
        static CustomVertexControl()
        {
            IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(CustomVertexControl));
        }
        public bool IsSelected 
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }
    }
}
