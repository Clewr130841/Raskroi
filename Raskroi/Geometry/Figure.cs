using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raskroi.Geometry
{
    public class Figure : Polygon
    {
        public Figure(Vector2[] vertices) : base(vertices)
        {
            if (vertices.Length < 3) throw new ArgumentException("A figure can consist of 2 or more vertices");
        }
    }
}
