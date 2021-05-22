using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Construction;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    /// <summary>
    /// Component for storing and mainintaining a 2D cell array for a layer of the level layout; and the 
    /// connections associated with adjacent rooms or regions. These must be pre-calculated as a graph.
    /// </summary>
    [Serializable]
    public class ConnectedLayerMap : LayerMapBase, ISerializable, ILayerMap
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

        public ConnectedLayerMap(SerializationInfo info, StreamingContext context)
                : base(info, context)
        {
            this.ConnectionGraph = (RegionGraph)info.GetValue("ConnectionGraph", typeof(RegionGraph));
        }


        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("ConnectionGraph", this.ConnectionGraph);
        }
    }
}
