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
        public Mode ProgramMode { get; set; } = Mode.Pointer;

        private Dictionary<int, MyPolygon> Polygons = new Dictionary<int, MyPolygon>();

        //Pointer Variables
        private bool IsDraggingOn = false;
        private DragObjectType CurrentDragObjectType = DragObjectType.Nothing;
        private Point DragStartingPoint = new Point();
        private int DragPolygonId = -1;
        private object DragObject = null;

        //Drawing Variables
        private bool PolygonDrawing = false;
        private MyPolygon CurrentlyDrawingPolygon = null;
        private int PolygonNumber = 0;
        private MyLine CurrentLine = null;

        //Adding Relation Variables
        private int RelationPolygonId = -1;
        private MyEdge RelationSelectedEdge = null;


        public MainWindow()
        {
            InitializeComponent();
            Panel.SetZIndex(ButtonGridRow, Globals.ButtonsGridZIndex);            
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
                            foreach (var edge in pol.Value.Edges)
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
                            CurrentLine?.DeleteDrawing();
                            CurrentLine = Draw.SimpleEdge(CurrentMousePosition, CurrentMousePosition, currentCanvas);
                        }
                        else
                        {
                            var DrawResult = CurrentlyDrawingPolygon.AddVerticleAndDraw(p);
                            switch (DrawResult)
                            {
                                case PolygonDrawResult.DrawFinished:
                                    CurrentLine.SetFirstPoint(CurrentMousePosition);

                                    Polygons.Add(PolygonNumber, CurrentlyDrawingPolygon);

                                    CurrentLine.DeleteDrawing();
                                    PolygonDrawing = false;
                                    CurrentlyDrawingPolygon = null;
                                    PolygonNumber++;
                                    break;
                                case PolygonDrawResult.NotEnoughEdges:
                                    MessageBox.Show("Not enough edges to finish polygon", Globals.WindowName, MessageBoxButton.OK, MessageBoxImage.Warning);
                                    break;
                                case PolygonDrawResult.DrawInProgress:
                                    CurrentLine.SetFirstPoint(CurrentMousePosition);
                                    break;
                                default:
                                    break;
                            }

                        }
                    }
                    break;
                case Mode.AddMiddleVerticle:
                    {
                        foreach (var pol in Polygons)
                        {
                            foreach (var edge in pol.Value.Edges)
                            {
                                //check if edge hit
                                if (edge.IsNearPoint(CurrentMousePosition, Globals.LineClickDistance) == true)
                                {
                                    pol.Value.AddMiddleVerticleOnEdge(edge);
                                    return;
                                }
                            }
                        }
                    }
                    break;
                case Mode.AddEqualRelation:
                case Mode.AddPerpendicularRelation:
                    {
                        RelationType relationType;
                        if (ProgramMode == Mode.AddEqualRelation)
                        {
                            relationType = RelationType.Equal;
                        }
                        else
                        {
                            relationType = RelationType.Perpendicular;
                        }
                        if (RelationPolygonId == -1)
                        {
                            foreach (var pol in Polygons)
                            {
                                foreach (var edge in pol.Value.Edges)
                                {
                                    //check if edge hit
                                    if (edge.relationType == RelationType.None)
                                    {
                                        if (edge.IsNearPoint(CurrentMousePosition, Globals.LineClickDistance) == true)
                                        {
                                            edge.SelectEdge();
                                            RelationSelectedEdge = edge;
                                            RelationPolygonId = pol.Key;
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (var edge in Polygons[RelationPolygonId].Edges)
                            {
                                //check if edge hit
                                if (edge.relationType == RelationType.None)
                                {
                                    if (edge.IsNearPoint(CurrentMousePosition, Globals.LineClickDistance) == true)
                                    {
                                        if (Object.ReferenceEquals(edge, RelationSelectedEdge) == false)
                                        {
                                            edge.relationEdge = RelationSelectedEdge;
                                            RelationSelectedEdge.relationEdge = edge;
                                            edge.relationType = relationType;
                                            RelationSelectedEdge.relationType = relationType;
                                            var result = Polygons[RelationPolygonId].ApplyRelationChanges(edge);
                                            if (result == true)
                                            {
                                                edge.relationIcon = new RelationIcon(edge, relationType, RelationPolygonId, Polygons[RelationPolygonId], currentCanvas);
                                                RelationSelectedEdge.relationIcon = new RelationIcon(RelationSelectedEdge, relationType, RelationPolygonId, Polygons[RelationPolygonId], currentCanvas);
                                            }
                                            else
                                            {
                                                edge.relationEdge = null;
                                                RelationSelectedEdge.relationEdge = null;
                                                edge.relationType = RelationType.None;
                                                RelationSelectedEdge.relationType = RelationType.None;
                                            }
                                        }
                                        RelationSelectedEdge.UnselectEdge();
                                        RelationSelectedEdge = null;
                                        RelationPolygonId = -1;
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case Mode.DeleteVerticleOrRelation:
                    {
                        foreach (var pol in Polygons)
                        {
                            foreach (var edge in pol.Value.Edges)
                            {
                                //check if point hit
                                if (MyPoint.AreNear(edge.first, CurrentMousePosition, Globals.VerticleClickRadiusSize) == true)
                                {
                                    if (Polygons[pol.Key].DeleteVerticle(edge.first) == true)
                                    {
                                        Polygons[pol.Key] = null;
                                        Polygons.Remove(pol.Key);
                                    }
                                    return;
                                }
                            }
                            foreach (var edge in pol.Value.Edges)
                            {
                                //check if edge hit
                                if (edge.IsNearPoint(CurrentMousePosition, Globals.LineClickDistance) == true)
                                {
                                    edge.DeleteRelation();
                                    return;
                                }
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
                Canvas currentCanvas = sender as Canvas;
                CurrentDragObjectType = DragObjectType.Nothing;
                IsDraggingOn = false;
                DragPolygonId = -1;
                DragObject = null;
                DragStartingPoint = new Point();
                currentCanvas.Cursor = Cursors.Arrow;
            }
        }

        private void PolygonCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Canvas currentCanvas = sender as Canvas;
            Point CurrentMousePosition = e.GetPosition(currentCanvas);
            switch (ProgramMode)
            {
                case Mode.Pointer://moving points and edges and polygons
                    {
                        if (IsDraggingOn == true)
                        {
                            switch (CurrentDragObjectType)
                            {
                                case DragObjectType.Verticle:
                                    MyPoint verticle = DragObject as MyPoint;
                                    Polygons[DragPolygonId].MoveVerticle(verticle, CurrentMousePosition);
                                    currentCanvas.Cursor = Cursors.Hand;
                                    return;
                                case DragObjectType.Edge:
                                    MyEdge movedEdge = DragObject as MyEdge;
                                    Polygons[DragPolygonId].MoveEdgeParallel(movedEdge, ref DragStartingPoint, ref CurrentMousePosition);
                                    currentCanvas.Cursor = Cursors.Hand;
                                    return;
                                case DragObjectType.Polygon:
                                    Polygons[DragPolygonId].MovePolygon(ref DragStartingPoint, ref CurrentMousePosition);
                                    currentCanvas.Cursor = Cursors.Hand;
                                    return;
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
                        }
                        currentCanvas.Cursor = Cursors.Arrow;
                    }
                    break;
                case Mode.Draw:
                    {
                        if (PolygonDrawing == true)
                        {
                            if (CurrentLine != null)
                            {
                                CurrentLine.SetSecondPoint(CurrentMousePosition);
                            }
                        }
                    }
                    break;
                case Mode.AddMiddleVerticle:
                    {
                        foreach (var pol in Polygons)
                        {
                            foreach (var edge in pol.Value.Edges)
                            {
                                //check if edge hit
                                if (edge.IsNearPoint(CurrentMousePosition, Globals.LineClickDistance) == true)
                                {
                                    currentCanvas.Cursor = Cursors.Hand;
                                    return;
                                }
                            }
                        }
                        currentCanvas.Cursor = Cursors.Arrow;
                    }
                    break;
                case Mode.AddEqualRelation:
                case Mode.AddPerpendicularRelation:
                    {
                        if (RelationPolygonId == -1)
                        {
                            foreach (var pol in Polygons)
                            {
                                foreach (var edge in pol.Value.Edges)
                                {
                                    //check if edge hit
                                    if (edge.relationType == RelationType.None && edge.IsNearPoint(CurrentMousePosition, Globals.LineClickDistance) == true)
                                    {
                                        currentCanvas.Cursor = Cursors.Hand;
                                        return;
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (var edge in Polygons[RelationPolygonId].Edges)
                            {
                                //check if edge hit
                                if (edge.relationType == RelationType.None || Object.ReferenceEquals(edge, RelationSelectedEdge) == true)
                                {
                                    if (edge.IsNearPoint(CurrentMousePosition, Globals.LineClickDistance) == true)
                                    {
                                        currentCanvas.Cursor = Cursors.Hand;
                                        return;
                                    }
                                }
                            }
                        }
                        currentCanvas.Cursor = Cursors.Arrow;
                    }
                    break;
                case Mode.DeleteVerticleOrRelation:
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
                                if (edge.relationType != RelationType.None && edge.IsNearPoint(CurrentMousePosition, Globals.LineClickDistance) == true)
                                {
                                    currentCanvas.Cursor = Cursors.Hand;
                                    return;
                                }
                            }
                        }
                        currentCanvas.Cursor = Cursors.Arrow;
                    }
                    break;
                default:
                    break;
            }
        }

        private bool ClearUnfinishedPolygon()
        {
            if (CurrentlyDrawingPolygon != null)
            {
                MessageBoxResult ans = MessageBox.Show("If you change mode during drawing,\n" +
                    "currently drawn polygon will be deleted!\n" +
                    "Do you want to continue drawing?", Globals.WindowName, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (ans == MessageBoxResult.Yes)
                {
                    return false;
                }
                else
                {
                    CurrentlyDrawingPolygon.DeleteDrawing();
                    CurrentLine?.DeleteDrawing();
                    CurrentLine = null;
                    CurrentlyDrawingPolygon = null;
                    PolygonDrawing = false;
                }
            }
            return true;
        }
        private bool DoesUserWantToChangeMode(object sender)
        {
            if (ProgramMode == Mode.Draw)
            {
                if (ClearUnfinishedPolygon() == false)
                {
                    var button = sender as RadioButton;
                    button.IsChecked = false;
                    DrawButton.IsChecked = true;
                    return false;
                }
            }
            else if (ProgramMode == Mode.AddEqualRelation || ProgramMode == Mode.AddPerpendicularRelation)
            {
                if (RelationPolygonId != -1)
                {
                    RelationSelectedEdge.UnselectEdge();
                }
                RelationSelectedEdge = null;
                RelationPolygonId = -1;
            }
            return true;
        }
        private void GenerateSamplePolygon()
        {
            //TODO:
            //generate sample polygon
            var pointsList = new List<MyPoint>
            {                
                new MyPoint(626d, 57d),
                new MyPoint(856d, 191d),
                new MyPoint(744d, 301d),
                new MyPoint(851d, 532d),
                new MyPoint(460d, 502d),
                new MyPoint(367d, 408d),
                new MyPoint(427d, 149d)
            };

            MyPolygon polygon = new MyPolygon(pointsList.Last(), PolygonCanvas);

            foreach(var point in pointsList)
            {
                polygon.AddVerticleAndDraw(point);
            }

            Polygons.Add(PolygonNumber, polygon);


            polygon.Edges[0].relationEdge = polygon.Edges[3];            
            polygon.Edges[3].relationEdge = polygon.Edges[0];
            polygon.Edges[0].relationType = RelationType.Perpendicular;
            polygon.Edges[3].relationType = RelationType.Perpendicular;

            polygon.ApplyRelationChanges(polygon.Edges[0]);

            polygon.Edges[0].relationIcon = new RelationIcon(polygon.Edges[0], RelationType.Perpendicular, PolygonNumber, polygon, PolygonCanvas);
            polygon.Edges[3].relationIcon = new RelationIcon(polygon.Edges[3], RelationType.Perpendicular, PolygonNumber, polygon, PolygonCanvas);

            polygon.Edges[1].relationEdge = polygon.Edges[6];
            polygon.Edges[6].relationEdge = polygon.Edges[1];
            polygon.Edges[1].relationType = RelationType.Equal;
            polygon.Edges[6].relationType = RelationType.Equal;

            polygon.ApplyRelationChanges(polygon.Edges[1]);

            polygon.Edges[1].relationIcon = new RelationIcon(polygon.Edges[1], RelationType.Equal, PolygonNumber, polygon, PolygonCanvas);
            polygon.Edges[6].relationIcon = new RelationIcon(polygon.Edges[6], RelationType.Equal, PolygonNumber, polygon, PolygonCanvas);

            polygon.Edges[2].relationEdge = polygon.Edges[5];
            polygon.Edges[5].relationEdge = polygon.Edges[2];
            polygon.Edges[2].relationType = RelationType.Perpendicular;
            polygon.Edges[5].relationType = RelationType.Perpendicular;

            polygon.ApplyRelationChanges(polygon.Edges[2]);

            polygon.Edges[2].relationIcon = new RelationIcon(polygon.Edges[2], RelationType.Perpendicular, PolygonNumber, polygon, PolygonCanvas);
            polygon.Edges[5].relationIcon = new RelationIcon(polygon.Edges[5], RelationType.Perpendicular, PolygonNumber, polygon, PolygonCanvas);

            PolygonNumber++;
        }

        private void ClearAll()
        {
            //delete currently drawing polygon
            CurrentlyDrawingPolygon?.DeleteDrawing();
            if (CurrentlyDrawingPolygon != null)
            {
                CurrentLine?.DeleteDrawing();
            }
            CurrentLine = null;
            CurrentlyDrawingPolygon = null;
            PolygonDrawing = false;

            //delete polygons
            foreach (var pol in Polygons)
            {
                pol.Value.DeleteDrawing();
            }

            //clear RelationIcon
            RelationIcon.RelationCounter.Clear();

            //clear variables
            ClearVariables();
            //set pointerbutton
            PointerButton.IsChecked = true;
        }
        private void ClearVariables()
        {
            Polygons.Clear();
            ProgramMode = Mode.Pointer;
            //Pointer Variables
            IsDraggingOn = false;
            CurrentDragObjectType = DragObjectType.Nothing;
            DragStartingPoint = new Point();
            DragPolygonId = -1;
            DragObject = null;

            //Drawing Variables
            PolygonDrawing = false;
            CurrentlyDrawingPolygon = null;
            PolygonNumber = 0;
            CurrentLine = null;

            //Adding Relation Variables
            RelationPolygonId = -1;
            RelationSelectedEdge = null;
        }
        private void PointerMode_Click(object sender, RoutedEventArgs e)
        {
            if (DoesUserWantToChangeMode(sender) == true)
            {
                ProgramMode = Mode.Pointer;
                var button = sender as RadioButton;
                button.IsChecked = true;
            }
        }
        private void DrawMode_Click(object sender, RoutedEventArgs e)
        {
            ProgramMode = Mode.Draw;
            var button = sender as RadioButton;
            button.IsChecked = true;
        }
        private void DeleteMode_Click(object sender, RoutedEventArgs e)
        {
            if (DoesUserWantToChangeMode(sender) == true)
            {
                ProgramMode = Mode.DeleteVerticleOrRelation;
                var button = sender as RadioButton;
                button.IsChecked = true;
            }
        }
        private void AddMiddleVerticleMode_Click(object sender, RoutedEventArgs e)
        {
            if (DoesUserWantToChangeMode(sender) == true)
            {
                ProgramMode = Mode.AddMiddleVerticle;
                var button = sender as RadioButton;
                button.IsChecked = true;
            }
        }
        private void AddEqualRelationMode_Click(object sender, RoutedEventArgs e)
        {
            if (DoesUserWantToChangeMode(sender) == true)
            {
                ProgramMode = Mode.AddEqualRelation;
                var button = sender as RadioButton;
                button.IsChecked = true;
            }
        }
        private void AddPerpendicularRelationMode_Click(object sender, RoutedEventArgs e)
        {
            if (DoesUserWantToChangeMode(sender) == true)
            {
                ProgramMode = Mode.AddPerpendicularRelation;
                var button = sender as RadioButton;
                button.IsChecked = true;
            }
        }
        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure?", Globals.WindowName, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                ClearAll();
            }
        }
        private void GenerateSamplePolygon_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure?\nOperation will clear all first.", Globals.WindowName, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                ClearAll();
                GenerateSamplePolygon();
            }
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.D1:
                    PointerMode_Click(PointerButton, null);
                    break;
                case Key.D2:
                    DrawMode_Click(DrawButton, null);
                    break;
                case Key.D3:
                    AddEqualRelationMode_Click(AddEqualRelationButton, null);
                    break;
                case Key.D4:
                    AddPerpendicularRelationMode_Click(AddPerpendicularRelationButton, null);
                    break;
                case Key.D5:
                    AddMiddleVerticleMode_Click(AddMiddleVerticleButton, null);
                    break;
                case Key.D6:
                    DeleteMode_Click(DeleteButton, null);
                    break;
                case Key.D9:
                    GenerateSamplePolygon_Click(GenerateSamplePolygonButton, null);
                    break;
                case Key.D0:
                    ClearAll_Click(ClearAllButton, null);
                    break;
                default:
                    break;
            }
        }

        private void PolygonCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            if (Globals.__GenerateSamplePolygon__ == true)
            {
                GenerateSamplePolygon();
            }
        }

        private void RedrawPolygonsBefore()
        {
            foreach(var pol in Polygons)
            {
                pol.Value.ChangePolygonLineDrawingMethodBefore();
            }
        }

        private void RedrawPolygonsAfter()
        {
            foreach (var pol in Polygons)
            {
                pol.Value.ChangePolygonLineDrawingMethodAfter();
            }
        }

        private void BresenhamLineMode_Click(object sender, RoutedEventArgs e)
        {
            RedrawPolygonsBefore();
            Globals.lineDrawingMode = LineDrawingMode.Bresenham;
            RedrawPolygonsAfter();
        }

        private void LibraryLineMode_Click(object sender, RoutedEventArgs e)
        {
            RedrawPolygonsBefore();
            Globals.lineDrawingMode = LineDrawingMode.Library;
            RedrawPolygonsAfter();
        }

        private void AntialiasingWULineMode_Click(object sender, RoutedEventArgs e)
        {
            RedrawPolygonsBefore();
            Globals.lineDrawingMode = LineDrawingMode.AntialiasingWU;
            RedrawPolygonsAfter();
        }

        private void BresenhamSymmetricLineMode_Click(object sender, RoutedEventArgs e)
        {
            RedrawPolygonsBefore();
            Globals.lineDrawingMode = LineDrawingMode.BresenhamSymmetric;
            RedrawPolygonsAfter();
        }
    }
}
