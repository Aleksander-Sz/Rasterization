using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CG_Project3
{
    internal class Vertex
    {
        public Point Point {  get; set; }
        public IShape Owner { get; set; }
        public VertexType Type { get; set; }
        public int Index { get; set; }
        public Vertex(Point point, IShape owner, VertexType type, int index = 0)
        {
            Point = point;
            Owner = owner;
            Type = type;
            Index = index;
        }
        public enum VertexType
        {
            Normal,
            Center,
            Circumference,
            Rectangle
        }
    }
}
