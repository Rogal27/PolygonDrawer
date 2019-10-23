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
                case LineDrawingMode.BresenhamSymmetric:
                case LineDrawingMode.AntialiasingWU:
                    lineBresenham = new Image();
                    lineBresenham.IsHitTestVisible = false;
                    Panel.SetZIndex(lineBresenham, Globals.LineZIndex);
                    this.canvas = canvas;
                    this.canvas.Children.Add(lineBresenham);
                    break;
                case LineDrawingMode.Library:
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
                    DrawBresenhamLine(Globals.DefaultEdgeColor);
                    break;
                case LineDrawingMode.Library:                    
                    lineWindowsControl.X1 = first.X;
                    lineWindowsControl.Y1 = first.Y;
                    lineWindowsControl.X2 = second.X;
                    lineWindowsControl.Y2 = second.Y;
                    break;
                case LineDrawingMode.AntialiasingWU:
                    //TODO:
                    firstPoint = new Point(first.X, first.Y);
                    secondPoint = new Point(second.X, second.Y);
                    DrawAntialiasedWULine(Globals.DefaultEdgeColor);
                    break;
                case LineDrawingMode.BresenhamSymmetric:
                    firstPoint = new Point(first.X, first.Y);
                    secondPoint = new Point(second.X, second.Y);
                    DrawBresenhamSymmetricLine(Globals.DefaultEdgeColor);
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
                    DrawBresenhamLine(Globals.DefaultEdgeColor);
                    break;
                case LineDrawingMode.Library:
                    lineWindowsControl.X1 = first.X;
                    lineWindowsControl.Y1 = first.Y;
                    break;
                case LineDrawingMode.AntialiasingWU:
                    //TODO
                    firstPoint = first;
                    DrawAntialiasedWULine(Globals.DefaultEdgeColor);
                    break;
                case LineDrawingMode.BresenhamSymmetric:
                    firstPoint = first;
                    DrawBresenhamSymmetricLine(Globals.DefaultEdgeColor);
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
                    DrawBresenhamLine(Globals.DefaultEdgeColor);
                    break;
                case LineDrawingMode.Library:
                    lineWindowsControl.X2 = second.X;
                    lineWindowsControl.Y2 = second.Y;
                    break;
                case LineDrawingMode.AntialiasingWU:
                    //TODO
                    secondPoint = second;
                    DrawAntialiasedWULine(Globals.DefaultEdgeColor);
                    break;
                case LineDrawingMode.BresenhamSymmetric:
                    secondPoint = second;
                    DrawBresenhamSymmetricLine(Globals.DefaultEdgeColor);
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
                    DrawBresenhamLine(Globals.SelectedEdgeColor);
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
                    //TODO
                    DrawAntialiasedWULine(Globals.SelectedEdgeColor);
                    break;
                case LineDrawingMode.BresenhamSymmetric:
                    DrawBresenhamSymmetricLine(Globals.SelectedEdgeColor);
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
                    DrawBresenhamLine(Globals.DefaultEdgeColor);
                    break;
                case LineDrawingMode.Library:
                    lineWindowsControl.Stroke = new SolidColorBrush(Globals.DefaultEdgeColor);
                    lineWindowsControl.Effect = null;
                    break;
                case LineDrawingMode.AntialiasingWU:
                    //TODO
                    DrawAntialiasedWULine(Globals.DefaultEdgeColor);
                    break;
                case LineDrawingMode.BresenhamSymmetric:
                    DrawBresenhamSymmetricLine(Globals.DefaultEdgeColor);
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
                    if (lineBresenham != null)
                    {
                        lineBresenham.Source = null;
                    }
                    break;
                case LineDrawingMode.BresenhamSymmetric:
                    if (lineBresenham != null)
                    {
                        lineBresenham.Source = null;
                    }
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

        public static List<SimplePoint> BresenhamSymmetric(Point first, Point second)
        {
            List<SimplePoint> rectangleList = new List<SimplePoint>();

            int x1 = (int)first.X;
            int y1 = (int)first.Y;
            int x2 = (int)second.X;
            int y2 = (int)second.Y;
            int dx, dy;
            int xi, yi;
            int xf = x1, yf = y1;
            int xb = x2, yb = y2;
            int dE, dNE;
            int d;
            int middle;
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
            if (y1 < y2)
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
                if (xf < xb)
                {
                    for (; xf < xb; xf += xi, xb -= xi)
                    {
                        if (d >= 0) //moving NE 
                        {
                            yf += yi;
                            yb -= yi;
                            d += dNE;
                        }
                        else //moving E
                        {
                            d += dE;
                        }
                        rectangleList.Add(new SimplePoint(xf, yf));
                        rectangleList.Add(new SimplePoint(xb, yb));
                    }
                }
                else
                {
                    for (; xf >= xb; xf += xi, xb -= xi)
                    {
                        if (d >= 0) //moving NE 
                        {
                            yf += yi;
                            yb -= yi;
                            d += dNE;
                        }
                        else //moving E
                        {
                            d += dE;
                        }
                        rectangleList.Add(new SimplePoint(xf, yf));
                        rectangleList.Add(new SimplePoint(xb, yb));
                    }
                }
            }
            else //moving by OY
            {
                dE = 2 * dx;
                dNE = 2 * (dx - dy);
                d = dNE - dy;
                if(yf<yb)
                {
                    for (; yf < yb; yf += yi, yb -= yi)
                    {
                        if (d >= 0) //moving NE
                        {
                            xf += xi;
                            xb -= xi;
                            d += dNE;
                        }
                        else //moving E
                        {
                            d += dE;
                        }
                        rectangleList.Add(new SimplePoint(xf, yf));
                        rectangleList.Add(new SimplePoint(xb, yb));
                    }
                }
                else
                {
                    for (; yf >= yb; yf += yi, yb -= yi)
                    {
                        if (d >= 0) //moving NE
                        {
                            xf += xi;
                            xb -= xi;
                            d += dNE;
                        }
                        else //moving E
                        {
                            d += dE;
                        }
                        rectangleList.Add(new SimplePoint(xf, yf));
                        rectangleList.Add(new SimplePoint(xb, yb));
                    }
                }
            }


            return rectangleList;
        }

        private static double frac(double d)
        {
            return d - Math.Floor(d);
        }

        private static Color MultiplyColor(Color color,double d)
        {
            return Color.FromArgb(color.R, color.G, color.B, (byte)d);
        }

        public static List<SimplePointColor> AntialiasingWU(Point first, Point second, Color color)
        {
            List<SimplePointColor> rectangleList = new List<SimplePointColor>();

            Color c1 = new Color();
            Color c2 = new Color();

            double x1 = first.X;
            double y1 = first.Y;
            double x2 = second.X;
            double y2 = second.Y;
            double m;
            int dx, dy;
            double xi, yi;
            int dE, dNE;
            int d;
            //int dx = x2 - x;
            //int dy = y2 - y;
            if (x1 < x2)
            {
                xi = 1d;
                dx = (int)(x2 - x1);
            }
            else
            {
                xi = -1d;
                dx = (int)(x1 - x2);
            }
            if (y1 < y2)
            {
                yi = 1d;
                dy = (int)(y2 - y1);
            }
            else
            {
                yi = -1;
                dy = (int)(y1 - y2);
            }
            //first pixel
            c1 = MultiplyColor(color, 1 - frac(y1));
            
            rectangleList.Add(new SimplePointColor(x1, y1, c1));
            //moving by OX:
            if (dx > dy)
            {
                if (dx == 0)
                    return rectangleList;
                m = dy / dx;
                dE = 2 * dy;
                dNE = 2 * (dy - dx);
                d = dNE - dx;
                for (; (int)x1 != (int)x2; x1 += xi)
                {
                    
                    if (d >= 0) //moving NE 
                    {
                        y1 += m;
                        d += dNE;
                    }
                    else //moving E
                    {
                        d += dE;
                    }
                    Debug.WriteLine($"Frac y1: {y1}");
                    c1 = MultiplyColor(color, 1 - frac(y1));
                    c2 = MultiplyColor(color, frac(y1));
                    rectangleList.Add(new SimplePointColor(x1, (int)y1, c1));
                    rectangleList.Add(new SimplePointColor(x1,(int)( y1 + yi), c2));
                }
            }
            else //moving by OY
            {
                if (dy == 0)
                    return rectangleList;
                m = dx / dy;
                
                dE = 2 * dx;
                dNE = 2 * (dx - dy);
                d = dNE - dy;
                for (; (int)y1 != (int)y2; y1 += yi)
                {
                    if (d >= 0) //moving NE
                    {
                        x1 += m;
                        d += dNE;
                    }
                    else //moving E
                    {
                        d += dE;
                    }
                    c1 = MultiplyColor(color, 1 - frac(x1));
                    c2 = MultiplyColor(color, frac(x1));
                    rectangleList.Add(new SimplePointColor((int)x1, y1, c1));
                    rectangleList.Add(new SimplePointColor((int)(x1 + xi), y1, c2));
                    //rectangleList.Add(new SimplePoint(x1, y1));
                    //rectangleList.Add(new SimplePoint(x1 - 1, y1));
                    //rectangleList.Add(new SimplePoint(x1 + 1, y1));
                }
            }


            return rectangleList;
        }

        public void DrawLine(List<SimplePoint> pointsList, Color color)
        {
            //if (Globals.__BresenhamOff__ == true)
            //    return;            
            
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

        public void DrawLine(List<SimplePointColor> pointsList, Color color)
        {
            //if (Globals.__BresenhamOff__ == true)
            //    return;            

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                foreach (var point in pointsList)
                {
                    Rect rect = new Rect(point.X, point.Y, 1, 1);
                    dc.DrawRectangle(new SolidColorBrush(point.color), null, rect);
                }

            }
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)canvas.ActualWidth, (int)canvas.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(dv);

            lineBresenham.Source = rtb;
        }

        public void DrawBresenhamLine(Color color)
        {
            List<SimplePoint> pointsList = BresenhamLine(firstPoint, secondPoint);
            DrawLine(pointsList, Globals.DefaultEdgeColor);
        }

        public void DrawBresenhamSymmetricLine(Color color)
        {
            List<SimplePoint> pointsList = BresenhamSymmetric(firstPoint, secondPoint);
            DrawLine(pointsList, Globals.DefaultEdgeColor);
        }

        public void DrawAntialiasedWULine(Color color)
        {
            List<SimplePointColor> pointsList = AntialiasingWU(firstPoint, secondPoint, color);
            DrawLine(pointsList, Globals.DefaultEdgeColor);
        }
    }
}
