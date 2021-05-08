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
    public class ConnectedLayerMap : LayerMapBase<GridLocation, Region<GridLocation>>, ISerializable, ILayerMap
    {
        private IDictionary<string, IEnumerable<RegionConnection<GridLocation>>> _connections;

        public IEnumerable<RegionConnection<GridLocation>> Connections(int column, int row)
        {
            if (this[column, row] == null)
                return new RegionConnection<GridLocation>[] { };

            return Connections(this[column, row].Id);
        }

        public IEnumerable<RegionConnection<GridLocation>> Connections(string regionId)
        {
            return _connections[regionId];
        }

        public ConnectedLayerMap(string layerName,
                                 GraphInfo<GridLocation> graph,
                                 IEnumerable<Region<GridLocation>> regions,
                                 int width,
                                 int height)
                : base(layerName, regions, width, height)
        {
            _connections = regions.ToDictionary(region => region.Id,
                                                region =>
                                                {
                                                    var connections = graph.Connections.Where(connection => connection.Vertex.ReferenceId == region.Id);

                                                    return connections.Select(connection => new RegionConnection<GridLocation>()
                                                    {
                                                        Location = connection.Location,
                                                        AdjacentLocation = connection.AdjacentLocation,
                                                        AdjacentRegionId = connection.AdjacentVertex.ReferenceId
                                                    });
                                                });
        }

        public ConnectedLayerMap(SerializationInfo info, StreamingContext context)
                : base(info, context)
        {
            _connections = (Dictionary<string, IEnumerable<RegionConnection<GridLocation>>>)info.GetValue("Connections", typeof(Dictionary<string, List<RegionConnection<GridLocation>>>));
        }


        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Connections", _connections);
        }
    }
}
