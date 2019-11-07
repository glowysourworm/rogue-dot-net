using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public static double EuclideanDistance(GridLocation location1, GridLocation location2)
        {
            return System.Math.Sqrt(System.Math.Pow((location2.Column - location1.Column), 2) + System.Math.Pow((location2.Row - location1.Row), 2));
        }

        public static double EuclideanSquareDistance(GridLocation location1, GridLocation location2)
        {
            return System.Math.Pow((location2.Column - location1.Column), 2) + System.Math.Pow((location2.Row - location1.Row), 2);
        }

        public static double EuclideanDistance(Vertex vertex1, Vertex vertex2)
        {
            return System.Math.Sqrt(System.Math.Pow((vertex2.X - vertex1.X), 2) + System.Math.Pow((vertex2.Y - vertex1.Y), 2));
        }

        public static double EuclideanSquareDistance(Vertex vertex1, Vertex vertex2)
        {
            return System.Math.Pow((vertex2.X - vertex1.X), 2) + System.Math.Pow((vertex2.Y - vertex1.Y), 2);
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
        public static int RoguianDistance(GridLocation location1, GridLocation location2)
        {
            var x = System.Math.Abs(location2.Column - location1.Column);
            var y = System.Math.Abs(location2.Row - location1.Row);
            return System.Math.Max(x, y);
        }
        public static int RoguianDistance(Vertex vertex1, Vertex vertex2)
        {
            var x = (int)System.Math.Abs(vertex2.X - vertex1.X);
            var y = (int)System.Math.Abs(vertex2.Y - vertex1.Y);
            return System.Math.Max(x, y);
        }

        /// <summary>
        /// Calculates the length of the specified vector using a Roguian metric
        /// </summary>
        public static int RoguianLength(Vector vector)
        {
            var x = (int)System.Math.Abs(vector.X);
            var y = (int)System.Math.Abs(vector.Y);
            return System.Math.Max(x, y);
        }
        #endregion
    }
}
