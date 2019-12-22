using Rogue.NET.Common.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Math.Geometry
{
    public class GraphEdgeCollection<T> where T : class
    {
        Dictionary<GraphEdge<T>, GraphEdge<T>> _edges;

        public void Add(GraphEdge<T> edge)
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

        public void Remove(GraphEdge<T> edge)
        {
            _edges.Remove(edge);
        }

        public IEnumerable<GraphEdge<T>> Get()
        {
            return _edges.Keys;
        }

        public bool Contains(GraphEdge<T> edge)
        {
            return _edges.ContainsKey(edge);
        }

        public void Filter(Func<GraphEdge<T>, bool> predicate)
        {
            _edges.Filter(element => predicate(element.Key));
        }

        public GraphEdgeCollection()
        {
            _edges = new Dictionary<GraphEdge<T>, GraphEdge<T>>();
        }

        public GraphEdgeCollection(IEnumerable<GraphEdge<T>> edges)
        {
            _edges = new Dictionary<GraphEdge<T>, GraphEdge<T>>();

            foreach (var edge in edges)
                _edges.Add(edge, edge);
        }
    }
}
