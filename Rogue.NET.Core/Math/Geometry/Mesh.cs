using Rogue.NET.Common.Extension;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Math.Geometry
{
    public class Mesh
    {
        List<Triangle> _triangles;

        public IEnumerable<Triangle> Triangles
        {
            get { return _triangles; }
        }

        public Mesh()
        {
            _triangles = new List<Triangle>();
        }

        public void AddTriangle(Triangle triangle)
        {
            _triangles.Add(triangle);
        }
        public void RemoveTriangle(Triangle triangle)
        {
            _triangles.Remove(triangle);
        }

        /// <summary>
        /// Returns set of distinct edges in the triangulation
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Edge> GetEdges()
        {
            return _triangles.SelectMany(triangle => triangle.Edges).Distinct();
        }

        /// <summary>
        /// Returns distinct set of vertices in the triangulation
        /// </summary>
        public IEnumerable<Vertex> GetVertices()
        {
            return _triangles.SelectMany(triangle => triangle.Vertices).Distinct();
        }

        /// <summary>
        /// Returns a set of edges PER VERTEX - WHICH WILL DUPLICATE EDGES WHEN ITERATED. The point of this is to
        /// be able to select edges from the point of view of each vertex (can create minimum spanning tree)
        /// </summary>
        public Dictionary<Vertex, IEnumerable<Edge>> GetConnections()
        {
            // Computes Distinct-By-Value vertex collection
            var vertices = _triangles.SelectMany(triangle => triangle.Vertices)
                                     .Distinct()
                                     .Actualize();

            var result = new Dictionary<Vertex, IEnumerable<Edge>>();

            // Foreach distinct vertex
            foreach (var vertex in vertices)
            {
                // Get all distinct edges that contain the vertex (by value)
                var edges = _triangles.SelectMany(triangle => triangle.Edges)
                                      .Where(edge => edge.Point1 == vertex || edge.Point2 == vertex)
                                      .Distinct()
                                      .Actualize();

                result.Add(vertex, edges);
            }

            return result;
        }
    }
}
