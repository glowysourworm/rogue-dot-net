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

        /// <summary>
        /// SERIAILZATION ONLY
        /// </summary>
        public TerrainLayerMap() : base() { }

        public TerrainLayerMap(string layerName, IEnumerable<Region<GridLocation>> regions, int width, int height, bool isImpassable)
                : base(layerName, regions, width, height)
        {
            this.IsImpassable = isImpassable;
        }


        public new void GetPropertyDefinitions(IPropertyPlanner planner)
        {
            base.GetPropertyDefinitions(planner);

            planner.Define<bool>("IsImpassable");
        }

        public new void GetProperties(IPropertyWriter writer)
        {
            base.GetProperties(writer);

            writer.Write("IsImpassable", this.IsImpassable);
        }

        public new void SetProperties(IPropertyReader reader)
        {
            base.SetProperties(reader);

            this.IsImpassable = reader.Read<bool>("IsImpassable");
        }
    }
}
