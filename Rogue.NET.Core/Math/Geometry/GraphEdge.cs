using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System;

namespace Rogue.NET.Core.Math.Geometry
{
    public class GraphEdge<T> where T : Region
    {
        public GraphVertex<T> Point1 { get; set; }
        public GraphVertex<T> Point2 { get; set; }

        public GraphEdge(GraphVertex<T> point1, GraphVertex<T> point2)
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
