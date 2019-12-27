using Rogue.NET.Core.Math.Geometry;

using System.Linq;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    public class ConnectedLayerInfo : LayerInfo
    {
        Graph<Region<GridLocation>> _regionGraph;

        public Graph<Region<GridLocation>> RegionGraph { get; private set; }

        public ConnectedLayerInfo(string layerName, Graph<Region<GridLocation>> regionGraph, bool isPassable)
                : base(layerName, regionGraph.Vertices
                                             .Select(vertex => vertex.Reference)
                                             .Distinct(), isPassable)
        {
            this.RegionGraph = regionGraph;
        }
    }
}
