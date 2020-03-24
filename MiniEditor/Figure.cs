using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

namespace MiniEditor
{
    public interface IGraphic
    {
        void Polygon(IEnumerable<Point> points, Color color);
        void Ellipse(IEnumerable<Point> points);
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
        object this[string name] { get; set; }
        void Draw(IGraphic graphic);
    }
    public interface IFigureDescriptor
    {
        string Name { get; }
        int NumberOfPoints { get; }
        IFigure Create(IEnumerable<Point> vertex);
    }
    public class FigureDescriptorMetadata
    {
        public string Name { get; set; }
    }


    public class Line : IFigure
    {
        public Line(Point a, Point b)
        {
            A = a; B = b;
        }
        Point A;
        Point B;
        public IEnumerable<string> Parameters { get; } = new[] { "A", "B" };

        public object this[string name]
        {
            get
            {
                switch (name)
                {
                    case "A": return A;
                    case "B": return B;
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
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            if (angle > 0)
            {
                A.X = A.X * cos - A.Y * sin;
                A.Y = A.X * sin + A.Y * cos;
                B.X = B.X * cos - B.Y * sin;
                B.Y = B.X * sin + B.Y * cos;
            }
            else
            {
                A.X = A.X * cos + A.Y * sin;
                A.Y = -A.X * sin + A.Y * cos;
                B.X = B.X * cos + B.Y * sin;
                B.Y = -B.X * sin + B.Y * cos;
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
            B = new Point { X = A.X, Y = C.Y };
            D = new Point { X = C.X, Y = A.Y };
        }
        Point A;
        Point C;
        Point B;
        Point D;
        public IEnumerable<string> Parameters { get; } = new[] { "A", "C" };

        public object this[string name]
        {
            get
            {
                switch (name)
                {
                    case "A": return A;
                    case "C": return C;
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
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            if (angle > 0)
            {
                A.X = A.X * cos - A.Y * sin;
                A.Y = A.X * sin + A.Y * cos;
                B.X = B.X * cos - B.Y * sin;
                B.Y = B.X * sin + B.Y * cos;
                C.X = C.X * cos - C.Y * sin;
                C.Y = C.X * sin + C.Y * cos;
                D.X = D.X * cos - D.Y * sin;
                D.Y = D.X * sin + D.Y * cos;
            }
            else
            {
                A.X = A.X * cos + A.Y * sin;
                A.Y = -A.X * sin + A.Y * cos;
                B.X = B.X * cos + B.Y * sin;
                B.Y = -B.X * sin + B.Y * cos;
                C.X = C.X * cos + C.Y * sin;
                C.Y = -C.X * sin + C.Y * cos;
                D.X = D.X * cos + D.Y * sin;
                D.Y = -D.X * sin + D.Y * cos;
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
        public IFigure Create(IEnumerable<Point> vertex)
        {
            var points = vertex.ToArray();
            if (points.Length != 2) throw new ArgumentOutOfRangeException($"Bad number of parameters {points.Length}");
            return new Rectangle(points[0], points[1]);
        }
    }



    public class Ellipse : IFigure
    {
        public Ellipse(Point p1, Point p2)
        {
            if (p1.X < p2.X && p1.Y > p2.Y)
            {
                P1 = p1; P2 = p2;
            }
            else
            {
                P1 = p2; P2 = p1;
            }
            height = P1.Y - P2.Y;
            width = P2.X - P1.X;
            RX = width / 2.0;
            RY = height / 2.0; //      
            C.X = P1.X + RX;  //    
            C.Y = P2.Y + RY;
        }

        Point P1, P2, C;
        double height, width, RX, RY;

        public IEnumerable<string> Parameters { get; } = new[] { "C", "RX", "RY" };

        public object this[string name]
        {
            get
            {
                return name switch
                {
                    "C" => C,
                    "RX" => RX,
                    "RY" => RX,
                    _ => throw new ArgumentOutOfRangeException($"Unknown parameter {name}"),
                };
            }
            set
            {
                if (value is Point p)
                {
                    C = p;
                }
                else if (value is double r)
                    switch (name)
                    {
                        case "RX": RX = r; break;
                        case "RY": RY = r; break;
                        default: throw new ArgumentOutOfRangeException($"Unknown parameter {name}");
                    }
                else
                    throw new ArgumentOutOfRangeException($"Unknown parameter type {value.GetType().Name}");
            }
        }

        public void Draw(IGraphic graphic)
        {
            graphic.Ellipse(new[] { P1, P2 });
        }
        //public void Move(Point vector)
        //{
        //    C.X += vector.X;
        //    C.Y += vector.Y;
        //}
        //public bool Contain(Point p)
        //{
        //    return R > Math.Sqrt((p.X - C.X) * (p.X - C.X) + (p.Y - C.Y) * (p.Y - C.Y));
        //}
        public void Scale(double scale)
        {
            RX *= scale;
            RY *= scale;
        }
        public string Name => "Ellipse";
    }
    [Export(typeof(IFigureDescriptor))]
    [ExportMetadata("Name", "Ellipse")]
    public class CircleDescriptor : IFigureDescriptor
    {
        public string Name => "Ellipse";
        public int NumberOfPoints => 2;
        public IFigure Create(IEnumerable<Point> vertex)
        {
            var points = vertex.ToArray();
            if (points.Length != 2) throw new ArgumentOutOfRangeException($"Bad number of parameters {points.Length}");
            return new Ellipse(points[0], points[1]);
        }
    }
}
