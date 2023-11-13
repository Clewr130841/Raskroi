﻿using Raskroi.Calculators.MaterialRequirements;
using Raskroi.Calculators.Sheating;
using Raskroi.Geometry.Shapes;
using Raskroi.Rendering;
using System.Drawing;

namespace Raskroi
{
    public class Program
    {
        static void Main(string[] args)
        {
            //Размер исходного материала
            var sourceMaterialSize = new SizeF(50f, 200f);

            //Наклоним обшивку на 15 градусов
            var sheatingAngle = 15 * (MathF.PI / 180f);

            //Расчитываю что фигура передается по часовой стрелке
            var wall = new ProfileFromTest();
            var context = new SheathingContext(sourceMaterialSize, sheatingAngle);

            var alg = new LinearMaterialRequirementsCalculator();

            //Расчитываем требуемые материалы
            var requirements = alg.CalcMaterialRequirements(wall, context);

            var bbox = wall.GetBoundingBox();
            
            var screenWidth = bbox.Width + MathF.Abs(bbox.TopLeft.X) + sourceMaterialSize.Width;
            var screenHeight = bbox.Height + MathF.Abs(bbox.TopLeft.Y) + sourceMaterialSize.Width;

            var hOffset = sourceMaterialSize.Width / 2f;

            var renderer = new SvgRenderer(screenWidth, screenHeight, -bbox.TopLeft.X + hOffset, -bbox.TopLeft.Y + hOffset);

            renderer.Draw(Color.Green, wall);
            foreach (var requirement in requirements)
            {
                renderer.Draw(Color.Red, requirement.GeometryInfo);
            }

            renderer.EndScene();
        }
    }
}