using System;
using MiniEditor;
using System.Collections.Generic;
using System.Text;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

public interface IGraphic
    {
        void Line(Point start, Point end, System.Windows.Media.SolidColorBrush brush);
        void Ellipse(Point center, Point rad, double ry, System.Windows.Media.Color color, IBrush fill);
        void Polygon(IEnumerable<Point> points, System.Windows.Media.Color color, IBrush fill);
        
    }
    public struct Point
    {
        public double X;
        public double Y;
    }
    public interface IBrush
    {
        Color MainColor { get; set; }
    }
    public interface IPen
    {
        Color MainColor { get; set; }
    }

    public class BuildFigure :IGraphic {

        public IEnumerable<Point> points;
        public System.Windows.Media.SolidColorBrush Brush;
        public Point Start, End; 
        

        public void Line(Point start, Point end, System.Windows.Media.SolidColorBrush brush) {
        System.Windows.Shapes.Line redLine = new System.Windows.Shapes.Line();
        this.Start = start;
        this.Start = end;
        this.Brush = brush;
    }

        public void Ellipse(Point center, Point rad, double ry, System.Windows.Media.Color color, IBrush fill)
        {
            //Ellipse el = new Ellipse();
            //Point point2 = rad;
            //el.Width = 2 * Math.Abs(point1.X - point2.X);
            //el.Height = 2 * Math.Abs(point1.Y - point2.Y);
            //el.Margin = new Thickness(point1.X / 2, point1.Y / 2, 0, 0);
            //el.Fill = System.Windows.Media.Brushes.Green;
            //el.Stroke = System.Windows.Media.Brushes.Red;
            //el.StrokeThickness = 3;
        }

        public void Polygon(IEnumerable<Point> points, System.Windows.Media.Color color, IBrush fill) { 
        }
 
    }
