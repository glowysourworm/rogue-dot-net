using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Math.Geometry
{
    public class ReferencedTriangle<T> where T : class
    {
        public ReferencedVertex<T> Point1 { get; set; }
        public ReferencedVertex<T> Point2 { get; set; }
        public ReferencedVertex<T> Point3 { get; set; }

        public ReferencedEdge<T> Edge12()
        {
            return new ReferencedEdge<T>(this.Point1, this.Point2);
        }
        public ReferencedEdge<T> Edge23()
        {
            return new ReferencedEdge<T>(this.Point2, this.Point3);
        }
        public ReferencedEdge<T> Edge31()
        {
            return new ReferencedEdge<T>(this.Point3, this.Point1);
        }

        public IEnumerable<ReferencedVertex<T>> Vertices
        {
            get { return new ReferencedVertex<T>[] { this.Point1, this.Point2, this.Point3 }; }
        }

        public IEnumerable<ReferencedEdge<T>> Edges
        {
            get { return new ReferencedEdge<T>[] { this.Edge12(), this.Edge23(), this.Edge31() }; }
        }

        public ReferencedTriangle(ReferencedVertex<T> point1, ReferencedVertex<T> point2, ReferencedVertex<T> point3)
        {
            this.Point1 = point1;
            this.Point2 = point2;
            this.Point3 = point3;
        }

        public bool SharesEdgeWith(ReferencedTriangle<T> triangle)
        {
            return this.Edges.Intersect(triangle.Edges).Count() > 0;
        }

        public bool CircumCircleContains(Vertex point)
        {
            // Procedure
            //
            // 1) Calculate determinant of the three vertices
            //      d > 0  (clock-wise)
            //      d = 0  (collinear)
            //      d < 0  (counter-clock-wise)
            //
            // 2) Calculate this same determinant for all configurations of vertices
            //    to find the counter-clockwise orientation. Label these point1, point2, point3.
            //
            // 3) Solve 3x3 determinant described here https://en.wikipedia.org/wiki/Delaunay_triangulation
            //      d > 0  (implies point "D" is inside the circum-circle)
            //
            // UPDATE:  PROBLEM WITH UI COORDINATES - MUST SWITCH THE SIGNS ON ORIENTATION.

            // Double Check:  There are only 2 possible orderings of the points

            // 1 -> 2 -> 3 (Results from crossing the vectors 12 X 23 - where subtracting the points gives you the vector)
            var d123 = Vertex.Orientation(this.Point1.Vertex, this.Point2.Vertex, this.Point3.Vertex);

            // 1 -> 3 -> 2
            var d132 = Vertex.Orientation(this.Point1.Vertex, this.Point3.Vertex, this.Point2.Vertex);

            // NOTE*** Must handle collinear case. This may be the incorrect way to handle this.
            if (d123 == 0 || d132 == 0)
                return false;

            // Re-number the vertices to be counter-clockwise (1 -> 2 -> 3)
            Vertex point1, point2, point3;

            // NOTE*** This is flipped in sign because of UI coordinates (y -> -y)
            //
            if (d123 > 0)
            {
                point1 = this.Point1.Vertex;
                point2 = this.Point2.Vertex;
                point3 = this.Point3.Vertex;
            }
            else if (d132 > 0)
            {
                point1 = this.Point1.Vertex;
                point2 = this.Point3.Vertex;
                point3 = this.Point2.Vertex;
            }
            else
                throw new Exception("Improper use of circum-circle algorithm");

            // 3) Solve the circum-circle interior determinant
            //

            var m00 = point1.X - point.X;
            var m10 = point1.Y - point.Y;
            var m20 = System.Math.Pow(m00, 2) + System.Math.Pow(m10, 2);
            var m01 = point2.X - point.X;
            var m11 = point2.Y - point.Y;
            var m21 = System.Math.Pow(m01, 2) + System.Math.Pow(m11, 2);
            var m02 = point3.X - point.X;
            var m12 = point3.Y - point.Y;
            var m22 = System.Math.Pow(m02, 2) + System.Math.Pow(m12, 2);

            var d = (m00 * ((m11 * m22) - (m21 * m12))) -
                    (m10 * ((m01 * m22) - (m21 * m02))) +
                    (m20 * ((m01 * m12) - (m11 * m02)));

            // Theorem:  Point lies in the circum-circle iff d > 0 (When 1 -> 2 -> 3 are sorted counter-clockwise)
            //

            // NOTE*** TODO:  MAY have to flip this in sign because of UI coordinates (y -> -y)
            //
            return d > 0;
        }
    }
}
