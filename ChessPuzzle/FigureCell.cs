namespace ChessPuzzle
{
    class FigureCell
    {
        public Point RelativePoint { get; private set; }

        public Color Color { get; private set; }

        public FigureCell(Point relativePoint, Color color)
        {
            RelativePoint = relativePoint;
            Color = color;
        }
    }
}