using System.Numerics;

namespace Raskroi.Geometry
{
    public interface IPolygon
    {
        Vector2[] Vertices { get; }
        Line[] GetLines();
        bool IfVectorInside(Vector2 vertex);
        IPolygon Transform(Matrix3x2 matrix);
        IBoundingBox GetBoundingBox();
        Vector2 GetCenter();
    }
}