namespace Pixellation.Models
{
    /// <summary>
    /// Represents a rectangle using only integer coordinates and properties.
    /// </summary>
    public struct IntRect
    {
        /// <summary>
        /// X component of the position.
        /// </summary>
        public int X;

        /// <summary>
        /// Y component of the position.
        /// </summary>
        public int Y;

        /// <summary>
        /// Width of the rectangle in pixels.
        /// </summary>
        public int Width;

        /// <summary>
        /// Height of the rectangle in pixels.
        /// </summary>
        public int Height;

        /// <summary>
        /// Coordinate for the upper right point of the rectangle.
        /// </summary>
        public int Right => X + Width;

        /// <summary>
        /// Coordinate for the lower left point of the rectangle.
        /// </summary>
        public int Bottom => Y + Height;

        /// <summary>
        /// Inits the rectangle.
        /// </summary>
        /// <param name="x">X component of position.</param>
        /// <param name="y">Y component of position.</param>
        /// <param name="w">Width in pixels.</param>
        /// <param name="h">Height in pixels.</param>
        public IntRect(int x = 0, int y = 0, int w = 0, int h = 0)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }

        /// <summary>
        /// Determines if the rectangle contains the specified <see cref="IntPoint"/>.
        /// </summary>
        /// <param name="p">Point to check.</param>
        /// <returns>True if contains, otherwise false.</returns>
        public bool Contains(IntPoint p)
        {
            return p.X >= X && p.X <= Right && p.Y >= Y && p.Y <= Bottom;
        }

        /// <summary>
        /// Determines if the rectangle contains the point specified by the given x and y component.
        /// </summary>
        /// <param name="x">X component of the coordinate.</param>
        /// <param name="y">Y component of the coordinate.</param>
        /// <returns>True if contains, otherwise false.</returns>
        public bool Contains(int x, int y)
        {
            return x >= X && x <= Right && y >= Y && y <= Bottom;
        }
    }
}