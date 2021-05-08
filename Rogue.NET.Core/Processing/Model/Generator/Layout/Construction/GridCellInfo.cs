using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using static Rogue.NET.Core.Math.Geometry.Metric;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Construction
{
    /// <summary>
    /// Component used for creating the LevelGrid during the generation phase.
    /// </summary>
    [Serializable]
    public class GridCellInfo : IGridLocator
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

        /// <summary>
        /// Event to signal modification of layout properties (IsWall, IsCorridor) - this should invalidate the
        /// layout during the build process
        /// </summary>
        public event SimpleEventHandler<GridCellInfo> IsLayoutModifiedEvent;

        bool _isWall;
        bool _isCorridor;

        public GridLocation Location { get; set; }
        public bool IsDoor { get; set; }
        public bool IsWall
        {
            get { return _isWall; }
            set
            {
                if (_isWall != value)
                {
                    _isWall = value;

                    if (this.IsLayoutModifiedEvent != null)
                        this.IsLayoutModifiedEvent(this);
                }
            }
        }
        public bool IsWallLight { get; set; }
        public bool IsCorridor
        {
            get { return _isCorridor; }
            set
            {
                if (_isCorridor != value)
                {
                    _isCorridor = value;

                    if (this.IsLayoutModifiedEvent != null)
                        this.IsLayoutModifiedEvent(this);
                }
            }
        }
        public int DoorSearchCounter { get; set; }
        public Light AmbientLight { get; set; }
        public Light WallLight { get; set; }
        public Light AccentLight { get; set; }
        public Dictionary<string, Light> TerrainLights { get; set; }

        public GridCellInfo(GridLocation location)
        {
            this.Location = location;
            this.AmbientLight = Light.White;
            this.AccentLight = Light.None;
            this.WallLight = Light.None;
            this.TerrainLights = new Dictionary<string, Light>();
        }
        public GridCellInfo(int column, int row)
        {
            this.Location = new GridLocation(column, row);
            this.AmbientLight = Light.White;
            this.AccentLight = Light.None;
            this.WallLight = Light.None;
            this.TerrainLights = new Dictionary<string, Light>();
        }
        public GridCellInfo(SerializationInfo info, StreamingContext context)
        {
            this.Location = (GridLocation)info.GetValue("Location", typeof(GridLocation));
            this.IsDoor = (bool)info.GetValue("IsDoor", typeof(bool));
            _isWall = (bool)info.GetValue("IsWall", typeof(bool));
            this.IsWallLight = (bool)info.GetValue("IsWallLight", typeof(bool));
            this.IsCorridor = (bool)info.GetValue("IsCorridor", typeof(bool));
            this.DoorSearchCounter = (int)info.GetValue("DoorSearchCounter", typeof(int));
            this.AmbientLight = (Light)info.GetValue("AmbientLight", typeof(Light));
            this.WallLight = (Light)info.GetValue("WallLight", typeof(Light));
            this.AccentLight = (Light)info.GetValue("AccentLight", typeof(Light));
            this.TerrainLights = (Dictionary<string, Light>)info.GetValue("TerrainLights", typeof(Dictionary<string, Light>));
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Location", this.Location);
            info.AddValue("IsDoor", this.IsDoor);
            info.AddValue("IsWall", this.IsWall);
            info.AddValue("IsWallLight", this.IsWallLight);
            info.AddValue("IsCorridor", this.IsCorridor);
            info.AddValue("DoorSearchCounter", this.DoorSearchCounter);
            info.AddValue("AmbientLight", this.AmbientLight);
            info.AddValue("WallLight", this.WallLight);
            info.AddValue("AccentLight", this.AccentLight);
            info.AddValue("TerrainLights", this.TerrainLights);
        }

        public override string ToString()
        {
            return this.Location?.ToString() ?? "{}";
        }
    }
}
