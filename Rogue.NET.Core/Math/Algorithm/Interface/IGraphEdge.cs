using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Math.Algorithm.Interface
{
    /// <summary>
    /// Interface for working with graph algorithms based on abstract graphs of connected nodes
    /// </summary>
    public interface IGraphEdge<T> where T : IGraphNode
    {
        T Node { get; }
        T AdjacentNode { get; }

        double Weight { get; }
    }
}
