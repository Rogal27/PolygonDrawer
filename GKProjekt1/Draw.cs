using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GKProjekt1
{
    public static class Draw
    {
        public static void Verticle(MyPoint p, Canvas canvas)
        {
            Ellipse verticle = new Ellipse()
            {
                Width = Globals.VerticleSize,
                Height = Globals.VerticleSize,
                Fill = new SolidColorBrush(Globals.DefaultVerticleColor)
            };
            Canvas.SetLeft(verticle, p.X - (double)Globals.VerticleSize / 2.0);
            Canvas.SetTop(verticle, p.Y - (double)Globals.VerticleSize / 2.0);
            Panel.SetZIndex(verticle, Globals.VerticleZIndex);
            canvas.Children.Add(verticle);
            p.ellipse = verticle;
        }

        public static void Edge(MyEdge edge, Canvas canvas)
        {
            switch (Globals.lineDrawingMode)
            {                
                case LineDrawingMode.Bresenham:
                    MyLine myLine1 = new MyLine(canvas);
                    myLine1.firstPoint = new Point(edge.first.X, edge.first.Y);
                    myLine1.secondPoint = new Point(edge.second.X, edge.second.Y);

                    edge.myLine = myLine1;

                    //algorytm Bresenhama
                    myLine1.DrawBresenhamLine(Globals.DefaultEdgeColor);
                    break;
                case LineDrawingMode.Library:
                    MyLine myLine2 = new MyLine(canvas);
                    Line line2 = new Line()
                    {
                        X1 = edge.first.X,
                        Y1 = edge.first.Y,
                        X2 = edge.second.X,
                        Y2 = edge.second.Y,
                        StrokeThickness = Globals.LineThickness,
                        Stroke = new SolidColorBrush(Globals.DefaultEdgeColor)
                    };
                    Panel.SetZIndex(line2, Globals.LineZIndex);
                    canvas.Children.Add(line2);
                    //myLine = new MyLine(canvas);
                    myLine2.lineWindowsControl = line2;
                    //myLine.canvas = canvas;
                    edge.myLine = myLine2;
                    break;
                case LineDrawingMode.AntialiasingWU:
                    //TODO
                    MyLine myLine3 = new MyLine(canvas);
                    myLine3.firstPoint = new Point(edge.first.X, edge.first.Y);
                    myLine3.secondPoint = new Point(edge.second.X, edge.second.Y);

                    edge.myLine = myLine3;

                    myLine3.DrawAntialiasedWULine(Globals.DefaultEdgeColor);
                    break;
                case LineDrawingMode.BresenhamSymmetric:
                    MyLine myLine4 = new MyLine(canvas);
                    myLine4.firstPoint = new Point(edge.first.X, edge.first.Y);
                    myLine4.secondPoint = new Point(edge.second.X, edge.second.Y);

                    edge.myLine = myLine4;

                    myLine4.DrawBresenhamSymmetricLine(Globals.DefaultEdgeColor);
                    break;
                default:
                    break;
            }
        }

        public static MyLine SimpleEdge(Point first, Point second, Canvas canvas)//used to draw temporary lines
        {
            switch (Globals.lineDrawingMode)
            {
                case LineDrawingMode.Bresenham:
                    MyLine myLine1 = new MyLine(canvas);

                    myLine1.firstPoint = first;
                    myLine1.secondPoint = second;

                    myLine1.DrawBresenhamLine(Globals.DefaultEdgeColor);

                    return myLine1;
                case LineDrawingMode.Library:
                    MyLine myLine2 = new MyLine(canvas);
                    Line line2 = new Line()
                    {
                        X1 = first.X,
                        Y1 = first.Y,
                        X2 = second.X,
                        Y2 = second.Y,
                        StrokeThickness = Globals.LineThickness,
                        Stroke = new SolidColorBrush(Globals.DefaultEdgeColor)
                    };
                    Panel.SetZIndex(line2, Globals.LineZIndex);
                    canvas.Children.Add(line2);                    
                    myLine2.lineWindowsControl = line2;
                    return myLine2;
                case LineDrawingMode.AntialiasingWU:
                    //TODO
                    MyLine myLine3 = new MyLine(canvas);

                    myLine3.firstPoint = first;
                    myLine3.secondPoint = second;

                    myLine3.DrawBresenhamLine(Globals.DefaultEdgeColor);

                    return myLine3;
                case LineDrawingMode.BresenhamSymmetric:
                    MyLine myLine4 = new MyLine(canvas);

                    myLine4.firstPoint = first;
                    myLine4.secondPoint = second;

                    myLine4.DrawBresenhamSymmetricLine(Globals.DefaultEdgeColor);

                    return myLine4;
                default:
                    return null;
            }
        }

    }
}
