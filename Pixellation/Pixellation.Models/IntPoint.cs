namespace Pixellation.Models
{
    /// <summary>
    /// Simple struct representing an X, Y point with int values.
    /// </summary>
    public struct IntPoint
    {
        /// <summary>
        /// X component
        /// </summary>
        public int X;

        /// <summary>
        /// Y component
        /// </summary>
        public int Y;

        /// <summary>
        /// IntPoint with given values.
        /// </summary>
        /// <param name="x">Value of X component.</param>
        /// <param name="y">Value of Y component.</param>
        public IntPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static IntPoint operator -(IntPoint p1, IntPoint p2)
        {
            return new IntPoint(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static IntPoint operator +(IntPoint p1, IntPoint p2)
        {
            return new IntPoint(p1.X + p2.X, p1.Y + p2.Y);
        }
    }
}