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

        public MyPolygon()
        {

        }

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
            //var previousEdge = Edges.Last();
            foreach (var edge in Edges)
            {
                if (Object.ReferenceEquals(edge.first, verticle) == true)
                {
                    var previousPoint = new Point(verticle.X, verticle.Y);
                    verticle.Move(endPoint);
                    //edge.MoveWithPoints();
                    //previousEdge.MoveWithPoints();
                    var result = ApplyRelationChanges(edge);
                    if (result == false)
                    {
                        verticle.Move(previousPoint);
                    }

                    return;
                }
                //previousEdge = edge;
            }
        }

        public bool ApplyRelationChanges(MyEdge movedEdge)
        {
            var (success, changedPolygon) = FixRelationsMovingVerticle(movedEdge);
            if (success == false)
            {
                MessageBox.Show("Unallowed Move!", Globals.WindowName, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                RedrawPolygon(changedPolygon);
            }
            return success;
        }        

        //unfinished
        private (bool success,MyPolygon changedPolygon) FixRelationsMovingVerticle(MyEdge startingEdge)
        {
            MyPolygon copyPolygon = this.CopyWithoutDrawing();
            var startingEdgeIndex = copyPolygon.Edges.FindIndex(x => x == startingEdge);
            bool endLoop = false;
            bool firstSuccess = false;
            bool secondSuccess = false;

            MyEdge relationEdge;
            double length;
            double relationEdgeLength;
            double vectorX;
            double vectorY;
            double scale;

            Vector v1 = new Vector();
            Vector v2 = new Vector();
            Point APoint = new Point();
            Point BPoint = new Point();
            Point CPoint = new Point();
            Point DPoint = new Point();
            Point? intersection;

            //going right (list order)
            for (int i = 0; i < copyPolygon.Edges.Count && endLoop == false; i++)
            {
                var edge = copyPolygon.Edges[(i + startingEdgeIndex) % copyPolygon.Edges.Count];
                switch (edge.relationType)
                {
                    case RelationType.Equal:
                        {
                            //moving second verticle
                            relationEdge = edge.relationEdge;
                            length = edge.Length();
                            relationEdgeLength = relationEdge.Length();
                            if (length < Globals.eps || relationEdgeLength < Globals.eps)
                            {
                                endLoop = true;
                                firstSuccess = true;
                                break;
                            }
                            if (Math.Abs(relationEdgeLength - length) < Globals.eps)
                            {
                                break;
                            }
                            vectorX = edge.second.X - edge.first.X;
                            vectorY = edge.second.Y - edge.first.Y;
                            scale = relationEdgeLength / length;
                            vectorX *= scale;
                            vectorY *= scale;
                            edge.second.X = edge.first.X + vectorX;
                            edge.second.Y = edge.first.Y + vectorY;
                            //Debug.WriteLine($"Edge second: ({edge.second.X};{edge.second.Y})");
                        }
                        break;
                    case RelationType.Perpendicular:
                        {
                            //moving second verticle
                            relationEdge = edge.relationEdge;

                            v1.X = relationEdge.second.X - relationEdge.first.X;
                            v1.Y = relationEdge.second.Y - relationEdge.first.Y;
                            v2.X = relationEdge.second.Y - relationEdge.first.Y;
                            v2.Y = relationEdge.first.X - relationEdge.second.X;

                            APoint.X = edge.first.X;
                            APoint.Y = edge.first.Y;
                            BPoint = APoint + v2;
                            CPoint.X = edge.second.X;
                            CPoint.Y = edge.second.Y;
                            DPoint = CPoint + v1;

                            intersection = PointExtension.IntersectionPoint(APoint, BPoint, CPoint, DPoint);

                            if (intersection.HasValue == true)
                            {
                                edge.second.X = intersection.Value.X;
                                edge.second.Y = intersection.Value.Y;
                            }
                            else
                            {
                                edge.first.X += 5.0;
                                edge.first.Y += 5.0;

                                v1.X = relationEdge.second.X - relationEdge.first.X;
                                v1.Y = relationEdge.second.Y - relationEdge.first.Y;
                                v2.X = relationEdge.second.Y - relationEdge.first.Y;
                                v2.Y = relationEdge.first.X - relationEdge.second.X;

                                APoint.X = edge.first.X;
                                APoint.Y = edge.first.Y;
                                BPoint = APoint + v2;
                                CPoint.X = edge.second.X;
                                CPoint.Y = edge.second.Y;
                                DPoint = CPoint + v1;

                                intersection = PointExtension.IntersectionPoint(APoint, BPoint, CPoint, DPoint);

                                if (intersection.HasValue == true)
                                {
                                    edge.second.X = intersection.Value.X;
                                    edge.second.Y = intersection.Value.Y;
                                }
                            }
                        }
                        break;
                    case RelationType.None:
                        endLoop = true;
                        firstSuccess = true;
                        break;
                    default:
                        break;
                }
            }
            if (firstSuccess == false)
            {
                if (CheckIfRelationsAreOK(copyPolygon) == false)
                    return (false, null);
            }
            endLoop = false;
            //going left (no list order)
            for (int i = copyPolygon.Edges.Count - 1; i >= 0 && endLoop == false; i--)
            {
                var edge = copyPolygon.Edges[(i + startingEdgeIndex) % copyPolygon.Edges.Count];
                switch (edge.relationType)
                {
                    case RelationType.Equal:
                        {
                            //moving second verticle
                            relationEdge = edge.relationEdge;
                            length = edge.Length();
                            relationEdgeLength = relationEdge.Length();
                            if (length < Globals.eps || relationEdgeLength < Globals.eps)
                            {
                                endLoop = true;
                                secondSuccess = true;
                                break;
                            }
                            if (Math.Abs(relationEdgeLength - length) < Globals.eps)
                            {
                                break;
                            }
                            vectorX = edge.first.X - edge.second.X;
                            vectorY = edge.first.Y - edge.second.Y;
                            scale = relationEdgeLength / length;
                            vectorX *= scale;
                            vectorY *= scale;
                            edge.first.X = edge.second.X + vectorX;
                            edge.first.Y = edge.second.Y + vectorY;
                        }
                        break;
                    case RelationType.Perpendicular:
                        {
                            relationEdge = edge.relationEdge;

                            v1.X = relationEdge.first.X - relationEdge.second.X;
                            v1.Y = relationEdge.first.Y - relationEdge.second.Y;
                            v2.X = relationEdge.first.Y - relationEdge.second.Y;
                            v2.Y = relationEdge.second.X - relationEdge.first.X;

                            APoint.X = edge.second.X;
                            APoint.Y = edge.second.Y;
                            BPoint = APoint + v2;
                            CPoint.X = edge.first.X;
                            CPoint.Y = edge.first.Y;
                            DPoint = CPoint + v1;

                            intersection = PointExtension.IntersectionPoint(APoint, BPoint, CPoint, DPoint);

                            if (intersection.HasValue == true)
                            {
                                edge.first.X = intersection.Value.X;
                                edge.first.Y = intersection.Value.Y;
                            }
                            else
                            {
                                edge.second.X += 5.0;
                                edge.second.Y += 5.0;

                                v1.X = relationEdge.first.X - relationEdge.second.X;
                                v1.Y = relationEdge.first.Y - relationEdge.second.Y;
                                v2.X = relationEdge.first.Y - relationEdge.second.Y;
                                v2.Y = relationEdge.second.X - relationEdge.first.X;

                                APoint.X = edge.second.X;
                                APoint.Y = edge.second.Y;
                                BPoint = APoint + v2;
                                CPoint.X = edge.first.X;
                                CPoint.Y = edge.first.Y;
                                DPoint = CPoint + v1;

                                intersection = PointExtension.IntersectionPoint(APoint, BPoint, CPoint, DPoint);

                                if (intersection.HasValue == true)
                                {
                                    edge.first.X = intersection.Value.X;
                                    edge.first.Y = intersection.Value.Y;
                                }
                            }
                        }
                        break;
                    case RelationType.None:
                        endLoop = true;
                        secondSuccess = true;
                        break;
                    default:
                        break;
                }
            }
            if (secondSuccess == false)
            {
                if (CheckIfRelationsAreOK(copyPolygon) == false)
                    return (false, null);
            }


            return (true,copyPolygon);
        }

        //BRESENHAM!
        private void RedrawPolygon(MyPolygon polygon)
        {
            for (int i = 0; i < Edges.Count; i++)
            {
                var edge = Edges[i];
                var newEdge = polygon.Edges[i];
                edge.first.Move(newEdge.first.X, newEdge.first.Y);
            }
            foreach(var edge in Edges)
            {
                edge.MoveWithPoints();
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
                        edge.DeleteDrawing(canvas);

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
            var startPointCopy = new Point(startPoint.X, startPoint.Y);
            var endPointCopy = new Point(endPoint.X, endPoint.Y);
            //var edgeIndex = Edges.IndexOf(edge);
            //if (edgeIndex == -1)
            //    return;
            //int edgesCount = Edges.Count;
            //var previousEdge2 = Edges[(edgeIndex - 1 + edgesCount) % edgesCount];
            //var nextEdge2 = Edges[(edgeIndex + 1 + edgesCount) % edgesCount];
            edge.MoveParallel(startPoint, endPoint);
            var result = ApplyRelationChanges(edge);
            startPoint = endPoint;
            if (result == false)
            {
                edge.MoveParallel(endPointCopy, startPointCopy);
                startPoint = startPointCopy;
                endPoint = endPointCopy;
            }
            
            //previousEdge2.MoveWithPoints();
            //nextEdge2.MoveWithPoints();
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

        public MyPolygon CopyWithoutDrawing()
        {
            List<MyPoint> verticleList = new List<MyPoint>();
            MyPolygon p = new MyPolygon();
            p.Edges = new List<MyEdge>();
            //add to verticleList
            foreach (var edge in Edges)
            {
                verticleList.Add(edge.first.CopyWithoutDrawing());
            }
            //create edges list from verticleList
            for (int i = 0; i < verticleList.Count; i++)
            {
                var edge = new MyEdge(verticleList[i], verticleList[(i + 1) % verticleList.Count]);
                p.Edges.Add(edge);
            }

            //copying relations
            for (int i = 0; i < Edges.Count; i++)
            {
                p.Edges[i].relationType = Edges[i].relationType;
                if (Edges[i].relationType != RelationType.None)
                {
                    var index = Edges.FindIndex(x => Object.ReferenceEquals(x, Edges[i].relationEdge) == true);
                    p.Edges[i].relationEdge = p.Edges[index];
                }
            }
            return p;
        }

        //BRESENHAM!
        public void DeleteDrawing()
        {
            foreach (var edge in Edges)
            {
                edge.DeleteRelation();
                edge.DeleteDrawing(canvas);
                canvas.Children.Remove(edge.first.ellipse);
                canvas.Children.Remove(edge.second.ellipse);
            }
        }

        private static bool CheckIfRelationsAreOK(MyPolygon polygon)
        {
            MyEdge relationEdge;
            Vector v1 = new Vector();
            Vector v2 = new Vector();
            foreach (var edge in polygon.Edges)
            {
                switch (edge.relationType)
                {
                    case RelationType.Equal:
                        relationEdge = edge.relationEdge;
                        var length = edge.Length();
                        var relationEdgeLength = relationEdge.Length();
                        if (Math.Abs(length - relationEdgeLength) > Globals.eps)
                        {
                            return false;
                        }
                        break;
                    case RelationType.Perpendicular:
                        relationEdge = edge.relationEdge;
                        v1.X = edge.second.X - edge.first.X;
                        v1.Y = edge.second.Y - edge.first.Y;
                        v2.X = relationEdge.second.X - relationEdge.first.X;
                        v2.Y = relationEdge.second.Y - relationEdge.first.Y;
                        double dotProduct = v1.X * v2.X + v1.Y * v2.Y;
                        if (Math.Abs(dotProduct) > Globals.eps)
                        {
                            return false;
                        }
                        break;
                    case RelationType.None:
                        break;
                    default:
                        break;
                }
            }
            return false;
        }
    }
}
