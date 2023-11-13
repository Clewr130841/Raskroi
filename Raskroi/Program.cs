using Microsoft.VisualBasic;
using Raskroi.Calculators.MaterialRequirements;
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
            //Хотел сделать не только выпуклые фигуры, но и любые, отсюда и пошел через поиск
            //пересечений отрезков в алгоритме, но времени и сил доделать это
            //не хватило, так что сократил до выпуклой, зато сделал
            //чтобы можно было считать под углом

            //Еще планировал сделать, чтобы не только справа налево был расчет,
            //а вообще по офсету или слева направо минимум, последний способ
            //вообще матрицу масшабирование применил, отразил фигуру,
            //расчитал так же слева-направо, потом развернул обратно потребность,
            //но тоже недоделал =)

            //Не сделал шаг, чтобы требуемые материалы пилились по минимальному размеру,
            //так что оставил ваше условие, что материалы не превысят высоту заготовки
            //но о5 же недопилил
            //но там в моем коде это сделать просто, расчитали палку, которая нужна
            //потом просто попилили по максималке, если высота нужной палки превышает
            //размер исходного материала
            var requirements = alg.CalcMaterialRequirements(wall, context);


            //Не расчитал оптимизацию материалов, хотел сделать 2 метода
            //1. Минимальный остаток, когда просто упорядовачиваем потребность от большей к меньшей палки,
            //остатки сохраняем в какую-то упорядоченную коллекцию и берем остаток, если можно, если нет, берем целую палку
            //2. Прочитал про симплекс, но тоже не успел реализовать
            //так что оптимизации раскроя нет

            //Не уложил это все DI контейнер
            //Не покрыл тестами

            //Если покажется, что я ленивый)) Я можно сказать ударник производства на работе 😂 Просто, в свободное время,
            //я не могу себя неинтересные мне вещи заставить программировать)) Ну и вроде минималку сделал,
            //найти потребность - это было интересно, так что в первый же день сделал ночью)


            //Рендеринг тоже достаточно ленивый, лишь бы нарисовал результат
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