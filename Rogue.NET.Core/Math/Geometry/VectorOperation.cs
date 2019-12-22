using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using System;
using static Rogue.NET.Core.Math.Geometry.Metric;

namespace Rogue.NET.Core.Math.Geometry
{
    public static class VectorOperation
    {
        public static GridVector Add(this IGridLocator location1, IGridLocator location2)
        {
            var componentX = location2.Column + location1.Column;
            var componentY = location2.Row + location1.Row;

            return new GridVector(componentX, componentY);
        }

        public static GridVector Subtract(this IGridLocator location1, IGridLocator location2)
        {
            var componentX = location2.Column - location1.Column;
            var componentY = location2.Row - location1.Row;

            return new GridVector(componentX, componentY);
        }

        public static GridVector Multiply(this IGridLocator location, float constant)
        {
            var componentX = location.Column * constant;
            var componentY = location.Row * constant;

            return new GridVector(componentX, componentY);
        }

        public static double Dot(this IGridLocator location1, IGridLocator location2)
        {
            return (location1.Column * location2.Column) + (location1.Row * location2.Row);
        }

        public static double Dot(this GridVector vector1, GridVector vector2)
        {
            return (vector1.ColumnDimension * vector2.ColumnDimension) + (vector1.RowDimension * vector2.RowDimension);
        }

        /// <summary>
        /// Returns the magnitude of the cross product (casted in 3 dimensions)
        /// </summary>
        public static double Cross(this IGridLocator vector1, IGridLocator vector2)
        {
            return (vector1.Column * vector2.Row) - (vector2.Column * vector1.Row);
        }

        /// <summary>
        /// Returns the determinant of the orientation cross product (the sign of the cross product resulting in 
        /// crossing two difference vectors that order points 1,2, and 3 in that order). The sign of the determinant
        /// returned shows the orientation of the ordering (clockwise, counter-clockwise, or collinear)
        /// </summary>
        public static double Orientation(IGridLocator point1, IGridLocator point2, IGridLocator point3)
        {
            // 1 -> 2 -> 3 (Results from crossing the vectors 12 X 23 - where subtracting the points gives you the vector)
            var vector12 = point2.Subtract(point1);
            var vector23 = point3.Subtract(point2);

            return vector12.Cross(vector23);
        }

        public static double Magnitude(this IGridLocator location, MetricType metricType)
        {
            switch (metricType)
            {
                case MetricType.Roguian:
                    return Metric.RoguianDistance(location.Column, location.Row);
                case MetricType.Euclidean:
                    return Metric.EuclideanDistance(location.Column, location.Row);
                case MetricType.TaxiCab:
                    return Metric.TaxiCabDistance(location.Column, location.Row);
                default:
                    throw new Exception("Unhandled Metric Type VectorOperation.cs");
            }
        }
    }
}
