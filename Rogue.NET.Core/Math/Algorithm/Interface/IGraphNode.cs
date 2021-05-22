using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Math.Algorithm.Interface
{
    /// <summary>
    /// Interface to expose a unique hash for the graph node
    /// </summary>
    public interface IGraphNode
    {
        int Hash { get; }
    }
}
