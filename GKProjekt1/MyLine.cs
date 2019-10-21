using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GKProjekt1
{
    public class MyLine
    {
        public Line lineWindowsControl { get; set; }
        public Image lineBresenham { get; set; }
        public Point firstPoint { get; set; }
        public Point secondPoint { get; set; }
        public Canvas canvas { get; set; }

        public MyLine()
        {
        }

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
                firstPoint = new Point(first.X, first.Y);
                secondPoint = new Point(second.X, second.Y);
                DrawLine(Globals.DefaultEdgeColor);
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
                firstPoint = first;
                DrawLine(Globals.DefaultEdgeColor);
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
                secondPoint = second;
                DrawLine(Globals.DefaultEdgeColor);
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
                DrawLine(Globals.SelectedEdgeColor);
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
                DrawLine(Globals.DefaultEdgeColor);
            }
        }

        public void DeleteDrawing()
        {
            if (Globals.__BresenhamOff__ == true)
            {
                canvas.Children.Remove(lineWindowsControl);
            }
            else
            {
                canvas.Children.Remove(lineBresenham);
            }
        }

        public static List<SimplePoint> BresenhamLine(Point first, Point second)
        {
            List<SimplePoint> rectangleList = new List<SimplePoint>();

            //not my bresenham!!!!
            int w = (int)(second.X - first.X);
            int h = (int)(second.Y - first.Y);
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
            int longest = Math.Abs(w);
            int shortest = Math.Abs(h);
            if (!(longest > shortest))
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
                dx2 = 0;
            }
            int numerator = longest >> 1;
            for (int i = 0; i <= longest; i++)
            {
                rectangleList.Add(new SimplePoint(first.X, first.Y));
                //rectangleList.Add(PutRectangle(first.X, first.Y, color));
                //rectangleList.Add(PutRectangle(first.X, first.Y+1, color));
                //rectangleList.Add(PutRectangle(first.X, first.Y-1, color));
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    first.X += dx1;
                    first.Y += dy1;
                }
                else
                {
                    first.X += dx2;
                    first.Y += dy2;
                }
            }

            return rectangleList;
        }

        //public static Rectangle PutRectangle(double X, double Y, Color color)
        //{
        //    Rectangle rectangle = new Rectangle
        //    {
        //        Width = 1.0,
        //        Height = 1.0,
        //        Fill = new SolidColorBrush(color)
        //    };
        //    Canvas.SetLeft(rectangle, X);
        //    Canvas.SetTop(rectangle, Y);
        //    Panel.SetZIndex(rectangle, Globals.LineZIndex);
        //    return rectangle;
        //}

        public void DrawLine(Color color)
        {
            if (Globals.__BresenhamOff__ == true)
                return;

            List<SimplePoint> pointsList = BresenhamLine(firstPoint, secondPoint);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                Brush brush = new SolidColorBrush(color);

                foreach (var point in pointsList)
                {
                    Rect rect = new Rect(point.X, point.Y, 1, 1);
                    dc.DrawRectangle(brush, null, rect);
                }

            }
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)canvas.ActualWidth, (int)canvas.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(dv);

            Image img = new Image();
            img.Source = rtb;

            img.IsHitTestVisible = false;
            
            canvas.Children.Add(img);

            Panel.SetZIndex(img, Globals.LineZIndex);
            canvas.Children.Remove(lineBresenham);
            lineBresenham = img;
        }

        //private void DrawRubbish2()
        //{
        //    DrawingVisual dv = new DrawingVisual();
        //    using (DrawingContext dc = dv.RenderOpen())
        //    {
        //        Random rand = new Random();

        //        for (int i = 0; i < 200; i++)
        //            dc.DrawRectangle(Brushes.Red, null, new Rect(rand.NextDouble() * 200, rand.NextDouble() * 200, 1, 1));

        //        dc.Close();
        //    }
        //    RenderTargetBitmap rtb = new RenderTargetBitmap(200, 200, 96, 96, PixelFormats.Pbgra32);
        //    rtb.Render(dv);
        //    Image img = new Image();
        //    img.Source = rtb;
        //    canvas.Children.Add(img);
        //}

        //private void RectangleInCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    var rect = sender as Image;
        //    Point p = e.GetPosition(rect);
        //    Debug.WriteLine($"Hit: ({p.X};{p.Y})");
        //}


    }
}
