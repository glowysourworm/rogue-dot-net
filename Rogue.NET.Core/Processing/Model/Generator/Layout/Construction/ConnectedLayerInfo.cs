using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Construction
{
    public class ConnectedLayerInfo<T> : LayerInfo<T> where T : class, IGridLocator
    {
        public GraphInfo<T> RegionGraph { get; private set; }

        public ConnectedLayerInfo(string layerName, GraphInfo<T> regionGraph, IEnumerable<Region<T>> regions, bool isPassable)
                : base(layerName, regions, isPassable)
        {
            this.RegionGraph = regionGraph;
        }
    }
}
