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
            if (Globals.__BresenhamOff__ == true)
            {
                Line line = new Line()
                {
                    X1 = edge.first.X,
                    Y1 = edge.first.Y,
                    X2 = edge.second.X,
                    Y2 = edge.second.Y,
                    StrokeThickness = Globals.LineThickness,
                    Stroke = new SolidColorBrush(Globals.DefaultEdgeColor)
                };
                Panel.SetZIndex(line, Globals.LineZIndex);
                canvas.Children.Add(line);
                MyLine myLine = new MyLine();
                myLine.lineWindowsControl = line;
                myLine.canvas = canvas;
                edge.myLine = myLine;
            }
            else
            {
                MyLine myLine = new MyLine(canvas);
                myLine.firstPoint = new Point(edge.first.X, edge.first.Y);
                myLine.secondPoint = new Point(edge.second.X, edge.second.Y);

                edge.myLine = myLine;

                //algorytm Bresenhama
                myLine.DrawLine(Globals.DefaultEdgeColor);                
            }
        }

        public static MyLine SimpleEdge(Point first, Point second, Canvas canvas)//used to draw temporary lines
        {
            if (Globals.__BresenhamOff__ == true)
            {
                Line line = new Line()
                {
                    X1 = first.X,
                    Y1 = first.Y,
                    X2 = second.X,
                    Y2 = second.Y,
                    StrokeThickness = Globals.LineThickness,
                    Stroke = new SolidColorBrush(Globals.DefaultEdgeColor)
                };
                Panel.SetZIndex(line, Globals.LineZIndex);
                canvas.Children.Add(line);
                MyLine myLine = new MyLine();
                myLine.lineWindowsControl = line;
                myLine.canvas = canvas;
                return myLine;
            }
            else
            {
                MyLine myLine = new MyLine(canvas);

                myLine.firstPoint = first;
                myLine.secondPoint = second;

                //algorytm Bresenhama
                myLine.DrawLine(Globals.DefaultEdgeColor);
                
                return myLine;
            }
        }

    }
}
