using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using static Rogue.NET.Core.Math.Geometry.Metric;

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
        public MetricType Type
        {
            get { return MetricType.Rogue; }
        }
        #endregion

        #region (public) (serialized) Volatile Properties (Change during game play)
        public bool IsExplored { get; set; }
        public bool IsRevealed { get; set; }
        #endregion

        #region (public) (serialized) Properties
        public GridLocation Location { get; private set; }
        public bool IsDoor { get; private set; }
        public bool IsWall { get; private set; }
        public bool IsWallLight { get; private set; }
        public int DoorSearchCounter { get; private set; }
        public Light[] Lights { get; private set; }
        #endregion

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
            this.Lights = new Light[] { Light.White };
        }
        public GridCell(GridLocation location,
                        bool isWall, bool isWallLight, bool isDoor, int doorSearchCounter,
                        Light ambientLight, Light wallLight, Light accentLight, IEnumerable<Light> terrainLights)
        {
            if (doorSearchCounter > 0 && !isDoor)
                throw new ArgumentException("Trying to initialize door with improper parameters");

            if (ambientLight == null)
                throw new ArgumentException("Ambient Lighting cannot be un-set! GridCell");

            this.Location = location;
            this.IsWall = isWall;
            this.IsWallLight = isWallLight;
            this.IsDoor = isDoor;
            this.DoorSearchCounter = doorSearchCounter;
            this.IsExplored = false;
            this.IsRevealed = false;

            // Initialize Lighting
            var lights = new List<Light>();

            lights.Add(ambientLight);

            if (wallLight != Light.None)
                lights.Add(wallLight);

            if (accentLight != Light.None)
                lights.Add(accentLight);

            lights.AddRange(terrainLights);

            this.Lights = lights.ToArray();
        }

        public GridCell(int column, int row, bool isWall, bool isWallLight, bool isDoor, int doorSearchCounter,
                        Light ambientLight, Light wallLight, Light accentLight, IEnumerable<Light> terrainLights)
                : this(new GridLocation(column, row), isWall, isWallLight, isDoor, doorSearchCounter, ambientLight,
                       wallLight, accentLight, terrainLights)
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

            var lightCount = info.GetInt32("LightCount");

            this.Lights = new Light[lightCount];

            for (int i = 0; i < lightCount; i++)
                this.Lights[i] = (Light)info.GetValue("Light" + i.ToString(), typeof(Light));

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
            info.AddValue("LightCount", this.Lights.Length);

            for (int i = 0; i < this.Lights.Length; i++)
                info.AddValue("Light" + i.ToString(), this.Lights[i]);
        }
        #endregion
    }
}
