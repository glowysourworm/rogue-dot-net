using Rogue.NET.Core.Model.Enums;
using System;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    [Serializable]
    public class Cell : ISerializable
    {
        // Need a field to identify doors even if they've been toggled off
        bool _isDoor;
        int _northDoorSearchCounter;
        int _southDoorSearchCounter;
        int _eastDoorSearchCounter;
        int _westDoorSearchCounter;

        #region (public) Volatile Properties (Change during game play)
        /// <summary>
        /// Set to "True" if Cell is also marked "IsLineOfSight"; and it's within the light radius of the
        /// Player. 
        /// </summary>
        public bool IsPhysicallyVisible { get; set; }
        /// <summary>
        /// Set to "True" if Cell is directly in the path from a ray originating from the Player. It will not be
        /// visible unless it's within the player's light radius. (Or, TBD - the light radius of another Doodad.. or potentially
        /// another character)
        /// </summary>
        public bool IsLineOfSight { get; set; }
        public bool IsExplored { get; set; }
        public bool IsRevealed { get; set; }
        #endregion

        #region (public) Identity Properties (IsDoor, IsInRoom, RoomId, etc...)
        public bool IsDoor
        {
            get { return _isDoor; }
        }
        #endregion

        #region (public) Properties
        public Compass Walls { get; set; }
        public Compass Doors { get; set; }
        public Compass VisibleDoors
        {
            get
            {
                var result = Compass.Null;
                if (this.NorthDoorSearchCounter <= 0 && ((this.Doors & Compass.N) != 0))
                    result |= Compass.N;

                if (this.SouthDoorSearchCounter <= 0 && ((this.Doors & Compass.S) != 0))
                    result |= Compass.S;

                if (this.EastDoorSearchCounter <= 0 && ((this.Doors & Compass.E) != 0))
                    result |= Compass.E;

                if (this.WestDoorSearchCounter <= 0 && ((this.Doors & Compass.W) != 0))
                    result |= Compass.W;

                return result;
            }
        }
        public Compass InVisibleDoors
        {
            get
            {
                var result = Compass.Null;
                if (this.NorthDoorSearchCounter > 0 && ((this.Doors & Compass.N) != 0))
                    result |= Compass.N;

                if (this.SouthDoorSearchCounter > 0 && ((this.Doors & Compass.S) != 0))
                    result |= Compass.S;

                if (this.EastDoorSearchCounter > 0 && ((this.Doors & Compass.E) != 0))
                    result |= Compass.E;

                if (this.WestDoorSearchCounter > 0 && ((this.Doors & Compass.W) != 0))
                    result |= Compass.W;

                return result;
            }
        }
        public CellPoint Location { get; set; }
        #endregion

        public void OpenDoor(Compass direction)
        {
            switch (direction)
            {
                case Compass.N:
                    if (this.NorthDoorSearchCounter <= 0)
                        this.Doors &= ~Compass.N;
                    break;
                case Compass.S:
                    if (this.SouthDoorSearchCounter <= 0)
                        this.Doors &= ~Compass.S;
                    break;
                case Compass.E:
                    if (this.EastDoorSearchCounter <= 0)
                        this.Doors &= ~Compass.E;
                    break;
                case Compass.W:
                    if (this.WestDoorSearchCounter <= 0)
                        this.Doors &= ~Compass.W;
                    break;
            }
        }
        public void SearchDoors()
        {
            if (_northDoorSearchCounter > 0)
                _northDoorSearchCounter--;

            if (_southDoorSearchCounter > 0)
                _southDoorSearchCounter--;

            if (_eastDoorSearchCounter > 0)
                _eastDoorSearchCounter--;

            if (_westDoorSearchCounter > 0)
                _westDoorSearchCounter--;
        }

        public int NorthDoorSearchCounter { get { return _northDoorSearchCounter; } }
        public int SouthDoorSearchCounter { get { return _southDoorSearchCounter; } }
        public int EastDoorSearchCounter { get { return _eastDoorSearchCounter; } }
        public int WestDoorSearchCounter { get { return _westDoorSearchCounter; } }

        /// <summary>
        /// USED DURING GENERATION. Sets this cell to be a door cell - along with all related parameters. Adds door
        /// to already existing doors for this cell.
        /// </summary>
        public void SetDoorOR(
            Compass direction, 
            int northSearchCounter, 
            int southSearchCounter, 
            int eastSearchCounter, 
            int westSearchCounter)
        {
            this.Doors |= direction;

            _isDoor = true;
            _northDoorSearchCounter += northSearchCounter;
            _southDoorSearchCounter += southSearchCounter;
            _eastDoorSearchCounter += eastSearchCounter;
            _westDoorSearchCounter += westSearchCounter;
        }

        public Cell()
        {
            this.Walls = Compass.Null;
            this.Location = new CellPoint(-1, -1);
            this.IsPhysicallyVisible = false;
            this.IsExplored = false;
            this.IsRevealed = false;
        }
        public Cell(CellPoint p, Compass walls)
        {
            this.Location = new CellPoint(p);
            this.Walls = walls;
            this.IsPhysicallyVisible = false;
            this.IsExplored = false;
        }
        public Cell(int col, int row, Compass walls)
        {
            this.Location = new CellPoint(row, col);
            this.Walls = walls;
            this.IsPhysicallyVisible = false;
            this.IsExplored = false;
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
            _isDoor = info.GetBoolean("IsDoor");
            this.Walls = (Compass)info.GetValue("Walls", typeof(Compass));
            this.Doors = (Compass)info.GetValue("Doors", typeof(Compass));
            this.Location = (CellPoint)info.GetValue("Location", typeof(CellPoint));
            _northDoorSearchCounter = info.GetInt32("NorthDoorSearchCounter");
            _southDoorSearchCounter = info.GetInt32("SouthDoorSearchCounter");
            _eastDoorSearchCounter = info.GetInt32("EastDoorSearchCounter");
            _westDoorSearchCounter = info.GetInt32("WestDoorSearchCounter");
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("IsExplored", this.IsExplored);
            info.AddValue("IsRevealed", this.IsRevealed);
            info.AddValue("IsDoor", this.IsDoor);
            info.AddValue("Walls", this.Walls);
            info.AddValue("Doors", this.Doors);
            info.AddValue("Location", this.Location);
            info.AddValue("NorthDoorSearchCounter", this.NorthDoorSearchCounter);
            info.AddValue("SouthDoorSearchCounter", this.SouthDoorSearchCounter);
            info.AddValue("EastDoorSearchCounter", this.EastDoorSearchCounter);
            info.AddValue("WestDoorSearchCounter", this.WestDoorSearchCounter);
        }
        #endregion
    }
}
