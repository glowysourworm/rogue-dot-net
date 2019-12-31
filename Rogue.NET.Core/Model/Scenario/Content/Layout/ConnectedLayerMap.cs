using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Geometry;

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
    public class ConnectedLayerMap : LayerMap, ISerializable
    {
        readonly Graph _graph;

        public IEnumerable<Region<GridLocation>> Connections(int column, int row)
        {
            var region = this[column, row];

            if (region == null)
                return new Region<GridLocation>[] { };

            return this.Regions.Where(otherRegion => region.GetConnectionIds().Contains(otherRegion.Id))
                               .Actualize();
        }

        public IEnumerable<Region<GridLocation>> Connections(string regionId)
        {
            var region = this.Regions.FirstOrDefault(region => region.Id == regionId);

            if (region == null)
                return new Region<GridLocation>[] { };

            return this.Regions.Where(otherRegion => region.GetConnectionIds().Contains(otherRegion.Id))
                               .Actualize();
        }

        public ConnectedLayerMap(string layerName,
                                 Graph graph,
                                 IEnumerable<Region<GridLocation>> regions,
                                 int width,
                                 int height)
                : base(layerName, regions, width, height)
        {
            _graph = graph;
        }

        public ConnectedLayerMap(SerializationInfo info, StreamingContext context)
                : base(info, context)
        {
            _graph = (Graph)info.GetValue("Graph", typeof(Graph));
        }


        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Graph", _graph);
        }
    }
}
