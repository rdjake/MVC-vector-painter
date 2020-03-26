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
    public struct Brush
    {
        public Color Fill;
        public Color Line;
        public double Thickness;
    }
    public interface IFigure
    {
        string Name { get; }

        IEnumerable<string> Parameters { get; }
        object this[string name] { get; set; }
        void Draw(IGraphic graphic);
    }
    public interface IFigureDescriptor
    {
        string Name { get; }
        int NumberOfPoints { get; }
        IFigure Create(IEnumerable<Point> vertex, Brush brush);
    }
    public class FigureDescriptorMetadata
    {
        public string Name { get; set; }
    }


    public class Line : IFigure
    {
        public Line(Point a, Point b, Brush br)
        {
            A = a;
            B = b;
            brush = br;
        }
        Point A;
        Point B;
        Brush brush;
        public IEnumerable<string> Parameters { get; } = new[] { "A", "B", "brush"};

        public object this[string name]
        {
            get
            {
                switch (name)
                {
                    case "A": return A;
                    case "B": return B;
                    case "brush": return brush;
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
                        default: throw new ArgumentOutOfRangeException($"Unknown parameter {name}");
                    }
                }
                if (value is Brush br) 
                {
                    switch (name)
                    {
                        case "brush": brush = br; break;
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
        public void Move(Point vector)
        {
            A.X += vector.X;
            A.Y += vector.Y;
            B.X += vector.X;
            B.Y += vector.Y;
        }

        public void Rotate(double angle)
        {
            if (angle > 0)
            {
                A.X = A.X * Math.Cos(angle) - A.Y * Math.Sin(angle);
                A.Y = A.X * Math.Sin(angle) + A.Y * Math.Cos(angle);
                B.X = B.X * Math.Cos(angle) - B.Y * Math.Sin(angle);
                B.Y = B.X * Math.Sin(angle) + B.Y * Math.Cos(angle);              
            }
            else
            {
                A.X = A.X * Math.Cos(angle) + A.Y * Math.Sin(angle);
                A.Y = -A.X * Math.Sin(angle) + A.Y * Math.Cos(angle);
                B.X = B.X * Math.Cos(angle) + B.Y * Math.Sin(angle);
                B.Y = -B.X * Math.Sin(angle) + B.Y * Math.Cos(angle);
            }
        }
        public string Name => "Line";
    }
    [Export(typeof(IFigureDescriptor))]
    [ExportMetadata("Name", "Line")]
    public class LineDescriptor : IFigureDescriptor
    {
        public string Name => "Line";
        public int NumberOfPoints => 2;

        public IFigure Create(IEnumerable<Point> vertex, Brush brush)
        {
            var points = vertex.ToArray();
            if (points.Length != 2) throw new ArgumentOutOfRangeException($"Bad number of parameters {points.Length}");
            return new Line(points[0], points[1], brush);
        }
    }



    public class Rectangle : IFigure
    {
        public Rectangle(Point a, Point c, Brush br)
        {
            A = a; C = c;
            B = new Point { X = A.X, Y = C.Y };
            D = new Point { X = C.X, Y = A.Y };
            brush = br;
        }
        Point A;
        Point C;
        Point B;
        Point D;
        Brush brush;
        public IEnumerable<string> Parameters { get; } = new[] { "A", "C", "brush" };

        public object this[string name]
        {
            get
            {
                switch (name)
                {
                    case "A": return A;
                    case "C": return C;
                    case "brush": return brush;
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
                        case "C": C = p; break;
                        default: throw new ArgumentOutOfRangeException($"Unknown parameter {name}");
                    }
                }
                if (value is Brush br)
                {
                    switch (name)
                    {
                        case "brush": brush = br; break;
                        default: throw new ArgumentOutOfRangeException($"Unknown parameter {name}");
                    }
                }
                else throw new ArgumentOutOfRangeException($"Unknown parameter type {value.GetType().Name}");
            }
        }

        public void Draw(IGraphic graphic)
        {

            graphic.Polygon(new[] { A, B, C, D }, new Color { R = 255, G = 0, B = 0, A = 128 });
        }
        public void Move(Point vector)
        {
            A.X += vector.X;
            A.Y += vector.Y;
            B.X += vector.X;
            B.Y += vector.Y;
            C.X += vector.X;
            C.Y += vector.Y;
            D.X += vector.X;
            D.Y += vector.Y;

        }
        public void Rotate(double angle)
        {
            if(angle>0)
            {
                A.X = A.X * Math.Cos(angle) - A.Y * Math.Sin(angle);
                A.Y = A.X * Math.Sin(angle) + A.Y * Math.Cos(angle);
                B.X = B.X * Math.Cos(angle) - B.Y * Math.Sin(angle);
                B.Y = B.X * Math.Sin(angle) + B.Y * Math.Cos(angle);
                C.X = C.X * Math.Cos(angle) - C.Y * Math.Sin(angle);
                C.Y = C.X * Math.Sin(angle) + C.Y * Math.Cos(angle);
                D.X = D.X * Math.Cos(angle) - D.Y * Math.Sin(angle);
                D.Y = D.X * Math.Sin(angle) + D.Y * Math.Cos(angle);
            }
            else
            {
                A.X = A.X * Math.Cos(angle) + A.Y * Math.Sin(angle);
                A.Y = -A.X * Math.Sin(angle) + A.Y * Math.Cos(angle);
                B.X = B.X * Math.Cos(angle) + B.Y * Math.Sin(angle);
                B.Y = -B.X * Math.Sin(angle) + B.Y * Math.Cos(angle);
                C.X = C.X * Math.Cos(angle) + C.Y * Math.Sin(angle);
                C.Y = -C.X * Math.Sin(angle) + C.Y * Math.Cos(angle);
                D.X = D.X * Math.Cos(angle) + D.Y * Math.Sin(angle);
                D.Y = -D.X * Math.Sin(angle) + D.Y * Math.Cos(angle);
            }
        }
        

        public string Name => "Rectangle";
    }
    [Export(typeof(IFigureDescriptor))]
    [ExportMetadata("Name", "Rectangle")]
    public class RectangleDescriptor : IFigureDescriptor
    {
        public string Name => "Rectangle";
        public int NumberOfPoints => 2;
        public IFigure Create(IEnumerable<Point> vertex, Brush brush)
        {
            var points = vertex.ToArray();
            if (points.Length != 2) throw new ArgumentOutOfRangeException($"Bad number of parameters {points.Length}");
            return new Rectangle(points[0], points[1], brush);
        }
    }



    public class Circle : IFigure
    {
        public Circle(Point c1, Point c2, Brush br)
        {
            C = c1; 
            C2 = c2;
            R = Math.Sqrt((c2.X - c1.X)* (c2.X - c1.X) + (c2.Y - c1.Y) * (c2.Y - c1.Y));
            brush = br;
        }

        Point C;
        Point C2;
        double R;
        Brush brush;
        public IEnumerable<string> Parameters { get; } = new[] { "C", "C2", "brush" };

        public object this[string name]
        {
            get
            {
                switch (name)
                {
                    case "C": return C;
                    case "C2": return C2;
                    case "brush": return brush;
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
                        case "C2": C2 = p; break;
                        default: throw new ArgumentOutOfRangeException($"Unknown parameter {name}");
                    }
                }
                if (value is Brush br)
                {
                    switch (name)
                    {
                        case "brush": brush = br; break;
                        default: throw new ArgumentOutOfRangeException($"Unknown parameter {name}");
                    }
                }
                else throw new ArgumentOutOfRangeException($"Unknown parameter type {value.GetType().Name}");
            }
        }

        public void Draw(IGraphic graphic)
        {
           //graphic.Circle(C, R, new Color { R = 255, G = 0, B = 0, A = 128 });
        }
        public void Move(Point vector)
        {
            C.X += vector.X;
            C.Y += vector.Y;
        }
        public bool Contain(Point p)
        {
            return R > Math.Sqrt((p.X - C.X) * (p.X - C.X) + (p.Y - C.Y) * (p.Y - C.Y));
        }
        public void Scale(double scale)
        {
            R *= scale;
        }
        public string Name => "Circle";
    }
    [Export(typeof(IFigureDescriptor))]
    [ExportMetadata("Name", "Circle")]
    public class CircleDescriptor : IFigureDescriptor
    {
        public string Name => "Circle";
        public int NumberOfPoints => 2;
        public IFigure Create(IEnumerable<Point> vertex, Brush brush)
        {
            var points = vertex.ToArray();
            if (points.Length != 2) throw new ArgumentOutOfRangeException($"Bad number of parameters {points.Length}");
            return new Circle(points[0], points[1], brush);
        }
    }
}
