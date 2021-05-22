using Rogue.NET.Core.Math.Geometry;

using System.Collections.Generic;

namespace Rogue.NET.Core.Math.Algorithm.Interface
{
    /// <summary>
    /// Interface to implement an abstract graph - works with graph algorithms
    /// </summary>
    public interface IGraph<TNode, TEdge> where TNode : IGraphNode
                                          where TEdge : IGraphEdge<TNode>
    {
        /// <summary>
        /// Fetches all vertices for the graph
        /// </summary>
        IEnumerable<TNode> Vertices { get; }

        /// <summary>
        /// Fetches adjacent edges for a given node (SHOULD BE OPTIMIZED FOR PERFORMANCE!)
        /// </summary>
        IEnumerable<TEdge> GetAdjacentEdges(TNode node);

        /// <summary>
        /// Finds edge using the graph's directed edge rule
        /// </summary>
        TEdge FindEdge(TNode node1, TNode node2);
    }
}
