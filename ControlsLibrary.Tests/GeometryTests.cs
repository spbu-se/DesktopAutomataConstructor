using NUnit.Framework;
using ControlsLibrary.Controls.Scene;
using GraphX.Measure;

namespace ControlsLibrary.Tests
{
    public class GeometryTests
    {
        [Test]
        public void GetDistanceTest()
        {
            Assert.AreEqual(0, Geometry.GetDistance(new Point(0, 0), new Point(0, 0)));
            Assert.AreEqual(5, Geometry.GetDistance(new Point(0, 0), new Point(3, 4)));
        }

        [Test]
        public void BelongsToLineTest()
        {
            Assert.True(Geometry.BelongsToLine(new Point(0, 0), new Point(0, 0), new Point(0, 0)));
            Assert.True(Geometry.BelongsToLine(new Point(0, 0), new Point(0, 2), new Point(0, 1)));
            Assert.True(Geometry.BelongsToLine(new Point(0, 0), new Point(2, 2), new Point(1, 1)));
            Assert.True(Geometry.BelongsToLine(new Point(0, 0), new Point(-2, -2), new Point(2, 2)));
            Assert.False(Geometry.BelongsToLine(new Point(0, 0), new Point(1, 1), new Point(-1, 10)));
        }
    }
}
