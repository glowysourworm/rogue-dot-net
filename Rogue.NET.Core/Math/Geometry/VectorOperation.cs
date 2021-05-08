using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

using System;

using static Rogue.NET.Core.Math.Geometry.Metric;

namespace Rogue.NET.Core.Math.Geometry
{
    public static class VectorOperation
    {
        public static Vector Add(this Vector vector1, Vector vector2)
        {
            var componentX = vector2.X + vector1.X;
            var componentY = vector2.Y + vector1.Y;

            return new Vector(componentX, componentY);
        }

        public static Vector Subtract(this Vector vector1, Vector vector2)
        {
            var componentX = vector2.X - vector1.X;
            var componentY = vector2.Y - vector1.Y;

            return new Vector(componentX, componentY);
        }

        public static Vector Subtract(this IGridLocator location1, IGridLocator location2)
        {
            if (location1.Type != location2.Type)
                throw new Exception("Trying to subtract two different types of IGridLocators:  VectorOperation.Subtract");

            var componentX = location2.Column - location1.Column;
            var componentY = location2.Row - location1.Row;

            return new Vector(componentX, componentY);
        }

        public static Vector Multiply(this Vector vector, float constant)
        {
            var componentX = vector.X * constant;
            var componentY = vector.Y * constant;

            return new Vector(componentX, componentY);
        }

        public static double Dot(this Vector vector1, Vector vector2)
        {
            return (vector1.X * vector2.X) + (vector1.Y * vector2.Y);
        }

        /// <summary>
        /// Returns the magnitude of the cross product (casted in 3 dimensions)
        /// </summary>
        public static double Cross(this Vector vector1, Vector vector2)
        {
            return (vector1.X * vector2.Y) - (vector2.X * vector1.Y);
        }

        /// <summary>
        /// Returns the determinant of the orientation cross product (the sign of the cross product resulting in 
        /// crossing two difference vectors that order points 1,2, and 3 in that order). The sign of the determinant
        /// returned shows the orientation of the ordering (clockwise, counter-clockwise, or collinear)
        /// </summary>
        public static double Orientation(IGridLocator point1, IGridLocator point2, IGridLocator point3)
        {
            if (point1.Type != MetricType.Euclidean ||
                point2.Type != MetricType.Euclidean ||
                point3.Type != MetricType.Euclidean)
                throw new Exception("Trying to use non-euclidean metric for orientation calculation:  VectorOperation.Orientation");

            // 1 -> 2 -> 3 (Results from crossing the vectors 12 X 23 - where subtracting the points gives you the vector)
            var vector12 = point2.Subtract(point1);
            var vector23 = point3.Subtract(point2);

            return vector12.Cross(vector23);
        }
    }
}
