using Rogue.NET.Core.Math.Algorithm.Interface;

using System.Collections.Generic;

namespace Rogue.NET.Core.Math.Geometry
{
    public class Graph : IGraph<GraphVertex, GraphEdge>
    {
        // Primary graph collection
        GraphEdgeCollection<GraphVertex, GraphEdge> _edges;

        public IEnumerable<GraphVertex> Vertices
        {
            get { return _edges.GetNodes(); }
        }

        public IEnumerable<GraphEdge> Edges
        {
            get { return _edges.GetEdges(); }
        }

        public Graph()
        {
            _edges = new GraphEdgeCollection<GraphVertex, GraphEdge>(false);
        }

        public Graph(IEnumerable<GraphEdge> edges)
        {
            _edges = new GraphEdgeCollection<GraphVertex, GraphEdge>(edges, false);
        }

        public void AddEdge(GraphEdge edge)
        {
            // Detects duplicate edges
            _edges.Add(edge);
        }

        /// <summary>
        /// Removes an existing edge in favor of a new one
        /// </summary>
        public void Modify(GraphEdge existingEdge, GraphEdge newEdge)
        {
            _edges.Remove(existingEdge);
            _edges.Add(newEdge);
        }

        public IEnumerable<GraphEdge> GetAdjacentEdges(GraphVertex node)
        {
            return _edges.GetAdjacentEdges(node);
        }

        public GraphEdge FindEdge(GraphVertex node1, GraphVertex node2)
        {
            return _edges.Find(node1, node2);
        }
    }
}
