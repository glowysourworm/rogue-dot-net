using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Construction
{
    /// <summary>
    /// Component used for creating the LevelGrid during the generation phase.
    /// </summary>
    public class GridCellInfo : IGridLocator
    {
        #region (public) IGridLocator
        public int Column
        {
            get { return this.Location?.Column ?? -1; }
        }

        public int Row
        {
            get { return this.Location?.Row ?? -1; }
        }
        #endregion

        public GridLocation Location { get; set; }
        public bool IsDoor { get; set; }
        public bool IsWall { get; set; }
        public bool IsWallLight { get; set; }
        public bool IsCorridor { get; set; }
        public int DoorSearchCounter { get; set; }
        public Light AmbientLight { get; set; }
        public Light WallLight { get; set; }
        public Light AccentLight { get; set; }
        public List<Light> TerrainLights { get; set; }

        public GridCellInfo(GridLocation location)
        {
            this.Location = location;
            this.AmbientLight = Light.White;
            this.AccentLight = Light.None;
            this.WallLight = Light.None;
            this.TerrainLights = new List<Light>();
        }
        public GridCellInfo(int column, int row)
        {
            this.Location = new GridLocation(column, row);
            this.AmbientLight = Light.White;
            this.AccentLight = Light.None;
            this.WallLight = Light.None;
            this.TerrainLights = new List<Light>();
        }

        public override string ToString()
        {
            return this.Location?.ToString() ?? "{}";
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new System.NotSupportedException("ISerializable not supported for GridCellInfo - See IGridLocator interface");
        }
    }
}
