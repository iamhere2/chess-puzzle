using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ChessPuzzle
{
    class Board
    {
        public const int LOW = 1;
        public const int HIGH = 8;
        public const int WIDTH = HIGH - LOW + 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInRange(Point p) => p.X >= LOW && p.X <= HIGH && p.Y >= LOW && p.Y <= HIGH;

        public static readonly Point[] AllPointsOrdered = CreateAllPointsOrdered().ToArray();

        private static IEnumerable<Point> CreateAllPointsOrdered()
        {
            for (int y = LOW; y <= HIGH; y++)
            {
                for (int x = LOW; x <= HIGH; x++)
                {
                    yield return new Point(x, y);
                }
            }
        }

        public struct Placement
        {
            public Figure Figure { get; }

            public Point Point { get; }

            public int Num { get; }

            public Placement(Figure figure, Point point, int num)
            {
                Ensure.Arg(figure, nameof(figure)).IsNotNull();

                Figure = figure;
                Num = num;
                Point = point;
            }

            public Color? GetColor(Point point) => FindFigureCell(point)?.Color;

            private FigureCell? FindFigureCell(Point checkPoint)
            {
                var placementPoint = Point;
                var shiftedCheckPoint = checkPoint.Shift(-placementPoint);
                return Figure.Cells.FirstOrNull(c => shiftedCheckPoint == c.RelativePoint);
            }

            public IEnumerable<Point> GetCellPoints()
            {
                var placementPoint = Point;
                return Figure.Cells.Select(c => c.RelativePoint.Shift(placementPoint));
            }
        }

        private Board(IEnumerable<Placement> figures)
        {
            Ensure.Arg(figures, nameof(figures)).IsNotNull();
            Figures = figures;

            //_occupiedPointsLazy = new Lazy<HashSet<Point>>(
            //    () => new HashSet<Point>(Figures.SelectMany(placement => placement.GetCellPoints())),
            //    isThreadSafe: false);

            _pointsMapLazy = new Lazy<BitArray>(() => 
                {
                    var bits = new BitArray(WIDTH * WIDTH, false);

                    foreach (var point in Figures.SelectMany(placement => placement.GetCellPoints()))
                        bits.Set(GetPointMapIndex(point), true);

                    return bits;
                },
                isThreadSafe: false);

            _oddCellsColorLazy = new Lazy<Color?>(CalcOddCellsColor, isThreadSafe: false);
        }

        #region Points map

        // public bool HasCell(Point p) => OccupiedPoints.Contains(p);

        // private HashSet<Point> OccupiedPoints => _occupiedPointsLazy.Value;

        // private readonly Lazy<HashSet<Point>> _occupiedPointsLazy;

        private readonly Lazy<BitArray> _pointsMapLazy;

        private BitArray PointsMap => _pointsMapLazy.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetPointMapIndex(Point p) => (p.Y - LOW) * WIDTH + (p.X - LOW);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasCell(Point p) => PointsMap.Get(GetPointMapIndex(p));

        #endregion

        public Point? FindFirstFreePoint()
        {
            for (int i = 0; i < PointsMap.Length; i++)
            {
                if (!PointsMap.Get(i))
                    return AllPointsOrdered[i];
            }

            return null;
        }

        public IEnumerable<Placement> Figures { get; private set; }

        public bool IsPossiblePlacement(Figure figure, Point point)
        {
            Ensure.Arg(figure, nameof(figure)).IsNotNull();

            return
                figure.Cells.All(fc =>
                    {
                        var p = fc.RelativePoint.Shift(point);
                        return IsInRange(p) && !HasCell(p);
                    });
        }

        public bool IsValidPlacement(Figure figure, Point point)
        {
            Ensure.Arg(figure, nameof(figure)).IsNotNull();
            Color? oddColor = OddCellsColor;

            return
                IsPossiblePlacement(figure, point)
                && (
                       oddColor == null
                    || CalculateOddCellsColor(point, figure.ColorOfOriginCell) == oddColor);
        }

        private Color? OddCellsColor => _oddCellsColorLazy.Value;
        private readonly Lazy<Color?> _oddCellsColorLazy;

        public Color? CalcOddCellsColor()
        {
            if (!Figures.Any())
                return null;

            var ff = Figures.First();

            var firstCellColor = ff.GetColor(ff.Point);
            Assert.That(firstCellColor.HasValue);

            return CalculateOddCellsColor(ff.Point, firstCellColor.Value);
        }

        private static Color CalculateOddCellsColor(Point probePoint, Color probeColor)
            => probePoint.IsOdd ? probeColor : probeColor.GetInverted();

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
            new Board(new ReadOnlyCollection<Placement>(Array.Empty<Placement>()));

        public static Board CreateByPlacementNewFigure(Board prev, Figure figure, Point point)
        {
            Ensure.Arg(prev, nameof(prev)).IsNotNull();
            Ensure.Arg(figure, nameof(figure)).IsNotNull();

            if (!prev.IsValidPlacement(figure, point))
                throw new ArgumentException("Invalid or impossible placement");

            return new Board(
                prev.Figures.Concat(new[] { new Placement(figure, point, prev.Figures.Count() + 1) }));
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
