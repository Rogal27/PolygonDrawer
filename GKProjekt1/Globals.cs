using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GKProjekt1
{
    public static class Globals
    {
        public static bool __Test__ = true;
        public static Brush DefaultVerticleColor = new SolidColorBrush(Color.FromRgb(70, 73, 235));
        public static Brush DefaultEdgeColor = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        public static int VerticleSize = 10;
        public static int VerticleClickRadiusSize = 15;
        public static int LineThickness = 4;
        public static int VerticleZIndex = 10;
        public static int LineZIndex = 5;
    }
}