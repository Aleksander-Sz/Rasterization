using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace CG_Project3
{
    internal interface IShape
    {
        public Color color { get; set; }
        public int width { get; set; }
        public bool AA { get; set; }
        public void Draw(byte[] bitmap, int stride);
        public void Move(int x, int y);
        public List<Vertex> GetVertices();
    }
    internal class Line : IShape // -----------------------------------------------------------
    {
        public Point a, b;
        public Color color { get; set; }
        public int width { get; set; }
        public bool AA { get; set; }
        public Line(Point a, Point b, Color color)
        {
            this.a = a;
            this.b = b;
            this.color = color;
            this.width = 1;
        }
        public Line(string text)
        {
            string[] elements = text.Split(',');
            a = new Point(Int32.Parse(elements[0]), Int32.Parse(elements[1]));
            b = new Point(Int32.Parse(elements[2]), Int32.Parse(elements[3]));
            color = Color.FromArgb(Convert.ToInt32(elements[4], 16));
            width = 1;
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
        public List<Vertex> GetVertices()
        {
            List<Vertex> rPoints = new List<Vertex>();
            rPoints.Add(new Vertex(a, this, Vertex.VertexType.Normal));
            rPoints.Add(new Vertex(b, this, Vertex.VertexType.Normal));
            //rPoints.Add(new Vertex(new Point((a.X + b.X) / 2, (a.Y + b.Y) / 2), this, Vertex.VertexType.Center));
            return rPoints;

        }
        public void Move(int x, int y)
        {
            a.X += x;
            a.Y += y;
            b.X += x;
            b.Y += y;
        }
    }
    internal class ThickLine : IShape // -----------------------------------------------------------
    {
        Point a, b;
        public bool AA { get; set; }
        private int _width;
        public int width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
                brush = new int[_width, _width];
                for (int xo = 0; xo < this._width; xo++)
                {
                    for (int yo = 0; yo < this._width; yo++)
                    {
                        if ((xo - (_width / 2)) * (xo - (_width / 2)) + (yo - (_width / 2)) * (yo - (_width / 2)) <= _width * _width / 4)
                        {
                            brush[xo, yo] = 1;
                        }
                        else
                            brush[xo, yo] = 0;
                    }
                }
            }
        }
        public Color color { get; set; }
        int[,] brush;
        public ThickLine(Point a, Point b, int width, Color color)
        {
            this.a = a;
            this.b = b;
            this.width = width;
            this.color = color;
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
        public List<Vertex> GetVertices()
        {
            List<Vertex> rPoints = new List<Vertex>();
            rPoints.Add(new Vertex(a, this, Vertex.VertexType.Normal));
            rPoints.Add(new Vertex(b, this, Vertex.VertexType.Normal));
            //rPoints.Add(new Vertex(new Point((a.X + b.X) / 2, (a.Y + b.Y) / 2), this, Vertex.VertexType.Center));
            return rPoints;

        }
        public void Move(int x, int y)
        {
            a.X += x;
            a.Y += y;
            b.X += x;
            b.Y += y;
        }
    }
    internal class Circle : IShape // -----------------------------------------------------------
    {
        public Point center;
        public bool AA { get; set; }
        private int _radius;
        public int radius
        {
            get
            {
                return _radius;
            }
            set
            {
                _radius = value;
                this.r.X = center.X + _radius;
                this.r.Y = center.Y;
                this.l.X = center.X - _radius;
                this.l.Y = center.Y;
                this.b.Y = center.Y + _radius;
                this.b.X = center.X;
                this.t.Y = center.Y - _radius;
                this.t.X = center.X;
            }
        }
        public int width { get; set; }
        public Color color { get; set; }
        private Point r, t, b, l;
        public Circle(Point center, int radius, Color color)
        {
            this.center = center;
            this._radius = radius;
            this.color = color;
            this.width = 1;
            this.r = new Point(center.X + radius, center.Y);
            this.l = new Point(center.X - radius, center.Y);
            this.b = new Point(center.X, center.Y + radius);
            this.t = new Point(center.X, center.Y - radius);
        }
        public Circle(string text)
        {
            string[] elements = text.Split(',');
            center = new Point(Int32.Parse(elements[0]), Int32.Parse(elements[1]));
            _radius = Int32.Parse(elements[2]);
            color = Color.FromArgb(Convert.ToInt32(elements[3], 16));
            width = 1;
        }
        public void Draw(byte[] bitmap, int stride) // loosely based on this: https://www.geeksforgeeks.org/mid-point-circle-drawing-algorithm/
        {
            int x = _radius;
            int y = 0;
            int P = 1 - _radius;
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
            return "C;" + center.X.ToString() + "," + center.Y.ToString() + "," + _radius.ToString() + "," + string.Format("{0:x6}", color.ToArgb());
        }
        private void PixelSet(byte[] pictureData, int i, Color c)
        {
            if (i < 0 || i + 2 >= pictureData.Length)
                return;
            pictureData[i] = (byte)c.B;
            pictureData[i + 1] = (byte)c.G;
            pictureData[i + 2] = (byte)c.R;
        }
        public List<Vertex> GetVertices()
        {
            List<Vertex> rPoints = new List<Vertex>();
            rPoints.Add(new Vertex(center, this, Vertex.VertexType.Normal));
            rPoints.Add(new Vertex(r, this, Vertex.VertexType.Circumference));
            rPoints.Add(new Vertex(l, this, Vertex.VertexType.Circumference));
            rPoints.Add(new Vertex(b, this, Vertex.VertexType.Circumference));
            rPoints.Add(new Vertex(t, this, Vertex.VertexType.Circumference));
            return rPoints;

        }
        public void Move(int x, int y)
        {
            center.X += x;
            center.Y += y;
            this.r.X = x + _radius;
            this.r.Y = y;
            this.l.X = x - _radius;
            this.l.Y = y;
            this.b.Y = y + _radius;
            this.b.X = x;
            this.t.Y = y - _radius;
            this.t.X = x;
        }
    }
    internal class AALine : IShape // -----------------------------------------------------------
    {
        public Point a, b;
        public Color color { get; set; }
        public int width { get; set; }
        public bool AA { get; set; }
        public Point midpoint;
        public AALine(Point a, Point b, int width, Color color, bool aa = false)
        {
            this.a = a;
            this.b = b;
            this.width = width + 2;
            this.color = color;
            this.AA = aa;
            midpoint = new Point((a.X + b.X) / 2, (a.Y + b.Y) / 2);
        }
        public AALine(string text)
        {
            string[] elements = text.Split(',');
            a = new Point(Int32.Parse(elements[0]), Int32.Parse(elements[1]));
            b = new Point(Int32.Parse(elements[2]), Int32.Parse(elements[3]));
            width = Int32.Parse(elements[4]);
            color = Color.FromArgb(Convert.ToInt32(elements[5], 16));
            AA = false;
            midpoint = new Point((a.X + b.X) / 2, (a.Y + b.Y) / 2);
        }
        public void Draw(byte[] bitmap, int stride)
        {
            RecalcCenter();
            int x = this.a.X;
            int y = this.a.Y;
            int dx = Math.Abs(this.b.X - this.a.X);
            int dy = Math.Abs(this.b.Y - this.a.Y);
            int sx = this.b.X > this.a.X ? 1 : -1;
            int sy = this.b.Y > this.a.Y ? 1 : -1;
            double D = 2 * dy - dx;
            double d;
            int i;
            double W = this.width / 2;
            double two_v_dx = 0;
            int halfWidth = this.width / 2;
            double lineLength = Math.Sqrt(dx * dx + dy * dy);
            if (dy <= dx)
            {
                for (int j = 0; j <= dx; j++)
                {
                    d = Math.Abs(two_v_dx) / Math.Sqrt((double)(dx * dx + dy * dy));


                    for (int offset = -halfWidth; offset <= halfWidth; offset++)
                    {
                        int yy = y + offset;
                        double distToCenter = Math.Abs(offset); // perpendicular offset in pixels
                        double brightness;
                        if (AA)
                            brightness = intensity(distToCenter);
                        else
                            brightness = 1;
                        if (brightness > 0)
                        {
                            i = (yy * stride) + (x * 3);
                            PixelSet(bitmap, i, brightness);
                        }
                    }
                    if (D > 0)
                    {
                        y+=sy;
                        D -= 2 * dx;
                    }
                    D += 2 * dy;
                    x+=sx;
                    two_v_dx += 2 * dy;
                }
            }
            else
            {
                double two_v_dy = 0;
                D = 2 * dx - dy;
                for (int j = 0; j <= dy; j++)
                {
                    for (int offset = -halfWidth; offset <= halfWidth; offset++)
                    {
                        int xx = x + offset;
                        double distToCenter = Math.Abs(offset);
                        double brightness;
                        if (AA)
                            brightness = intensity(distToCenter);
                        else
                            brightness = 1;
                        if (brightness > 0)
                        {
                            i = (y * stride) + (xx * 3);
                            PixelSet(bitmap, i, brightness);
                        }
                    }

                    if (D > 0)
                    {
                        x += sx;
                        D -= 2 * dy;
                    }
                    D += 2 * dx;
                    y += sy;
                    two_v_dy += 2 * dx;
                }
            }

        }
        public override string ToString()
        {
            return "A;" + a.X.ToString() + "," + a.Y.ToString() + "," + b.X.ToString() + "," + b.Y.ToString() + "," + this.width.ToString() + "," + string.Format("{0:x6}", color.ToArgb());
        }
        private void PixelSet(byte[] pictureData, int i, double intensity)
        {
            if (i < 0 || i + 2 >= pictureData.Length)
                return;
            pictureData[i]     = (byte)Math.Round(Clamp(pictureData[i]     * (1.0 - intensity) + (this.color.B*intensity)));
            pictureData[i + 2] = (byte)Math.Round(Clamp(pictureData[i + 2] * (1.0 - intensity) + (this.color.R*intensity)));
            pictureData[i + 1] = (byte)Math.Round(Clamp(pictureData[i + 1] * (1.0 - intensity) + (this.color.G*intensity)));
        }
        private double Clamp(double value)
        {
            if (value >= 255)
                return 255;
            return value;
        }
        private double intensity(double d)
        {
            double radius = this.width / 2.0;
            if (d > radius) return 0;
            return 1.0 - (d / radius); // linear fade
        }
        public List<Vertex> GetVertices()
        {
            List<Vertex> rPoints = new List<Vertex>();
            rPoints.Add(new Vertex(a, this, Vertex.VertexType.Normal));
            rPoints.Add(new Vertex(b, this, Vertex.VertexType.Normal));
            rPoints.Add(new Vertex(midpoint, this, Vertex.VertexType.Center));
            return rPoints;

        }
        public void Move(int x, int y)
        {
            a.X += x;
            a.Y += y;
            b.X += x;
            b.Y += y;
        }
        public void RecalcCenter()
        {
            midpoint.X = (a.X + b.X) / 2;
            midpoint.Y = (a.Y + b.Y) / 2;
        }
    }
    class Polygon : IShape // -----------------------------------------------------------
    {
        protected List<Point> points;
        protected List<Point> midpoints;
        private bool aa;
        public bool AA
        {
            get
            {
                return aa;
            }
            set
            {
                aa = value;
                foreach(AALine line in lines)
                {
                    line.AA = value;
                }
            }
        }
        private int _width;
        private Color _color;
        public int width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
                foreach (AALine line in lines)
                {
                    line.width = value;
                }
                //GenerateLines();
            }
        }
        public Color color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
                foreach (AALine line in lines)
                {
                    line.color = value;
                }
                //GenerateLines();
            }
        }
        private List<AALine> lines;
        private bool _closed;
        public bool Closed
        {
            get
            {
                return _closed;
            }
            set
            {
                _closed = value;
                GenerateLines();
            }
        }
        public Polygon(Color color, List<Point> points, int width = 1, bool closed = true, bool aa = false)
        {
            this._color = color;
            this.points = points;
            this._width = width;
            this.lines = new List<AALine>();
            this.midpoints = new List<Point>();
            this._closed = closed;
            this.AA = aa;
            GenerateLines();
        }
        public Polygon(string text) : this(
            Color.FromArgb(Convert.ToInt32(text.Split('|')[0].Split(',')[0], 16)),
            new List<Point>(),
            Int32.Parse(text.Split('|')[0].Split(',')[1]),
            (text.Split('|')[0].Split(',')[2]=="C"))
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
            //throw new Exception(points.Count.ToString());
            this.lines.Clear();
            for(int i = 0; i<points.Count-1;i++)
            {
                lines.Add(new AALine(points[i], points[i+1],this._width,this._color,this.AA));
            }
            if(_closed)
            {
                lines.Add(new AALine(points.Last(), points[0], this._width, this._color,this.AA));
            }
            for( int i = 0; i<lines.Count; i++)
            {
                if (i>=midpoints.Count)
                {
                    midpoints.Add(lines[i].midpoint);
                }
                else
                    lines[i].midpoint = midpoints[i];
            }
        }
        public override string ToString()
        {
            string pointsText = "";
            foreach (Point point in points)
            {
                pointsText += point.X.ToString() + "," + point.Y.ToString() + ",";
            }
            return "P;" + string.Format("{0:x6}", this._color.ToArgb()) + "," + this._width.ToString() + "," + ((this._closed==true) ? "C" : "O") + "|" + pointsText;
        }
        public void Draw(byte[] bitmap, int stride)
        {
            if (_closed && lines.Count != points.Count)
                GenerateLines();
            if (!_closed && lines.Count != points.Count - 1)
                GenerateLines();
            foreach(AALine line in lines)
            {
                line.Draw(bitmap, stride);
            }
        }
        public void Add(Point point)
        {
            points.Add(point);
            if(this._closed)
            {
                GenerateLines();
                return;
            }
            lines.Add(new AALine(points[points.Count - 2], point, this._width, this._color, this.AA));
        }
        public List<Vertex> GetVertices()
        {
            List<Vertex> rPoints = new List<Vertex>();
            for(int i = 0; i<points.Count; i++)
            {
                rPoints.Add(new Vertex(points[i], this, Vertex.VertexType.Normal));
                if(_closed||i<points.Count-1)
                    rPoints.Add(new Vertex(lines[i].midpoint, lines[i], Vertex.VertexType.Center));
            }
            return rPoints;

        }
        public void Move(int x, int y)
        {
            foreach(Point point in points)
            {
                point.X += x;
                point.Y += y;
            }
            GenerateLines();
        }
    }
    class PacMan : IShape // -----------------------------------------------------------
    {
        public int width { get; set; }
        private Color _color;
        public Color color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
                this.ab = new Line(this.center, this.b, _color);
                this.ac = new Line(this.center, this.c, _color);
            }
        }
        public bool AA { get; set; }
        public Point center { get; set; }
        private int radius;
        private double startAngle;
        private double endAngle;
        public Point b {  get; set; }
        public Point c { get; set; }
        private Line ab;
        private Line ac;
        private int stride;
        private bool startAngleSmaller;
        public PacMan(Point a, Point b, Point c, Color color)
        {
            this._color = color;
            this.center = a;
            this.b = b;
            int dx = b.X - a.X;
            int dy = b.Y - a.Y;
            this.radius = (int)Math.Sqrt((double)(dx * dx + dy * dy));
            double angle;
                angle = Math.Atan2(dy,dx);
            this.startAngle = angle;

            dx = c.X - a.X;
            dy = c.Y - a.Y;
            angle = Math.Atan2(dy,dx);
            this.endAngle = angle;

            startAngleSmaller = (this.startAngle < this.endAngle);

            this.c = new Point((int)((double)radius*Math.Cos(endAngle)) + center.X, (int)((double)radius * Math.Sin(endAngle))+center.Y);
            this.ab = new Line(this.center, this.b, color);
            this.ac = new Line(this.center, this.c, color);
        }
        public void Draw(byte[] bitmap, int stride)
        {
            int i;
            //start with the cirle :::::::::::::::::::::
            this.stride = stride;
            int x = radius;
            int y = 0;
            int P = 1 - radius;
            PixelSet(bitmap, x, 0, color);
            PixelSet(bitmap, -x,0, color);
            PixelSet(bitmap, 0,x, color);
            PixelSet(bitmap, 0, -x, color);
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
                
                
                PixelSet(bitmap, x, y, color);
                PixelSet(bitmap, -x, y, color);
                PixelSet(bitmap, x, -y, color);
                PixelSet(bitmap, -x, -y, color);
                if (x != y)
                {
                    PixelSet(bitmap, y, x, color);
                    PixelSet(bitmap, y, -x, color);
                    PixelSet(bitmap, -y, x, color);
                    PixelSet(bitmap, -y, -x, color);
                }
            }

            //draw the lines :::::::::::::::::::::::::::
            ab.Draw(bitmap, stride);
            ac.Draw(bitmap, stride);
        }
        public void Move(int x, int y)
        {
            center.X += x;
            center.Y += y;
            b.X += x;
            b.Y += y;
            c.X += x;
            c.Y += y;
        }
        public List<Vertex> GetVertices()
        {
            List<Vertex> rPoints = new List<Vertex>();
            rPoints.Add(new Vertex(center, this, Vertex.VertexType.Normal));
            return rPoints;
        }
        private void PixelSet(byte[] pictureData, int x, int y, Color c)
        {
            int i = (y + this.center.Y) * this.stride + (x + this.center.X) * 3;
            if (i < 0 || i + 2 >= pictureData.Length)
                return;
            if (!CheckPoint(x, y))
                return;
            pictureData[i] = (byte)c.B;
            pictureData[i + 1] = (byte)c.G;
            pictureData[i + 2] = (byte)c.R;
        }
        private bool CheckPoint(int dx, int dy)
        {
            //return true;
            double angle = Math.Atan2(dy,dx);

            if(startAngleSmaller)
            {
                if (angle <= startAngle || angle >= endAngle)
                    return false;
            }
            else
            {
                if (angle >= startAngle || angle <= endAngle)
                    return false;
            }
            return true;
        }
    }
}