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
