using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;

namespace ChessPuzzle
{
    class Board
    {
        public const int Low = 1;
        public const int High = 8;

        public struct Placement
        {
            public Figure Figure { get; }

            public Point Point { get; }

            public int Num { get; }

            public Placement(Figure figure, Point point, int num)
            {
                Contract.Requires(figure != null);
                Figure = figure;
                Num = num;
                Point = point;
            }

            public Color? GetColor(Point point)
            {
                var figureCell = FindFigureCell(point);
                return figureCell == null ? (Color?)null : figureCell.Color;
            }

            private FigureCell FindFigureCell(Point point)
            {
                var placementPoint = Point;
                return 
                    Figure.Cells.FirstOrDefault(
                        c =>
                            point.X == placementPoint.X + c.RelativePoint.X &&
                            point.Y == placementPoint.Y + c.RelativePoint.Y);
            }

            public IEnumerable<Point> GetCellPoints()
            {
                var placementPoint = Point;
                return
                    Figure.Cells.Select(
                        c => Point.Of(
                            placementPoint.X + c.RelativePoint.X,
                            placementPoint.Y + c.RelativePoint.Y));
            }
        }

        public bool HasCell(Point p)
        {
            if (_hasCellCache == null)
            {
                _hasCellCache = new HashSet<Point>(
                    Figures.SelectMany(placement => placement.GetCellPoints()));
            }

            return _hasCellCache.Contains(p);
        }

        private HashSet<Point>? _hasCellCache = null;

        public IEnumerable<Placement> Figures { get; private set; }

        public bool IsPossiblePlacement(Figure figure, Point point)
        {
            Contract.Requires(figure != null);

            foreach (var fc in figure.Cells)
            {
                var rp = fc.RelativePoint;

                int x = point.X + rp.X;
                if (x < Low || x > High)
                    return false;

                int y = point.Y + rp.Y;
                if (y < Low || y > High)
                    return false;

                if (HasCell(Point.Of(x, y)))
                    return false;
            }

            return true;
        }

        public bool IsValidPlacement(Figure figure, Point point)
        {
            Contract.Requires(figure != null);

            if (!IsPossiblePlacement(figure, point))
                return false;

            Color? oddColor = GetOddCellsColor();

            if (!oddColor.HasValue)
                return true;

            Color placementOddColor = CalculateOddCellsColor(point, figure.ColorOfOriginCell);

            return placementOddColor == oddColor.Value;
        }

        public Color? GetOddCellsColor()
        {
            if (!Figures.Any())
                return null;

            var ff = Figures.First();
            return CalculateOddCellsColor(ff.Point, ff.GetColor(ff.Point).Value);
        }

        private static Color CalculateOddCellsColor(Point probePoint, Color probeColor) 
            => probePoint.IsOdd ? probeColor : probeColor.GetInverted();

        public Color? GetColor(Point point) 
            => GetPointInfo(point).Color;

        public PointInfo GetPointInfo(Point point)
        {
            foreach (var placement in Figures)
            {
                Color? color = placement.GetColor(point);
                if (color.HasValue)
                    return new PointInfo(placement, color);
            }

            return PointInfo.Empty;
        }

        public static Board CreateEmpty() => 
            new Board(new ReadOnlyCollection<Placement>(new Placement[] { }));

        public static Board CreateByPlacementNewFigure(Board prev, Figure figure, Point point)
        {
            Contract.Requires(prev != null);
            Contract.Requires(figure != null);

            if (!prev.IsValidPlacement(figure, point))
                throw new ArgumentException("Invalid or impossible placement");

            return new Board(
                prev.Figures.Concat(new[] { new Placement(figure, point, prev.Figures.Count() + 1) }));
        }

        private Board(IEnumerable<Placement> figures)
        {
            Contract.Requires(figures != null);
            Figures = figures;
        }

        public struct PointInfo
        {
            public Placement? Placement { get; }

            public Color? Color { get; }

            public PointInfo(Placement? placement, Color? color)
            {
                Placement = placement;
                Color = color;
            }

            public static readonly PointInfo Empty = new PointInfo(null, null);
        }

    }

}
