using Rogue.NET.Core.Model.Enums;
using System;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    [Serializable]
    public class Cell
    {
        public bool IsExplored { get; set; }
        public bool IsRevealed { get; set; }
        public bool IsPhysicallyVisible { get; set; }
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
                    result &= Compass.N;

                if (this.SouthDoorSearchCounter <= 0 && ((this.Doors & Compass.S) != 0))
                    result &= Compass.S;

                if (this.EastDoorSearchCounter <= 0 && ((this.Doors & Compass.E) != 0))
                    result &= Compass.E;

                if (this.WestDoorSearchCounter <= 0 && ((this.Doors & Compass.W) != 0))
                    result &= Compass.W;

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
