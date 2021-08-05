using GraphX.Controls;
using System.Windows;

namespace ControlsLibrary.Controls.Scene
{
    class CustomEdgeControl: EdgeControl
    {
        public CustomEdgeControl(VertexControl source, VertexControl target, object edge, bool showArrows = true)
            :base(source, target, edge, showArrows)
        {
            IsSelected = false;
        }

        public static readonly DependencyProperty IsSelectedProperty;
        static CustomEdgeControl()
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
