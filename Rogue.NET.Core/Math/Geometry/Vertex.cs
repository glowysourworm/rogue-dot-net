using System;

namespace Rogue.NET.Core.Math.Geometry
{
    public struct Vertex
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Vertex(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public Vertex(VertexInt vertex)
        {
            this.X = (int)vertex.X;
            this.Y = (int)vertex.Y;
        }

        public double EuclideanSquareDistance(Vertex vertex)
        {
            return (this.X * vertex.X) + (this.Y * vertex.Y);
        }

        public Vector Subtract(Vertex vertex)
        {
            var componentX = this.X - vertex.X;
            var componentY = this.Y - vertex.Y;

            var angle = System.Math.Atan2(componentY, componentX);
            var origin = vertex;

            return new Vector(componentX, componentY);
        }

        public double Dot(Vertex vertex)
        {
            return (this.X * vertex.X) + (this.Y * vertex.Y);
        }

        /// <summary>
        /// Returns the determinant of the orientation cross product (the sign of the cross product resulting in 
        /// crossing two difference vectors that order points 1,2, and 3 in that order). The sign of the determinant
        /// returned shows the orientation of the ordering (clockwise, counter-clockwise, or collinear)
        /// </summary>
        public static double Orientation(Vertex point1, Vertex point2, Vertex point3)
        {
            // 1 -> 2 -> 3 (Results from crossing the vectors 12 X 23 - where subtracting the points gives you the vector)
            var vector12 = point2.Subtract(point1);
            var vector23 = point3.Subtract(point2);

            return vector12.Cross(vector23);
        }

        public static bool operator ==(Vertex v1, Vertex v2)
        {
            return v1.Equals(v2);
        }

        public static bool operator !=(Vertex v1, Vertex v2)
        {
            return !v1.Equals(v2);
        }

        public override bool Equals(object obj)
        {
            if (obj is Vertex)
            {
                var vertex = (Vertex)obj;

                return vertex.X == this.X && vertex.Y == this.Y;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "x=" + this.X.ToString("F3") + "  y=" + this.Y.ToString("F3");
        }
    }
}
