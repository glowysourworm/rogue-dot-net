using System;

namespace Rogue.NET.Core.Math.Geometry
{
    public struct Vector
    {
        public double X { get; set; }
        public double Y { get; set; }

        public double Magnitude
        {
            get { return System.Math.Sqrt((this.X * this.X) + (this.Y * this.Y)); }
        }

        public Vector(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public static Vector Create(double angleRadians, double length)
        {
            return new Vector(length * System.Math.Cos(angleRadians), length * System.Math.Sin(angleRadians));
        }

        public double Dot(Vector vector)
        {
            return (this.X * vector.X) + (this.Y * vector.Y);
        }

        public Vector Multiply(double constant)
        {
            return new Vector(this.X * constant, this.Y * constant);
        }

        /// <summary>
        /// Returns the magnitude of the cross product (casted in 3 dimensions)
        /// </summary>
        public double Cross(Vector vector)
        {
            return (this.X * vector.Y) - (vector.X * this.Y);
        }

        public Vector Subtract(Vector vector)
        {
            return new Vector(this.X - vector.X, this.Y - vector.Y);
        }

        public Vector Add(Vector vector)
        {
            return new Vector(this.X + vector.X, this.Y + vector.Y);
        }

        public static bool operator ==(Vector vector1, Vector vector2)
        {
            return vector1.Equals(vector2);
        }

        public static bool operator !=(Vector vector1, Vector vector2)
        {
            return !vector1.Equals(vector2);
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector)
            {
                var vector = (Vector)obj;

                return (vector.X == this.X && vector.Y == this.Y);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "x=" + this.X.ToString("F3") + " y=" + this.Y.ToString("F3");
        }
    }
}
