using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Processing.Model.Extension;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Math.Geometry
{
    [Serializable]
    public class Graph : ISerializable
    {
        // Maintain edges by vertex
        Dictionary<GraphVertex, GraphEdgeCollection> _vertexDict;

        // Also, maintain edges separately
        GraphEdgeCollection _edges;

        /// <summary>
        /// Returns distinct edges in the graph
        /// </summary>
        public IEnumerable<GraphEdge> Edges
        {
            get { return _edges.Get(); }
        }

        /// <summary>
        /// Returns distinct set of vertices in the graph
        /// </summary>
        public IEnumerable<GraphVertex> Vertices
        {
            get { return _vertexDict.Keys; }
        }

        public Graph()
        {
            _vertexDict = new Dictionary<GraphVertex, GraphEdgeCollection>();
            _edges = new GraphEdgeCollection();
        }

        public Graph(IEnumerable<GraphEdge> edges)
        {
            _vertexDict = new Dictionary<GraphVertex, GraphEdgeCollection>();
            _edges = new GraphEdgeCollection();

            // Add all edges to the graph
            foreach (var edge in edges)
                AddEdge(edge);
        }

        public Graph(SerializationInfo info, StreamingContext context)
        {
            _vertexDict = (Dictionary<GraphVertex, GraphEdgeCollection>)info.GetValue("Vertices", typeof(Dictionary<GraphVertex, GraphEdgeCollection>));
            _edges = (GraphEdgeCollection)info.GetValue("Edges", typeof(GraphEdgeCollection));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Vertices", _vertexDict);
            info.AddValue("Edges", _edges);
        }

        public void AddVertex(GraphVertex vertex)
        {
            _vertexDict.Add(vertex, new GraphEdgeCollection());
        }

        public void AddEdge(GraphEdge edge)
        {
            if (edge.Point1.Column == edge.Point2.Column &&
                edge.Point1.Row == edge.Point2.Row)
                throw new ArgumentException("Invalid graph edge - starts and ends at the same location");

            // Detects duplicate edges
            _edges.Add(edge);

            // Add Point1
            if (!_vertexDict.ContainsKey(edge.Point1))
                _vertexDict.Add(edge.Point1, new GraphEdgeCollection(new GraphEdge[] { edge }));

            else
                _vertexDict[edge.Point1].Add(edge);

            // Add Point2
            if (!_vertexDict.ContainsKey(edge.Point2))
                _vertexDict.Add(edge.Point2, new GraphEdgeCollection(new GraphEdge[] { edge }));

            else
                _vertexDict[edge.Point2].Add(edge);
        }

        /// <summary>
        /// O(1) Vertex lookup
        /// </summary>
        public bool Contains(GraphVertex vertex)
        {
            return _vertexDict.ContainsKey(vertex);
        }

        public IEnumerable<GraphVertex> Find(string referenceId)
        {
            return _vertexDict.Keys.Where(key => key.ReferenceId == referenceId);
        }

        public IEnumerable<GraphEdge> FindEdges(string reference1Id, string reference2Id)
        {
            return _edges.Get()
                         .Where(edge => (edge.Point1.ReferenceId == reference1Id &&
                                         edge.Point2.ReferenceId == reference2Id) ||
                                        (edge.Point1.ReferenceId == reference2Id &&
                                         edge.Point2.ReferenceId == reference1Id))
                         .Actualize();
        }

        public void Remove(GraphEdge edge)
        {
            foreach (var key in _vertexDict.Keys)
            {
                if (_vertexDict[key].Contains(edge))
                    _vertexDict[key].Remove(edge);
            }

            _edges.Remove(edge);
        }

        /// <summary>
        /// Removes edges according to the supplied predicate
        /// </summary>
        public void FilterByEdge(Func<GraphEdge, bool> predicate)
        {
            _edges.Filter(predicate);
            _vertexDict.ForEach(element => element.Value.Filter(predicate));
        }

        public void FilterByVertex(Func<GraphVertex, bool> predicate)
        {
            // Remove vertices with edge collections
            var elements = _vertexDict.Filter(element => predicate(element.Key));

            // Remove edges with any of the filtered points
            _edges.Filter(edge => elements.ContainsKey(edge.Point1) ||
                                  elements.ContainsKey(edge.Point2));
        }

        /// <summary>
        /// Returns a set of connections for the vertex. The point of this is to be able to select edges from 
        /// the point of view of each vertex to work with triangulation.
        /// </summary>
        public IEnumerable<GraphEdge> this[GraphVertex vertex]
        {
            get { return _vertexDict[vertex].Get(); }
        }
    }
}
