using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GKProjekt1
{
    public class RelationIcon
    {
        private static Dictionary<int, (int EqualCounter, int PerpendicularCounter)> RelationCounter = new Dictionary<int, (int EqualCounter, int PerpendicularCounter)>();
        
        public RelationType relation { get; }

        MyEdge edge { get; set; }

        public Canvas canvas { get; set; }

        public Image image { get; set; }

        public TextBlock text { get; set; }

        public RelationIcon(MyEdge edge, RelationType relation, int PolygonId, MyPolygon polygon, Canvas canvas)
        {
            if (relation != RelationType.None)
            {
                if (RelationCounter.ContainsKey(PolygonId) == false)
                {
                    RelationCounter[PolygonId] = (0, 0);
                }

                this.edge = edge;
                this.canvas = canvas;
                this.relation = relation;
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

                var middleX = (edge.first.X + edge.second.X) / 2.0 - (double)Globals.BitmapSize / 2.0;
                var middleY = (edge.first.Y + edge.second.Y) / 2.0 - (double)Globals.BitmapSize / 2.0;

                var vectorX = edge.second.Y - edge.first.Y;
                var vectorY = edge.first.X - edge.second.X;

                var newMiddleX = middleX + vectorX * Globals.ImagePositionScale / edge.Length();
                var newMiddleY = middleY + vectorY * Globals.ImagePositionScale / edge.Length();

                if (polygon.IsPointInside(new System.Windows.Point(newMiddleX, newMiddleY)) == true)
                {
                    vectorX *= -1.0;
                    vectorY *= -1.0;
                    newMiddleX = middleX + vectorX * Globals.ImagePositionScale / edge.Length();
                    newMiddleY = middleY + vectorY * Globals.ImagePositionScale / edge.Length();
                }

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

                Canvas.SetLeft(image, newMiddleX);
                Canvas.SetTop(image, newMiddleY);

                Canvas.SetLeft(text, newMiddleX+Globals.BitmapSize);
                Canvas.SetTop(text, newMiddleY + (Globals.BitmapSize - Globals.RelationFontSize) / 1.5);

                Panel.SetZIndex(image, Globals.ImageZIndex);
                Panel.SetZIndex(text, Globals.ImageZIndex);
                canvas.Children.Add(image);
                canvas.Children.Add(text);
            }
        }
    }
}
