using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace GKProjekt1
{
    public class MyPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public Ellipse Verticle { get; set; }

        public MyPoint()
        {

        }

        public MyPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public void Offset(double offsetX, double offsetY)
        {
            X += offsetX;
            Y += offsetY;
        }
    }
}
