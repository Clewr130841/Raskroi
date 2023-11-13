using System.Numerics;

namespace Raskroi.Geometry
{
    /// <summary>
    /// Вообщем-то полигон, может состоять из произвольного набора вершин
    /// </summary>
    public class Polygon : IPolygon
    {
        public Polygon(params Vector2[] vertices)
        {
            if (vertices == null)
            {
                throw new ArgumentNullException(nameof(vertices));
            }

            Vertices = vertices;
        }

        public Vector2[] Vertices { get; }

        /// <summary>
        /// Возвращает прямоугольник, в который может быть вписана фигура
        /// </summary>
        public IBoundingBox GetBoundingBox()
        {
            var minX = float.MaxValue;
            var maxX = float.MinValue;
            var minY = float.MaxValue;
            var maxY = float.MinValue;

            foreach (var vertex in Vertices)
            {
                minX = MathF.Min(vertex.X, minX);
                minY = MathF.Min(vertex.Y, minY);

                maxX = MathF.Max(vertex.X, maxX);
                maxY = MathF.Max(vertex.Y, maxY);
            }

            return new BoundingBox(
                new Vector2(minX, minY),
                new Vector2(maxX, minY),
                new Vector2(maxX, maxY),
                new Vector2(minX, maxY)
            );
        }

        /// <summary>
        /// Получаем центр (может даже центр тяжести) всей фигуры
        /// </summary>
        public Vector2 GetCenter()
        {
            var minX = float.MaxValue;
            var maxX = float.MinValue;
            var minY = float.MaxValue;
            var maxY = float.MinValue;

            foreach (var vertex in Vertices)
            {
                minX = MathF.Min(vertex.X, minX);
                minY = MathF.Min(vertex.Y, minY);

                maxX = MathF.Max(vertex.X, maxX);
                maxY = MathF.Max(vertex.Y, maxY);
            }

            return new Vector2(minX + (maxX - minX) / 2f, minY + (maxY - minY) / 2f);
        }

        /// <summary>
        /// Отдает массив отрезков из которых состоит фигура
        /// </summary>
        public Line[] GetLines()
        {
            var lines = new Line[Vertices.Length];

            for (var i = 1; i < Vertices.Length; i++)
            {
                lines[i - 1] = new Line(Vertices[i - 1], Vertices[i]);
            }

            lines[Vertices.Length - 1] = new Line(Vertices[Vertices.Length - 1], Vertices[0]);

            return lines;
        }

        /// <summary>
        /// Проверка на то, что вектор лежит внутри фигуры
        /// </summary>
        public bool IfVectorInside(Vector2 vertex)
        {
            var isInside = false;

            for (int i = 0, j = Vertices.Length - 1; i < Vertices.Length; j = i++)
            {
                var xi = Vertices[i].X;
                var yi = Vertices[i].Y;
                var xj = Vertices[j].X;
                var yj = Vertices[j].Y;

                var intersect = yi > vertex.Y != yj > vertex.Y && vertex.X < ((xj - xi) * (vertex.Y - yi)) / (yj - yi) + xi;

                if (intersect)
                {
                    return false;
                }
            }

            return isInside;
        }

        /// <summary>
        /// Возвращает измененную при помощи матрицы фигуру
        /// </summary>
        public IPolygon Transform(Matrix3x2 matrix)
        {
            var vertices = new Vector2[Vertices.Length];

            for (var i2 = 0; i2 < Vertices.Length; i2++)
            {
                vertices[i2] = Vector2.Transform(Vertices[i2], matrix);
            }

            return new Polygon(vertices);
        }
    }
}
