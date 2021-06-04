using Rogue.NET.Common.Serialization.Component.Interface;
using Rogue.NET.Common.Serialization.Interface;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    [Serializable]
    public class TerrainLayerMap : LayerMapBase, ILayerMap, IRecursiveSerializable
    {
        public bool IsImpassable { get; private set; }

        public TerrainLayerMap(string layerName, IEnumerable<Region<GridLocation>> regions, int width, int height, bool isImpassable)
                : base(layerName, regions, width, height)
        {
            this.IsImpassable = isImpassable;
        }

        public TerrainLayerMap(IPropertyReader reader) : base(reader)
        {
            this.IsImpassable = reader.Read<bool>("IsImpassable");
        }

        public new void GetProperties(IPropertyWriter writer)
        {
            base.GetProperties(writer);

            writer.Write("IsImpassable", this.IsImpassable);
        }
    }
}
