using Rogue.NET.Core.Media.SymbolEffect.Utility;
using System;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    [Serializable]
    public class GridCell : ISerializable
    {
        #region (public) (serialized) Volatile Properties (Change during game play)
        public bool IsExplored { get; set; }
        public bool IsRevealed { get; set; }
        /// <summary>
        /// Effective lighting color calculated from combining ALL light sources - using alpha channel blending
        /// </summary>
        public Color EffectiveLighting { get; set; }
        #endregion

        #region (public) (serialized) Properties
        public GridLocation Location { get; set; }
        public bool IsDoor { get; private set; }
        public bool IsWall { get; private set; }
        public bool IsCorridor { get; private set; }
        public int DoorSearchCounter { get; private set; }
        /// <summary>
        /// Light value for this particular cell - calculated during generation (NOTE** Does NOT act as a light source)
        /// </summary>
        public Color Lighting { get; set; }
        #endregion

        // TODO:TERRAIN - REMOVE THIS
        public double DijkstraWeight { get; set; }

        public void Search()
        {
            this.DoorSearchCounter--;

            if (this.DoorSearchCounter < 0)
                this.DoorSearchCounter = 0;
        }

        public GridCell()
        {
            this.Location = new GridLocation(-1, -1);
            this.IsExplored = false;
            this.IsRevealed = false;
            this.IsWall = false;
            this.IsDoor = false;
        }
        public GridCell(GridLocation location, bool isWall, bool isDoor, int doorSearchCounter, bool isCorridor, Color lighting)
        {
            if (doorSearchCounter > 0 && !isDoor)
                throw new ArgumentException("Trying to initialize door with improper parameters");

            this.Location = new GridLocation(location);
            this.IsWall = isWall;
            this.IsDoor = isDoor;
            this.IsCorridor = isCorridor;
            this.DoorSearchCounter = doorSearchCounter;
            this.IsExplored = false;
            this.IsRevealed = false;
            this.Lighting = lighting;

            // Initialize this to be the lighting value
            this.EffectiveLighting = lighting;
        }

        public GridCell(int column, int row, bool isWall, bool isDoor, int doorSearchCounter, bool isCorridor, Color lighting) 
                : this(new GridLocation(column, row), isWall, isDoor, doorSearchCounter, isCorridor, lighting)
        {
        }

        public override string ToString()
        {
            return this.Location == null ? "" : this.Location.ToString();
        }

        #region ISerializable - Used Custom Serialization to minimize footprint
        public GridCell(SerializationInfo info, StreamingContext context)
        {
            this.IsExplored = info.GetBoolean("IsExplored");
            this.IsRevealed = info.GetBoolean("IsRevealed");
            this.IsDoor = info.GetBoolean("IsDoor");
            this.IsWall = info.GetBoolean("IsWall");
            this.IsCorridor = info.GetBoolean("IsCorridor");
            this.Location = (GridLocation)info.GetValue("Location", typeof(GridLocation));
            this.DoorSearchCounter = info.GetInt32("DoorSearchCounter");
            this.Lighting = ColorFilter.Convert(info.GetString("Lighting"));
            this.EffectiveLighting = ColorFilter.Convert(info.GetString("EffectiveLighting"));
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("IsExplored", this.IsExplored);
            info.AddValue("IsRevealed", this.IsRevealed);
            info.AddValue("IsDoor", this.IsDoor);
            info.AddValue("IsWall", this.IsWall);
            info.AddValue("IsCorridor", this.IsCorridor);
            info.AddValue("Location", this.Location);
            info.AddValue("DoorSearchCounter", this.DoorSearchCounter);
            info.AddValue("Lighting", ColorFilter.ConvertBack(this.Lighting));
            info.AddValue("EffectiveLighting", ColorFilter.ConvertBack(this.EffectiveLighting));
        }
        #endregion
    }
}
