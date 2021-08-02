using ControlsLibrary.ViewModel;
using GraphX.Controls;
using System;
using System.Linq;

namespace ControlsLibrary.Controls.Scene
{
    class ParallelEdgesProblemSolver
    {
        /// <summary>
        /// Creates edge routing points to avoid overlapping of an edge by a parallel one
        /// </summary>
        /// <param name="edgeControl">Edge control which was overlapped or overlaps other</param>
        public static void AvoidParallelEdges(GraphArea graphArea, EdgeControl edgeControl)
        {
            var edge = edgeControl.Edge as EdgeViewModel;
            if (edge == null)
            {
                return;
            }
            var parallelEdge = graphArea.LogicCore.Graph.Edges.FirstOrDefault(e => e.Source == edge.Target && edge.Source == e.Target);

            if (parallelEdge == null)
            {
                return;
            }

            var sourcePos = edgeControl.Source.GetCenterPosition().ToGraphX();
            var targetPos = edgeControl.Target.GetCenterPosition().ToGraphX();

            var middleX = (sourcePos.X + targetPos.X) / 2;
            var middleY = (sourcePos.Y + targetPos.Y) / 2;

            var distance = Geometry.GetDistance(sourcePos, targetPos);
            var diagonal = Math.Min(Math.Max(distance / 25, 20), 80);

            var bypassPoint1 = new GraphX.Measure.Point(middleX, middleY);
            var bypassPoint2 = new GraphX.Measure.Point(middleX, middleY);

            if ((sourcePos.X - targetPos.X) * (sourcePos.Y - targetPos.Y) > 0)
            {
                bypassPoint1.X -= diagonal;
                bypassPoint1.Y += diagonal;
                bypassPoint2.X += diagonal;
                bypassPoint2.Y -= diagonal;
            }
            else
            {
                bypassPoint1.X -= diagonal;
                bypassPoint1.Y -= diagonal;
                bypassPoint2.X += diagonal;
                bypassPoint2.Y += diagonal;
            }

            edge.RoutingPoints = new[] { sourcePos, bypassPoint1, targetPos };
            parallelEdge.RoutingPoints = new[] { targetPos, bypassPoint2, targetPos };
            graphArea.UpdateAllEdges();
        }

    }
}
