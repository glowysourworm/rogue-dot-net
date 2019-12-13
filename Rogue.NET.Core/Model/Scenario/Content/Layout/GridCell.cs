using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using System;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    [Serializable]
    public class GridCell : ISerializable, IGridLocator
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

        #region (public) (serialized) Volatile Properties (Change during game play)
        public bool IsExplored { get; set; }
        public bool IsRevealed { get; set; }
        /// <summary>
        /// Effective lighting color calculated from combining ALL light sources - using alpha channel blending
        /// </summary>
        public Light EffectiveLighting { get; set; }
        #endregion

        #region (public) (serialized) Properties
        public GridLocation Location { get; set; }
        public bool IsDoor { get; private set; }
        public bool IsWall { get; private set; }
        public bool IsWallLight { get; private set; }
        public int DoorSearchCounter { get; private set; }
        /// <summary>
        /// Light value for this particular cell - calculated during generation (NOTE** Does NOT act as a light source)
        /// </summary>
        public Light BaseLight { get; set; }

        /// <summary>
        /// Light value for the wall light source - this is used during game-play calculations for 1 / r^2 lighting 
        /// (Necessary to calculate shadows during game-play)
        /// </summary>
        public Light WallLight { get; set; }
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
            this.EffectiveLighting = Light.None;
            this.BaseLight = Light.None;
            this.WallLight = Light.None;
        }
        public GridCell(GridLocation location, bool isWall, bool isWallLight, bool isDoor, int doorSearchCounter, Light baseLight, Light wallLight)
        {
            if (doorSearchCounter > 0 && !isDoor)
                throw new ArgumentException("Trying to initialize door with improper parameters");

            this.Location = location;
            this.IsWall = isWall;
            this.IsWallLight = isWallLight;
            this.IsDoor = isDoor;
            this.DoorSearchCounter = doorSearchCounter;
            this.IsExplored = false;
            this.IsRevealed = false;
            this.BaseLight = baseLight;
            this.WallLight = wallLight;

            // Initialize this to be the lighting value
            this.EffectiveLighting = baseLight;
        }

        public GridCell(int column, int row, bool isWall, bool isWallLight, bool isDoor, int doorSearchCounter, Light baseLight, Light wallLight)
                : this(new GridLocation(column, row), isWall, isWallLight, isDoor, doorSearchCounter, baseLight, wallLight)
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
            this.IsWallLight = info.GetBoolean("IsWallLight");
            this.Location = (GridLocation)info.GetValue("Location", typeof(GridLocation));
            this.DoorSearchCounter = info.GetInt32("DoorSearchCounter");
            this.BaseLight = (Light)info.GetValue("BaseLight", typeof(Light));
            this.WallLight = (Light)info.GetValue("WallLight", typeof(Light));
            this.EffectiveLighting = (Light)info.GetValue("EffectiveLighting", typeof(Light));
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("IsExplored", this.IsExplored);
            info.AddValue("IsRevealed", this.IsRevealed);
            info.AddValue("IsDoor", this.IsDoor);
            info.AddValue("IsWall", this.IsWall);
            info.AddValue("IsWallLight", this.IsWallLight);
            info.AddValue("Location", this.Location);
            info.AddValue("DoorSearchCounter", this.DoorSearchCounter);
            info.AddValue("BaseLight", this.BaseLight);
            info.AddValue("WallLight", this.WallLight);
            info.AddValue("EffectiveLighting", this.EffectiveLighting);
        }
        #endregion
    }
}
