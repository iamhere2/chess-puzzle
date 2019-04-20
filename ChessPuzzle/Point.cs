﻿using System;
using System.Collections.Generic;

namespace ChessPuzzle
{
    struct Point : IEquatable<Point>
    {
        public int X { get; private set; }

        public int Y { get; private set; }

        public Point(int x, int y) : this()
        {
            X = x;
            Y = y;
        }

        public static Point Origin = Of(0,0);

        public static Point Of(int x, int y) => new Point(x, y);

        public bool Equals(Point other) => X == other.X && Y == other.Y;

        public override int GetHashCode() => (X << 16) ^ Y;

        public class EqualityComparer : IEqualityComparer<Point>
        {
            public bool Equals(Point a, Point b) => a.Equals(b);

            public int GetHashCode(Point p) => p.GetHashCode();

            public static IEqualityComparer<Point> Instance = new EqualityComparer();
        }

        public bool IsOdd => ((X + Y) & 1) == 1;
    }
}