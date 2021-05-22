using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Construction
{
    public class ConnectedLayerInfo<T> : LayerInfo<T> where T : class, IGridLocator
    {
        public RegionGraph RegionGraph { get; private set; }

        public ConnectedLayerInfo(string layerName, RegionGraph regionGraph, IEnumerable<Region<T>> regions, bool isPassable)
                : base(layerName, regions, isPassable)
        {
            this.RegionGraph = regionGraph;
        }
    }
}
