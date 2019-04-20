using System;

namespace ChessPuzzle
{
    static class ColorHelper
    {
        public static Color GetInverted(this Color color)
            => color switch
                {
                    Color.White => Color.Black,
                    Color.Black => Color.White,
                    _ => throw new ArgumentOutOfRangeException(nameof(color))
                };
    }
}
