using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Raskroi.Geometry
{
    public record Line(Vector2 V1, Vector2 V2)
    {
        /// <summary>
        /// Возвращает точку пересечения 2х отрезков,
        /// точка может лежать за пределами начала/конца линий,
        /// по сути это точка пересечения 2х лучей
        /// </summary>
        public Vector2? GetIntersection(Line line)
        {
            return GetIntersection(this, line);
        }

        /// <summary>
        /// Возвращает точку пересечения 2х отрезков,
        /// точка может лежать за пределами начала/конца линий,
        /// по сути это точка пересечения 2х лучей
        /// </summary>
        public static Vector2? GetIntersection(Line a, Line b)
        {
            if (a == null)
            {
                throw new ArgumentNullException(nameof(a));
            }

            if (b == null)
            {
                throw new ArgumentNullException(nameof(b));
            }

            var x1 = a.V1.X;
            var y1 = a.V1.Y;
            var x2 = a.V2.X;
            var y2 = a.V2.Y;

            var x3 = b.V1.X;
            var y3 = b.V1.Y;
            var x4 = b.V2.X;
            var y4 = b.V2.Y;

            var a1 = y2 - y1;
            var b1 = x1 - x2;
            var c1 = x2 * y1 - x1 * y2;

            var a2 = y4 - y3;
            var b2 = x3 - x4;
            var c2 = x4 * y3 - x3 * y4;

            var determinant = a1 * b2 - a2 * b1;

            if (MathF.Abs(determinant) <= float.Epsilon)
            {
                return null;
            }

            var x = (b1 * c2 - b2 * c1) / determinant;
            var y = (a2 * c1 - a1 * c2) / determinant;

            return new Vector2(x, y);
        }
    };
}
