using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raskroi.Geometry.Shapes
{
    public class ProfileFromTest : Figure
    {
        public ProfileFromTest() : base(new[]
        {
            new Vector2(0, 0),
            new Vector2(-170.3f, 583.8f),
            new Vector2(594.9f, 996.4f),
            new Vector2(1049.4f, 754.9f),
            new Vector2(1049.4f, 33.9f),
        })
        {
        }
    }
}
