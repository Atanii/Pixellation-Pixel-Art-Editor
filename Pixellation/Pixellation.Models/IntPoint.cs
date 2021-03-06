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

        /// <summary>
        /// Minus operator for <see cref="IntPoint"/>.
        /// </summary>
        /// <param name="p1">First <see cref="IntPoint"/>.</param>
        /// <param name="p2">Second <see cref="IntPoint"/>.</param>
        /// <returns>Resulting <see cref="IntPoint"/>.</returns>
        public static IntPoint operator -(IntPoint p1, IntPoint p2)
        {
            return new IntPoint(p1.X - p2.X, p1.Y - p2.Y);
        }

        /// <summary>
        /// Plus operator for <see cref="IntPoint"/>.
        /// </summary>
        /// <param name="p1">First <see cref="IntPoint"/>.</param>
        /// <param name="p2">Second <see cref="IntPoint"/>.</param>
        /// <returns>Resulting <see cref="IntPoint"/>.</returns>
        public static IntPoint operator +(IntPoint p1, IntPoint p2)
        {
            return new IntPoint(p1.X + p2.X, p1.Y + p2.Y);
        }
    }
}