using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

namespace MiniEditor
{
    public interface IGraphic
    {
        void Polygon(IEnumerable<Point> points, Color color);
    }
    public struct Point
    {
        public double X;
        public double Y;
    }
    public struct Color
    {
        public byte R, G, B, A;
    }

    public interface IFigure
    {
        string Name { get; }
        IEnumerable<string> Parameters { get; }
        object this[string name] { get;set; }
        void Draw(IGraphic graphic);
    }
    public interface IFigureDescriptor
    {
        string Name { get;  }
        int NumberOfPoints { get; }
        IFigure Create(IEnumerable<Point> vertex);
    }
    public class FigureDescriptorMetadata
    {
        public string Name { get; set; }
    }


    public class Line:IFigure
    {
        public Line (Point a,Point b)
        {
            A = a;B = b;
        }
        Point A;
        Point B;
        public IEnumerable<string> Parameters { get; } = new[] { "A", "B" };

        public object this[string name] 
        { 
            get 
            {
                switch(name)
                {
                    case "A":return A;
                    case "B":return B;
                    default:throw new ArgumentOutOfRangeException($"Unknown parameter {name}");
                }

            } 
            set 
            {
                if(value is Point p)
                {
                    switch (name)
                    {
                        case "A": A=p;break;
                        case "B": B=p;break;
                        default: throw new ArgumentOutOfRangeException($"Unknown parameter {name}");
                    }
                }
                else throw new ArgumentOutOfRangeException($"Unknown parameter type {value.GetType().Name}");
            }
        }

        public void Draw(IGraphic graphic)
        {
            graphic.Polygon(new[] { A, B }, new Color { R = 255, G = 0, B = 0, A = 128 });
        }

        public string Name => "Line";
    }
    [Export(typeof(IFigureDescriptor))]
    [ExportMetadata("Name", "Line")]
    public class LineDescriptor : IFigureDescriptor
    {
        public string Name => "Line";
        public int NumberOfPoints => 2;

        public IFigure Create(IEnumerable<Point> vertex)
        {
            var points = vertex.ToArray();
            if (points.Length != 2) throw new ArgumentOutOfRangeException($"Bad number of parameters {points.Length}");
            return new Line(points[0], points[1]);
        }
    }
}
