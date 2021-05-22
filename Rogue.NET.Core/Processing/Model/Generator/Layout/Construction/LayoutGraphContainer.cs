using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Construction
{
    /// <summary>
    /// Container for the two graphs associated with the region connections
    /// </summary>
    public class LayoutGraphContainer<T> where T : class, IGridLocator
    {
        /// <summary>
        /// Graph of connections between room regions - has to be updated when regions are re-generated
        /// </summary>
        public RegionGraphInfo<T> ConnectionGraph { get; private set; }

        /// <summary>
        /// Graph of connections between all region connection points 
        /// </summary>
        public Graph VertexGraph { get; private set; }

        public LayoutGraphContainer(RegionGraphInfo<T> connectionGraph, Graph vertexGraph)
        {
            Set(connectionGraph, vertexGraph);
        }

        public void Set(RegionGraphInfo<T> connectionGraph, Graph vertexGraph)
        {
            this.ConnectionGraph = connectionGraph;
            this.VertexGraph = vertexGraph;
        }
    }
}
