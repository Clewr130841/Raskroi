using Raskroi.Geometry;
using System.Numerics;

namespace Raskroi.Tests
{
    public class Tests
    {
        [Test]
        [TestCase(
            0, 0, 1, 1,
            0, 1, 1, 0,
            0.5f, 0.5f
        )]
        public void IntersectionTest(float a1X, float a1Y, float a2X, float a2Y, float b1X, float b1Y, float b2X, float b2Y, float xResult, float yResult)
        {
            var line1 = new Line(new Vector2(a1X, a1Y), new Vector2(a2X, a2Y));
            var line2 = new Line(new Vector2(b1X, b1Y), new Vector2(b2X, b2Y));

            var intersection = line1.GetIntersection(line2);

            Assert.IsTrue(intersection.HasValue);

            Assert.IsTrue(
                Math.Abs(intersection.Value.X - xResult) <= float.Epsilon &&
                Math.Abs(intersection.Value.Y - yResult) <= float.Epsilon
            );
        }

        [Test]
        [TestCase(
            0, 0, 0, 1,
            2, 0, 2, 1
        )]
        public void NoIntersectionTest(float a1X, float a1Y, float a2X, float a2Y, float b1X, float b1Y, float b2X, float b2Y)
        {
            var line1 = new Line(new Vector2(a1X, a1Y), new Vector2(a2X, a2Y));
            var line2 = new Line(new Vector2(b1X, b1Y), new Vector2(b2X, b2Y));

            var intersection = line1.GetIntersection(line2);

            Assert.IsTrue(!intersection.HasValue);
        }
    }
}