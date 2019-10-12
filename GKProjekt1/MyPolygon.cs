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
        public Point StartingVerticle { get; set; }
        public Ellipse StartingVerticleEllipse { get; set; }

        public Point LastVerticle { get; set; }
        public Ellipse LastVerticleEllipse { get; set; }

        public Dictionary<MyEdge, (MyEdge, RelationType)> Relations = new Dictionary<MyEdge, (MyEdge, RelationType)>();

        public Canvas canvas { get; set; }

        public MyPolygon(Point startingVerticle)
        {
            StartingVerticle = startingVerticle;
            LastVerticle = startingVerticle;
        }

        public void DrawStartingVerticle(Canvas canvas)
        {
            this.canvas = canvas;
            Ellipse ellipse = Draw.Verticle(StartingVerticle, this.canvas);
            StartingVerticleEllipse = ellipse;
            LastVerticleEllipse = ellipse;
        }

        public bool AddVerticle(Point p)
        {
            if (PointExtension.AreNear(p, StartingVerticle, (double)Globals.VerticleClickRadiusSize / 2.0) == true)
            {
                MyEdge e = new MyEdge(LastVerticle, StartingVerticle);              
                Edges.Add(e);
                return false;
            }
            else
            {
                MyEdge e = new MyEdge(LastVerticle, p);
                LastVerticle = p;
                Edges.Add(e);
                return true;
            }
        }

        public bool AddVerticleAndDraw(Point p)
        {
            if (PointExtension.AreNear(p, StartingVerticle, (double)Globals.VerticleClickRadiusSize / 2.0) == true)
            {
                Line line = Draw.Edge(LastVerticle, StartingVerticle, canvas);
                MyEdge e = new MyEdge(LastVerticle, StartingVerticle, LastVerticleEllipse, StartingVerticleEllipse, line);
                Edges.Add(e);
                return false;
            }
            else
            {
                Line line = Draw.Edge(LastVerticle, p, canvas);
                Ellipse secondEllipse = Draw.Verticle(p, canvas);
                MyEdge e = new MyEdge(LastVerticle, p, LastVerticleEllipse, secondEllipse, line);
                LastVerticle = p;
                LastVerticleEllipse = secondEllipse;
                Edges.Add(e);
                return true;
            }
        }

        public void DeleteDrawing()
        {
            foreach(var edge in Edges)
            {
                canvas.Children.Remove(edge.line);
                canvas.Children.Remove(edge.firstEllipse);
            }
        }

        public void AddRelation()
        {
            throw new NotImplementedException();
        }
    }
}
