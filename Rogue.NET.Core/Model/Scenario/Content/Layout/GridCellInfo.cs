using System.Windows.Media;

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
        public bool IsCorridor { get; set; }
        public int DoorSearchCounter { get; set; }
        public Color Lighting { get; set; }

        public GridCellInfo(GridLocation location)
        {
            this.Location = location;
        }
        public GridCellInfo(int column, int row)
        {
            this.Location = new GridLocation(column, row);
        }
    }
}
