using System;
using System.Globalization;

namespace ChessPuzzle
{
    static class BoardPrinter
    {
        public static void Print(Board board)
        {
            Ensure.Arg(board, nameof(board)).IsNotNull();

            WithSaveAndRestoreColors(() =>
            {
                foreach (var point in Board.AllPointsOrdered)
                {
                    Print(board.GetPointInfo(point));

                    if (point.X == Board.High)
                        Console.WriteLine();
                }
            });
        }

        private static void Print(Board.PointInfo pi)
        {
            SetColors(pi.Color);
            Console.Write(pi.Placement?.Num.ToString("00", CultureInfo.InvariantCulture) ?? "  ");
        }

        private static void SetColors(Color? color)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = color switch
            {
                Color.White => ConsoleColor.White,
                Color.Black => ConsoleColor.Black,
                _ => ConsoleColor.DarkGray
            };
        }

        private static void WithSaveAndRestoreColors(Action action)
        {
            var oldBkColor = Console.BackgroundColor;
            var oldFgColor = Console.ForegroundColor;

            try
            {
                action();
            }
            finally
            {
                Console.BackgroundColor = oldBkColor;
                Console.ForegroundColor = oldFgColor;
            }
        }
    }
}
