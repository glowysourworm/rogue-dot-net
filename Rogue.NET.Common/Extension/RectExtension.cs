using System.Windows;

namespace Rogue.NET.Common.Extension
{
    public static class RectExtension
    {
        public static Point BottomMiddle(this Rect rect)
        {
            return new Point(rect.X + (rect.Width / 2.0), rect.Bottom);
        }
        public static Point TopMiddle(this Rect rect)
        {
            return new Point(rect.X + (rect.Width / 2.0), rect.Top);
        }
        public static Point LeftMiddle(this Rect rect)
        {
            return new Point(rect.Left, rect.Y + (rect.Height / 2.0));
        }
        public static Point RightMiddle(this Rect rect)
        {
            return new Point(rect.Right, rect.Y + (rect.Height / 2.0));
        }

        /// <summary>
        /// returns a point along the bottom of the Rect at the fractional distance from the left side
        /// </summary>
        public static Point BottomAtFraction(this Rect rect, double fraction)
        {
            return new Point(rect.X + (rect.Width * fraction), rect.Bottom);
        }

        /// <summary>
        /// returns a point along the bottom of the Rect at the fractional distance from the left side
        /// </summary>
        public static Point TopAtFraction(this Rect rect, double fraction)
        {
            return new Point(rect.X + (rect.Width * fraction), rect.Top);
        }
    }
}
