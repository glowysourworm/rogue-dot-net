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
        public GridLocation Location { get; set; }
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
            this.Location = new GridLocation(-1, -1);
            this.IsExplored = false;
            this.IsRevealed = false;
        }
        public Cell(GridLocation p, Compass walls)
        {
            this.Location = new GridLocation(p);
            this.Walls = walls;
            this.IsExplored = false;
        }
        public Cell(int column, int row, Compass walls)
        {
            this.Location = new GridLocation(column, row);
            this.Walls = walls;
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
            this.Location = (GridLocation)info.GetValue("Location", typeof(GridLocation));
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
