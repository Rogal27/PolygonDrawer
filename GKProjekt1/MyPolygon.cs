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
        //public Ellipse StartingVerticleEllipse { get; set; }

        public MyPoint LastVerticle { get; set; }
        //public Ellipse LastVerticleEllipse { get; set; }

        public Dictionary<MyEdge, (MyEdge, RelationType)> Relations = new Dictionary<MyEdge, (MyEdge, RelationType)>();

        public Canvas canvas { get; set; }

        public MyPolygon(MyPoint startingVerticle, Canvas canvas)
        {
            StartingVerticle = startingVerticle;
            LastVerticle = startingVerticle;
            this.canvas = canvas;
            Draw.Verticle(StartingVerticle, this.canvas);
        }

        //public void DrawStartingVerticle(Canvas canvas)
        //{
        //    this.canvas = canvas;
        //    Draw.Verticle(StartingVerticle, this.canvas);
        //}

        //public bool AddVerticle(Point p)
        //{
        //    if (PointExtension.AreNear(p, StartingVerticle, (double)Globals.VerticleClickRadiusSize / 2.0) == true)
        //    {
        //        MyEdge e = new MyEdge(LastVerticle, StartingVerticle);              
        //        Edges.Add(e);
        //        return false;
        //    }
        //    else
        //    {
        //        MyEdge e = new MyEdge(LastVerticle, p);
        //        LastVerticle = p;
        //        Edges.Add(e);
        //        return true;
        //    }
        //}

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
            var previousEdge = ClearEdges.Last();

            for (int i = 0; i < ClearEdges.Count; i++)
            {                
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
                        //if (MyPoint.VectorProduct(previousEdge.first - edge.first, edge.second - edge.first) *
                        //    MyPoint.VectorProduct(nextEdge.second - edge.first, edge.second - edge.first) < 0)
                        //{
                        //    intersectCounter++;

                        if ((previousEdge.first.Y > Ray.first.Y && edge.second.Y < Ray.first.Y) ||
                                (previousEdge.first.Y < Ray.first.Y && edge.second.Y > Ray.first.Y))
                        {
                            intersectCounter++;
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

        public void DeleteDrawing()
        {
            foreach(var edge in Edges)
            {
                canvas.Children.Remove(edge.line);
                canvas.Children.Remove(edge.first.ellipse);
                canvas.Children.Remove(edge.second.ellipse);
            }
        }

        public void AddRelation()
        {
            throw new NotImplementedException();
        }
    }
}
