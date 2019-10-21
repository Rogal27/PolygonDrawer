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
        public static bool __BresenhamOff__ = true;
        public static string WindowName = "Polygons";
        //colors
        public static Color DefaultVerticleColor = Color.FromRgb(70, 73, 235);
        public static Color DefaultEdgeColor = Color.FromRgb(0, 0, 0);
        public static Color SelectedEdgeColor = Color.FromRgb(255, 0, 255);
        public static Color RelationFontColor = Color.FromRgb(255, 0, 0);
        //buttons grid
        public static int ButtonsGridZIndex = 100;
        //verticle
        public static int VerticleSize = 10;
        public static int VerticleClickRadiusSize = 15;
        public static int VerticleZIndex = 10;
        //edges
        public static int LineThickness = 4;
        public static int LineClickDistance = 5;        
        public static int LineZIndex = 5;
        //selected edge
        public static int SelectedEdgeBlurRadius = 10;
        //bitmaps
        public static string EqualBitmapSource = "..\\Bitmaps\\EqualBitmap.bmp";
        public static string PerpendicularBitmapSource = "..\\Bitmaps\\PerpendicularBitmap.bmp";
        public static int BitmapSize = 24;
        public static int ImageZIndex = 15;
        public static double ImagePositionScale = 20.0; 
        //relation text
        public static int RelationFontSize = 16;
        //Math
        public static double eps = 1e-3;
    }
}