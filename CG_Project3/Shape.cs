using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CG_Project3
{
    internal interface IShape
    {
        public void Draw(byte[] bitmap);
        public string ToString();
    }
    internal class Line : IShape
    {
        Point a, b;
        Color color;
        public Line(Point a, Point b, Color color)
        {
            this.a = a;
            this.b = b;
            this.color = color;
        }
        public Line(string text)
        {
            string[] elements = text.Split(',');
            a = new Point(Int32.Parse(elements[0]), Int32.Parse(elements[1]));
            b = new Point(Int32.Parse(elements[2]), Int32.Parse(elements[3]));
            color = Color.FromArgb(Convert.ToInt32(elements[4], 16));
        }
        public void Draw(byte[] bitmap)
        {
            ;
        }
        public override string ToString()
        {
            return "L;" + a.X.ToString() + "," + a.Y.ToString() + "," + b.X.ToString() + "," + b.Y.ToString() + "," + string.Format("{0:x6}", color.ToArgb());
        }
    }
}
