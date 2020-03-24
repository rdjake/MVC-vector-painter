using System;
using System.Collections.Generic;
using System.Text;

namespace WpfApp
{

    public interface IGraphic
    {
        double PixelSize { get; }
        void PolyLine(IEnumerable<Point> points, Color color);
        void Polygon(IEnumerable<Point> points, Color color, IBrush fill);
        void Circle(Point center, double rad, Color color, IBrush fill);
        void Ellipse(Point center, Point rad, double ry, Color color, IBrush fill);
    }
    public struct Point
    {
        public double X;
        public double Y;
    }
    public struct Color
    {
        byte R, G, B, A;
    }
    public interface IBrush
    {
        Color MainColor { get; }
    }
    public interface IPen
    {
        Color MainColor { get; }
    }
}
