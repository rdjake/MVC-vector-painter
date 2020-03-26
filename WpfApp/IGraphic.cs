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
using Rectangle = System.Windows.Shapes.Rectangle;
public interface IGraphic
{
    void Polyline(List<System.Windows.Point> points, MiniEditor.Brush brush);
    //void Polygon(List<System.Windows.Point> points, MiniEditor.Brush brush);
    void Rectangle(List<System.Windows.Point> points, MiniEditor.Brush brush);
    void Circle(List<System.Windows.Point> points, MiniEditor.Brush brush);
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


    public void Polyline(List<System.Windows.Point> points, MiniEditor.Brush brush)
    {
        Polyline polyline = new Polyline();

        polyline.Stroke = new SolidColorBrush(Color.FromRgb(brush.Line.R, brush.Line.G, brush.Line.B));
        polyline.StrokeThickness = brush.Thickness;

        foreach (var point in points)
        {
            polyline.Points.Add(new System.Windows.Point(point.X, point.Y));
        }

        canvas.Children.Add(polyline);
    }
    //public void Polygon(List<System.Windows.Point> points, MiniEditor.Brush brush)
    //{
    //    Polygon polygon = new Polygon();
    //    polygon.Stroke = new SolidColorBrush(Color.FromRgb(brush.Line.R, brush.Line.G, brush.Line.B));
    //    polygon.StrokeThickness = brush.Thickness;

    //    foreach (var point in points)
    //    {
    //        polygon.Points.Add(new System.Windows.Point(point.X, point.Y));
    //    }

    //    canvas.Children.Add(polygon);
    //}

    public void Rectangle(List<System.Windows.Point> points, MiniEditor.Brush brush)
    {
        // Polygon rectangle = new Polygon();
        Rectangle rectangle = new Rectangle();
        rectangle.Stroke = new SolidColorBrush(Color.FromRgb(brush.Line.R, brush.Line.G, brush.Line.B));
        rectangle.Fill = new SolidColorBrush(Color.FromRgb(brush.Fill.R, brush.Fill.G, brush.Fill.B));
        rectangle.StrokeThickness = brush.Thickness;

        var C = points.ToArray();         
        //rectangle.Points.Add(new System.Windows.Point(C[0].X, C[0].Y));
        //rectangle.Points.Add(new System.Windows.Point(C[1].X, C[0].Y));
        //rectangle.Points.Add(new System.Windows.Point(C[1].X, C[1].Y));
        //rectangle.Points.Add(new System.Windows.Point(C[0].X, C[1].Y));
 
        double radius = (C[1].X - C[0].X);
        double radius2 = (C[1].Y - C[0].Y);
        rectangle.Width = Math.Abs(radius);
        rectangle.Height = Math.Abs(radius2);

        canvas.Children.Add(rectangle);
        double Left, Top;
        if (radius > 0) Left = C[0].X;
        else Left = C[1].X;
        if (radius2 > 0) Top = C[0].Y;
        else Top = C[1].Y;
        Canvas.SetLeft(rectangle, Left);
        Canvas.SetTop(rectangle, Top);

    }
    

    public void Circle(List<System.Windows.Point> Points, MiniEditor.Brush brush)
    {
        var C = Points.ToArray();
        Ellipse myEllipse = new Ellipse();

        myEllipse.Stroke = new SolidColorBrush(Color.FromRgb(brush.Line.R, brush.Line.G, brush.Line.B));
        myEllipse.Fill = new SolidColorBrush(Color.FromRgb(brush.Fill.R, brush.Fill.G, brush.Fill.B));
        myEllipse.StrokeThickness = brush.Thickness;
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
