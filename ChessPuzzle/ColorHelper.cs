using System;

namespace ChessPuzzle
{
    static class ColorHelper
    {
        public static Color GetInverted(this Color color)
        {
            switch (color)
            {
                case Color.White: return Color.Black;
                case Color.Black: return Color.White;
                default:
                    throw new ArgumentOutOfRangeException("color");
            }
        }
    }
}
