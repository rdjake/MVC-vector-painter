﻿using System;
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



    public class Rectangle : IFigure
    {
        public Rectangle(Point a, Point c)
        {
            A = a; C = c;
        }
        Point A;
        Point C;
        Point B = new Point { X = A.X, Y = C.Y };
        Point D = new Point { X = C.X, Y = A.Y };
        public IEnumerable<string> Parameters { get; } = new[] { "A", "B","C","D" };

        public object this[string name]
        {
            get
            {
                switch (name)
                {
                    case "A": return A;
                    case "B": return B;
                    case "C": return C;
                    case "D": return D;
                    default: throw new ArgumentOutOfRangeException($"Unknown parameter {name}");
                }

            }
            set
            {
                if (value is Point p)
                {
                    switch (name)
                    {
                        case "A": A = p; break;
                        case "B": B = p; break;
                        case "C": C = p; break;
                        case "D": D = p; break;
                        default: throw new ArgumentOutOfRangeException($"Unknown parameter {name}");
                    }
                }
                else throw new ArgumentOutOfRangeException($"Unknown parameter type {value.GetType().Name}");
            }
        }

        public void Draw(IGraphic graphic)
        {
            graphic.Polygon(new[] { A, B,C,D }, new Color { R = 255, G = 0, B = 0, A = 128 });           
        }

        public string Name => "Rectangle";
    }
    [Export(typeof(IFigureDescriptor))]
    [ExportMetadata("Name", "Rectangle")]
    public class RectangleDescriptor : IFigureDescriptor
    {
        public string Name => "Rectangle";
        public int NumberOfPoints => 4;
        public IFigure Create(IEnumerable<Point> vertex)
        {
            var points = vertex.ToArray();
            if (points.Length != 2) throw new ArgumentOutOfRangeException($"Bad number of parameters {points.Length}");
            return new Rectangle(points[0], points[1]);
        }
    }



    public class Circle : IFigure
    {
        public Circle(Point c, double r)
        {
           C = c; R = r;
        }
      
        Point C;
        double R;
        public IEnumerable<string> Parameters { get; } = new[] {"C"};

        public object this[string name]
        {
            get
            {
                switch (name)
                {                  
                    case "C": return c;                  
                    default: throw new ArgumentOutOfRangeException($"Unknown parameter {name}");
                }

            }
            set
            {
                if (value is Point p)
                {
                    switch (name)
                    {                      
                        case "C": C = p; break;                    
                        default: throw new ArgumentOutOfRangeException($"Unknown parameter {name}");
                    }
                }
                else throw new ArgumentOutOfRangeException($"Unknown parameter type {value.GetType().Name}");
            }
        }

        public void Draw(IGraphic graphic)
        {
            graphic.Circle(C, R,new Color { R = 255, G = 0, B = 0, A = 128 });
        }

        public string Name => "Circle";
    }
    [Export(typeof(IFigureDescriptor))]
    [ExportMetadata("Name", "Circle")]
    public class CircleDescriptor : IFigureDescriptor
    {
        public string Name => "Circle";
        public int NumberOfPoints => 1; 
        public IFigure Create(IEnumerable<Point> vertex)
        {
            var points = vertex.ToArray();
            if (points.Length != 1) throw new ArgumentOutOfRangeException($"Bad number of parameters {points.Length}");
            return new Circle(points[0]);
        }
    }







}
