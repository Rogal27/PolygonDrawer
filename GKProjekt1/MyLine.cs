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
    public class MyLine
    {
        public Line lineWindowsControl { get; set; }

        public List<Rectangle> rectangleLine { get; set; } = new List<Rectangle>();

        public Point firstPoint { get; set; }
        public Point secondPoint { get; set; }

        public MyLine()
        {
            if(Globals.__BresenhamOff__ == true)
            {

            }
            else
            {

            }
        }

        //public void SetPoints(Point first, Point second)
        //{
        //    if (Globals.__BresenhamOff__ == true)
        //    {
        //        lineWindowsControl.X1 = first.X;
        //        lineWindowsControl.Y1 = first.Y;
        //        lineWindowsControl.X2 = second.X;
        //        lineWindowsControl.Y2 = second.Y;
        //    }
        //    else
        //    {

        //    }
        //}

        public void SetPoints(MyPoint first, MyPoint second)
        {
            if (Globals.__BresenhamOff__ == true)
            {
                lineWindowsControl.X1 = first.X;
                lineWindowsControl.Y1 = first.Y;
                lineWindowsControl.X2 = second.X;
                lineWindowsControl.Y2 = second.Y;
            }
            else
            {

            }
        }

        public void SetFirstPoint(Point first)
        {
            if (Globals.__BresenhamOff__ == true)
            {
                lineWindowsControl.X1 = first.X;
                lineWindowsControl.Y1 = first.Y;
            }
            else
            {

            }
        }

        public void SetSecondPoint(Point second)
        {
            if (Globals.__BresenhamOff__ == true)
            {
                lineWindowsControl.X2 = second.X;
                lineWindowsControl.Y2 = second.Y;
            }
            else
            {

            }
        }

        public void SelectEdge()
        {
            if (Globals.__BresenhamOff__ == true)
            {
                var effect = new DropShadowEffect();
                effect.BlurRadius = Globals.SelectedEdgeBlurRadius;
                effect.Color = Globals.SelectedEdgeColor;
                effect.Direction = 0;
                effect.ShadowDepth = 0;
                effect.Opacity = 0.9;
                lineWindowsControl.Stroke = new SolidColorBrush(Globals.SelectedEdgeColor);
                lineWindowsControl.Effect = effect;
            }
            else
            {

            }
        }

        public void UnselectEdge()
        {
            if (Globals.__BresenhamOff__ == true)
            {
                lineWindowsControl.Stroke = new SolidColorBrush(Globals.DefaultEdgeColor);
                lineWindowsControl.Effect = null;
            }
            else
            {

            }
        }

        public void DeleteDrawing(Canvas canvas)
        {
            if (Globals.__BresenhamOff__ == true)
            {
                canvas.Children.Remove(lineWindowsControl);
            }
            else
            {

            }
        }
    }
}
