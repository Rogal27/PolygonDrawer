using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace GKProjekt1
{
    public static class Draw
    {
        public static Ellipse Verticle(Point p, Canvas canvas)
        {
            Ellipse verticle = new Ellipse()
            {
                Width = Globals.VerticleSize,
                Height = Globals.VerticleSize,
                Fill = Globals.DefaultVerticleColor
            };
            Canvas.SetLeft(verticle, p.X - (double)Globals.VerticleSize / 2.0);
            Canvas.SetTop(verticle, p.Y - (double)Globals.VerticleSize / 2.0);
            Panel.SetZIndex(verticle, Globals.VerticleZIndex);
            canvas.Children.Add(verticle);
            return verticle;
        }

        public static Line Edge(MyEdge edge, Canvas canvas)
        {
            if (Globals.__Test__ == true)
            {
                Line line = new Line()
                {
                    X1 = edge.first.X,
                    Y1 = edge.first.Y,
                    X2 = edge.second.X,
                    Y2 = edge.second.Y,
                    StrokeThickness = Globals.LineThickness,
                    Stroke = Globals.DefaultEdgeColor
                };
                Panel.SetZIndex(line, Globals.LineZIndex);
                canvas.Children.Add(line);
                return line;
            }
            else
            {
                //algorytm Bresenhama
                return null;
            }
        }

        public static Line Edge(Point first, Point second, Canvas canvas)
        {
            if (Globals.__Test__ == true)
            {
                Line line = new Line()
                {
                    X1 = first.X,
                    Y1 = first.Y,
                    X2 = second.X,
                    Y2 = second.Y,
                    StrokeThickness = Globals.LineThickness,
                    Stroke = Globals.DefaultEdgeColor
                };
                Panel.SetZIndex(line, Globals.LineZIndex);
                canvas.Children.Add(line);
                return line;
            }
            else
            {
                //algorytm Bresenhama
                return null;
            }
        }
    }
}
