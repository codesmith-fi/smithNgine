using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codesmith.SmithNgine.Primitives
{
    /// <summary>
    /// A container class for a simple two dimensional point
    /// with X and Y
    /// Defines some basic operations 
    /// </summary>
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
        public Point() : this(0, 0) { }

        public Point(Point other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            X = other.X;
            Y = other.Y;
        }

        public void Set(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void Set(Point other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            X = other.X;
            Y = other.Y;
        }

        public void Add(Point other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            X += other.X;
            Y += other.Y;
        }

        public void Add(int deltaX, int deltaY)
        {
            X += deltaX;
            Y += deltaY;
        }

        public void Clear()
        {
            X = 0;
            Y = 0;
        }

        public void Clear(int value)
        {
            X = value;
            Y = value;
        }

        public Point Center()
        {
            return new Point(X / 2, Y / 2);
        }

        public Point Offset(int offset)
        {
            return new Point(X + offset, Y + offset);
        }       

        public Point Offset(int offsetX, int offsetY)
        {
            return new Point(X + offsetX, Y + offsetY);
        }

        public Point Offset(Point offset)
        {
            if (offset == null) throw new ArgumentNullException(nameof(offset));
            return new Point(X + offset.X, Y + offset.Y);
        }   

        public Point Clone()
        {
            return new Point(X, Y);
        }

        public static Point operator +(Point a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        }

        public static Point operator -(Point a, Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y);
        }

        public static Point operator *(Point a, int scalar)
        {
            return new Point(a.X * scalar, a.Y * scalar);
        }   

        public static Point operator /(Point a, int scalar)
        {
            if (scalar == 0) throw new DivideByZeroException("Cannot divide by zero.");
            return new Point(a.X / scalar, a.Y / scalar);
        }

        public static bool operator ==(Point a, Point b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Point a, Point b)
        {
            return !(a == b);
        }   

        public override bool Equals(object obj)
        {
            if (obj is Point other)
            {
                return this == other;
            }
            return false;
        }

        public bool IsValid()
        {
            return X >= 0 && Y >= 0;
        }

        public bool IsInside(Point other)
        {
            return IsValid() && X < other.X && Y < other.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }   

        public override string ToString()
        {
            return $"Position2D(X: {X}, Y: {Y})";
        }
    }
}