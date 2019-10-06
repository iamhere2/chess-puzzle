using System;

namespace ChessPuzzle
{
    struct FigureCell : IEquatable<FigureCell>
    {
        public Point RelativePoint { get; private set; }

        public Color Color { get; private set; }

        public FigureCell(Point relativePoint, Color color)
        {
            RelativePoint = relativePoint;
            Color = color;
        }

        public FigureCell Shift(Point p) => new FigureCell(RelativePoint.Shift(p), Color);

        public bool Equals(FigureCell other) => RelativePoint == other.RelativePoint && Color == other.Color;

        public static bool operator ==(FigureCell a, FigureCell b) => a.Equals(b);

        public static bool operator !=(FigureCell a, FigureCell b) => !a.Equals(b);

        public override bool Equals(object? obj) => obj is FigureCell fc && fc.Equals(this);

        public override int GetHashCode() => HashCode.Combine(RelativePoint, Color);
    }
}