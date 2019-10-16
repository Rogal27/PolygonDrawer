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
        public MyPoint first { get; set; }
        public MyPoint second { get; set; }
        public Line line { get; set; }
        //public Ellipse firstEllipse { get; set; }
        //public Ellipse secondEllipse { get; set; }

        public MyEdge(MyPoint first, MyPoint second)
        {
            this.first = first;
            this.second = second;
        }

        public MyEdge(MyPoint first, MyPoint second, Line line)
        {
            this.first = first;
            this.second = second;
            //this.firstEllipse = firstEllipse;
            //this.secondEllipse = secondEllipse;
            this.line = line;
        }

        public void MoveWithPoints()
        {
            line.X1 = first.X;
            line.Y1 = first.Y;
            line.X2 = second.X;
            line.Y2 = second.Y;
        }

        private Point FindClosestPointOnEdge(Point p)
        {
            Point EdgePoint = new Point();
            (double, double) vectorAP = (p.X - first.X, p.Y - first.Y);       //Vector from A to P   
            (double, double) vectorAB = (second.X - first.X, second.Y - first.Y);       //Vector from A to B  

            double magnitudeAB = vectorAB.Item1 * vectorAB.Item1 + vectorAB.Item2 * vectorAB.Item2;   //Magnitude of AB vector (it's length squared)     
            double ABAPproduct = vectorAB.Item1 * vectorAP.Item1 + vectorAB.Item2 + vectorAP.Item2;    //The DOT product of a_to_p and a_to_b     
            double distance = ABAPproduct / magnitudeAB; //The normalized "distance" from a to your closest point  

            if (distance < 0)     //Check if P projection is over vectorAB     
            {
                EdgePoint.X = first.X;
                EdgePoint.Y = first.Y;

            }
            else if (distance > 1)
            {
                EdgePoint.X = second.X;
                EdgePoint.Y = second.Y;
            }
            else
            {
                EdgePoint.X = first.X + vectorAB.Item1 * distance;
                EdgePoint.Y = first.Y + vectorAB.Item2 * distance;
            }
            return EdgePoint;
        }

        public void MoveParallel(Point p)
        {
            Point offset = FindClosestPointOnEdge(p);
            var x = p.X - offset.X;
            var y = p.Y - offset.Y;
            first.Move(first.X + x, first.Y + y);
            second.Move(second.X + x, second.Y + y);
            MoveWithPoints();
        }

        public bool IsNearPoint(Point p, double distance)
        {
            var x = Math.Min(first.X, second.X);
            var y = Math.Min(first.Y, second.Y);
            //var newPoint = new Point(p.X - x, p.Y - y);

            if ((p.X + distance) > x &&
                (p.Y + distance) > y &&
                (p.X - x - distance) < Math.Abs(first.X - second.X) &&
                (p.Y - y - distance) < Math.Abs(first.Y - second.Y))
            {
                //check near line
                var result =  Math.Abs((second.X - first.X) * (first.Y - p.Y) - (first.X - p.X) * (second.Y - first.Y)) /
                    Math.Sqrt(Math.Pow(second.X - first.X, 2) + Math.Pow(second.Y - first.Y, 2));
                if (result < distance)
                    return true;
            }
            return false;
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
