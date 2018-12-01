using Rogue.NET.Core.Model.Enums;
using System;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    [Serializable]
    public class Cell
    {
        public bool IsExplored { get; set; }
        public bool IsRevealed { get; set; }

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
        public bool IsDoor
        {
            get { return this.Doors != Compass.Null; }
        }
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

        public void ToggleDoor(Compass direction)
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
        public int NorthDoorSearchCounter { get; set; }
        public int SouthDoorSearchCounter { get; set; }
        public int EastDoorSearchCounter { get; set; }
        public int WestDoorSearchCounter { get; set; }

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
    }
}
