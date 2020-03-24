using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using ReactiveUI;

#nullable enable
namespace Wpffirst
{
    public interface IGraphic
    {
        double PixelSize { get; }
        void PolyLine(IEnumerable<Point> points, Color color);
        void Polygon(IEnumerable<Point> points, Color color, IBrush fill);
        void Circle(Point center, double rad, Color color,IBrush fill);
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


    public interface IFigure:ICloneable
    {
        IEnumerable<Point> Vertex { get; set; }

        ReadOnlySpan<string> ParameterNames { get; }
        object this[string name] { get;set; }


        void Draw(IGraphic graphic);
        void Rotate(double angle);
        void Move(Point vector);
        void Scale(double scale);
        IBrush Fill { get;set;}
        IPen Boundary { get; set; }
        bool Contain(Point p);
        (Point Min,Point Max) Gabarits { get; }
        double Z { get; set; }

        IFigure Intersect(IFigure figure);
        IFigure Union(IFigure figure);
        IFigure Subtract(IFigure figure);

    }
    public interface IFigureDescriptor
    {
        string Name { get; }
        int PointNumber { get; }
        IFigure Create(IEnumerable<Point> vertex);
    }
    public static class FigureFabric
    {
        static IFigureDescriptor[] figures = new IFigureDescriptor[] {};
        public static IEnumerable<IFigureDescriptor> AvailableFigures=>figures;
        public static IFigure? Create(string name,IEnumerable<Point> points) { return null; }       
    }

    interface IViewModel:IReactiveObject
    {
        ReactiveCommand<IFigure, Unit> AddFigure { get; }
        ReactiveCommand<IFigure, Unit> RemoveFigure { get; }
        ReactiveCommand<Point, IFigure?> Select { get; }

    }





}
