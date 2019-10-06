using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessPuzzle
{
    class Resolver
    {
        public Resolver()
        {
            States = new Stack<SolutionState>();
        }

        public Stack<SolutionState> States { get; private set; }

        public SolutionState? FinalState { get; set; }

        public bool SearchSolution(SolutionState initialState)
        {
            Ensure.Arg(initialState, nameof(initialState)).IsNotNull();

            ClearStates();

            if (initialState.IsFinal)
            {
                FinalState = initialState;
                return true;
            }

            // Берем начальное состояние
            var currentState = initialState;

            while (true)
            {
                // Мы пришли или вернулись на некоторое состояние
                OnStateEnter(currentState);

                // Убедимся, что действия уже были рассчитаны или рассчитаем их
                currentState.EnsureDecisionsCalculated();

                var possibleDecisions = currentState.PossibleDecisions;
                Assert.That(possibleDecisions != null);

                // Есть ли еще нерассмотренные варианты действий?);
                if (possibleDecisions.Any())
                {
                    // Берем очередное действие, вычеркивая из списка
                    var decision = possibleDecisions.Dequeue();

                    // Рассчитываем состояние после действия
                    var nextState = currentState.CreateNextState(decision);

                    // Если состояние конечное - то выходим, решение найдено
                    if (nextState.IsFinal)
                    {
                        FinalState = nextState;
                        return true;
                    }

                    // Не конечное - тогда добавляем текущее состояние в стек,
                    // переходим к новому состоянию и рассчитываем действия для него
                    States.Push(currentState);
                    currentState = nextState;
                }
                else
                {
                    // На этом состоянии больше делать нечего - возвращаемся или признаем, что решения нет
                    if (States.Any())
                        currentState = States.Pop();
                    else
                        return false;
                }
            }
        }

        public class StateEventArgs : EventArgs
        {
            public SolutionState State { get; private set; }

            public StateEventArgs(SolutionState state) => State = state;
        }

        private void OnStateEnter(SolutionState state) => StateEnter?.Invoke(this, new StateEventArgs(state));

        public event EventHandler<StateEventArgs>? StateEnter;

        private void ClearStates()
        {
            States.Clear();
            FinalState = null;
        }
    }
}
