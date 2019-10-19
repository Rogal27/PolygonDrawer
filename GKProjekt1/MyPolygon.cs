using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace GKProjekt1
{
    public class MyPolygon
    {
        public List<MyEdge> Edges { get; set; } = new List<MyEdge>();
        public MyPoint StartingVerticle { get; set; }
        public MyPoint LastVerticle { get; set; }
        public Canvas canvas { get; set; }

        public MyPolygon(MyPoint startingVerticle, Canvas canvas)
        {
            StartingVerticle = startingVerticle;
            LastVerticle = startingVerticle;
            this.canvas = canvas;
            Draw.Verticle(StartingVerticle, this.canvas);
        }

        //BRESENHAM!
        public PolygonDrawResult AddVerticleAndDraw(MyPoint p)
        {
            if (MyPoint.AreNear(p, StartingVerticle, (double)Globals.VerticleClickRadiusSize / 2.0) == true)
            {
                if (Edges.Count < 2)
                    return PolygonDrawResult.NotEnoughEdges;
                //Line line = Draw.Edge(LastVerticle, StartingVerticle, canvas);
                MyEdge e = new MyEdge(LastVerticle, StartingVerticle);
                Draw.Edge(e, canvas);
                Edges.Add(e);
                return PolygonDrawResult.DrawFinished;
            }
            else
            {
                //Line line = Draw.Edge(LastVerticle, p, canvas);
                //Ellipse secondEllipse = Draw.Verticle(p, canvas);
                Draw.Verticle(p, canvas);
                MyEdge e = new MyEdge(LastVerticle, p);
                Draw.Edge(e, canvas);
                LastVerticle = p;
                Edges.Add(e);
                return PolygonDrawResult.DrawInProgress;
            }
        }       

        //BRESENHAM!
        public void MoveVerticle(MyPoint verticle, Point endPoint)
        {
            var previousEdge = Edges.Last();
            foreach (var edge in Edges)
            {
                if (Object.ReferenceEquals(edge.first, verticle) == true)
                {
                    verticle.Move(endPoint);
                    edge.MoveWithPoints();
                    previousEdge.MoveWithPoints();
                    return;
                }
                previousEdge = edge;
            }
        }

        //BRESENHAM
        public bool DeleteVerticle(MyPoint verticle)
        {
            if (Edges.Count > 3)
            {
                var previousEdge = Edges.Last();
                foreach (var edge in Edges)
                {
                    if (Object.ReferenceEquals(edge.first, verticle) == true)
                    {
                        previousEdge.DeleteRelation();
                        edge.DeleteRelation();

                        previousEdge.second = edge.second;
                        previousEdge.MoveWithPoints();

                        canvas.Children.Remove(edge.first.ellipse);
                        canvas.Children.Remove(edge.line);

                        Edges.Remove(edge);

                        return false;
                    }
                    previousEdge = edge;
                }
            }
            else
            {
                var result = MessageBox.Show("Do you want to delete polygon?", Globals.WindowName, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    DeleteDrawing();
                    return true;
                }
            }
            return false;
        }

        //BRESENHAM!
        public void AddMiddleVerticleOnEdge(MyEdge edge)
        {
            edge.DeleteRelation();
            var index = Edges.IndexOf(edge);
            var middleX = (edge.first.X + edge.second.X) / 2.0;
            var middleY = (edge.first.Y + edge.second.Y) / 2.0;
            MyPoint middleVerticle = new MyPoint(middleX, middleY);
            Draw.Verticle(middleVerticle, canvas);
            MyEdge secondHalf = new MyEdge(middleVerticle, edge.second);
            Draw.Edge(secondHalf, canvas);
            edge.second = middleVerticle;
            edge.MoveWithPoints();
            Edges.Insert(index + 1, secondHalf);
        }

        //BRESENHAM!
        public void MoveEdgeParallel(MyEdge edge, ref Point startPoint, ref Point endPoint)
        {
            var edgeIndex = Edges.IndexOf(edge);
            if (edgeIndex == -1)
                return;
            int edgesCount = Edges.Count;
            var previousEdge2 = Edges[(edgeIndex - 1 + edgesCount) % edgesCount];
            var nextEdge2 = Edges[(edgeIndex + 1 + edgesCount) % edgesCount];
            edge.MoveParallel(startPoint, endPoint);
            startPoint = endPoint;
            previousEdge2.MoveWithPoints();
            nextEdge2.MoveWithPoints();
        }

        //BRESENHAM!
        public void MovePolygon(ref Point startPoint, ref Point endPoint)
        {
            var offsetX = endPoint.X - startPoint.X;
            var offsetY = endPoint.Y - startPoint.Y;
            foreach (var edge in Edges)
            {
                edge.first.Move(edge.first.X + offsetX, edge.first.Y + offsetY);
            }
            foreach (var edge in Edges)
            {
                edge.MoveWithPoints();
            }
            startPoint = endPoint;
        }

        public bool IsPointInside(Point p)
        {
            //testing RemoveCollinearEdges
            //var edgeFirst = Edges[0];
            //var edgeThird = Edges[2];
            //var myEdge1 = new MyEdge(edgeFirst.second, new MyPoint(edgeFirst.second.X + 50, edgeFirst.second.Y));
            //var myEdge2 = new MyEdge(new MyPoint(edgeFirst.second.X + 50, edgeFirst.second.Y), new MyPoint(edgeFirst.second.X + 100, edgeFirst.second.Y));
            //edgeThird.first = myEdge2.second;
            //Edges.RemoveAt(1);
            //Edges.Insert(1, myEdge2);
            //Edges.Insert(1, myEdge1);

            List<MyEdge> ClearEdges = RemoveCollinearEdges();
            //TODO:
            //implement algorithm
            //Debug.WriteLine($"Canvas actualwidth: {canvas.ActualWidth}");
            //Debug.WriteLine($"Canvas width: {canvas.Width}");
            MyEdge Ray = new MyEdge(new MyPoint(p.X, p.Y), new MyPoint(p.X + canvas.ActualWidth, p.Y));
            int intersectCounter = 0;
            //var previousEdge = ClearEdges.Last();

            for (int i = 0; i < ClearEdges.Count; i++)
            {
                var previousEdge = ClearEdges[(i - 1 + ClearEdges.Count) % ClearEdges.Count];
                var edge = ClearEdges[i];
                var nextEdge = ClearEdges[(i + 1) % ClearEdges.Count];
                if (MyEdge.DoIntersect(edge, Ray) == true)
                {
                    var firstCollinear = MyPoint.CheckIfCollinear(Ray.first, edge.first, Ray.second);
                    var secondCollinear = MyPoint.CheckIfCollinear(Ray.first, edge.second, Ray.second);

                    if (firstCollinear == true && secondCollinear == true)
                    {
                        //if previosEdge.first i nextEdge.second leza
                        //po przeciwnych stronach polprostej Ray to intersectCounter++
                        if ((previousEdge.first.Y > Ray.first.Y && nextEdge.second.Y < Ray.first.Y) ||
                            (previousEdge.first.Y < Ray.first.Y && nextEdge.second.Y > Ray.first.Y))
                        {
                            intersectCounter++;
                        }
                    }
                    else if (firstCollinear == true)
                    {
                        //if previousEdge.first i edge.second leza
                        //po przeciwnych stronach polprostej Ray to intersectCounter++
                        if ((previousEdge.first.Y > Ray.first.Y && edge.second.Y < Ray.first.Y) ||
                                (previousEdge.first.Y < Ray.first.Y && edge.second.Y > Ray.first.Y))
                        {
                            intersectCounter++;
                            i++;
                        }
                    }
                    else if (secondCollinear == true)
                    {
                        //if edge.first i nextEdge.second leza
                        //po przeciwnych stronach polprostej Ray to intersectCounter++
                        if ((edge.first.Y > Ray.first.Y && nextEdge.second.Y < Ray.first.Y) ||
                            (edge.first.Y < Ray.first.Y && nextEdge.second.Y > Ray.first.Y))
                        {
                            intersectCounter++;
                            i++;
                        }
                    }
                    else
                    {
                        intersectCounter++;
                    }
                }
                previousEdge = edge;
            }
            return intersectCounter % 2 == 1;
        }       

        private List<MyEdge> RemoveCollinearEdges()
        {
            List<MyEdge> ClearEdges = new List<MyEdge>();
            //var nextEdge = Edges.Last();
            MyEdge tmpEdge = null;
            MyEdge edge = null;
            bool AreCollinear = false;

            for (int i = 0; i < Edges.Count; i++)
            {
                if (AreCollinear == true)
                {
                    edge = tmpEdge;
                }
                else
                {
                    edge = Edges[i];
                }
                var nextEdge = Edges[(i + 1) % Edges.Count];
                if (MyPoint.CheckIfCollinear(edge.first, edge.second, nextEdge.second) == true)
                {
                    tmpEdge = new MyEdge(edge.first, nextEdge.second);
                    AreCollinear = true;
                    if (i == Edges.Count - 1)
                    {
                        var finalEdge = new MyEdge(tmpEdge.first, ClearEdges.First().second);
                        ClearEdges.Add(finalEdge);
                        ClearEdges.RemoveAt(0);
                    }
                }
                else
                {
                    ClearEdges.Add(edge);
                    AreCollinear = false;
                }
            }
            return ClearEdges;
        }

        //BRESENHAM!
        public void DeleteDrawing()
        {
            foreach(var edge in Edges)
            {
                edge.DeleteRelation();
                canvas.Children.Remove(edge.line);
                canvas.Children.Remove(edge.first.ellipse);
                canvas.Children.Remove(edge.second.ellipse);
            }
        }
    }
}
