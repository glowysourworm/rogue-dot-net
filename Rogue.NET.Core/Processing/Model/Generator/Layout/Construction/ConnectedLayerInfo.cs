using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Construction
{
    public class ConnectedLayerInfo<T> : LayerInfo<T> where T : class, IGridLocator
    {
        public Graph RegionGraph { get; private set; }

        public IEnumerable<ConnectedRegion<T>> ConnectionRegions { get; private set; }

        public override IEnumerable<Region<T>> Regions
        {
            get { throw new Exception("Connection layer should use ConnectionRegions ConnectedLayerInfo<T>"); }
        }

        public ConnectedLayerInfo(string layerName, Graph regionGraph, IEnumerable<ConnectedRegion<T>> regions, bool isPassable)
                : base(layerName, regions, isPassable)
        {
            this.RegionGraph = regionGraph;
            this.ConnectionRegions = regions;
        }
    }
}
