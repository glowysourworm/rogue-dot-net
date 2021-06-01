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

        /// <summary>
        /// SERIALIZATION ONLY
        /// </summary>
        public ConnectedLayerMap() : base() { }

        public ConnectedLayerMap(string layerName,
                                 RegionGraph graph,
                                 IEnumerable<Region<GridLocation>> regions,
                                 int width,
                                 int height)
                : base(layerName, regions, width, height)
        {
            this.ConnectionGraph = graph;
        }

        public new void GetPropertyDefinitions(IPropertyPlanner planner)
        {
            base.GetPropertyDefinitions(planner);

            planner.Define<RegionGraph>("ConnectionGraph");
        }

        public new void GetProperties(IPropertyWriter writer)
        {
            base.GetProperties(writer);

            writer.Write("ConnectionGraph", this.ConnectionGraph);
        }

        public new void SetProperties(IPropertyReader reader)
        {
            base.SetProperties(reader);

            this.ConnectionGraph = reader.Read<RegionGraph>("ConnectionGraph");
        }
    }
}
