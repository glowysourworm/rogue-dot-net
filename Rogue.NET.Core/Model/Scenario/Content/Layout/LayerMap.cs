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
    public class LayerMap : ISerializable
    {
        public string Name { get; private set; }

        // Keep a grid of region references per cell in the region
        Region[,] _regionMap;

        // Also, keep a collection of the regions for this layer
        IEnumerable<Region> _regions;

        /// <summary>
        /// Gets the region for the specified location
        /// </summary>
        public Region this[int column, int row]
        {
            get { return _regionMap[column, row]; }
        }

        public LayerMap(string layerName, IEnumerable<Region> regions, int width, int height)
        {
            Initialize(layerName, regions, width, height);
        }

        public LayerMap(SerializationInfo info, StreamingContext context)
        {
            var name = info.GetString("Name");
            var width = info.GetInt32("Width");
            var height = info.GetInt32("Height");
            var regionCount = info.GetInt32("RegionCount");

            var regions = new List<Region>();

            for (int i = 0; i < regionCount; i++)
            {
                var region = (Region)info.GetValue("Region" + i.ToString(), typeof(Region));

                regions.Add(region);
            }

            Initialize(name, regions, width, height);
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

        private void Initialize(string layerName, IEnumerable<Region> regions, int width, int height)
        {
            this.Name = layerName;

            _regions = regions;
            _regionMap = new Region[width, height];

            // Iterate regions and initialize the map
            foreach (var region in regions)
            {
                foreach (var cell in region.Cells)
                {
                    if (_regionMap[cell.Column, cell.Row] != null)
                        throw new Exception("Invalid Region construction - duplicate cell locations");

                    _regionMap[cell.Column, cell.Row] = region;
                }
            }
        }

        /// <summary>
        /// Returns all regions in the map
        /// </summary>
        public IEnumerable<Region> GetRegions()
        {
            return _regions;
        }
    }
}
