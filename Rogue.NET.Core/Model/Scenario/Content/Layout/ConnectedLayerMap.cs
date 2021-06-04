using Rogue.NET.Common.Serialization.Component.Interface;
using Rogue.NET.Common.Serialization.Interface;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    /// <summary>
    /// Component for storing and mainintaining a 2D cell array for a layer of the level layout; and the 
    /// connections associated with adjacent rooms or regions. These must be pre-calculated as a graph.
    /// </summary>
    [Serializable]
    public class ConnectedLayerMap : LayerMapBase, IRecursiveSerializable, ILayerMap
    {
        public RegionGraph ConnectionGraph { get; private set; }

        public ConnectedLayerMap(string layerName,
                                 RegionGraph graph,
                                 IEnumerable<Region<GridLocation>> regions,
                                 int width,
                                 int height)
                : base(layerName, regions, width, height)
        {
            this.ConnectionGraph = graph;
        }

        public ConnectedLayerMap(IPropertyReader reader) : base(reader)
        {
            this.ConnectionGraph = reader.Read<RegionGraph>("ConnectionGraph");
        }

        public new void GetProperties(IPropertyWriter writer)
        {
            base.GetProperties(writer);

            writer.Write("ConnectionGraph", this.ConnectionGraph);
        }
    }
}
