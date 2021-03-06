﻿using System;
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

        public PolygonDrawResult AddVerticleAndDraw(MyPoint p)
        {
            if (MyPoint.AreNear(p, StartingVerticle, (double)Globals.VerticleClickRadiusSize / 2.0) == true)
            {
                if (Edges.Count < 2)
                    return PolygonDrawResult.NotEnoughEdges;
                MyEdge e = new MyEdge(LastVerticle, StartingVerticle);
                Draw.Edge(e, canvas);
                Edges.Add(e);
                return PolygonDrawResult.DrawFinished;
            }
            else
            {
                Draw.Verticle(p, canvas);
                MyEdge e = new MyEdge(LastVerticle, p);
                Draw.Edge(e, canvas);
                LastVerticle = p;
                Edges.Add(e);
                return PolygonDrawResult.DrawInProgress;
            }
        }

        public void MoveVerticle(MyPoint verticle, Point endPoint)
        {
            foreach (var edge in Edges)
            {
                if (Object.ReferenceEquals(edge.first, verticle) == true)
                {
                    var previousPoint = new Point(verticle.X, verticle.Y);
                    verticle.Move(endPoint);
                    var result = ApplyRelationChanges(edge);
                    if (result == false)
                    {
                        verticle.Move(previousPoint);
                    }

                    return;
                }
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

        private (bool success, MyPolygon changedPolygon) FixRelationsMovingVerticle(MyEdge startingEdge)
        {
            MyPolygon copyPolygon = this.CopyWithoutDrawing();
            var startingEdgeIndex = copyPolygon.Edges.FindIndex(x => x == startingEdge);
            bool endLoop = false;
            bool firstSuccess = false;
            bool secondSuccess = false;

            //going right (list order)
            for (int i = 0; i < copyPolygon.Edges.Count && endLoop == false; i++)
            {
                var edge = copyPolygon.Edges[(i + startingEdgeIndex) % copyPolygon.Edges.Count];
                switch (edge.relationType)
                {
                    case RelationType.Equal:
                        RelationFixer.FixEqualRelation(edge.first, edge.second, edge, ref endLoop, ref firstSuccess);
                        break;
                    case RelationType.Perpendicular:
                        RelationFixer.FixPerpendicularRelation(edge.first, edge.second, edge.relationEdge.first, edge.relationEdge.second);
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
                        RelationFixer.FixEqualRelation(edge.second, edge.first, edge, ref endLoop, ref secondSuccess);
                        break;
                    case RelationType.Perpendicular:
                        RelationFixer.FixPerpendicularRelation(edge.second, edge.first, edge.relationEdge.second, edge.relationEdge.first);
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
                return (false, null);
            }
            return (true, copyPolygon);
        }

        private void RedrawPolygon(MyPolygon polygon)
        {
            for (int i = 0; i < Edges.Count; i++)
            {
                var edge = Edges[i];
                var newEdge = polygon.Edges[i];
                edge.first.Move(newEdge.first.X, newEdge.first.Y);
            }
            foreach (var edge in Edges)
            {
                edge.MoveWithPoints();
            }
        }

        public void ChangePolygonLineDrawingMethodBefore()
        {
            foreach (var edge in Edges)
            {
                edge.DeleteDrawing();
            }
        }

        public void ChangePolygonLineDrawingMethodAfter()
        {
            foreach (var edge in Edges)
            {
                Draw.Edge(edge, canvas);
            }
        }

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
                        edge.DeleteDrawing();

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

        public void MoveEdgeParallel(MyEdge edge, ref Point startPoint, ref Point endPoint)
        {
            var startPointCopy = new Point(startPoint.X, startPoint.Y);
            var endPointCopy = new Point(endPoint.X, endPoint.Y);

            edge.MoveParallel(startPoint, endPoint);
            var result = ApplyRelationChanges(edge);
            startPoint = endPoint;
            if (result == false)
            {
                edge.MoveParallel(endPointCopy, startPointCopy);
                startPoint = startPointCopy;
                endPoint = endPointCopy;
            }
        }

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

            MyEdge Ray = new MyEdge(new MyPoint(p.X, p.Y), new MyPoint(p.X + canvas.ActualWidth, p.Y));
            int intersectCounter = 0;

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
                        //po przeciwnych stronach polprostej Ray to intersectCounter++ i i++
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
                        //po przeciwnych stronach polprostej Ray to intersectCounter++ i i++
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
            }
            return intersectCounter % 2 == 1;
        }

        private List<MyEdge> RemoveCollinearEdges()
        {
            List<MyEdge> ClearEdges = new List<MyEdge>();
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

        public void DeleteDrawing()
        {
            foreach (var edge in Edges)
            {
                edge.DeleteRelation();
                edge.DeleteDrawing();
                canvas.Children.Remove(edge.first.ellipse);
                canvas.Children.Remove(edge.second.ellipse);
            }
        }
    }
}
