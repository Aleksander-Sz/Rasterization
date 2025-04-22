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
            if(dy<=dx)
            {
                d = dy - (dx / 2);
                /*i = y * stride + x * 3;
                bitmap[i] = bitmap[i + 1] = bitmap[i + 2] = (byte)255;*/
                for (int step = 0; step <= dx; step++)
                {
                    x +=sx;
                    if(d<0)
                    {
                        // E chosen
                        d = d + dy;
                    }
                    else
                    {
                        // NE chosen
                        d = d + dy - dx;
                        y+=sy;
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
                    y +=sy;
                    if (d < 0)
                    {
                        // E chosen
                        d = d + dx;
                    }
                    else
                    {
                        // NE chosen
                        d = d + dx - dy;
                        x+=sx;
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
            brush = new int[width,width];
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
                    i = (y + yo - width/2) * stride + (x + xo - width/2) * 3;
                    if (brush[xo,yo]==1)
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
        public void Draw(byte[] bitmap, int stride)
        {
            ;
        }
        public override string ToString()
        {
            return "C;" + center.X.ToString() + "," + center.Y.ToString() + "," + radius.ToString() + "," + string.Format("{0:x6}", color.ToArgb());
        }
    }
}
