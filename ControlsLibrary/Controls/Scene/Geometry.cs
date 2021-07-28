using GraphX.Measure;
using System;
using System.Collections.Generic;

namespace ControlsLibrary.Controls.Scene
{
    /// <summary>
    /// Utility for handling various geometry-related tasks like determining distance between point and line.
    /// </summary>
    public static class Geometry
    {
        private const double delta = 3;

        /// <summary>
        /// Checking whether point belongs to line.
        /// </summary>
        /// <param name="lineStart">First line point.</param>
        /// <param name="lineEnd">Second line point.</param>
        /// <param name="point">Point for checking.</param>
        /// <returns>True if belongs, otherwise false.</returns>
        public static bool BelongsToLine(Point lineStart, Point lineEnd, Point point)
        {
            var vec1 = new Point(point.X - lineStart.X, point.Y - lineStart.Y);
            var vec2 = new Point(lineEnd.X - lineStart.X, lineEnd.Y - lineStart.Y);

            var val1 = Math.Pow(vec2.X, 2) + Math.Pow(vec2.Y, 2);
            var val2 = (vec1.X * vec2.X) + (vec1.Y * vec2.Y);

            var t = val2 / val1;

            var x = lineStart.X + (vec2.X * t);
            var y = lineStart.Y + (vec2.Y * t);

            return Math.Sqrt(Math.Pow(point.X - x, 2) + Math.Pow(point.Y - y, 2)).CompareTo(delta) <= 0;
        }

        /// <summary>
        /// Getting distance between two points.
        /// </summary>
        /// <param name="p1">First point.</param>
        /// <param name="p2">Second point.</param>
        /// <returns>Distance between two points.</returns>
        public static double GetDistance(Point p1, Point p2) => (p1 - p2).Length;
    }
}