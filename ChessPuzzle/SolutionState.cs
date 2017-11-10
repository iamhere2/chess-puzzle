using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;

namespace ChessPuzzle
{
    class SolutionState
    {
        public class Decision
        {
            public Figure SelectedFigure { get; set; }

            public Figure TransformedFigure { get; set; }

            public Point PlacementPoint { get; set; }
        }

        public Board Board { get; private set; }

        public bool IsDecisionsCalculated { get; private set; }

        public Queue<Decision> PossibleDecisions { get; private set; }

        public IList<Figure> RestFigures { get; private set; }

        private Dictionary<Figure, IList<Figure>> FigureTransformations { get; set; }

        public bool IsFinal { get { return !RestFigures.Any(); } }

        private SolutionState(Board board, IList<Figure> restFigures, Dictionary<Figure, IList<Figure>> figureTransformations)
        {
            Contract.Requires(board != null);
            Contract.Requires(restFigures != null);
            Contract.Requires(figureTransformations != null);

            Board = board;
            RestFigures = new ReadOnlyCollection<Figure>(restFigures);
            FigureTransformations = figureTransformations;
        }

        public static SolutionState CreateInitial(IList<Figure> figures)
        {
            return new SolutionState(Board.CreateEmpty(), figures, GetFigureTransformations(figures));
        }

        private static Dictionary<Figure, IList<Figure>> GetFigureTransformations(IEnumerable<Figure> figures)
        {
            return
                figures.ToDictionary(f => f, FigureTransformer.GetTransformations);
        }

        public SolutionState CreateNextState(Decision decision)
        {
            Contract.Requires(decision != null);

            var newBoard = 
                Board.CreateByPlacementNewFigure(
                    Board, 
                    decision.TransformedFigure, 
                    decision.PlacementPoint); 

            return 
                new SolutionState(newBoard, RestFigures.Where(f => f != decision.SelectedFigure).ToList(), FigureTransformations);
        }

        public void EnsureDecisionsCalculated()
        {
            if (!IsDecisionsCalculated)
            {
                CalculateDecisions();
                IsDecisionsCalculated = true;
            }
        }

        private void CalculateDecisions()
        {
            PossibleDecisions = new Queue<Decision>();

            // Найдем первую свободную точку - это будет целевая
            var targetPoint = FindFreePoint();

            // Для всех оставшихся фигур
            foreach (var restFigure in RestFigures)
            {
                // Для всех трансформаций
                foreach (var transformedFigure in FigureTransformations[restFigure])
                {
                    // Для всех (пробных) клеток трансформированной фигуры
                    foreach (var cell in transformedFigure.Cells)
                    {
                        // Расчитываем точку расположения для совмещения пробной клетки с целевой точкой
                        var relativePoint = cell.RelativePoint;
                        var placementPoint = Point.Of(targetPoint.X - relativePoint.X, targetPoint.Y - relativePoint.Y);

                        // Если это допустимое расположение - то добавляем в список решений, иначе - идем дальше
                        if (Board.IsValidPlacement(transformedFigure, placementPoint))
                        {
                            PossibleDecisions.Enqueue(
                                new Decision
                                    {
                                        SelectedFigure = restFigure,
                                        TransformedFigure = transformedFigure,
                                        PlacementPoint = placementPoint
                                    });
                        }
                    }
                }
            }
        }

        private Point FindFreePoint()
        {
            for (int y = Board.Low; y <= Board.High; y++)
            {
                for (int x = Board.Low; x <= Board.High; x++)
                {
                    var p = Point.Of(x, y);
                    if (!Board.HasCell(p))
                        return p;
                }
            }

            throw new InvalidOperationException("Free point not found");
        }
    }
}