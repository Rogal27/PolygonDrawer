using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace GKProjekt1
{
    public class MyEdge
    {
        public MyPoint first { get; set; }
        public MyPoint second { get; set; }
        public Line line { get; set; }
        public RelationIcon relationIcon { get; set; } = null;
        public RelationType relationType { get; set; } = RelationType.None;
        public MyEdge relationEdge { get; set; } = null;

        public MyEdge(MyPoint first, MyPoint second)
        {
            this.first = first;
            this.second = second;
        }

        public MyEdge(MyPoint first, MyPoint second, Line line)
        {
            this.first = first;
            this.second = second;
            this.line = line;
        }

        //BRESENHAM!
        public void MoveWithPoints()
        {
            line.X1 = first.X;
            line.Y1 = first.Y;
            line.X2 = second.X;
            line.Y2 = second.Y;
            relationIcon?.MoveIcon();
        }

        //BRESENHAM!
        public void MoveParallel(Point startPoint, Point endPoint)
        {
            var x = endPoint.X - startPoint.X;
            var y = endPoint.Y - startPoint.Y;
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

        public static bool DoIntersect(MyEdge e1, MyEdge e2)
        {
            var d1 = MyPoint.VectorProduct((e2.second - e2.first), (e1.first - e2.first));
            var d2 = MyPoint.VectorProduct((e2.second - e2.first), (e1.second - e2.first));
            var d3 = MyPoint.VectorProduct((e1.second - e1.first), (e2.first - e1.first));
            var d4 = MyPoint.VectorProduct((e1.second - e1.first), (e2.second - e1.first));

            var d12 = d1 * d2;
            var d34 = d3 * d4;

            if (d12 > 0 || d34 > 0)
                return false;

            if (d12 < 0 || d34 < 0)
                return true;

            return MyPoint.OnRectangle(e1.first, e2.first, e2.second) ||
                MyPoint.OnRectangle(e1.second, e2.first, e2.second) ||
                MyPoint.OnRectangle(e2.first, e1.first, e1.second) ||
                MyPoint.OnRectangle(e2.second, e1.first, e1.second);
        }

        public double Length()
        {
            double x = first.X - second.X;
            double y = first.Y - second.Y;
            return Math.Sqrt(x * x + y * y);
        }

        //BRESENHAM!
        public void SelectEdge()
        {
            var effect = new DropShadowEffect();
            effect.BlurRadius = Globals.SelectedEdgeBlurRadius;
            effect.Color = Globals.SelectedEdgeColor;
            effect.Direction = 0;
            effect.ShadowDepth = 0;
            effect.Opacity = 0.9;
            line.Stroke = new SolidColorBrush(Globals.SelectedEdgeColor);
            line.Effect = effect;
        }

        //BRESENHAM!
        public void UnselectEdge()
        {
            line.Stroke = new SolidColorBrush(Globals.DefaultEdgeColor);
            line.Effect = null;
        }

        public void DeleteRelation()
        {
            if (relationType != RelationType.None)
            {
                relationIcon.Delete();
                relationIcon = null;
                relationEdge.relationIcon.Delete();
                relationEdge.relationIcon = null;
                relationType = RelationType.None;
                relationEdge.relationType = RelationType.None;
                relationEdge.relationEdge = null;
                relationEdge = null;
            }
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
