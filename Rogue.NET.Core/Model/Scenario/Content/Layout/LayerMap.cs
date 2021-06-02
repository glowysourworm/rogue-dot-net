using Rogue.NET.Common.Serialization.Interface;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    [Serializable]
    public class LayerMap : LayerMapBase, ILayerMap, IRecursiveSerializable
    {
        public LayerMap(string layerName, IEnumerable<Region<GridLocation>> regions, int width, int height)
                : base(layerName, regions, width, height)
        {
        }

        public LayerMap(IPropertyReader reader) : base(reader) { }

        public new void GetProperties(IPropertyWriter writer)
        {
            base.GetProperties(writer);
        }
    }
}
