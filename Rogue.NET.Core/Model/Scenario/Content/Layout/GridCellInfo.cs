using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    /// <summary>
    /// Component used for creating the LevelGrid during the generation phase.
    /// </summary>
    public class GridCellInfo
    {
        public GridLocation Location { get; set; }
        public bool IsDoor { get; set; }
        public bool IsWall { get; set; }
        public bool IsWallLight { get; set; }
        public bool IsCorridor { get; set; }
        public bool IsMandatory { get; set; }
        public LayoutMandatoryLocationType MandatoryType { get; set; }
        public int DoorSearchCounter { get; set; }
        public Light BaseLight { get; set; }
        public Light WallLight { get; set; }

        public GridCellInfo(GridLocation location)
        {
            this.Location = location;
            this.BaseLight = new Light();
            this.WallLight = new Light();
        }
        public GridCellInfo(int column, int row)
        {
            this.Location = new GridLocation(column, row);
            this.BaseLight = new Light();
            this.WallLight = new Light();
        }
    }
}
