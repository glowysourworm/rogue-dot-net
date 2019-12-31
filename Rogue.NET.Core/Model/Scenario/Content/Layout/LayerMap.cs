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
    public class LayerMap : ISerializable, IDeserializationCallback
    {
        public string Name { get; private set; }

        // Keep a grid of region references per cell in the region
        Region<GridLocation>[,] _regionMap;

        // Also, keep a collection of the regions for this layer
        IEnumerable<Region<GridLocation>> _regions;

        /// <summary>
        /// Total layer map boundary - built by encompassing the individual regions
        /// </summary>
        public RegionBoundary Boundary { get; private set; }

        /// <summary>
        /// Gets the region for the specified location
        /// </summary>
        public Region<GridLocation> this[int column, int row]
        {
            get { return _regionMap[column, row]; }
        }

        /// <summary>
        /// Gets the region for the specified location
        /// </summary>
        public Region<GridLocation> this[IGridLocator location]
        {
            get { return _regionMap[location.Column, location.Row]; }
        }

        public IEnumerable<Region<GridLocation>> Regions
        {
            get { return _regions; }
        }

        public IEnumerable<GridLocation> GetLocations()
        {
            return _regions.SelectMany(region => region.Locations)
                           .Actualize();
        }

        public IEnumerable<GridLocation> GetNonOccupiedLocations()
        {
            return _regions.SelectMany(region => region.NonOccupiedLocations)
                           .Actualize();
        }

        public bool IsOccupied(IGridLocator location)
        {
            return _regions.Any(region => region.IsOccupied(location));
        }

        /// <summary>
        /// Sets up occupied data for this location for all involved regions
        /// </summary>
        public void SetOccupied(IGridLocator location, bool occupied)
        {
            foreach (var region in _regions)
            {
                if (region[location] != null)
                    region.SetOccupied(location, occupied);
            }
        }

        public LayerMap(string layerName, IEnumerable<Region<GridLocation>> regions, int width, int height)
        {
            // Get regions from the region graph
            Initialize(layerName, regions, width, height);
        }

        public LayerMap(SerializationInfo info, StreamingContext context)
        {
            var name = info.GetString("Name");
            var width = info.GetInt32("Width");
            var height = info.GetInt32("Height");
            var regionCount = info.GetInt32("RegionCount");

            var regions = new List<Region<GridLocation>>();

            for (int i = 0; i < regionCount; i++)
            {
                var region = (Region<GridLocation>)info.GetValue("Region" + i.ToString(), typeof(Region<GridLocation>));

                regions.Add(region);
            }

            Initialize(name, regions, width, height);
        }

        public void OnDeserialization(object sender)
        {
            if (sender == null)
                return;

            var grid = sender as GridCell[,];

            if (grid == null)
                throw new Exception("Improper use of OnDeserialization()  LayerMap");

            foreach (var region in _regions)
                region.OnDeserialization(grid);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", this.Name);
            info.AddValue("Width", _regionMap.GetLength(0));
            info.AddValue("Height", _regionMap.GetLength(1));
            info.AddValue("RegionCount", _regions.Count());

            for (int i = 0; i < _regions.Count(); i++)
                info.AddValue("Region" + i.ToString(), _regions.ElementAt(i));
        }

        private void Initialize(string layerName, IEnumerable<Region<GridLocation>> regions, int width, int height)
        {
            this.Name = layerName;

            _regions = regions;
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

            // TODO: Serialize this
            this.Boundary = new RegionBoundary(left, top, right - left + 1, bottom - top + 1);
        }
    }
}
