using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    [Serializable]
    public class LayerMap : LayerMapBase, ILayerMap
    {
        /// <summary>
        /// SERIALZIATION ONLY
        /// </summary>
        public LayerMap() : base() { }

        public LayerMap(string layerName, IEnumerable<Region<GridLocation>> regions, int width, int height)
                : base(layerName, regions, width, height)
        {
        }
    }
}
