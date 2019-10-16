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

        private bool FirstMouseClick = true;
        private bool PolygonDrawing = false;
        private bool IsDraggingOn = false;
        private MyPolygon CurrentlyDrawingPolygon = null;
        private int PolygonNumber = 0;

        private Line CurrentLine = null;

        private double counter = 0.0;

        private Dictionary<int, MyPolygon> Polygons = new Dictionary<int, MyPolygon>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Canvas currentCanvas = sender as Canvas;
            Point p = e.GetPosition(currentCanvas);
            //Debug.WriteLine("Click"); 
            switch (ProgramMode)
            {
                case Mode.Pointer:
                    IsDraggingOn = true;
                    break;
                case Mode.Draw:
                    {
                        if (PolygonDrawing == false)
                        {
                            MyPolygon polygon = new MyPolygon(p);
                            polygon.DrawStartingVerticle(currentCanvas);
                            CurrentlyDrawingPolygon = polygon;
                            //FirstMouseClick = false;
                            PolygonDrawing = true;
                            if (CurrentLine != null)
                            {
                                currentCanvas.Children.Remove(CurrentLine);
                            }
                            CurrentLine = Draw.Edge(p, p, currentCanvas);
                        }
                        else
                        {
                            var DrawResult = CurrentlyDrawingPolygon.AddVerticleAndDraw(p);
                            switch (DrawResult)
                            {
                                case PolygonDrawResult.DrawFinished:
                                    CurrentLine.X1 = p.X;
                                    CurrentLine.Y1 = p.Y;
                                    PolygonDrawing = false;
                                    Polygons.Add(PolygonNumber, CurrentlyDrawingPolygon);

                                    PolygonNumber++;

                                    CurrentlyDrawingPolygon = null;
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
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ProgramMode == Mode.Pointer)
            {
                IsDraggingOn = false;
            }
        }

        private void PolygonCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Canvas currentCanvas = sender as Canvas;

            Point p = e.GetPosition(currentCanvas);
            //Debug.WriteLine($"MouseMove X:{p.X},Y:{p.Y} #{counter++}");
            switch (ProgramMode)
            {
                case Mode.Pointer://moving points and edges and polygons
                    if (IsDraggingOn == true)
                    {
                        foreach(var pol in Polygons)
                        {
                            foreach(var edge in pol.Value.Edges)
                            {
                                //check if point hit
                                if (PointExtension.AreNear(edge.first, p, Globals.VerticleClickRadiusSize) == true)
                                {
                                    double X = p.X - (double)Globals.VerticleSize / 2.0;
                                    double Y = p.Y - (double)Globals.VerticleSize / 2.0;
                                    Canvas.SetLeft(edge.firstEllipse, X);
                                    Canvas.SetTop(edge.firstEllipse, Y);
                                }
                            }
                        }
                    }
                    break;
                case Mode.Draw:
                    if (PolygonDrawing == true)
                    {
                        if (CurrentLine != null)
                        {
                            CurrentLine.X2 = p.X;
                            CurrentLine.Y2 = p.Y;
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
                    "currently drown polygon will be deleted!\n" +
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
