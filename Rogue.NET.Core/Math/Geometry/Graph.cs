using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Math.Geometry
{
    public class Graph<T> where T : Region
    {
        List<GraphEdge<T>> _edges;

        /// <summary>
        /// Returns edges in the graph
        /// </summary>
        public IEnumerable<GraphEdge<T>> Edges
        {
            get { return _edges; }
        }

        /// <summary>
        /// Returns distinct set of vertices in the graph
        /// </summary>
        public IEnumerable<GraphVertex<T>> Vertices
        {
            get
            {
                return _edges.SelectMany(edge => new GraphVertex<T>[] { edge.Point1, edge.Point2 })
                             .DistinctBy(vertex => vertex.Reference)
                             .Actualize();
            }
        }

        public Graph()
        {
            _edges = new List<GraphEdge<T>>();
        }

        public Graph(IEnumerable<GraphEdge<T>> edges)
        {
            _edges = new List<GraphEdge<T>>(edges);
        }

        /// <summary>
        /// Returns a set of edges PER VERTEX - WHICH WILL DUPLICATE EDGES WHEN ITERATED. The point of this is to
        /// be able to select edges from the point of view of each vertex to work with the triangulation.
        /// </summary>
        public Dictionary<GraphVertex<T>, IEnumerable<GraphVertex<T>>> GetConnections()
        {
            var result = new Dictionary<GraphVertex<T>, IEnumerable<GraphVertex<T>>>();

            // Foreach distinct vertex
            foreach (var vertex in this.Vertices)
            {
                // Get all distinct vertices that connect to the vertex
                var connectingVertices = this.Edges.Where(edge => edge.Point1.Reference == vertex.Reference ||
                                                             edge.Point2.Reference == vertex.Reference)
                                                   .Select(edge =>
                                                   {
                                                       // Return opposing vertex
                                                       if (edge.Point1.Reference == vertex.Reference)
                                                           return edge.Point2;
                                                       else
                                                           return edge.Point1;
                                                   })
                                                   .Actualize();

                result.Add(vertex, connectingVertices);
            }

            return result;
        }
    }
}
