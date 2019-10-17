﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GKProjekt1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Mode ProgramMode { get; set; }

        private Dictionary<int, MyPolygon> Polygons = new Dictionary<int, MyPolygon>();

        //private bool FirstMouseClick = true;

        //Pointer Variables
        private bool IsDraggingOn = false;
        private DragObjectType CurrentDragObjectType = DragObjectType.Nothing;
        private Point DragStartingPoint = new Point();
        private int DragPolygonId = -1;
        private object DragObject = null;

        //Drawing variables
        private bool PolygonDrawing = false;
        private MyPolygon CurrentlyDrawingPolygon = null;
        private int PolygonNumber = 0;
        private Line CurrentLine = null;

        //private double counter = 0.0;

        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Canvas currentCanvas = sender as Canvas;
            Point CurrentMousePosition = e.GetPosition(currentCanvas);
            MyPoint p = new MyPoint(CurrentMousePosition.X, CurrentMousePosition.Y);
            
            switch (ProgramMode)
            {
                case Mode.Pointer:
                    {                       
                        foreach (var pol in Polygons)
                        {
                            foreach (var edge in pol.Value.Edges)
                            {
                                //check if point hit
                                if (MyPoint.AreNear(edge.first, CurrentMousePosition, Globals.VerticleClickRadiusSize) == true)
                                {
                                    CurrentDragObjectType = DragObjectType.Verticle;
                                    IsDraggingOn = true;
                                    DragPolygonId = pol.Key;
                                    DragObject = edge.first;
                                    return;
                                }                                                       
                            }
                            foreach(var edge in pol.Value.Edges)
                            {
                                //check if edge hit
                                if (edge.IsNearPoint(CurrentMousePosition, Globals.LineClickDistance) == true)
                                {
                                    CurrentDragObjectType = DragObjectType.Edge;
                                    IsDraggingOn = true;
                                    DragPolygonId = pol.Key;
                                    DragObject = edge;
                                    DragStartingPoint = CurrentMousePosition;
                                    return;
                                }
                            }
                            //check if inside Polygon
                            if (pol.Value.IsPointInside(CurrentMousePosition) == true)
                            {
                                //Debug.WriteLine($"Hit: {pol.Key}");
                                CurrentDragObjectType = DragObjectType.Polygon;
                                IsDraggingOn = true;
                                DragPolygonId = pol.Key;
                                DragObject = pol.Value;
                                DragStartingPoint = CurrentMousePosition;
                                return;
                            }
                        }
                        CurrentDragObjectType = DragObjectType.Nothing;
                    }
                    break;
                case Mode.Draw:
                    {
                        if (PolygonDrawing == false)
                        {
                            MyPolygon polygon = new MyPolygon(p, currentCanvas);
                            CurrentlyDrawingPolygon = polygon;
                            PolygonDrawing = true;
                            if (CurrentLine != null)
                            {
                                currentCanvas.Children.Remove(CurrentLine);
                            }
                            CurrentLine = Draw.SimpleEdge(CurrentMousePosition, CurrentMousePosition, currentCanvas);
                        }
                        else
                        {
                            var DrawResult = CurrentlyDrawingPolygon.AddVerticleAndDraw(p);
                            switch (DrawResult)
                            {
                                case PolygonDrawResult.DrawFinished:
                                    CurrentLine.X1 = p.X;
                                    CurrentLine.Y1 = p.Y;
                                    
                                    Polygons.Add(PolygonNumber, CurrentlyDrawingPolygon);

                                    currentCanvas.Children.Remove(CurrentLine);
                                    PolygonDrawing = false;
                                    CurrentlyDrawingPolygon = null;
                                    PolygonNumber++;
                                    break;
                                case PolygonDrawResult.NotEnoughEdges:
                                    MessageBox.Show("Not enough edges to finish polygon", "Polygons", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    break;
                                case PolygonDrawResult.DrawInProgress:
                                    CurrentLine.X1 = p.X;
                                    CurrentLine.Y1 = p.Y;
                                    break;
                                default:
                                    break;
                            }

                        }
                    }
                    break;
                default:
                    break;
            }
            return;
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ProgramMode == Mode.Pointer)
            {
                CurrentDragObjectType = DragObjectType.Nothing;
                IsDraggingOn = false;
                DragPolygonId = -1;
                DragObject = null;
                DragStartingPoint = new Point();
            }
        }

        private void PolygonCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Canvas currentCanvas = sender as Canvas;
            Point CurrentMousePosition = e.GetPosition(currentCanvas);
            switch (ProgramMode)
            {
                case Mode.Pointer://moving points and edges and polygons
                    if (IsDraggingOn == true)
                    {
                        switch (CurrentDragObjectType)
                        {
                            case DragObjectType.Verticle:
                                MyPoint verticle = DragObject as MyPoint;
                                Polygons[DragPolygonId].MoveVerticle(verticle, CurrentMousePosition);
                                //var previousEdge1 = Polygons[DragPolygonId].Edges.Last();
                                //foreach(var edge in Polygons[DragPolygonId].Edges)
                                //{
                                //    if (Object.ReferenceEquals(edge.first, verticle) == true)
                                //    {
                                //        verticle.Move(CurrentMousePosition);
                                //        edge.MoveWithPoints();
                                //        previousEdge1.MoveWithPoints();
                                //        return;
                                //    }
                                //    previousEdge1 = edge;
                                //}
                                break;
                            case DragObjectType.Edge:
                                MyEdge movedEdge = DragObject as MyEdge;
                                Polygons[DragPolygonId].MoveEdgeParallel(movedEdge, ref DragStartingPoint, ref CurrentMousePosition);
                                //var edgeIndex = Polygons[DragPolygonId].Edges.IndexOf(movedEdge);
                                //int edgesCount = Polygons[DragPolygonId].Edges.Count;
                                //var previousEdge2 = Polygons[DragPolygonId].Edges[(edgeIndex - 1 + edgesCount) % edgesCount];
                                //var nextEdge2 = Polygons[DragPolygonId].Edges[(edgeIndex + 1 + edgesCount) % edgesCount];
                                //movedEdge.MoveParallel(DragStartingPoint, CurrentMousePosition);
                                //DragStartingPoint = CurrentMousePosition;
                                //previousEdge2.MoveWithPoints();
                                //nextEdge2.MoveWithPoints();
                                break;
                            case DragObjectType.Polygon:
                                Polygons[DragPolygonId].MovePolygon(ref DragStartingPoint, ref CurrentMousePosition);
                                break;
                            case DragObjectType.Nothing:

                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        foreach (var pol in Polygons)
                        {
                            foreach (var edge in pol.Value.Edges)
                            {
                                //check if point hit
                                if (MyPoint.AreNear(edge.first, CurrentMousePosition, Globals.VerticleClickRadiusSize) == true)
                                {
                                    currentCanvas.Cursor = Cursors.Hand;
                                    return;
                                }
                            }
                            foreach (var edge in pol.Value.Edges)
                            {
                                //check if edge hit
                                if (edge.IsNearPoint(CurrentMousePosition, Globals.LineClickDistance) == true)
                                {
                                    currentCanvas.Cursor = Cursors.Hand;
                                    return;
                                }
                            }
                            //check if inside Polygon
                            if (pol.Value.IsPointInside(CurrentMousePosition) == true)
                            {
                                currentCanvas.Cursor = Cursors.SizeAll;
                                return;
                            }
                        }
                        currentCanvas.Cursor = Cursors.Arrow;
                    }
                    break;
                case Mode.Draw:
                    if (PolygonDrawing == true)
                    {
                        if (CurrentLine != null)
                        {
                            CurrentLine.X2 = CurrentMousePosition.X;
                            CurrentLine.Y2 = CurrentMousePosition.Y;
                        }
                    }
                    break;
            }            
        }

        private bool ClearUnfinishedPolygon()
        {
            if (CurrentlyDrawingPolygon != null)
            {
                MessageBoxResult ans = MessageBox.Show("If you change mode during drawing,\n" +
                    "currently drawn polygon will be deleted!\n" +
                    "Do you want to continue drawing?", "Polygons", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (ans == MessageBoxResult.Yes)
                {
                    return false;
                }
                else
                {
                    CurrentlyDrawingPolygon.DeleteDrawing();
                    if (CurrentLine != null)
                    {
                        CurrentlyDrawingPolygon.canvas.Children.Remove(CurrentLine);
                    }
                    CurrentLine = null;
                    CurrentlyDrawingPolygon = null;
                    PolygonDrawing = false;
                }
            }
            return true;
        }

        private void PointerMode_Click(object sender, RoutedEventArgs e)
        {
            if (ClearUnfinishedPolygon() == true)
            {
                ProgramMode = Mode.Pointer;
            }
            else
            {
                PointerButton.IsChecked = false;
                DrawButton.IsChecked = true;
            }
        }

        private void DrawMode_Click(object sender, RoutedEventArgs e)
        {
            ProgramMode = Mode.Draw;
        }
    }
}
