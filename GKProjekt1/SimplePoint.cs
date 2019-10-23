using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GKProjekt1
{
    public class SimplePoint
    {
        public double X { get; set; }
        public double Y { get; set; }

        public SimplePoint()
        {

        }

        public SimplePoint(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

    public class SimplePointColor
    {
        public double X { get; set; }
        public double Y { get; set; }
        public Color color { get; set; }

        public SimplePointColor()
        {

        }

        public SimplePointColor(double x, double y, Color c)
        {
            X = x;
            Y = y;
            color = c;
        }
    }
}
