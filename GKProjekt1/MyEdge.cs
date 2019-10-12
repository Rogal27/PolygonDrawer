using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace GKProjekt1
{
    public class MyEdge
    {
        public Point first { get; set; }
        public Point second { get; set; }

        public Line line { get; set; }
        public Ellipse firstEllipse { get; set; }
        public Ellipse secondEllipse { get; set; }

        public MyEdge(Point first, Point second)
        {
            this.first = first;
            this.second = second;
        }
        public MyEdge(Point first, Point second, Ellipse firstEllipse, Ellipse secondEllipse, Line line)
        {
            this.first = first;
            this.second = second;
            this.firstEllipse = firstEllipse;
            this.secondEllipse = secondEllipse;
            this.line = line;
        }
        public static bool operator ==(MyEdge e1, MyEdge e2)
        {
            if (e1.first == e2.first && e1.second == e2.second)
                return true;
            if (e1.first == e2.second && e1.second == e2.first)
                return true;
            return false;
        }
        public static bool operator !=(MyEdge e1, MyEdge e2)
        {
            return !(e1 == e2);
        }
        public override bool Equals(object obj)
        {
            if (obj is MyEdge e)
            {
                return this == e;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
