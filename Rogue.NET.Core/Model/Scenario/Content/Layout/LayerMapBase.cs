using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    /// <summary>
    /// Component for storing and mainintaining a 2D cell array for a layer of the level layout
    /// </summary>
    [Serializable]
    public abstract class LayerMapBase : ISerializable
    {
        public string Name { get; private set; }

        // Keep a grid of region references per cell in the region
        Region<GridLocation>[,] _regionMap;

        // Also, keep a collection of the regions for this layer
        List<Region<GridLocation>> _regions;

        /// <summary>
        /// Layer map boundary - built by encompassing the individual regions
        /// </summary>
        public RegionBoundary Boundary { get; private set; }

        /// <summary>
        /// Total layer map boundary - with the encompassing LayoutGrid's dimensions
        /// </summary>
        public RegionBoundary ParentBoundary { get; private set; }

        public GridLocation Get(int column, int row)
        {
            if (_regionMap[column, row] != null)
                return _regionMap[column, row][column, row];

            return default(GridLocation);
        }

        public Region<GridLocation> this[int column, int row]
        {
            get { return _regionMap[column, row]; }
        }

        public Region<GridLocation> this[IGridLocator location]
        {
            get { return _regionMap[location.Column, location.Row]; }
        }

        public IEnumerable<GridLocation> GetLocations()
        {
            return _regions.SelectMany(region => region.Locations)
                           .Actualize();
        }

        public IEnumerable<Region<GridLocation>> Regions { get { return _regions; } }

        public LayerMapBase(string layerName, IEnumerable<Region<GridLocation>> regions, int width, int height)
        {
            // Get regions from the region graph
            Initialize(layerName, regions, width, height);
        }

        public LayerMapBase(SerializationInfo info, StreamingContext context)
        {
            var name = info.GetString("Name");
            var width = info.GetInt32("Width");
            var height = info.GetInt32("Height");
            var regions = (SimpleList<Region<GridLocation>>)info.GetValue("Regions", typeof(SimpleList<Region<GridLocation>>));

            Initialize(name, regions, width, height);
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", this.Name);
            info.AddValue("Width", _regionMap.GetLength(0));
            info.AddValue("Height", _regionMap.GetLength(1));
            info.AddValue("Regions", _regions.ToSimpleList());
        }

        protected void Initialize(string layerName, IEnumerable<Region<GridLocation>> regions, int width, int height)
        {
            this.Name = layerName;

            _regions = new List<Region<GridLocation>>(regions);
            _regionMap = new Region<GridLocation>[width, height];

            // Iterate regions and initialize the map
            foreach (var region in regions)
            {
                foreach (var location in region.Locations)
                {
                    if (_regionMap[location.Column, location.Row] != null)
                        throw new Exception("Invalid Region construction - duplicate cell locations");

                    _regionMap[location.Column, location.Row] = region;
                }
            }

            // Calculate layer boundary
            var left = int.MaxValue;
            var right = int.MinValue;
            var top = int.MaxValue;
            var bottom = int.MinValue;

            foreach (var region in _regions)
            {
                if (region.Boundary.Left < left)
                    left = region.Boundary.Left;

                if (region.Boundary.Right > right)
                    right = region.Boundary.Right;

                if (region.Boundary.Top < top)
                    top = region.Boundary.Top;

                if (region.Boundary.Bottom > bottom)
                    bottom = region.Boundary.Bottom;
            }

            this.Boundary = new RegionBoundary(left, top, right - left + 1, bottom - top + 1);
            this.ParentBoundary = new RegionBoundary(0, 0, width, height);
        }
    }
}
