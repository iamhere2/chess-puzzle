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

        private Board(ICollection<Placement> figures)
        {
            Ensure.Arg(figures, nameof(figures)).IsNotNull();
            Figures = figures;

            PointsMap = CalcPointsMap();
            OddCellsColor = CalcOddCellsColor();
        }

        private Board(Board prevBoard, Figure newFigure, Point newFigurePlacementPoint)
        {
            if (!prevBoard.IsValidPlacement(newFigure, newFigurePlacementPoint))
                throw new ArgumentException("Invalid or impossible placement");

            var newPlacement = new Placement(newFigure, newFigurePlacementPoint, prevBoard.Figures.Count + 1);

            var figures = new List<Placement>(prevBoard.Figures) { newPlacement };
            Figures = figures;
          
            OddCellsColor = prevBoard.OddCellsColor ?? CalcOddCellsColor();

            PointsMap = new BitArray(prevBoard.PointsMap);
            foreach (var point in newPlacement.GetCellPoints())
                PointsMap.Set(GetPointMapIndex(point), true);
        }


        #region Points map

        private BitArray PointsMap { get; }

        private BitArray CalcPointsMap()
        {
            var bits = new BitArray(WIDTH * WIDTH, false);

            foreach (var point in Figures.SelectMany(placement => placement.GetCellPoints()))
                bits.Set(GetPointMapIndex(point), true);

            return bits;
        }

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

        public ICollection<Placement> Figures { get; private set; }

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

        private Color? OddCellsColor { get; }

        private Color? CalcOddCellsColor()
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

            return new Board(prev, figure, point);
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
