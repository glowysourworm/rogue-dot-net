using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Common.Serialization.Interface;
using Rogue.NET.Core.Math.Algorithm.Interface;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

using System;
using System.Collections.Generic;
using System.Linq;

using static Rogue.NET.Core.Math.Geometry.Metric;
using static Rogue.NET.Core.Model.Scenario.Content.Layout.LayoutGrid;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Construction
{
    /// <summary>
    /// Component used for creating the LevelGrid during the generation phase.
    /// </summary>
    [Serializable]
    public class GridCellInfo : IGridLocator, IGraphNode
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

        #region (public) IGraphNode
        public int Hash
        {
            get { return this.CreateHashCode(this.Column, this.Row); }
        }
        #endregion

        /// <summary>
        /// Event to signal modification of layout properties (IsWall, IsCorridor) - this should invalidate the
        /// layout during the build process. The string parameter is the name of the property modified.
        /// </summary>
        public event SimpleEventHandler<string, GridCellInfo> IsLayoutModifiedEvent;

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
                        this.IsLayoutModifiedEvent("IsWall", this);
                }
            }
        }
        public bool IsCorridor
        {
            get { return _isCorridor; }
            set
            {
                if (_isCorridor != value)
                {
                    _isCorridor = value;

                    if (this.IsLayoutModifiedEvent != null)
                        this.IsLayoutModifiedEvent("IsCorridor", this);
                }
            }
        }
        /// <summary>
        /// This flag shows that a cell HAS BEEN ADDED to support terrain that overlaps the layout. This
        /// should NOT be used to query for terrain! It coincides with the "Empty Space" flag for the terrain
        /// mask - and just basically means that the cell should be considered as EXTRA - expanding the
        /// layout where the terrain overlapped the original layout. SHOULD NOT MODIFY THE TOPOLOGY
        /// </summary>
        public bool IsTerrainSupport { get; set; }

        /// <summary>
        /// This flag shows that a cell has been added to support additional cells for the IConnectionBuilder. This
        /// should NOT be used to query for the Connection regions! It is treated as a NON-WALKABLE cell until it is
        /// integrated into the CORRIDOR, WALKABLE, and PLACEMENT layers during FINALIZATION.
        /// </summary>
        // public bool IsConnectionSupport { get; set; }

        public bool IsWallLight { get; set; }
        public int DoorSearchCounter { get; set; }
        public Light AmbientLight { get; set; }
        public Light WallLight { get; set; }
        public Light AccentLight { get; set; }
        public SimpleDictionary<string, Light> TerrainLights { get; set; }

        public bool IsLayer(LayoutLayer layer)
        {
            switch (layer)
            {
                // USED DURING TERRAIN FINALIZATION
                case LayoutLayer.FullNoTerrainSupport: return !this.IsTerrainSupport;
                case LayoutLayer.ConnectionRoom: return !this.IsCorridor && !this.IsWall && !this.IsDoor && !this.IsTerrainSupport;

                case LayoutLayer.Walkable: return !this.IsWall;
                case LayoutLayer.Placement: return !this.IsDoor && !this.IsWall;
                case LayoutLayer.Room: return !this.IsCorridor && !this.IsWall && !this.IsDoor;
                case LayoutLayer.Corridor: return this.IsCorridor;
                case LayoutLayer.Wall: return this.IsWall;
                case LayoutLayer.TerrainSupport: return this.IsTerrainSupport;
                default:
                    throw new Exception("Unhandled layout type:  LayoutContainer.IsLayer");
            }
        }

        public bool IsLayerExclusive(LayoutLayer layer)
        {
            if (!IsLayer(layer))
                return false;

            var layers = Enum.GetValues(typeof(LayoutLayer))
                             .Cast<LayoutLayer>();

            foreach (var otherLayer in layers)
            {
                if (otherLayer == layer)
                    continue;

                if (IsLayer(otherLayer))
                    return false;
            }

            return true;
        }

        public GridCellInfo(GridLocation location)
        {
            Initialize(location.Column, location.Row);
        }
        public GridCellInfo(int column, int row)
        {
            Initialize(column, row);
        }

        public GridCellInfo(IPropertyReader reader)
        {
            this.Location = reader.Read<GridLocation>("Location");
            this.IsDoor = reader.Read<bool>("IsDoor");
            this.IsWall = reader.Read<bool>("IsWall");
            this.IsWallLight = reader.Read<bool>("IsWallLight");
            this.IsCorridor = reader.Read<bool>("IsCorridor");
            this.IsTerrainSupport = reader.Read<bool>("IsTerrainSupport");
            this.DoorSearchCounter = reader.Read<int>("DoorSearchCounter");
            this.AmbientLight = reader.Read<Light>("AmbientLight");
            this.WallLight = reader.Read<Light>("WallLight");
            this.AccentLight = reader.Read<Light>("AccentLight");
            this.TerrainLights = reader.Read<SimpleDictionary<string, Light>>("TerrainLights");
        }

        private void Initialize(int column, int row)
        {
            this.Location = new GridLocation(column, row);
            this.AmbientLight = Light.White;
            this.AccentLight = Light.None;
            this.WallLight = Light.None;
            this.TerrainLights = new SimpleDictionary<string, Light>();
        }

        public void GetProperties(IPropertyWriter writer)
        {
            writer.Write("Location", this.Location);
            writer.Write("IsDoor", this.IsDoor);
            writer.Write("IsWall", this.IsWall);
            writer.Write("IsWallLight", this.IsWallLight);
            writer.Write("IsCorridor", this.IsCorridor);
            writer.Write("IsTerrainSupport", this.IsTerrainSupport);
            writer.Write("DoorSearchCounter", this.DoorSearchCounter);
            writer.Write("AmbientLight", this.AmbientLight);
            writer.Write("WallLight", this.WallLight);
            writer.Write("AccentLight", this.AccentLight);
            writer.Write("TerrainLights", this.TerrainLights);
        }

        public override string ToString()
        {
            return this.Location?.ToString() ?? "{}";
        }
    }
}
