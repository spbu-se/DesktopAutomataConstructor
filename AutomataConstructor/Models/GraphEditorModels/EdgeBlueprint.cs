using GraphX.Controls;
using GraphX.Controls.Models;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AutomataConstructor.Models.GraphEditorModels
{
    internal class EdgeBlueprint : IDisposable
    {
        public VertexControl Source { get; set; }
        public Point TargetPosition { get; set; }
        public Path EdgePath { get; set; }

        public EdgeBlueprint(VertexControl source, Brush brush)
        {
            EdgePath = new Path() { Stroke = brush, Data = new LineGeometry() };
            Source = source;
        }

        void Source_PositionChanged(object sender, EventArgs eventArgs)
            => UpdateGeometry(Source.GetCenterPosition(), TargetPosition);

        public void UpdateTargetPosition(Point point)
        {
            TargetPosition = point;
            UpdateGeometry(Source.GetCenterPosition(), point);
        }

        private void UpdateGeometry(Point start, Point finish)
        {
            EdgePath.Data = new LineGeometry(start, finish);
            (EdgePath.Data as LineGeometry).Freeze();
        }

        public void Dispose()
        {
            Source.PositionChanged -= Source_PositionChanged;
            Source = null;
        }
    }
}
