using System;

namespace Rogue.NET.Core.Math.Geometry
{
    public struct Ray
    {
        public Vertex Origin { get; set; }
        public double Angle { get; set; }

        public Ray(Vertex origin, double angle)
        {
            this.Origin = origin;
            this.Angle = angle;
        }

        public Vector UnitVector()
        {
            return new Vector(System.Math.Cos(this.Angle), System.Math.Sin(this.Angle));
        }

        public static bool operator ==(Ray ray1, Ray ray2)
        {
            return ray1.Equals(ray2);
        }

        public static bool operator !=(Ray ray1, Ray ray2)
        {
            return !ray1.Equals(ray2);
        }

        public override bool Equals(object obj)
        {
            if (obj is Ray)
            {
                var ray = (Ray)obj;

                return (ray.Origin == this.Origin && ray.Angle == this.Angle);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "Origin=(" + this.Origin.ToString() + ") Angle=" + this.Angle.ToString("F3");
        }
    }
}
