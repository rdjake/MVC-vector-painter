﻿using System;
using MiniEditor;
using System.Collections.Generic;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;

public interface IGraphic
{
    void Polyline(IEnumerable<Point> points, Color color, uint thickness);
    void Polygon(IEnumerable<Point> points, Color color, uint thickness);
    void Circle(Point A, Point B, Color color, uint thickness);
}

public interface IBrush
{
    Color MainColor { get; set; }
}
public interface IPen
{
    Color MainColor { get; set; }
}

public class BuildFigure : IGraphic
{

    Canvas canvas;

    public BuildFigure(Canvas canvas)
    {
        this.canvas = canvas;
    }


    public void Polyline(IEnumerable<Point> points, Color color, uint thickness)
    {
        Polyline polyline = new Polyline();

        polyline.Stroke = new SolidColorBrush(Color.FromRgb(color.R, color.G, color.B));
        polyline.StrokeThickness = thickness;

        foreach (var point in points)
        {
            polyline.Points.Add(new System.Windows.Point(point.X, point.Y));
        }

        canvas.Children.Add(polyline);
    }
    public void Polygon(IEnumerable<Point> points, Color color, uint thickness)
    {
        Polygon polygon = new Polygon();

        polygon.Stroke = new SolidColorBrush(Color.FromRgb(color.R, color.G, color.B));
        polygon.StrokeThickness = thickness;

        foreach (var point in points)
        {
            polygon.Points.Add(new System.Windows.Point(point.X, point.Y));
        }

        canvas.Children.Add(polygon);
    }

    public void Circle(Point A, Point B, Color color, uint thickness)
    {
        double radius = Math.Sqrt(Math.Pow(A.X - B.X, 2) + Math.Pow(A.Y - B.Y, 2));
        Ellipse myEllipse = new Ellipse();

        myEllipse.Stroke = new SolidColorBrush(Color.FromRgb(color.R, color.G, color.B));
        myEllipse.StrokeThickness = thickness;

        myEllipse.Width = 2 * radius;
        myEllipse.Height = 2 * radius;

        canvas.Children.Add(myEllipse);

        Canvas.SetLeft(myEllipse, A.X - radius);
        Canvas.SetTop(myEllipse, A.Y - radius);
    }
}
