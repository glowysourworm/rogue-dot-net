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

        /// <summary>
        /// returns a point along the left of the Rect at the fractional distance from the top
        /// </summary>
        public static Point LeftAtFraction(this Rect rect, double fraction)
        {
            return new Point(rect.Left, rect.Top + (rect.Height * fraction));
        }

        /// <summary>
        /// returns a point along the right of the Rect at the fractional distance from the top
        /// </summary>
        public static Point RightAtFraction(this Rect rect, double fraction)
        {
            return new Point(rect.Right, rect.Top + (rect.Height * fraction));
        }

        /// <summary>
        /// returns a point inside the rectangle at the given fractional distance (from the origin of the Rect)
        /// </summary>
        public static Point PointAtFraction(this Rect rect, double fractionX, double fractionY)
        {
            return new Point(rect.Left + (rect.Width * fractionX), rect.Top + (rect.Height * fractionY));
        }

        public static Point Center(this Rect rect)
        {
            return new Point(rect.X + (rect.Width / 2.0), rect.Y + (rect.Height / 2.0));
        }
    }
}
