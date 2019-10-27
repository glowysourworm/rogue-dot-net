using System;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    [Serializable]
    public class Cell : ISerializable
    {
        #region (public) Volatile Properties (Change during game play)
        public bool IsExplored { get; set; }
        public bool IsRevealed { get; set; }
        #endregion

        /// <summary>
        /// TODO:TERRAIN  (Remove these)
        /// </summary>
        public double TerrainValue { get; set; }
        public double DijkstraWeight { get; set; }

        #region (public) Properties
        public GridLocation Location { get; set; }
        public bool IsDoor { get; private set; }
        public bool IsWall { get; private set; }
        public int DoorSearchCounter { get; private set; }
        #endregion

        public void Search()
        {
            this.DoorSearchCounter--;

            if (this.DoorSearchCounter < 0)
                this.DoorSearchCounter = 0;
        }

        /// <summary>
        /// USED DURING GENERATION. Sets this cell to be a door cell - along with all related parameters. Adds door
        /// to already existing doors for this cell.
        /// </summary>
        public void SetDoor(int searchCounter)
        {
            this.IsDoor = true;
            this.DoorSearchCounter = searchCounter;
        }

        /// <summary>
        /// USED DURING GENERATION. Sets this cell to be a wall cell - along with all related parameters. Removes
        /// door cell parameters if they exist.
        /// </summary>
        /// <param name="isWall"></param>
        public void SetWall(bool isWall)
        {
            this.IsWall = isWall;
            this.IsDoor = false;
            this.DoorSearchCounter = 0;
        }

        public Cell()
        {
            this.Location = new GridLocation(-1, -1);
            this.IsExplored = false;
            this.IsRevealed = false;
        }
        public Cell(GridLocation location, bool isWall)
        {
            this.Location = new GridLocation(location);
            this.IsWall = isWall;
            this.IsExplored = false;
            this.IsRevealed = false;
        }
        public Cell(int column, int row, bool isWall)
        {
            this.Location = new GridLocation(column, row);
            this.IsWall = isWall;
            this.IsExplored = false;
            this.IsRevealed = false;
        }
        public override string ToString()
        {
            return this.Location == null ? "" : this.Location.ToString();
        }

        #region ISerializable - Used Custom Serialization to minimize footprint
        public Cell(SerializationInfo info, StreamingContext context)
        {
            this.IsExplored = info.GetBoolean("IsExplored");
            this.IsRevealed = info.GetBoolean("IsRevealed");
            this.IsDoor = info.GetBoolean("IsDoor");
            this.IsWall = info.GetBoolean("IsWall");
            this.Location = (GridLocation)info.GetValue("Location", typeof(GridLocation));
            this.DoorSearchCounter = info.GetInt32("DoorSearchCounter");
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("IsExplored", this.IsExplored);
            info.AddValue("IsRevealed", this.IsRevealed);
            info.AddValue("IsDoor", this.IsDoor);
            info.AddValue("IsWall", this.IsWall);
            info.AddValue("Location", this.Location);
            info.AddValue("DoorSearchCounter", this.DoorSearchCounter);
        }
        #endregion
    }
}
