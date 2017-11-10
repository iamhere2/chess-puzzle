using System;
using System.Diagnostics.Contracts;

namespace ChessPuzzle
{
    class BoardPrinter
    {
        public static void Print(Board board)
        {
            Contract.Requires(board != null);

            var oldBkColor = Console.BackgroundColor;
            var oldFgColor = Console.ForegroundColor;

            for (int y = Board.Low; y <= Board.High; y++)
            {
                for (int x = Board.Low; x <= Board.High; x++)
                {
                    var pointInfo = board.GetPointInfo(Point.Of(x, y));

                    var color = pointInfo.Color;
                    SetColors(color);

                    var placement = pointInfo.Placement;
                    var numStr = placement.HasValue ? placement.Value.Num.ToString("00") : "  ";
                    Console.Write(numStr);
                }

                Console.WriteLine();
            }

            Console.BackgroundColor = oldBkColor;
            Console.ForegroundColor = oldFgColor;
        }

        private static void SetColors(Color? color)
        {
            if (!color.HasValue)
            {
                Console.BackgroundColor = ConsoleColor.DarkGray;
                return;
            }

            switch (color.Value)
            {
                case Color.White:
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case Color.Black:
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
