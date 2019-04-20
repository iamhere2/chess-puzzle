using System;

namespace ChessPuzzle
{
    class Program
    {
        static void Main() => SolvePuzzle();

        private static void SolvePuzzle()
        {
            var figures =
                new[]
                    {
                        new Figure(Color.Black, Point.Of(1,0), Point.Of(2,0), Point.Of(3,0), Point.Of(4,0)), 
                        new Figure(Color.White, Point.Of(1,0), Point.Of(1,1), Point.Of(0,1)), 
                        new Figure(Color.Black, Point.Of(1,0), Point.Of(0,1), Point.Of(-1,0), Point.Of(0,-1)), 
                        new Figure(Color.Black, Point.Of(1,0), Point.Of(2,0), Point.Of(3,0), Point.Of(3,1)), 
                        new Figure(Color.Black, Point.Of(1,0), Point.Of(1,1), Point.Of(0,1), Point.Of(2,0)), 
                        new Figure(Color.White, Point.Of(1,0), Point.Of(-1,0), Point.Of(1,1), Point.Of(-1,1)), 
                        new Figure(Color.White, Point.Of(0,1), Point.Of(0,2), Point.Of(1,2), Point.Of(-1,2)), 
                        new Figure(Color.Black, Point.Of(1,0), Point.Of(2,0), Point.Of(2,1), Point.Of(3,1)), 
                        new Figure(Color.Black, Point.Of(1,0), Point.Of(1,1), Point.Of(2,1), Point.Of(2,2)), 
                        new Figure(Color.Black, Point.Of(-1,1), Point.Of(0,1), Point.Of(1,1), Point.Of(1,2)), 
                        new Figure(Color.White, Point.Of(1,0), Point.Of(2,0), Point.Of(2,1), Point.Of(2,2)), 
                        new Figure(Color.White, Point.Of(1,0), Point.Of(2,0), Point.Of(2,1), Point.Of(3,0)), 
                    };

            var initialState = SolutionState.CreateInitial(figures);

            var r = new Resolver();

            long stateCounter = 0;
            DateTime startTime = DateTime.Now;
            r.StateEnter +=
                (sender, args) =>
                    {
                        stateCounter++;
                        if (stateCounter % 1 == 1000)
                        {
                            Console.Clear();
                            Console.WriteLine("State #{0:########}, time:{1}", stateCounter, DateTime.Now - startTime);
                            BoardPrinter.Print(args.State.Board);
                        }
                    };

            bool found = r.SearchSolution(initialState);

            Console.Clear();
            if (found && r.FinalState != null)
            {
                Console.WriteLine("Resolved :)\r\nStates tested:{0}\r\nTime:{1}", stateCounter, DateTime.Now - startTime);
                BoardPrinter.Print(r.FinalState.Board);
            }
            else
            {
                Console.WriteLine("Solution not found :(\r\nStates tested:{0}\r\nTime:{1}", stateCounter, DateTime.Now - startTime);
            }

            Console.ReadLine();
        }

        private static void TestTwoFiguresPlacement()
        {
            var s0 = Board.CreateEmpty();

            var f1 = new Figure(Color.Black, Point.Of(1, 0), Point.Of(2, 0), Point.Of(3, 0), Point.Of(3, 1));
            var s1 = Board.CreateByPlacementNewFigure(s0, f1, Point.Of(1, 1));
            Console.WriteLine("s1:");
            BoardPrinter.Print(s1);

            var f2 = new Figure(Color.White, Point.Of(1, 0), Point.Of(-1, 0), Point.Of(2, 0), Point.Of(2, -1), Point.Of(-1, -1));
            var s2 = Board.CreateByPlacementNewFigure(s1, f2, Point.Of(4, 3));
            Console.WriteLine("s2:");
            BoardPrinter.Print(s2);

            Console.ReadLine();
        }

        private static void TestSimplePlacement()
        {
            var s1 = Board.CreateEmpty();
            Console.WriteLine("s1:");
            BoardPrinter.Print(s1);

            var f = new Figure(Color.Black, Point.Of(1, 0), Point.Of(2, 0), Point.Of(3, 0));

            //var f = new Figure(Color.Black, Point.Of(1, 0), Point.Of(0, 1), Point.Of(-1, 0), Point.Of(0, -1)); 


            var s2 = Board.CreateByPlacementNewFigure(s1, f, Point.Of(3, 3));
            Console.WriteLine("s2:");
            BoardPrinter.Print(s2);

            foreach (var tf in FigureTransformer.GetTransformations(f))
            {
                if (s1.IsPossiblePlacement(tf, Point.Of(4, 4)))
                {
                    var si = Board.CreateByPlacementNewFigure(s1, tf, Point.Of(4, 4));

                    Console.WriteLine("si:");
                    BoardPrinter.Print(si);
                }
            }

            Console.ReadLine();
        }
    }
}
