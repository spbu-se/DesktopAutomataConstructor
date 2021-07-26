using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace ControlsLibrary.Controls.Scene
{
    public class AdornerSelectedArea : Adorner
    {
        private Rect selectedRect;
        private SolidColorBrush renderBrush;
        private Pen renderPen;
        public AdornerSelectedArea(UIElement adornedElement)
            : base(adornedElement)
        {
            selectedRect = new Rect(new Size(0, 0));
            renderBrush = new SolidColorBrush(Colors.Blue);
            renderBrush.Opacity = 0.2;
            renderPen = new Pen(new SolidColorBrush(Colors.Navy), 1.5);
        }

        public Rect SelectedRect { get; set; }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(renderBrush, renderPen, selectedRect);
        }
    }
}

