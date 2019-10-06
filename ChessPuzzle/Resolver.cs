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

        private SolutionState? CurrentState { get; set; }

        public bool SearchSolution(SolutionState initialState)
        {
            Ensure.Arg(initialState, nameof(initialState)).IsNotNull();

            Reset();

            // Берем начальное состояние за текущее
            CurrentState = initialState;

            // Бежим по дереву состояний, пока не найдем или оно не кончится
            while (true)
            {
                // Мы пришли или вернулись на некоторое состояние,
                // надо его проверить и оповестить, а если оно конечное - то все
                if (CheckCurrentState())
                    return true;

                // Если не конечное - идем по дереву, если еще есть куда
                if (!ProceedToNextState())
                    return false;
            }
        }

        private bool ProceedToNextState()
        {
            // Берем очередное нерассмотренное действие в текущем состоянии, если есть
            var nextDecision = GetNextDecisionIfAny();
            if (nextDecision == null)
            {
                // Нету. На этом состоянии больше делать нечего
                // Возвращаемся, если есть куда
                return ProceedBackward();
            }
            else
            {
                // Есть нерассмотренное действие - значит, переодим к новому состоянию
                ProceedForward(nextDecision);
                return true;
            }
        }

        private SolutionState.Decision? GetNextDecisionIfAny()
        {
            Assert.That(CurrentState != null);
            return CurrentState.GetNextDecisionIfAny();
        }

        private void ProceedForward(SolutionState.Decision nextDecision)
        {
            Assert.That(CurrentState != null);

            // Добавляем текущее состояние в стек
            States.Push(CurrentState);

            // Рассчитываем новое состояние после действия и переходим к нему
            CurrentState = CurrentState.CreateNextState(nextDecision);
        }

        private bool ProceedBackward()
        {
            if (States.Any())
            {
                // Есть куда возвращаться
                CurrentState = States.Pop();
                return true;
            }
            else
            {
                // Некуда возвращаться, решения нет
                return false;
            }
        }

        private bool CheckCurrentState()
        {
            Assert.That(CurrentState != null);

            OnStateEnter(CurrentState);

            // Если текущее состояние конечное - то запишем его и сообщим, что решение найдено
            if (CurrentState.IsFinal)
            {
                FinalState = CurrentState;
                return true;
            }
            else
            {
                return false;
            }
        }

        public class StateEventArgs : EventArgs
        {
            public SolutionState State { get; private set; }

            public StateEventArgs(SolutionState state) => State = state;
        }

        private void OnStateEnter(SolutionState state) => StateEnter?.Invoke(this, new StateEventArgs(state));

        public event EventHandler<StateEventArgs>? StateEnter;

        private void Reset()
        {
            States.Clear();
            FinalState = null;
        }
    }
}
