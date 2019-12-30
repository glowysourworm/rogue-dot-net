using Rogue.NET.Common.Extension;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Math.Geometry
{
    [Serializable]
    public class GraphEdgeCollection : ISerializable
    {
        Dictionary<GraphEdge, GraphEdge> _edges;

        public void Add(GraphEdge edge)
        {
            if (_edges.Keys.Any(otherEdge =>
            {
                return (otherEdge.Point1 == edge.Point1 &&
                        otherEdge.Point2 == edge.Point2) ||
                       (otherEdge.Point1 == edge.Point2 &&
                        otherEdge.Point2 == edge.Point1);
            }))
                throw new Exception("Trying to add duplicate edge to graph");

            _edges.Add(edge, edge);
        }

        public void Remove(GraphEdge edge)
        {
            _edges.Remove(edge);
        }

        public IEnumerable<GraphEdge> Get()
        {
            return _edges.Keys;
        }

        public bool Contains(GraphEdge edge)
        {
            return _edges.ContainsKey(edge);
        }

        public void Filter(Func<GraphEdge, bool> predicate)
        {
            _edges.Filter(element => predicate(element.Key));
        }

        public GraphEdgeCollection()
        {
            _edges = new Dictionary<GraphEdge, GraphEdge>();
        }

        public GraphEdgeCollection(IEnumerable<GraphEdge> edges)
        {
            _edges = new Dictionary<GraphEdge, GraphEdge>();

            foreach (var edge in edges)
                _edges.Add(edge, edge);
        }

        public GraphEdgeCollection(SerializationInfo info, StreamingContext context)
        {
            _edges = (Dictionary<GraphEdge, GraphEdge>)info.GetValue("Edges", typeof(Dictionary<GraphEdge, GraphEdge>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Edges", _edges);
        }
    }
}
