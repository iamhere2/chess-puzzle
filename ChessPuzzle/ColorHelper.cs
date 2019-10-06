using System;
using System.Runtime.CompilerServices;

namespace ChessPuzzle
{
    static class ColorHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color GetInverted(this Color color)
            => color switch
                {
                    Color.White => Color.Black,
                    Color.Black => Color.White,
                    _ => throw new ArgumentOutOfRangeException(nameof(color))
                };
    }
}
