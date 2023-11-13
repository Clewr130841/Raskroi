using Raskroi.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Raskroi.Rendering
{
    public class SvgRenderer
    {
        const string SVG_START = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?>
<!DOCTYPE svg PUBLIC ""-//W3C//DTD SVG 1.1//EN"" ""http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd"">
<svg version = ""1.1""
     baseProfile=""full""
     xmlns = ""http://www.w3.org/2000/svg"" 
     xmlns:xlink = ""http://www.w3.org/1999/xlink""
     xmlns:ev = ""http://www.w3.org/2001/xml-events""
     height = ""{0}px""  width = ""{1}px"">     
";

        const string SWG_END = "</svg>";

        StringBuilder _strBuilder;

        float _viewportWidth;
        float _viewportHeight;
        float _offsetX;
        float _offsetY;

        public SvgRenderer(float viewportWidth, float viewportHeight, float offsetX, float offsetY)
        {
            _viewportWidth = viewportWidth;
            _viewportHeight = viewportHeight;
            _offsetX = offsetX;
            _offsetY = offsetY;
            _strBuilder = new StringBuilder();
            Clear();
        }
        public string ToHexString(Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

        public void Clear()
        {
            _strBuilder.Clear();
            _strBuilder.AppendLine(string.Format(SVG_START, _viewportHeight.ToString("0.#", CultureInfo.InvariantCulture), _viewportWidth.ToString("0.#", CultureInfo.InvariantCulture)));
        }

        public void EndScene()
        {
            var sb = new StringBuilder(_strBuilder.ToString());
            sb.AppendLine(SWG_END);
            var name = Guid.NewGuid().ToString("N") + ".svg";

            File.WriteAllText(name, sb.ToString());
            new Process
            {
                StartInfo = new ProcessStartInfo(name)
                {
                    UseShellExecute = true
                }
            }.Start();
        }

        public void Draw(Color color, params Line[] lines)
        {
            for (var i = 0; i < lines.Length; i++)
            {
                _strBuilder.Append($@"<line x1=""{(lines[i].V1.X + _offsetX).ToString("0.#", CultureInfo.InvariantCulture)}"" 
                                           y1=""{(lines[i].V1.Y + _offsetY).ToString("0.#", CultureInfo.InvariantCulture)}"" 
                                           x2=""{(lines[i].V2.X + _offsetX).ToString("0.#", CultureInfo.InvariantCulture)}""
                                           y2=""{(lines[i].V2.Y + _offsetY).ToString("0.#", CultureInfo.InvariantCulture)}"" 
                                           stroke=""{ToHexString(color)}"" 
                />");
            }
        }

        public void Draw(Color color, IMesh mesh)
        {
            foreach (var polygon in mesh.Polygons)
            {
                Draw(color, polygon);
            }
        }

        public void Draw(Color color, IPolygon polygon)
        {
            _strBuilder.Append($"<polygon style=\"fill: {ToHexString(Color.Transparent)}; fill-opacity:0.1; stroke: {ToHexString(color)};\" points=\"");


            foreach (var verticle in polygon.Vertices)
            {
                _strBuilder.Append((verticle.X + _offsetX).ToString("0.#", CultureInfo.InvariantCulture));
                _strBuilder.Append(" ");
                _strBuilder.Append((verticle.Y + _offsetY).ToString("0.#", CultureInfo.InvariantCulture));
                _strBuilder.Append(" ");
            }

            _strBuilder.AppendLine("\"></polygon>");
        }
    }
}
