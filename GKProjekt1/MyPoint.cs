using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace GKProjekt1
{
    public class MyPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public Ellipse ellipse { get; set; }

        public MyPoint()
        {
            X = 0.0;
            Y = 0.0;
        }

        public MyPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public void Move(double newX, double newY)
        {
            X = newX;
            Y = newY;
            Canvas.SetLeft(ellipse, newX - (double)Globals.VerticleSize / 2.0);
            Canvas.SetTop(ellipse, newY - (double)Globals.VerticleSize / 2.0);
        }

        public void Move(Point p)
        {
            X = p.X;
            Y = p.Y;
            Canvas.SetLeft(ellipse, p.X - (double)Globals.VerticleSize / 2.0);
            Canvas.SetTop(ellipse, p.Y - (double)Globals.VerticleSize / 2.0);
        }

        public static bool AreNear(MyPoint p1, MyPoint p2, double radius)
        {
            return Math.Pow(radius, 2) > Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2);
        }
        public static bool AreNear(Point p1, MyPoint p2, double radius)
        {
            return Math.Pow(radius, 2) > Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2);
        }

        public static bool AreNear(MyPoint p1, Point p2, double radius)
        {
            return Math.Pow(radius, 2) > Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2);
        }

        public static bool CheckIfCollinear(MyPoint p1, MyPoint p2, MyPoint p3)
        {
            return (p1.X - p2.X) * (p3.Y - p2.Y) - (p3.X - p2.X) * (p1.Y - p2.Y) == 0;
        }

        public static bool operator ==(MyPoint p1, MyPoint p2)
        {
            if (p1.X == p2.X && p1.Y == p2.Y)
                return true;
            else
                return false;
        }

        public static bool operator !=(MyPoint p1, MyPoint p2)
        {
            return !(p1 == p2);
        }

        public override bool Equals(object obj)
        {
            if (obj is MyPoint e)
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
