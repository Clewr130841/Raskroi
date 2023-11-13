using System.Numerics;

namespace Raskroi.Geometry
{
    public interface IMesh
    {
        IPolygon[] Polygons { get; }

        IMesh Transform(Matrix3x2 matrix);

        IBoundingBox GetBoundingBox();

        IMesh Union(IMesh mesh);
        IMesh Union(IPolygon mesh);
    }
}