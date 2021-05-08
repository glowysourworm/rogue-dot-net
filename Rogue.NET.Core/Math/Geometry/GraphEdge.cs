using System;

namespace Rogue.NET.Core.Math.Geometry
{
    [Serializable]
    public class GraphEdge
    {
        public GraphVertex Point1 { get; set; }
        public GraphVertex Point2 { get; set; }

        public double Distance
        {
            get { return Metric.Distance(this.Point1, this.Point2); }
        }

        public GraphEdge(GraphVertex point1, GraphVertex point2)
        {
            this.Point1 = point1;
            this.Point2 = point2;
        }

        public override bool Equals(object obj)
        {
            if (obj is GraphEdge)
            {
                var edge = obj as GraphEdge;

                return this.Point1.Equals(edge.Point1) &&
                       this.Point2.Equals(edge.Point2);
            }

            return false;
        }

        public bool IsEquivalent(GraphEdge edge)
        {
            return (this.Point1.Equals(edge.Point1) &&
                    this.Point2.Equals(edge.Point2)) ||
                   (this.Point1.Equals(edge.Point2) &&
                    this.Point2.Equals(edge.Point1));
        }

        /// <summary>
        /// Returns true if the edge is equivalent to the edge specified by two vertices IN EITHER DIRECTION
        /// </summary>
        public bool IsEquivalent(GraphVertex vertex1, GraphVertex vertex2)
        {
            return (this.Point1.Equals(vertex1) &&
                    this.Point2.Equals(vertex2)) ||
                   (this.Point1.Equals(vertex2) &&
                    this.Point2.Equals(vertex1));
        }

        public override int GetHashCode()
        {
            var hash = 397;

            hash = (17 * hash) + this.Point1.GetHashCode();
            hash = (17 * hash) + this.Point2.GetHashCode();

            return hash;
        }

        public override string ToString()
        {
            return "(" + this.Point1.ToString() + ") -> (" + this.Point2.ToString() + ")";
        }
    }
}
