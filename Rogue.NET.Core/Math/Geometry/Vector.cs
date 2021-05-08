using System;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Math.Geometry
{
    /// <summary>
    /// Assumed to be double vector using a euclidean metric
    /// </summary>
    public class Vector
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Vector(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public static Vector Create(double angleRadians, double length)
        {
            return new Vector(length * System.Math.Cos(angleRadians),
                              length * System.Math.Sin(angleRadians));
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector)
            {
                var vector = (Vector)obj;

                return vector.X == this.X &&
                       vector.Y == this.Y;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("X={0} Y={1}", this.X.ToString("F2"), this.Y.ToString("F2"));
        }
    }
}
