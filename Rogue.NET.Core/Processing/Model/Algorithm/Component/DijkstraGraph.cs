using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Algorithm;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Math.Geometry
{
    /// <summary>
    /// Component for storing pre-calculated data about a graph:  Traversal routes and distances
    /// </summary>
    [Serializable]
    public class DijkstraGraph : ISerializable
    {
        readonly Graph _graph;
        readonly List<GraphTraversal> _traversals;

        /// <summary>
        /// Returns traversal list for the graph
        /// </summary>
        public List<GraphTraversal> Traversals { get { return _traversals; } }

        /// <summary>
        /// Returns adjacent connections for the specified reference (vertex) Id
        /// </summary>
        public IEnumerable<string> GetConnections(string referencId)
        {
            return _graph.Find(referencId)
                         .Select(vertex => vertex.ReferenceId)
                         .Actualize();
        }

        public DijkstraGraph(Graph graph)
        {
            _graph = graph;
            _traversals = new List<GraphTraversal>();

            foreach (var vertex1 in graph.Vertices)
            {
                foreach (var vertex2 in graph.Vertices)
                {
                    if (vertex1 != vertex2 &&
                        !_traversals.Any(traversal => traversal.Source.Equals(vertex1)))
                    {
                        var traversal = DijkstrasAlgorithm.Run(graph, vertex1, vertex2);

                        if (traversal != null)
                            _traversals.Add(traversal);

                        else
                            throw new Exception("Trying to calculate a graph traversal on an un-connected graph");
                    }
                }
            }
        }

        public DijkstraGraph(SerializationInfo info, StreamingContext context)
        {
            _traversals = (List<GraphTraversal>)info.GetValue("Traversals", typeof(List<GraphTraversal>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Traversals", _traversals);
        }
    }
}
