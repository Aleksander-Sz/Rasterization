using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CG_Project3
{
    internal interface IShape
    {
        public void Draw(byte[] bitmap, int stride);
    }
    internal class Line : IShape
    {
        public Point a, b;
        public Color color;
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
        public void Draw(byte[] bitmap, int stride)
        {
            int x = this.a.X;
            int y = this.a.Y;
            int dx = Math.Abs(this.b.X - this.a.X);
            int dy = Math.Abs(this.b.Y - this.a.Y);
            int sx = this.b.X > this.a.X ? 1 : -1;
            int sy = this.b.Y > this.a.Y ? 1 : -1;
            int d;
            int i;
            if (dy <= dx)
            {
                d = dy - (dx / 2);
                /*i = y * stride + x * 3;
                bitmap[i] = bitmap[i + 1] = bitmap[i + 2] = (byte)255;*/
                for (int step = 0; step <= dx; step++)
                {
                    x += sx;
                    if (d < 0)
                    {
                        // E chosen
                        d = d + dy;
                    }
                    else
                    {
                        // NE chosen
                        d = d + dy - dx;
                        y += sy;
                    }
                    i = y * stride + x * 3;
                    PixelSet(bitmap, i, this.color);
                }
            }
            else
            {
                d = dx - (dy / 2);
                /*i = y * stride + x * 3;
                bitmap[i] = bitmap[i + 1] = bitmap[i + 2] = (byte)255;*/
                for (int step = 0; step <= dy; step++)
                {
                    y += sy;
                    if (d < 0)
                    {
                        // E chosen
                        d = d + dx;
                    }
                    else
                    {
                        // NE chosen
                        d = d + dx - dy;
                        x += sx;
                    }
                    i = y * stride + x * 3;
                    PixelSet(bitmap, i, this.color);
                }
            }
        }
        public override string ToString()
        {
            return "L;" + a.X.ToString() + "," + a.Y.ToString() + "," + b.X.ToString() + "," + b.Y.ToString() + "," + string.Format("{0:x6}", color.ToArgb());
        }
        private void PixelSet(byte[] pictureData, int i, Color c)
        {
            if (i < 0 || i + 2 >= pictureData.Length)
                return;
            pictureData[i] = (byte)c.B;
            pictureData[i + 1] = (byte)c.G;
            pictureData[i + 2] = (byte)c.R;
        }
    }
    internal class ThickLine : IShape
    {
        Point a, b;
        int width;
        Color color;
        int[,] brush;
        public ThickLine(Point a, Point b, int width, Color color)
        {
            this.a = a;
            this.b = b;
            this.width = width;
            this.color = color;
            brush = new int[width, width];
            for (int xo = 0; xo < this.width; xo++)
            {
                for (int yo = 0; yo < this.width; yo++)
                {
                    if ((xo - (width / 2)) * (xo - (width / 2)) + (yo - (width / 2)) * (yo - (width / 2)) <= width * width / 4)
                    {
                        brush[xo, yo] = 1;
                    }
                    else
                        brush[xo, yo] = 0;
                }
            }
        }
        public ThickLine(string text) : this(
            new Point(Int32.Parse(text.Split(',')[0]), Int32.Parse(text.Split(',')[1])),
            new Point(Int32.Parse(text.Split(',')[2]), Int32.Parse(text.Split(',')[3])),
            Int32.Parse(text.Split(',')[4]),
            Color.FromArgb(Convert.ToInt32(text.Split(',')[5], 16)))
        { }
        public void Draw(byte[] bitmap, int stride)
        {
            int x = this.a.X;
            int y = this.a.Y;
            int dx = Math.Abs(this.b.X - this.a.X);
            int dy = Math.Abs(this.b.Y - this.a.Y);
            int sx = this.b.X > this.a.X ? 1 : -1;
            int sy = this.b.Y > this.a.Y ? 1 : -1;
            int d;
            int i;
            if (dy <= dx)
            {
                d = dy - (dx / 2);
                /*i = y * stride + x * 3;
                bitmap[i] = bitmap[i + 1] = bitmap[i + 2] = (byte)255;*/
                for (int step = 0; step <= dx; step++)
                {
                    x += sx;
                    if (d < 0)
                    {
                        // E chosen
                        d = d + dy;
                    }
                    else
                    {
                        // NE chosen
                        d = d + dy - dx;
                        y += sy;
                    }
                    StampBrush(bitmap, stride, x, y);
                }
            }
            else
            {
                d = dx - (dy / 2);
                /*i = y * stride + x * 3;
                bitmap[i] = bitmap[i + 1] = bitmap[i + 2] = (byte)255;*/
                for (int step = 0; step <= dy; step++)
                {
                    y += sy;
                    if (d < 0)
                    {
                        // E chosen
                        d = d + dx;
                    }
                    else
                    {
                        // NE chosen
                        d = d + dx - dy;
                        x += sx;
                    }
                    StampBrush(bitmap, stride, x, y);
                }
            }
        }
        public override string ToString()
        {
            return "T;" + a.X.ToString() + "," + a.Y.ToString() + "," + b.X.ToString() + "," + b.Y.ToString() + "," + width.ToString() + "," + string.Format("{0:x6}", color.ToArgb());
        }
        private void PixelSet(byte[] pictureData, int i, Color c)
        {
            if (i < 0 || i + 2 >= pictureData.Length)
                return;
            pictureData[i] = (byte)c.B;
            pictureData[i + 1] = (byte)c.G;
            pictureData[i + 2] = (byte)c.R;
        }
        private void StampBrush(byte[] bitmap, int stride, int x, int y)
        {
            int i;
            for (int xo = 0; xo < this.width; xo++)
            {
                for (int yo = 0; yo < this.width; yo++)
                {
                    i = (y + yo - width / 2) * stride + (x + xo - width / 2) * 3;
                    if (brush[xo, yo] == 1)
                        PixelSet(bitmap, i, this.color);
                }
            }
        }
    }
    internal class Circle : IShape
    {
        public Point center;
        public int radius;
        public Color color;
        public Circle(Point center, int radius, Color color)
        {
            this.center = center;
            this.radius = radius;
            this.color = color;
        }
        public Circle(string text)
        {
            string[] elements = text.Split(',');
            center = new Point(Int32.Parse(elements[0]), Int32.Parse(elements[1]));
            radius = Int32.Parse(elements[2]);
            color = Color.FromArgb(Convert.ToInt32(elements[3], 16));
        }
        public void Draw(byte[] bitmap, int stride) // loosely based on this: https://www.geeksforgeeks.org/mid-point-circle-drawing-algorithm/
        {
            int x = radius;
            int y = 0;
            int P = 1 - radius;
            int i;
            i = (this.center.Y) * stride + (x + this.center.X) * 3;
            PixelSet(bitmap, i, color);
            i = (this.center.Y) * stride + (-x + this.center.X) * 3;
            PixelSet(bitmap, i, color);
            i = (x + this.center.Y) * stride + (this.center.X) * 3;
            PixelSet(bitmap, i, color);
            i = (-x + this.center.Y) * stride + (this.center.X) * 3;
            PixelSet(bitmap, i, color);
            while (x > y)
            {
                y++;

                if (P <= 0)
                    P = P + 2 * y + 1;
                else
                {
                    x--;
                    P = P + 2 * y - 2 * x + 1;
                }

                if (x < y)
                    break;
                i = (y + this.center.Y) * stride + (x + this.center.X) * 3;
                PixelSet(bitmap, i, color);
                i = (y + this.center.Y) * stride + (-x + this.center.X) * 3;
                PixelSet(bitmap, i, color);
                i = (-y + this.center.Y) * stride + (x + this.center.X) * 3;
                PixelSet(bitmap, i, color);
                i = (-y + this.center.Y) * stride + (-x + this.center.X) * 3;
                PixelSet(bitmap, i, color);
                if (x != y)
                {
                    i = (x + this.center.Y) * stride + (y + this.center.X) * 3;
                    PixelSet(bitmap, i, color);
                    i = (x + this.center.Y) * stride + (-y + this.center.X) * 3;
                    PixelSet(bitmap, i, color);
                    i = (-x + this.center.Y) * stride + (y + this.center.X) * 3;
                    PixelSet(bitmap, i, color);
                    i = (-x + this.center.Y) * stride + (-y + this.center.X) * 3;
                    PixelSet(bitmap, i, color);
                }
            }
        }
        public override string ToString()
        {
            return "C;" + center.X.ToString() + "," + center.Y.ToString() + "," + radius.ToString() + "," + string.Format("{0:x6}", color.ToArgb());
        }
        private void PixelSet(byte[] pictureData, int i, Color c)
        {
            if (i < 0 || i + 2 >= pictureData.Length)
                return;
            pictureData[i] = (byte)c.B;
            pictureData[i + 1] = (byte)c.G;
            pictureData[i + 2] = (byte)c.R;
        }
    }
    class Polygon : IShape
    {
        protected List<Point> points;
        private int width;
        private Color color;
        private List<ThickLine> lines;
        public bool Closed;
        public Polygon(Color color, List<Point> points, int width = 1, bool closed = true)
        {
            this.color = color;
            this.points = points;
            this.width = width;
            this.lines = new List<ThickLine>();
            this.Closed = closed;
            GenerateLines();
        }
        public Polygon(string text) : this(
            Color.FromArgb(Convert.ToInt32(text.Split('|')[0].Split(',')[0], 16)),
            new List<Point>(),
            Int32.Parse(text.Split('|')[0].Split(',')[1]))
        {
            string[] pointsText = text.Split('|')[1].Split(',');
            for (int i = 0; i < pointsText.Length / 2; i++)
            {
                points.Add(new Point(Int32.Parse(pointsText[i * 2]), Int32.Parse(pointsText[i * 2 + 1])));
            }
            GenerateLines();
        }
        private void GenerateLines()
        {
            this.lines.Clear();
            for(int i = 0; i<points.Count-1;i++)
            {
                lines.Add(new ThickLine(points[i], points[i+1],this.width,this.color));
            }
            if(Closed)
            {
                lines.Add(new ThickLine(points[points.Count - 1], points[0], this.width, this.color));
            }
        }
        public override string ToString()
        {
            string pointsText = "";
            foreach (Point point in points)
            {
                pointsText += point.X.ToString() + "," + point.Y.ToString() + ",";
            }
            return "P;" + string.Format("{0:x6}", color.ToArgb()) + "," + this.width.ToString() + "|" + pointsText;
        }
        public void Draw(byte[] bitmap, int stride)
        {
            if (Closed && lines.Count != points.Count)
                GenerateLines();
            foreach(ThickLine line in lines)
            {
                line.Draw(bitmap, stride);
            }
        }
        public void Add(Point point)
        {
            points.Add(point);
            lines.Add(new ThickLine(points[points.Count - 1], point, this.width, this.color));
        }
    }
}