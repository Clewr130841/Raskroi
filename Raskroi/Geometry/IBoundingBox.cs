using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raskroi.Geometry
{
    public interface IBoundingBox : IPolygon
    {
        Vector2 TopLeft { get; }
        Vector2 TopRight { get; }
        Vector2 BottomRight { get; }
        Vector2 BottomLeft { get; }

        float Height { get; }
        float Width { get; }
    }
}
