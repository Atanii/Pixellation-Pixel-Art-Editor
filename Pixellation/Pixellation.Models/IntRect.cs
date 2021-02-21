namespace Pixellation.Models
{
    public struct IntRect
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public int Right => X + Width;
        public int Bottom => Y + Height;

        public IntRect(int x = 0, int y = 0, int w = 0, int h = 0)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }

        public bool Contains(IntPoint p)
        {
            return p.X >= X && p.X <= Right && p.Y >= Y && p.Y <= Bottom;
        }

        public bool Contains(int x, int y)
        {
            return x >= X && x <= Right && y >= Y && y <= Bottom;
        }
    }
}