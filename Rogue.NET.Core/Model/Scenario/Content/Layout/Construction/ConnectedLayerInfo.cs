using Rogue.NET.Core.Math.Geometry;

using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout.Construction
{
    public class ConnectedLayerInfo : LayerInfo
    {
        public Graph RegionGraph { get; private set; }

        public ConnectedLayerInfo(string layerName, Graph regionGraph, IEnumerable<Region<GridLocation>> regions, bool isPassable)
                : base(layerName, regions, isPassable)
        {
            // this.RegionGraph = new DijkstraGraph(regionGraph);

            this.RegionGraph = regionGraph;
        }
    }
}
