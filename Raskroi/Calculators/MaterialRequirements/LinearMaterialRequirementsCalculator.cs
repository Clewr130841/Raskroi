using Raskroi.Calculators.Sheating;
using Raskroi.Geometry;
using System.Drawing;
using System.Numerics;

namespace Raskroi.Calculators.MaterialRequirements
{
    /// <summary>
    /// Расчитывает потребность линейных материалов для выпуклых поверхностей
    /// </summary>
    public class LinearMaterialRequirementsCalculator
    {
        const float _2PI = (MathF.PI * 2f);

        /// <summary>
        /// Функция расчета обшивки линейных материалов
        /// </summary>
        public IEnumerable<LinearMaterialRequirement> CalcMaterialRequirements(IPolygon surface, SheathingContext context)
        {
            if (surface == null)
            {
                throw new ArgumentNullException(nameof(surface));
            }

            if (surface.Vertices.Length < 3)
            {
                throw new ArgumentException($"Bad surface");
            }

            //Расчитываем потребность в материалах
            //Расчитываем BoundingBox пока для получения центра всей фигуры
            var bBox = surface.GetBoundingBox();

            //Угол поворота, для того, чтобы сделать обшивку под углом
            var angle = context.Angle % _2PI; //Перекрутить не страшно, но тогда не получится сделать оптимизацию с проверкой на нулевой угол поворота

            //Пока создадим дубль контекста, дальше подумаем, че с этим делать
            var contextInner = new SheathingContext(context.SourceMaterialSize, angle);

            //Берем центр всей поверхности, будем крутить поверхность вокруг него
            var bBoxCenter = bBox.GetCenter();

            //Поворачиваем фигуру, если угол не 0,
            if (angle != 0)
            {
                surface = surface.Transform(Matrix3x2.CreateRotation(-angle, bBoxCenter));
            }

            bBox = surface.GetBoundingBox();

            //Посчитаем карты высот для каждого столбца решетки,
            //под которую расчитываем потребность
            var minMaxHeightMap = CalcMinMaxHeightMap(surface, bBox, bBoxCenter, context.SourceMaterialSize.Width);

            return CalcRequirements(minMaxHeightMap, bBox, bBoxCenter, contextInner);
        }

        /// <summary>
        /// Расчитывает потребность линейных материалов исходя из карты минимальных/максимальных высот на решетке
        /// </summary>
        private IEnumerable<LinearMaterialRequirement> CalcRequirements(float[][] minMaxHeightMap, IBoundingBox bBox, Vector2 bBoxCenter, SheathingContext context)
        {
            var requirements = new List<LinearMaterialRequirement>();

            //Фигура может быть повернута
            var rotation = Matrix3x2.CreateRotation(-context.Angle, bBoxCenter);

            for (var i = 0; i < minMaxHeightMap.Length; i++)
            {
                if (minMaxHeightMap[i] != null)
                {
                    var minX = bBox.TopLeft.X + context.SourceMaterialSize.Width * i;
                    var maxX = minX + context.SourceMaterialSize.Width;

                    var minY = minMaxHeightMap[i][0];
                    var maxY = minMaxHeightMap[i][1];

                    var height = maxY - minY;

                    //Вообщем, если потребность превысит оригинальную
                    //высоту материала - можно бы тут разделить

                    IPolygon polygon = new Polygon(
                        new Vector2(minX, minY),
                        new Vector2(maxX, minY),
                        new Vector2(maxX, maxY),
                        new Vector2(minX, maxY)
                    );

                    if (context.Angle == 0) //Небольшая оптимизация
                    {
                        polygon = polygon.Transform(rotation);
                    }

                    var requirement = new LinearMaterialRequirement(height, polygon);
                    requirements.Add(requirement);
                }
            }

            return requirements;
        }

        /// <summary>
        /// Расчитывает карту минимальных и максимальных высот на решетке с шагом ширины материала
        /// </summary>
        private float[][] CalcMinMaxHeightMap(IPolygon figure, IBoundingBox bBox, Vector2 bBoxCenter, float pitch)
        {
            //Высчитываем шаги решетки
            var gridSteps = (int)Math.Ceiling(bBox.Width / pitch);
            //Получим разлиновку, назовем гридом
            var gridLines = CalcGridLines(bBox, bBoxCenter, gridSteps, pitch);

            //Получаем отрезки модели, заранее считаем,
            //что все вершины присланы по часовой стрелке,
            //иначе будет хрень
            var modelLines = figure.GetLines();

            var heightMap = new float[gridSteps][];

            float maxY, minY;

            for (var i = 0; i < modelLines.Length; i++)
            {
                var modelLine = modelLines[i];

                Vector2 p1;
                Vector2 p2;

                //Распологаем точки слева-направо
                if (modelLine.V1.X > modelLine.V2.X)
                {
                    p1 = modelLine.V2;
                    p2 = modelLine.V1;
                }
                else
                {
                    p1 = modelLine.V1;
                    p2 = modelLine.V2;
                }

                //Считаем стартовый индекс прута решетки
                var firstGridIndex = (p1.X - bBox.TopLeft.X) / pitch;
                //Считаем количество шагов, сначала полное кол-во
                var fullLineGridSteps = (p2.X - p1.X) / pitch;
                var gridIndex = (int)firstGridIndex;

                //Округляем до целого количества
                var lastGridIndex = fullLineGridSteps + firstGridIndex;
                var lastGridIndexInt = (int)lastGridIndex;

                //Если вдруг все лягло под сетку в ноль, уменьшим кол-во шагов на один,
                //т.к. последний посчитается за пределами цикла
                if (lastGridIndex % 1f <= float.Epsilon)
                {
                    lastGridIndexInt--;
                }

                var prevPoint = p1;

                for (; gridIndex < lastGridIndexInt; gridIndex++)
                {
                    var gridLine = gridLines[gridIndex + 1];

                    //Решил пойти через поиск точки пересечения с решеткой

                    var intersectionPoint = gridLine.GetIntersection(modelLine);

                    if (!intersectionPoint.HasValue)
                    {
                        throw new InvalidOperationException();
                    }

                    minY = MathF.Min(prevPoint.Y, intersectionPoint.Value.Y);
                    maxY = MathF.Max(prevPoint.Y, intersectionPoint.Value.Y);

                    if (heightMap[gridIndex] == null)
                    {
                        heightMap[gridIndex] = new[] { minY, maxY };
                    }
                    else
                    {
                        heightMap[gridIndex][0] = MathF.Min(heightMap[gridIndex][0], minY);
                        heightMap[gridIndex][1] = MathF.Max(heightMap[gridIndex][1], maxY);
                    }

                    prevPoint = intersectionPoint.Value;
                }

                minY = MathF.Min(prevPoint.Y, p2.Y);
                maxY = MathF.Max(prevPoint.Y, p2.Y);

                if (heightMap[gridIndex] == null)
                {
                    heightMap[gridIndex] = new[] { minY, maxY };
                }
                else
                {
                    heightMap[gridIndex][0] = MathF.Min(heightMap[gridIndex][0], minY);
                    heightMap[gridIndex][1] = MathF.Max(heightMap[gridIndex][1], maxY);
                }
            }

            return heightMap;
        }

        /// <summary>
        /// Функция строит решетку на весь BoundingBox, слева-направо
        /// </summary>
        private Line[] CalcGridLines(IBoundingBox bBox, Vector2 bBoxCenter, int steps, float stepWidth)
        {
            var startPosition = bBox.TopLeft;
            var lines = new Line[steps + 1];

            var bottomY = startPosition.Y + bBox.Height;

            for (var i = 0; i <= steps; i++)
            {
                var nextX = startPosition.X + stepWidth;

                lines[i] = new Line(
                    startPosition,
                    new Vector2(startPosition.X, bottomY)
                );

                startPosition.X = nextX;
            }

            return lines;
        }
    }
}
