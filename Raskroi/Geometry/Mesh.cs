using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raskroi.Geometry
{
    /// <summary>
    /// Меш, объект состоящий из набора полигонов
    /// </summary>
    public class Mesh : IMesh
    {
        public Mesh(IPolygon[] polygons)
        {
            Polygons = polygons;
        }

        public IPolygon[] Polygons { get; }

        public IMesh Transform(Matrix3x2 matrix)
        {
            var result = new IPolygon[Polygons.Length];

            for (var i = 0; i < Polygons.Length; i++)
            {
                result[i] = Polygons[i].Transform(matrix);
            }

            return new Mesh(result);
        }

        public IBoundingBox GetBoundingBox()
        {
            var minX = float.MaxValue;
            var maxX = float.MinValue;
            var minY = float.MaxValue;
            var maxY = float.MinValue;

            foreach (var polygon in Polygons)
            {
                foreach (var vertex in polygon.Vertices)
                {
                    minX = Math.Min(vertex.X, minX);
                    minY = Math.Min(vertex.Y, minY);

                    maxX = Math.Max(vertex.X, maxX);
                    maxY = Math.Max(vertex.Y, maxY);
                }
            }

            return new BoundingBox(
                new Vector2(minX, minY),
                new Vector2(maxX, minY),
                new Vector2(maxX, maxY),
                new Vector2(minX, maxY)
            );
        }

        public IMesh Union(IMesh mesh)
        {
            return new Mesh(this.Polygons.Concat(mesh.Polygons).ToArray());
        }

        public IMesh Union(IPolygon polygon)
        {
            return new Mesh(this.Polygons.Concat(new[] { polygon }).ToArray());
        }
    }
}
