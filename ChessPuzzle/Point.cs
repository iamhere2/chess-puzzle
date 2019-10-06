using System;
using System.Collections.Generic;

namespace ChessPuzzle
{
    struct Point : IEquatable<Point>, IComparable<Point>
    {
        public short X { get; private set; }

        public short Y { get; private set; }

        public Point(short x, short y) : this()
        {
            X = x;
            Y = y;
        }

        public Point(int x, int y) : this((short)x, (short)y)
        {
        }

        public static Point Origin = Of(0, 0);

        public static Point Of(int x, int y) => new Point(x, y);

        public Point Shift(Point p) => new Point(X + p.X, Y + p.Y);

        public bool Equals(Point other) => X == other.X && Y == other.Y;

        public static Point operator -(Point p) => new Point(-p.X, -p.Y);

        public static bool operator ==(Point a, Point b) => a.Equals(b);

        public static bool operator !=(Point a, Point b) => !a.Equals(b);

        public override bool Equals(object? obj) => obj is Point p && p.Equals(this);

        public int CompareTo(Point other) => GetHashCode().CompareTo(other.GetHashCode());

        public override int GetHashCode() => (Y << 16) ^ X;

        public bool IsOdd => ((X + Y) & 1) == 1;

        public class EqualityComparer : IEqualityComparer<Point>
        {
            public bool Equals(Point a, Point b) => a.Equals(b);

            public int GetHashCode(Point p) => p.GetHashCode();

            public static IEqualityComparer<Point> Instance = new EqualityComparer();
        }
    }
}