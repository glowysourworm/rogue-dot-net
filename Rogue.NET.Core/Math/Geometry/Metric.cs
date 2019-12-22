using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

using System.Windows;

namespace Rogue.NET.Core.Math.Geometry
{
    public static class Metric
    {
        /// <summary>
        /// Enumeration used to pass to methods to specify which static metric to invoke
        /// </summary>
        public enum MetricType
        {
            Roguian,
            Euclidean
        }

        #region Euclidean Distance
        public static double EuclideanDistance(double x, double y)
        {
            return System.Math.Sqrt((x * x) + (y * y));
        }

        public static double EuclideanSquareDistance(double x, double y)
        {
            return (x * x) + (y * y);
        }

        public static double EuclideanDistance(Point point1, Point point2)
        {
            return System.Math.Sqrt(System.Math.Pow((point2.X - point1.X), 2) + System.Math.Pow((point2.Y - point1.Y), 2));
        }

        public static double EuclideanSquareDistance(Point point1, Point point2)
        {
            return System.Math.Pow((point2.X - point1.X), 2) + System.Math.Pow((point2.Y - point1.Y), 2);
        }

        public static double EuclideanDistance(IGridLocator location1, IGridLocator location2)
        {
            return System.Math.Sqrt(System.Math.Pow((location2.Column - location1.Column), 2) + System.Math.Pow((location2.Row - location1.Row), 2));
        }

        public static double EuclideanDistance(int column1, int row1, int column2, int row2)
        {
            return System.Math.Sqrt(System.Math.Pow((column2 - column1), 2) + System.Math.Pow((row2 - row1), 2));
        }

        public static double EuclideanSquareDistance(IGridLocator location1, IGridLocator location2)
        {
            return System.Math.Pow((location2.Column - location1.Column), 2) + System.Math.Pow((location2.Row - location1.Row), 2);
        }
        #endregion

        #region Roguian Distance
        public static int RoguianDistance(int x, int y)
        {
            return System.Math.Abs(x - y);
        }
        public static int RoguianDistance(int x1, int y1, int x2, int y2)
        {
            var x = (int)System.Math.Abs(x2 - x1);
            var y = (int)System.Math.Abs(y2 - y1);
            return System.Math.Max(x, y);
        }
        public static int RoguianDistance(Point point1, Point point2)
        {
            var x = (int)System.Math.Abs(point2.X - point1.X);
            var y = (int)System.Math.Abs(point2.Y - point1.Y);
            return System.Math.Max(x, y);
        }
        public static int RoguianDistance(IGridLocator location1, IGridLocator location2)
        {
            var x = System.Math.Abs(location2.Column - location1.Column);
            var y = System.Math.Abs(location2.Row - location1.Row);
            return System.Math.Max(x, y);
        }
        #endregion
    }
}
