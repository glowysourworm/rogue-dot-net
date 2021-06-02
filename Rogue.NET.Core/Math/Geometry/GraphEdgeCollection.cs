using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Serialization.Interface;
using Rogue.NET.Core.Math.Algorithm.Interface;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Math.Geometry
{
    /// <summary>
    /// This should be the primary collection for creating an IGraph. Gives fast lookup of edges and nodes. Also, supports
    /// directed (or) un-directed graph lookup and duplicate edge validation.
    /// </summary>
    [Serializable]
    public class GraphEdgeCollection<TNode, TEdge> : IRecursiveSerializable where TNode : IGraphNode
                                                                            where TEdge : IGraphEdge<TNode>
    {
        Dictionary<SpatialIndex, TEdge> _edges;
        Dictionary<TNode, Dictionary<SpatialIndex, TEdge>> _nodeEdges;
        bool _directedGraph;

        /// <summary>
        /// Supports indexing of directed and non-directed graphs for equality comparison and hashing
        /// </summary>
        [Serializable]
        public struct SpatialIndex
        {
            public int Node1Hash { get; private set; }
            public int Node2Hash { get; private set; }
            public bool UseOrdering { get; private set; }

            public SpatialIndex(TEdge edge, bool useOrdering)
            {
                if (edge.Node.Hash == edge.AdjacentNode.Hash)
                    throw new Exception("SpatialIndex hash collision:  GraphEdgeCollection.cs");

                this.Node1Hash = edge.Node.Hash;
                this.Node2Hash = edge.AdjacentNode.Hash;
                this.UseOrdering = useOrdering;
            }

            public SpatialIndex(TNode node1, TNode node2, bool useOrdering)
            {
                if (node1.Hash == node2.Hash)
                    throw new Exception("SpatialIndex hash collision:  GraphEdgeCollection.cs");

                this.Node1Hash = node1.Hash;
                this.Node2Hash = node2.Hash;
                this.UseOrdering = useOrdering;
            }

            public bool IsEmpty()
            {
                return this.Node1Hash == default(int) &&
                       this.Node2Hash == default(int) &&
                       this.UseOrdering == default(bool);
            }

            public override bool Equals(object obj)
            {
                var index = (SpatialIndex)obj;

                if (!index.IsEmpty())
                {
                    if (index.UseOrdering != this.UseOrdering)
                        return false;

                    if (this.UseOrdering)
                    {
                        return index.Node1Hash == this.Node1Hash &&
                               index.Node2Hash == this.Node2Hash;
                    }

                    else
                    {
                        return (index.Node1Hash == this.Node1Hash &&
                                index.Node2Hash == this.Node2Hash) ||
                               (index.Node1Hash == this.Node2Hash &&
                                index.Node2Hash == this.Node1Hash);
                    }
                }

                return false;
            }

            // APPLY ORDERING TO SPATIAL INDEX
            public override int GetHashCode()
            {
                // DIRECTED GRAPH
                if (this.UseOrdering)
                    return this.CreateHashCode(this.Node1Hash, this.Node2Hash);

                // UN-DIRECTED GRAPH
                else
                {
                    return this.CreateHashCode(System.Math.Min(this.Node1Hash, this.Node2Hash),
                                               System.Math.Max(this.Node1Hash, this.Node2Hash));
                }
            }

            public override string ToString()
            {
                return string.Format("Hash({0})", this.GetHashCode());
            }
        }

        public int Count { get { return _edges.Count; } }

        public void Add(TEdge edge)
        {
            if (edge.Node.Hash == edge.AdjacentNode.Hash)
                throw new Exception("Trying to add self-referencing edge to a graph:  GraphEdgeCollection.Add");

            _edges.Add(new SpatialIndex(edge.Node, edge.AdjacentNode, _directedGraph), edge);

            // Forward entry
            if (_nodeEdges.ContainsKey(edge.Node))
                _nodeEdges[edge.Node].Add(new SpatialIndex(edge, _directedGraph), edge);

            else
                _nodeEdges.Add(edge.Node, new Dictionary<SpatialIndex, TEdge>() { { new SpatialIndex(edge, _directedGraph), edge } });

            // Reverse entry - will not fail spatial index
            if (_nodeEdges.ContainsKey(edge.AdjacentNode))
                _nodeEdges[edge.AdjacentNode].Add(new SpatialIndex(edge, _directedGraph), edge);

            else
                _nodeEdges.Add(edge.AdjacentNode, new Dictionary<SpatialIndex, TEdge>() { { new SpatialIndex(edge, _directedGraph), edge } });
        }

        public void Remove(TEdge edge)
        {
            var index = new SpatialIndex(edge, _directedGraph);

            _edges.Remove(index);


            // Forward entry
            if (_nodeEdges.ContainsKey(edge.Node))
            {
                if (_nodeEdges[edge.Node].ContainsKey(index))
                    _nodeEdges[edge.Node].Remove(index);
            }

            // Reverse entry - will not fail spatial index
            if (_nodeEdges.ContainsKey(edge.AdjacentNode))
            {
                if (_nodeEdges[edge.AdjacentNode].ContainsKey(index))
                    _nodeEdges[edge.AdjacentNode].Remove(index);
            }
        }

        public bool Contains(TEdge edge)
        {
            return _edges.ContainsKey(new SpatialIndex(edge, _directedGraph));
        }

        /// <summary>
        /// Tries to locate an edge with the given nodes. Throws an exception if one can't be located
        /// </summary>
        public TEdge Find(TNode node1, TNode node2)
        {
            return _edges[new SpatialIndex(node1, node2, _directedGraph)];
        }

        public bool HasEdge(TNode node1, TNode node2)
        {
            return _edges.ContainsKey(new SpatialIndex(node1, node2, _directedGraph));
        }

        public IEnumerable<TEdge> GetAdjacentEdges(TNode node)
        {
            return _nodeEdges[node].Values;
        }

        public IEnumerable<TEdge> GetEdges()
        {
            return _edges.Values;
        }

        public IEnumerable<TNode> GetNodes()
        {
            return _nodeEdges.Keys;
        }

        public GraphEdgeCollection(bool directedGraph)
        {
            Initialize(null, directedGraph);
        }

        public GraphEdgeCollection(IEnumerable<TEdge> edges, bool directedGraph)
        {
            Initialize(edges, directedGraph);
        }
        public GraphEdgeCollection(IPropertyReader reader)
        {
            var edges = reader.Read<List<TEdge>>("Edges");
            var directedGraph = reader.Read<bool>("DirectedGraph");

            Initialize(edges, directedGraph);
        }
        public void GetProperties(IPropertyWriter writer)
        {
            writer.Write("Edges", _edges.Values.ToList());
            writer.Write("DirectedGraph", _directedGraph);
        }

        private void Initialize(IEnumerable<TEdge> edges, bool directedGraph)
        {
            _edges = new Dictionary<SpatialIndex, TEdge>();
            _nodeEdges = new Dictionary<TNode, Dictionary<SpatialIndex, TEdge>>();
            _directedGraph = directedGraph;

            // Initialize for edges and adjacent vertex lookup
            if (edges != null)
            {
                foreach (var edge in edges)
                    Add(edge);
            }
        }


    }
}
