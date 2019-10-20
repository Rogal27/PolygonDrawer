using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GKProjekt1
{
    public static class PointExtension
    {
        public static bool AreNear(Point p1, Point p2, double radius)
        {
            return Math.Pow(radius, 2) > Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2);
        }

        public static Point? IntersectionPoint(Point A, Point B, Point C, Point D)
        {
            double d = (D.Y - C.Y) * (B.X - A.X) - (D.X - C.X) * (B.Y - A.Y);

            double n_a = (D.X - C.X) * (A.Y - C.Y) - (D.Y - C.Y) * (A.X - C.X);

            double n_b = (B.X - A.X) * (A.Y - C.Y) - (B.Y - A.Y) * (A.X - C.X);

            if (Math.Abs(d) < Globals.eps)
                return null;

            double ua = n_a / d;
            double ub = n_b / d;

            Point intersection = new Point();

            intersection.X = A.X + (ua * (B.X - A.X));
            intersection.Y = A.Y + (ua * (B.Y - A.Y));

            return intersection;
        }
    }
}
