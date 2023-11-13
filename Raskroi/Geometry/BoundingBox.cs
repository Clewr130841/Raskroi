using System.Numerics;

namespace Raskroi.Geometry
{
    public class BoundingBox : Polygon, IBoundingBox
    {
        public BoundingBox(Vector2 topLeft, Vector2 topRight, Vector2 bottomRight, Vector2 bottomLeft) : base(topLeft, topRight, bottomRight, bottomLeft)
        {
        }

        public Vector2 TopLeft => Vertices[0];
        public Vector2 TopRight => Vertices[1];
        public Vector2 BottomRight => Vertices[2];
        public Vector2 BottomLeft => Vertices[3];

        public float Height => BottomRight.Y - TopLeft.Y;
        public float Width =>  TopRight.X - TopLeft.X;
    }
}
