using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;

namespace ChessPuzzle
{
    class Figure
    {
        public Figure(Color colorOfOriginCell, params Point[] otherCellPoints) : this(colorOfOriginCell, (IEnumerable<Point>)otherCellPoints)
        {
        }

        public Figure(Color colorOfOriginCell, IEnumerable<Point> otherCellPoints)
        {
            Contract.Requires(otherCellPoints != null);
            Contract.Requires(otherCellPoints.Any());
            Contract.Ensures(Cells != null);

            _colorOfOriginCell = colorOfOriginCell;
            Cells = new ReadOnlyCollection<FigureCell>(CalculateCells(colorOfOriginCell, otherCellPoints));
        }

        /// <summary>Cells of figure</summary>
        /// <remarks>
        /// Contract: first cell has point (0,0) = Point.Origin
        /// </remarks>
        public ReadOnlyCollection<FigureCell> Cells { get; private set; }

        public Color ColorOfOriginCell
        {
            get { return _colorOfOriginCell; }
        }

        private readonly Color _colorOfOriginCell;

        private static IList<FigureCell> CalculateCells(Color colorOfOriginCell, IEnumerable<Point> otherCellPoints)
        {
            Contract.Requires(otherCellPoints != null);

            return
                new [] { Point.Origin }
                .Concat(otherCellPoints)
                .Select(
                    p => new FigureCell(p, CalculateColor(colorOfOriginCell, p)))
                .ToList();
        }

        private static Color CalculateColor(Color colorOfOriginCell, Point relativePoint)
        {
            return relativePoint.IsOdd ? colorOfOriginCell.GetInverted() : colorOfOriginCell;
        }
    }
}
