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

        //public MyLine()
        //{

        //}
        public MyLine(Canvas canvas)
        {
            this.canvas = canvas;
            switch (Globals.lineDrawingMode)
            {
                case LineDrawingMode.Bresenham:
                    lineBresenham = new Image();
                    lineBresenham.IsHitTestVisible = false;
                    Panel.SetZIndex(lineBresenham, Globals.LineZIndex);
                    this.canvas = canvas;
                    this.canvas.Children.Add(lineBresenham);
                    break;
                case LineDrawingMode.Library:
                    break;
                case LineDrawingMode.AntialiasingWU:
                    break;
                case LineDrawingMode.BresenhamSymmetric:
                    break;
                default:
                    break;
            }
        }

        public void SetPoints(MyPoint first, MyPoint second)
        {
            switch (Globals.lineDrawingMode)
            {
                case LineDrawingMode.Bresenham:
                    firstPoint = new Point(first.X, first.Y);
                    secondPoint = new Point(second.X, second.Y);
                    DrawLine(Globals.DefaultEdgeColor);
                    break;
                case LineDrawingMode.Library:                    
                    lineWindowsControl.X1 = first.X;
                    lineWindowsControl.Y1 = first.Y;
                    lineWindowsControl.X2 = second.X;
                    lineWindowsControl.Y2 = second.Y;
                    break;
                case LineDrawingMode.AntialiasingWU:
                    //TODO:
                    lineWindowsControl.X1 = first.X;
                    lineWindowsControl.Y1 = first.Y;
                    lineWindowsControl.X2 = second.X;
                    lineWindowsControl.Y2 = second.Y;
                    break;
                case LineDrawingMode.BresenhamSymmetric:
                    lineWindowsControl.X1 = first.X;
                    lineWindowsControl.Y1 = first.Y;
                    lineWindowsControl.X2 = second.X;
                    lineWindowsControl.Y2 = second.Y;
                    break;
                default:
                    break;
            }
        }

        public void SetFirstPoint(Point first)
        {
            switch (Globals.lineDrawingMode)
            {
                case LineDrawingMode.Bresenham:
                    firstPoint = first;
                    DrawLine(Globals.DefaultEdgeColor);
                    break;
                case LineDrawingMode.Library:
                    lineWindowsControl.X1 = first.X;
                    lineWindowsControl.Y1 = first.Y;
                    break;
                case LineDrawingMode.AntialiasingWU:
                    lineWindowsControl.X1 = first.X;
                    lineWindowsControl.Y1 = first.Y;
                    break;
                case LineDrawingMode.BresenhamSymmetric:
                    lineWindowsControl.X1 = first.X;
                    lineWindowsControl.Y1 = first.Y;
                    break;
                default:
                    break;
            }
        }

        public void SetSecondPoint(Point second)
        {
            switch (Globals.lineDrawingMode)
            {
                case LineDrawingMode.Bresenham:
                    secondPoint = second;
                    DrawLine(Globals.DefaultEdgeColor);
                    break;
                case LineDrawingMode.Library:
                    lineWindowsControl.X2 = second.X;
                    lineWindowsControl.Y2 = second.Y;
                    break;
                case LineDrawingMode.AntialiasingWU:
                    lineWindowsControl.X2 = second.X;
                    lineWindowsControl.Y2 = second.Y;
                    break;
                case LineDrawingMode.BresenhamSymmetric:
                    lineWindowsControl.X2 = second.X;
                    lineWindowsControl.Y2 = second.Y;
                    break;
                default:
                    break;
            }
        }

        public void SelectEdge()
        {
            DropShadowEffect effect = new DropShadowEffect(); 
            switch (Globals.lineDrawingMode)
            {
                case LineDrawingMode.Bresenham:
                    DrawLine(Globals.SelectedEdgeColor);
                    break;
                case LineDrawingMode.Library:
                    effect = new DropShadowEffect();
                    effect.BlurRadius = Globals.SelectedEdgeBlurRadius;
                    effect.Color = Globals.SelectedEdgeColor;
                    effect.Direction = 0;
                    effect.ShadowDepth = 0;
                    effect.Opacity = 0.9;
                    lineWindowsControl.Stroke = new SolidColorBrush(Globals.SelectedEdgeColor);
                    lineWindowsControl.Effect = effect;
                    break;
                case LineDrawingMode.AntialiasingWU:
                    effect = new DropShadowEffect();
                    effect.BlurRadius = Globals.SelectedEdgeBlurRadius;
                    effect.Color = Globals.SelectedEdgeColor;
                    effect.Direction = 0;
                    effect.ShadowDepth = 0;
                    effect.Opacity = 0.9;
                    lineWindowsControl.Stroke = new SolidColorBrush(Globals.SelectedEdgeColor);
                    lineWindowsControl.Effect = effect;
                    break;
                case LineDrawingMode.BresenhamSymmetric:
                    effect = new DropShadowEffect();
                    effect.BlurRadius = Globals.SelectedEdgeBlurRadius;
                    effect.Color = Globals.SelectedEdgeColor;
                    effect.Direction = 0;
                    effect.ShadowDepth = 0;
                    effect.Opacity = 0.9;
                    lineWindowsControl.Stroke = new SolidColorBrush(Globals.SelectedEdgeColor);
                    lineWindowsControl.Effect = effect;
                    break;
                default:
                    break;
            }
        }

        public void UnselectEdge()
        {
            switch (Globals.lineDrawingMode)
            {
                case LineDrawingMode.Bresenham:
                    DrawLine(Globals.DefaultEdgeColor);
                    break;
                case LineDrawingMode.Library:
                    lineWindowsControl.Stroke = new SolidColorBrush(Globals.DefaultEdgeColor);
                    lineWindowsControl.Effect = null;
                    break;
                case LineDrawingMode.AntialiasingWU:
                    lineWindowsControl.Stroke = new SolidColorBrush(Globals.DefaultEdgeColor);
                    lineWindowsControl.Effect = null;
                    break;
                case LineDrawingMode.BresenhamSymmetric:
                    lineWindowsControl.Stroke = new SolidColorBrush(Globals.DefaultEdgeColor);
                    lineWindowsControl.Effect = null;
                    break;
                default:
                    break;
            }
        }

        public void DeleteDrawing()
        {
            switch (Globals.lineDrawingMode)
            {
                case LineDrawingMode.Bresenham:
                    if (lineBresenham != null)
                    {
                        lineBresenham.Source = null;
                    }
                    break;
                case LineDrawingMode.Library:
                    canvas.Children.Remove(lineWindowsControl);
                    break;
                case LineDrawingMode.AntialiasingWU:
                    canvas.Children.Remove(lineWindowsControl);
                    break;
                case LineDrawingMode.BresenhamSymmetric:
                    canvas.Children.Remove(lineWindowsControl);
                    break;
                default:
                    break;
            }
        }

        public static List<SimplePoint> BresenhamLine(Point first, Point second)
        {
            List<SimplePoint> rectangleList = new List<SimplePoint>();

            int x1 = (int)first.X;
            int y1 = (int)first.Y;
            int x2 = (int)second.X;
            int y2 = (int)second.Y;
            int dx, dy;
            int xi, yi;
            int dE, dNE;
            int d;
            //int dx = x2 - x;
            //int dy = y2 - y;
            if (x1 < x2)
            {
                xi = 1;
                dx = x2 - x1;
            }
            else
            {
                xi = -1;
                dx = x1 - x2;
            }
            if(y1 < y2)
            {
                yi = 1;
                dy = y2 - y1;
            }
            else
            {
                yi = -1;
                dy = y1 - y2;
            }
            //first pixel
            rectangleList.Add(new SimplePoint(x1, y1));
            //moving by OX:
            if (dx > dy)
            {
                dE = 2 * dy;
                dNE = 2 * (dy - dx);
                d = dNE - dx;
                for (; x1 != x2; x1 += xi)
                {
                    if (d >= 0) //moving NE 
                    {
                        y1 += yi;
                        d += dNE;
                    }
                    else //moving E
                    {
                        d += dE;
                    }
                    rectangleList.Add(new SimplePoint(x1, y1));
                    //rectangleList.Add(new SimplePoint(x1, y1 - 1));
                    //rectangleList.Add(new SimplePoint(x1, y1 + 1));
                }
            }
            else //moving by OY
            {
                dE = 2 * dx;
                dNE = 2 * (dx - dy);
                d = dNE - dy;
                for (; y1 != y2; y1 += yi)
                {
                    if (d >= 0) //moving NE
                    {
                        x1 += xi;
                        d += dNE;
                    }
                    else //moving E
                    {
                        d += dE;
                    }
                    rectangleList.Add(new SimplePoint(x1, y1));
                    //rectangleList.Add(new SimplePoint(x1 - 1, y1));
                    //rectangleList.Add(new SimplePoint(x1 + 1, y1));
                }
            }


            return rectangleList;
        }

        private static List<SimplePoint> BresenhamLine2(Point first, Point second)
        {
            List<SimplePoint> rectangleList = new List<SimplePoint>();

            int firstX = (int)first.X;
            int firstY = (int)first.Y;
            int secondX = (int)second.X;
            int secondY = (int)second.Y;
            int w = secondX - firstX;
            int h = secondY - firstY;
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
                rectangleList.Add(new SimplePoint(firstX, firstY));
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    firstX += dx1;
                    firstY += dy1;
                }
                else
                {
                    firstX += dx2;
                    firstY += dy2;
                }
            }
            return rectangleList;
        }

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

            lineBresenham.Source = rtb;
        }
    }
}
