using GraphX.Controls;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ControlsLibrary.Controls.Scene
{
    /// <summary>
    /// Control for virtual edge that is used when one clicks on a first node and draws an edge to a second node.
    /// Actual edge is created only when drawing is finished (by clicking on a target node).
    /// </summary>
    internal class EdgeBlueprint : IDisposable
    {
        /// <summary>
        /// The source vertex of an edge
        /// </summary>
        public VertexControl Source { get; set; }

        /// <summary>
        /// The current target position of an edge
        /// </summary>
        public Point TargetPosition { get; set; }

        /// <summary>
        /// The actual calculated path of an edge
        /// </summary>
        public Path EdgePath { get; set; }

        /// <summary>
        /// The basic constructor
        /// </summary>
        /// <param name="source">Source vertex of a new edge</param>
        /// <param name="brush">Brush to draw a bluepring</param>
        public EdgeBlueprint(VertexControl source, Brush brush)
        {
            EdgePath = new Path() { Stroke = brush, Data = new LineGeometry() };
            Source = source;
        }

        private void SourcePositionChanged(object sender, EventArgs eventArgs)
            => UpdateGeometry(Source.GetCenterPosition(), TargetPosition);

        /// <summary>
        /// Handles target position update
        /// </summary>
        /// <param name="point">New target position</param>
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
            Source.PositionChanged -= SourcePositionChanged;
            Source = null;
        }
    }
}