using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    [Serializable]
    public class TerrainLayerMap : LayerMapBase, ILayerMap
    {
        public bool IsImpassable { get; private set; }

        public TerrainLayerMap(string layerName, IEnumerable<Region<GridLocation>> regions, int width, int height, bool isImpassable)
                : base(layerName, regions, width, height)
        {
            this.IsImpassable = isImpassable;
        }

        public TerrainLayerMap(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            var isImpassable = info.GetBoolean("IsImpassable");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("IsImpassable", this.IsImpassable);
        }
    }
}
