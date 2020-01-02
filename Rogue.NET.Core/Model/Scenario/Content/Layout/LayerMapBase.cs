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
    public abstract class LayerMapBase<T, TRegion> : ISerializable, 
                                                     IDeserializationCallback 
                                                     where TRegion : Region<T> 
                                                     where T : class, IGridLocator
    {
        public string Name { get; private set; }

        // Keep a grid of region references per cell in the region
        TRegion[,] _regionMap;

        // Also, keep a collection of the regions for this layer
        IEnumerable<TRegion> _regions;

        /// <summary>
        /// Total layer map boundary - built by encompassing the individual regions
        /// </summary>
        public RegionBoundary Boundary { get; private set; }

        public TRegion this[int column, int row]
        {
            get { return _regionMap[column, row]; }
        }

        public TRegion this[IGridLocator location]
        {
            get { return _regionMap[location.Column, location.Row]; }
        }

        public IEnumerable<TRegion> Regions
        {
            get { return _regions; }
        }

        public IEnumerable<T> GetLocations()
        {
            return _regions.SelectMany(region => region.Locations)
                           .Actualize();
        }

        public IEnumerable<T> GetNonOccupiedLocations()
        {
            return _regions.SelectMany(region => region.NonOccupiedLocations)
                           .Actualize();
        }

        public bool IsOccupied(IGridLocator location)
        {
            return _regions.Any(region => region.IsOccupied(location));
        }

        public void SetOccupied(IGridLocator location, bool occupied)
        {
            foreach (var region in _regions)
            {
                if (region[location] != null)
                    region.SetOccupied(location, occupied);
            }
        }

        public LayerMapBase(string layerName, IEnumerable<TRegion> regions, int width, int height)
        {
            // Get regions from the region graph
            Initialize(layerName, regions, width, height);
        }

        public LayerMapBase(SerializationInfo info, StreamingContext context)
        {
            var name = info.GetString("Name");
            var width = info.GetInt32("Width");
            var height = info.GetInt32("Height");
            var regionCount = info.GetInt32("RegionCount");

            // NOTE*** DOING TYPE INSPECTION TO SIMPLIFY THE INHERITANCE SPECIFICATION. 
            //          
            //         THE ALTERNATIVE IS:  LayerMap<Region<GridLocation>> and ConnectedLayerMap<ConnectedRegion<GridLocation>>
            //
            var regions = new List<TRegion>();

            for (int i = 0; i < regionCount; i++)
            {
                var region = (TRegion)info.GetValue("Region" + i.ToString(), typeof(TRegion));

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

        protected void Initialize(string layerName, IEnumerable<TRegion> regions, int width, int height)
        {
            this.Name = layerName;

            _regions = regions;
            _regionMap = new TRegion[width, height];

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
