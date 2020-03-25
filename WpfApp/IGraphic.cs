using System;
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
    void Polyline(List<System.Windows.Point> points, Color color, uint thickness);
    void Polygon(List<System.Windows.Point> points, Color color, uint thickness);
    void Rectangle(List<System.Windows.Point> points, Color color, uint thickness);
    void Circle(List<System.Windows.Point> points, Color color, uint thickness);
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


    public void Polyline(List<System.Windows.Point> points, Color color, uint thickness)
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
    public void Polygon(List<System.Windows.Point> points, Color color, uint thickness)
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

    public void Rectangle(List<System.Windows.Point> points, Color color, uint thickness)
    {
        Polygon polygon = new Polygon();

        polygon.Stroke = new SolidColorBrush(Color.FromRgb(color.R, color.G, color.B));
        polygon.StrokeThickness = thickness;

        var C = points.ToArray();

        polygon.Points.Add(new System.Windows.Point(C[0].X, C[0].Y));
        polygon.Points.Add(new System.Windows.Point(C[1].X, C[0].Y));
        polygon.Points.Add(new System.Windows.Point(C[1].X, C[1].Y));
        polygon.Points.Add(new System.Windows.Point(C[0].X, C[1].Y));


        canvas.Children.Add(polygon);
    }

    public void Circle(List<System.Windows.Point> Points, Color color, uint thickness)
    {
        var C = Points.ToArray();
        Ellipse myEllipse = new Ellipse();

        myEllipse.Stroke = new SolidColorBrush(Color.FromRgb(color.R, color.G, color.B));
        myEllipse.StrokeThickness = thickness;
        double radius = (C[1].X - C[0].X);
        double radius2 = (C[1].Y - C[0].Y);
        myEllipse.Width = 2 * Math.Abs(radius);
        myEllipse.Height = 2 * Math.Abs(radius2);

        canvas.Children.Add(myEllipse);
        double Left, Top;
        if (radius > 0) Left = C[0].X - radius;
        else Left = C[1].X;
        if (radius2 > 0) Top = C[0].Y - radius2;
        else Top = C[1].Y;
        Canvas.SetLeft(myEllipse, Left);
        Canvas.SetTop(myEllipse, Top);
    }
}
