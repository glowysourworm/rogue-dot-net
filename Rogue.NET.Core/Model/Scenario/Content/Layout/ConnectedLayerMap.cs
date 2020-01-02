using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
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
    public class ConnectedLayerMap : LayerMapBase<GridLocation, ConnectedRegion<GridLocation>>, ISerializable, ILayerMap
    {
        readonly Graph _graph;

        public new IEnumerable<Region<GridLocation>> Regions
        {
            get { return this.Regions.Cast<Region<GridLocation>>(); }
        }

        public new Region<GridLocation> this[int column, int row]
        {
            get { return this[column, row] as Region<GridLocation>; }
        }

        public new Region<GridLocation> this[IGridLocator location]
        {
            get { return this[location] as Region<GridLocation>; }
        }

        public IEnumerable<ConnectedRegion<GridLocation>> Connections(int column, int row)
        {
            var region = this[column, row] as ConnectedRegion<GridLocation>;

            if (region == null)
                return new ConnectedRegion<GridLocation>[] { };

            return this.Regions
                       .Cast<ConnectedRegion<GridLocation>>()
                       .Where(otherRegion => region.Connections.ContainsKey(otherRegion.Id))
                       .Actualize();
        }

        public IEnumerable<ConnectedRegion<GridLocation>> Connections(string regionId)
        {
            var region = this.Regions.FirstOrDefault(region => region.Id == regionId) as ConnectedRegion<GridLocation>;

            if (region == null)
                return new ConnectedRegion<GridLocation>[] { };

            return this.Regions
                       .Cast<ConnectedRegion<GridLocation>>()
                       .Where(otherRegion => region.Connections.ContainsKey(otherRegion.Id))
                       .Actualize();
        }

        public ConnectedLayerMap(string layerName,
                                 Graph graph,
                                 IEnumerable<ConnectedRegion<GridLocation>> regions,
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
