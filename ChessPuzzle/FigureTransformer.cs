using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace ChessPuzzle
{
    class FigureTransformer
    {
        public static IList<Figure> GetTransformations(Figure figure)
        {
            Contract.Requires(figure != null);

            return
                GetRotates(figure)
                .Concat(GetRotates(FlipX(figure)))
                .Distinct(FigureLinearTransitionComparer.Instance)
                .ToList();
        }

        public static IEnumerable<Figure> GetRotates(Figure figure)
        {
            Contract.Requires(figure != null);

            var rotate1 = RotateRight(figure);
            var rotate2 = RotateRight(rotate1);
            var rotate3 = RotateRight(rotate2);

            return new[] { figure, rotate1, rotate2, rotate3 };
        }

        public static Figure RotateRight(Figure figure)
        {
            Contract.Requires(figure != null);

            return Transform(figure, RotateRightPoint);
        }

        public static Figure Transform(Figure figure, Func<Point, Point> pointTransform)
        {
            Contract.Requires(figure != null);
            Contract.Requires(pointTransform != null);

            return new Figure(
                figure.ColorOfOriginCell,
                figure.Cells.Skip(1).Select(c => pointTransform(c.RelativePoint)));
        }

        private static Point RotateRightPoint(Point p)
        {
            return Point.Of(p.Y, -p.X);
        }

        private static Figure FlipX(Figure f)
        {
            return Transform(f, FlipXPoint);
        }

        private static Point FlipXPoint(Point p)
        {
            return Point.Of(-p.X, p.Y);
        }

        class FigureLinearTransitionComparer : IEqualityComparer<Figure>
        {
            public static readonly FigureLinearTransitionComparer Instance = new FigureLinearTransitionComparer();
            
            public bool Equals(Figure a, Figure b)
            {
                var normCellsA = GetNormalizedCells(a);
                var normCellsB = GetNormalizedCells(b);

                return
                    !normCellsA
                    .Zip(normCellsB, (ca, cb) => ca.RelativePoint.Equals(cb.RelativePoint) && ca.Color == cb.Color)
                    .Any(eq => !eq);
            }

            public int GetHashCode(Figure f)
            {
                var normCells = GetNormalizedCells(f);
                return
                    normCells.Aggregate(
                        normCells.Count << 16, 
                        (hc, c) => 
                            hc ^ c.RelativePoint.X << 8 ^ c.RelativePoint.Y << 1 + (int)c.Color);
            }

            private static IList<FigureCell> GetNormalizedCells(Figure f)
            {
                int minX = f.Cells.Min(c => c.RelativePoint.X);
                int minY = f.Cells.Min(c => c.RelativePoint.Y);

                return 
                    f.Cells
                    .Select(
                        c => new FigureCell(
                            Point.Of(c.RelativePoint.X - minX, c.RelativePoint.Y - minY), 
                            c.Color))
                    .OrderBy(c => c, FigureCellComparer.Instance)
                    .ToList();
            }

            class FigureCellComparer : IComparer<FigureCell>
            {
                public static readonly FigureCellComparer Instance = new FigureCellComparer();

                public int Compare(FigureCell cellA, FigureCell cellB)
                {
                    Contract.Requires(cellA != null);
                    Contract.Requires(cellB != null);

                    int res = ((int)cellA.Color).CompareTo((int)cellB.Color);
                    if (res == 0)
                    {
                        res = cellA.RelativePoint.Y.CompareTo(cellB.RelativePoint.Y);
                        if (res == 0)
                        {
                            res = cellA.RelativePoint.X.CompareTo(cellB.RelativePoint.X);
                        }
                    }

                    return res;
                }
            }
        }
    }
}
