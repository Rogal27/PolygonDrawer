using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GKProjekt1
{
    public static class RelationFixer
    {
        public static void FixEqualRelation(MyPoint first, MyPoint second, MyEdge edge, ref bool endLoop, ref bool success)
        {
            var relationEdge = edge.relationEdge;
            var length = edge.Length();
            var relationEdgeLength = relationEdge.Length();
            if (length < Globals.eps || relationEdgeLength < Globals.eps)
            {
                endLoop = true;
                success = true;
                return;
            }
            var scale = relationEdgeLength / length;
            second.X = first.X + (second.X - first.X) * scale;
            second.Y = first.Y + (second.Y - first.Y) * scale;
        }

        public static void FixPerpendicularRelation(MyPoint edgeFirst, MyPoint edgeSecond, MyPoint relationFirst, MyPoint relationSecond, bool recursive = true)
        { 
            Vector v1 = new Vector(relationSecond.X - relationFirst.X, relationSecond.Y - relationFirst.Y);
            Vector v2 = new Vector(relationSecond.Y - relationFirst.Y, relationFirst.X - relationSecond.X);

            Point APoint = new Point(edgeFirst.X, edgeFirst.Y);
            Point BPoint = APoint + v2;
            Point CPoint = new Point(edgeSecond.X, edgeSecond.Y);
            Point DPoint = CPoint + v1;

            Point? intersection = PointExtension.IntersectionPoint(APoint, BPoint, CPoint, DPoint);

            if (intersection.HasValue == true)
            {
                edgeSecond.X = intersection.Value.X;
                edgeSecond.Y = intersection.Value.Y;
            }
            else
            {
                edgeFirst.X += 5.0;
                edgeFirst.Y += 5.0;

                if (recursive == true)
                    FixPerpendicularRelation(edgeFirst, edgeSecond, relationFirst, relationSecond, false);
            }
        }
    }
}
