using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    [Serializable]
    public class LayerMap : LayerMapBase<GridLocation, Region<GridLocation>>, ILayerMap
    {
        public LayerMap(string layerName, IEnumerable<Region<GridLocation>> regions, int width, int height)
                : base(layerName, regions, width, height)
        {
        }

        public LayerMap(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
