using Rogue.NET.Common.Serialization.Interface;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

using System;
using System.Collections.Generic;

using static Rogue.NET.Core.Math.Geometry.Metric;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    [Serializable]
    public class GridCell : IRecursiveSerializable, IGridLocator
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
        public bool IsOccupied { get; set; }
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

        #region IRecursiveSerializable - Used Custom Serialization to minimize footprint
        public void GetPropertyDefinitions(IPropertyPlanner planner)
        {
            planner.Define<bool>("IsExplored");
            planner.Define<bool>("IsRevealed");
            planner.Define<bool>("IsOccupied");
            planner.Define<bool>("IsDoor");
            planner.Define<bool>("IsWall");
            planner.Define<bool>("IsWallLight");
            planner.Define<GridLocation>("Location");
            planner.Define<int>("DoorSearchCounter");

            for (int i = 0; i < this.Lights.Length; i++)
                planner.Define<Light>("Light" + i.ToString());
        }

        public void GetProperties(IPropertyWriter writer)
        {
            writer.Write("IsExplored", this.IsExplored);
            writer.Write("IsRevealed", this.IsRevealed);
            writer.Write("IsDoor", this.IsDoor);
            writer.Write("IsWall", this.IsWall);
            writer.Write("IsWallLight", this.IsWallLight);
            writer.Write("Location", this.Location);
            writer.Write("DoorSearchCounter", this.DoorSearchCounter);
            writer.Write("LightCount", this.Lights.Length);

            for (int i = 0; i < this.Lights.Length; i++)
                writer.Write("Light" + i.ToString(), this.Lights[i]);
        }

        public void SetProperties(IPropertyReader reader)
        {
            this.IsExplored = reader.Read<bool>("IsExplored");
            this.IsRevealed = reader.Read<bool>("IsRevealed");
            this.IsOccupied = reader.Read<bool>("IsOccupied");
            this.IsDoor = reader.Read<bool>("IsDoor");
            this.IsWall = reader.Read<bool>("IsWall");
            this.IsWallLight = reader.Read<bool>("IsWallLight");
            this.Location = reader.Read<GridLocation>("Location");
            this.DoorSearchCounter = reader.Read<int>("DoorSearchCounter");


            var lightCount = reader.Read<int>("LightCount");

            this.Lights = new Light[lightCount];

            for (int i = 0; i < lightCount; i++)
                this.Lights[i] = reader.Read<Light>("Light" + i.ToString());
        }
        #endregion
    }
}
