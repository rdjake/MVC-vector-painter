using System;
using MiniEditor;
using System.Collections.Generic;
using System.Text;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;

public interface IGraphic
    {
        System.Windows.Shapes.Line Line(Point start, Point end, System.Windows.Media.SolidColorBrush brush);

        System.Windows.Shapes.Ellipse Ellipse(Point center, Point rad, double ry, System.Windows.Media.Color color, System.Windows.Media.SolidColorBrush brush);

        System.Windows.Shapes.Polygon Polygon(IEnumerable<Point> points, System.Windows.Media.Color color, System.Windows.Media.SolidColorBrush brush);
        
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
        
        public System.Windows.Shapes.Line Line(Point start, Point end, System.Windows.Media.SolidColorBrush brush) {
            System.Windows.Shapes.Line redLine = new System.Windows.Shapes.Line();
            this.Start = start;
            this.Start = end;
            this.Brush = brush;
            redLine.X1 = start.X;
            redLine.Y1 = start.Y;
            redLine.X2 = end.X;
            redLine.Y2 = end.Y;
            redLine.StrokeThickness = 4;
            redLine.Stroke = brush;
            return redLine;
        }

        public System.Windows.Shapes.Ellipse Ellipse(Point center, Point rad, double ry, System.Windows.Media.Color color, System.Windows.Media.SolidColorBrush brush)
        {
            Ellipse el = new Ellipse();
            Point point2 = rad;
            Point point1 = center;
            el.Width = 2 * Math.Abs(point1.X - point2.X);
            el.Height = 2 * Math.Abs(point1.Y - point2.Y);
            el.Margin = new Thickness(point1.X / 2, point1.Y / 2, 0, 0);
            el.Fill = System.Windows.Media.Brushes.Green;
            el.Stroke = System.Windows.Media.Brushes.Red;
            el.StrokeThickness = 3;
            return el;
        }

        public System.Windows.Shapes.Polygon Polygon(IEnumerable<Point> points, System.Windows.Media.Color color, System.Windows.Media.SolidColorBrush brush) { 
        return null;
        }
 
    }
