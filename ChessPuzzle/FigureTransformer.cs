using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessPuzzle
{
    class FigureTransformer
    {
        public static IList<Figure> GetTransformations(Figure figure)
        {
            Ensure.Arg(figure, nameof(figure)).IsNotNull();

            return
                GetRotates(figure)
                .Concat(GetRotates(FlipX(figure)))
                .Distinct(FigureLinearTransitionComparer.Instance)
                .ToList();
        }

        public static IEnumerable<Figure> GetRotates(Figure figure)
        {
            Ensure.Arg(figure, nameof(figure)).IsNotNull();

            var rotate1 = RotateRight(figure);
            var rotate2 = RotateRight(rotate1);
            var rotate3 = RotateRight(rotate2);

            return new[] { figure, rotate1, rotate2, rotate3 };
        }

        public static Figure RotateRight(Figure figure)
        {
            Ensure.Arg(figure, nameof(figure)).IsNotNull();

            return Transform(figure, RotateRightPoint);
        }

        public static Figure Transform(Figure figure, Func<Point, Point> pointTransform)
        {
            Ensure.Arg(figure, nameof(figure)).IsNotNull();
            Ensure.Arg(pointTransform, nameof(pointTransform)).IsNotNull();

            return new Figure(
                figure.ColorOfOriginCell,
                figure.Cells.Skip(1).Select(c => pointTransform(c.RelativePoint)));
        }

        private static Point RotateRightPoint(Point p) => Point.Of(p.Y, -p.X);

        private static Figure FlipX(Figure f) => Transform(f, FlipXPoint);

        private static Point FlipXPoint(Point p) => Point.Of(-p.X, p.Y);

        class FigureLinearTransitionComparer : IEqualityComparer<Figure>
        {
            public static readonly FigureLinearTransitionComparer Instance = new FigureLinearTransitionComparer();

            public bool Equals(Figure? a, Figure? b)
                => (a is null) && (b is null) 
                || (a != null && b != null &&
                    GetNormalizedCells(a).SequenceEqual(GetNormalizedCells(b)));

            public int GetHashCode(Figure f)
            {
                var normCells = GetNormalizedCells(f).ToList();
                var hash = new HashCode();

                hash.Add(normCells.Count);

                foreach (var c in normCells)
                    hash.Add(c.GetHashCode());

                return hash.ToHashCode();
            }

            private static IEnumerable<FigureCell> GetNormalizedCells(Figure f)
            {
                var minP = f.Cells.Select(c => c.RelativePoint).Min();
                return
                    f.Cells.Select(c => c.Shift(-minP))
                    .OrderBy(c => c, FigureCellComparer.Instance);
            }

            class FigureCellComparer : IComparer<FigureCell>
            {
                public static readonly FigureCellComparer Instance = new FigureCellComparer();

                public int Compare(FigureCell cellA, FigureCell cellB)
                {
                    Ensure.Arg(cellA, nameof(cellA)).IsNotNull();
                    Ensure.Arg(cellB, nameof(cellB)).IsNotNull();

                    int colorDiff = ((int)cellA.Color).CompareTo((int)cellB.Color);

                    return colorDiff == 0
                        ? cellA.RelativePoint.CompareTo(cellB.RelativePoint)
                        : colorDiff;
                }
            }
        }
    }
}
