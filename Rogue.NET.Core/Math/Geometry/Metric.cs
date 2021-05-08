using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

using System;

namespace Rogue.NET.Core.Math.Geometry
{
    public static class Metric
    {
        /// <summary>
        /// Enumeration used to pass to methods to specify which static metric to invoke
        /// </summary>
        public enum MetricType
        {
            /// <summary>
            /// Diagonal movements equal cardinal movements
            /// </summary>
            Rogue,

            /// <summary>
            /// Euclidean distance
            /// </summary>
            Euclidean
        }

        public static double Length(IGridLocator locationFromOrigin)
        {
            switch (locationFromOrigin.Type)
            {
                case MetricType.Rogue:
                    return System.Math.Max(locationFromOrigin.Column, locationFromOrigin.Row);
                case MetricType.Euclidean:
                    return System.Math.Sqrt(System.Math.Pow(locationFromOrigin.Column, 2) + System.Math.Pow(locationFromOrigin.Row, 2));
                default:
                    throw new Exception("Invalid MetricType Metric.cs");
            }
        }

        public static double Distance(IGridLocator location1, IGridLocator location2)
        {
            if (location1.Type != location2.Type)
                throw new Exception("Trying to use mis-matching metric:  Metric.Distance");

            switch (location1.Type)
            {
                case MetricType.Rogue:
                    {
                        var dx = System.Math.Abs(location2.Column - location1.Column);
                        var dy = System.Math.Abs(location2.Row - location1.Row);
                        return System.Math.Max(dx, dy);
                    }
                case MetricType.Euclidean:
                    {
                        var dx = System.Math.Abs(location2.Column - location1.Column);
                        var dy = System.Math.Abs(location2.Row - location1.Row);
                        return System.Math.Sqrt(System.Math.Pow(dx, 2) + System.Math.Pow(dy, 2));
                    }
                default:
                    throw new Exception("Invalid MetricType Metric.cs");
            }
        }

        /// <summary>
        /// *** NOTE OVERRIDES IGridLocator METRIC!  Forces a distance calculation for the specified metric
        /// </summary>
        public static double ForceDistance(IGridLocator location1, IGridLocator location2, MetricType metric)
        {
            if (location1.Type != location2.Type)
                throw new Exception("WARNING*** Trying to use mis-matching metric:  Metric.ForceDistance");

            switch (metric)
            {
                case MetricType.Rogue:
                    {
                        var dx = System.Math.Abs(location2.Column - location1.Column);
                        var dy = System.Math.Abs(location2.Row - location1.Row);
                        return System.Math.Max(dx, dy);
                    }
                case MetricType.Euclidean:
                    {
                        var dx = System.Math.Abs(location2.Column - location1.Column);
                        var dy = System.Math.Abs(location2.Row - location1.Row);
                        return System.Math.Sqrt(System.Math.Pow(dx, 2) + System.Math.Pow(dy, 2));
                    }
                default:
                    throw new Exception("Invalid MetricType Metric.cs");
            }
        }

        /// <summary>
        /// FORCES a distance calculation for the specified metric
        /// </summary>
        public static double ForceDistance(int column1, int row1, int column2, int row2, MetricType metric)
        {
            switch (metric)
            {
                case MetricType.Rogue:
                    {
                        var dx = System.Math.Abs(column2 - column1);
                        var dy = System.Math.Abs(row2 - row1);
                        return System.Math.Max(dx, dy);
                    }
                case MetricType.Euclidean:
                    {
                        var dx = System.Math.Abs(column2 - column1);
                        var dy = System.Math.Abs(row2 - row1);
                        return System.Math.Sqrt(System.Math.Pow(dx, 2) + System.Math.Pow(dy, 2));
                    }
                default:
                    throw new Exception("Invalid MetricType Metric.cs");
            }
        }

        public static double SquareDistance(IGridLocator location1, IGridLocator location2)
        {
            if (location1.Type != location2.Type)
                throw new Exception("Trying to use mis-matching metric:  Metric.SquareDistance");

            switch (location1.Type)
            {
                case MetricType.Rogue:
                    throw new Exception("Trying to use Square Distance for Roguain Metric:  Metric.SquareDistance");
                case MetricType.Euclidean:
                    {
                        var dx = System.Math.Abs(location2.Column - location1.Column);
                        var dy = System.Math.Abs(location2.Row - location1.Row);
                        return System.Math.Pow(dx, 2) + System.Math.Pow(dy, 2);
                    }
                default:
                    throw new Exception("Invalid MetricType Metric.cs");
            }
        }
    }
}
