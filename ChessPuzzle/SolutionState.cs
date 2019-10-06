using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ChessPuzzle
{
    class SolutionState
    {
        public class Decision
        {
            public Decision(Figure selectedFigure, Figure transformedFigure, Point placementPoint)
            {
                SelectedFigure = selectedFigure;
                TransformedFigure = transformedFigure;
                PlacementPoint = placementPoint;
            }
            public Figure SelectedFigure { get; }

            public Figure TransformedFigure { get; }

            public Point PlacementPoint { get; }
        }

        public Board Board { get; private set; }

        public bool IsDecisionsCalculated { get; private set; }

        public Queue<Decision>? PossibleDecisions { get; private set; }

        public IList<Figure> RestFigures { get; private set; }

        private Dictionary<Figure, IList<Figure>> FigureTransformations { get; set; }

        public bool IsFinal => !RestFigures.Any();

        private SolutionState(Board board, IList<Figure> restFigures, Dictionary<Figure, IList<Figure>> figureTransformations)
        {
            Ensure.Arg(board, nameof(board)).IsNotNull();
            Ensure.Arg(restFigures, nameof(restFigures)).IsNotNull();
            Ensure.Arg(figureTransformations, nameof(figureTransformations)).IsNotNull();

            Board = board;
            RestFigures = new ReadOnlyCollection<Figure>(restFigures);
            FigureTransformations = figureTransformations;
        }

        public static SolutionState CreateInitial(IList<Figure> figures)
            => new SolutionState(Board.CreateEmpty(), figures, GetFigureTransformations(figures));

        private static Dictionary<Figure, IList<Figure>> GetFigureTransformations(IEnumerable<Figure> figures)
            => figures.ToDictionary(f => f, FigureTransformer.GetTransformations);

        public SolutionState CreateNextState(Decision decision)
        {
            Ensure.Arg(decision, nameof(decision)).IsNotNull();

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

        public Decision? GetNextDecisionIfAny()
        {
            EnsureDecisionsCalculated();
            Decision? res = null;
            PossibleDecisions?.TryDequeue(out res);
            return res;
        }

        private void CalculateDecisions()
        {
            PossibleDecisions = new Queue<Decision>();

            // Найдем первую свободную точку - это будет целевая
            var targetPointOrNull = Board.FindFirstFreePoint();

            // Если не нашлась - значит, возможных размещений больше нет
            if (targetPointOrNull == null)
                return;

            var targetPoint = targetPointOrNull.Value;

            // Пытаемся разместить фигуры из сотавшихся
            foreach (var restFigure in RestFigures)
                TryPlaceFigure(targetPoint, restFigure);
        }

        private void TryPlaceFigure(Point targetPoint, Figure figure)
        {
            // Для всех трансформаций фигуры
            foreach (var transformedFigure in FigureTransformations[figure])
            {
                // Для всех (пробных) клеток трансформированной фигуры
                foreach (var cell in transformedFigure.Cells)
                {
                    // Расчитываем точку расположения для совмещения пробной клетки с целевой точкой
                    var placementPoint = targetPoint.Shift(-cell.RelativePoint);

                    // Если это допустимое расположение - то добавляем в список решений, иначе - идем дальше
                    if (Board.IsValidPlacement(transformedFigure, placementPoint))
                    {
                        Assert.That(PossibleDecisions != null);
                        PossibleDecisions.Enqueue(
                            new Decision(selectedFigure: figure, transformedFigure, placementPoint));
                    }
                }
            }
        }
    }
}