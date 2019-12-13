using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using System;

namespace Rogue.NET.Core.Math.Geometry
{
    public class GraphEdge<T, V> where T : class, IGridLocator
    {
        public GraphVertex<T, V> Point1 { get; set; }
        public GraphVertex<T, V> Point2 { get; set; }

        public GraphEdge(GraphVertex<T, V> point1, GraphVertex<T, V> point2)
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
