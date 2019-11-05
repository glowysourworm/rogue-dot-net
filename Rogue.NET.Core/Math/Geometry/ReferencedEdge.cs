using System;

namespace Rogue.NET.Core.Math.Geometry
{
    public class ReferencedEdge<T> where T : class
    {
        public ReferencedVertex<T> Point1 { get; set; }
        public ReferencedVertex<T> Point2 { get; set; }

        public double Length
        {
            get { return this.Point2.Vertex.Subtract(this.Point1.Vertex).Magnitude; }
        }

        public ReferencedEdge(ReferencedVertex<T> point1, ReferencedVertex<T> point2)
        {
            this.Point1 = point1;
            this.Point2 = point2;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "(" + this.Point1.Vertex.ToString() + ") -> (" + this.Point2.Vertex.ToString() + ")";
        }
    }
}
