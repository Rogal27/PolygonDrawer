using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GKProjekt1
{
    public class RelationIcon
    {
        private static Dictionary<int, (int EqualCounter, int PerpendicularCounter)> RelationCounter = new Dictionary<int, (int EqualCounter, int PerpendicularCounter)>();
        
        public RelationType relation { get; }

        public MyEdge edge { get; set; }

        public MyPolygon polygon { get; set; }

        public Image image { get; set; }

        public TextBlock text { get; set; }

        public RelationIcon(MyEdge _edge, RelationType _relation, int PolygonId, MyPolygon _polygon, Canvas _canvas)
        {
            if (relation != RelationType.None)
            {
                if (RelationCounter.ContainsKey(PolygonId) == false)
                {
                    RelationCounter[PolygonId] = (0, 0);
                }

                this.edge = _edge;
                this.relation = _relation;
                this.polygon = _polygon;
                edge.relationType = relation;
                image = new Image();
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                if (relation == RelationType.Equal)
                {
                    bitmap.UriSource = new Uri(Globals.EqualBitmapSource, UriKind.Relative);
                }
                else
                {
                    bitmap.UriSource = new Uri(Globals.PerpendicularBitmapSource, UriKind.Relative);
                }
                bitmap.EndInit();
                image.Source = bitmap;
                image.Width = Globals.BitmapSize;
                image.Height = Globals.BitmapSize;                

                text = new TextBlock();
                text.Foreground = new SolidColorBrush(Globals.RelationFontColor);
                text.FontSize = Globals.RelationFontSize;
                if (relation == RelationType.Equal)
                {
                    text.Text = $"{RelationCounter[PolygonId].EqualCounter / 2}";
                    RelationCounter[PolygonId] = (RelationCounter[PolygonId].EqualCounter + 1, RelationCounter[PolygonId].PerpendicularCounter);
                }
                else
                {
                    text.Text = $"{RelationCounter[PolygonId].PerpendicularCounter / 2}";
                    RelationCounter[PolygonId] = (RelationCounter[PolygonId].EqualCounter, RelationCounter[PolygonId].PerpendicularCounter + 1);
                }

                var points = CalculatePoints();

                Canvas.SetLeft(image, points.imagePoint.X);
                Canvas.SetTop(image, points.imagePoint.Y);

                Canvas.SetLeft(text, points.textPoint.X);
                Canvas.SetTop(text, points.textPoint.Y);

                Panel.SetZIndex(image, Globals.ImageZIndex);
                Panel.SetZIndex(text, Globals.ImageZIndex);
                polygon.canvas.Children.Add(image);
                polygon.canvas.Children.Add(text);
            }
        }

        public void MoveIcon()
        {
            var points = CalculatePoints();
            Canvas.SetLeft(image, points.imagePoint.X);
            Canvas.SetTop(image, points.imagePoint.Y);

            Canvas.SetLeft(text, points.textPoint.X);
            Canvas.SetTop(text, points.textPoint.Y);
        }

        public void Delete()
        {
            polygon.canvas.Children.Remove(image);
            polygon.canvas.Children.Remove(text);
        }

        private (Point imagePoint, Point textPoint) CalculatePoints()
        {
            double middleX = (edge.first.X + edge.second.X) / 2.0 - (double)Globals.BitmapSize / 2.0;
            double middleY = (edge.first.Y + edge.second.Y) / 2.0 - (double)Globals.BitmapSize / 2.0;

            double vectorX = edge.second.Y - edge.first.Y;
            double vectorY = edge.first.X - edge.second.X;

            double newMiddleX = middleX + vectorX * Globals.ImagePositionScale / edge.Length();
            double newMiddleY = middleY + vectorY * Globals.ImagePositionScale / edge.Length();

            if (polygon.IsPointInside(new Point(newMiddleX, newMiddleY)) == true)
            {
                vectorX *= -1.0;
                vectorY *= -1.0;
                newMiddleX = middleX + vectorX * Globals.ImagePositionScale / edge.Length();
                newMiddleY = middleY + vectorY * Globals.ImagePositionScale / edge.Length();
            }

            Point imagePoint = new Point(newMiddleX, newMiddleY);
            Point textPoint = new Point(newMiddleX + Globals.BitmapSize, newMiddleY + (Globals.BitmapSize - Globals.RelationFontSize) / 1.5);
            return (imagePoint, textPoint);
        }
    }
}
