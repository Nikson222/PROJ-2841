namespace _Game._Scripts.Game.Models
{
    public readonly struct BoardPosition
    {
        public int X { get; }
        public int Y { get; }

        public BoardPosition(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}