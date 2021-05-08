using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Construction
{
    /// <summary>
    /// Simple graph info class to store connection points along with the graph data
    /// </summary>
    public class GraphInfo<T> : Graph where T : class, IGridLocator
    {
        public IEnumerable<RegionConnectionInfo<T>> Connections { get; private set; }

        /// <summary>
        /// Returns connection(s) for these thwo referenced regions
        /// </summary>
        public IEnumerable<RegionConnectionInfo<T>> GetConnections(string referenceId1, string referenceId2)
        {
            return this.Connections.Where(connection => (connection.Vertex.ReferenceId == referenceId1 &&
                                                         connection.AdjacentVertex.ReferenceId == referenceId2) ||
                                                        (connection.Vertex.ReferenceId == referenceId2 &&
                                                         connection.AdjacentVertex.ReferenceId == referenceId1));
        }

        public GraphInfo(IEnumerable<RegionConnectionInfo<T>> connections) : base()
        {
            this.Connections = connections;
        }

        public GraphInfo(IEnumerable<RegionConnectionInfo<T>> connections, IEnumerable<GraphEdge> edges) : base(edges)
        {
            this.Connections = connections;
        }
    }
}
