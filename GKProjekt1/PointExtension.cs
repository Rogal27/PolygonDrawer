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
    }
}
