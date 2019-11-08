using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    /// <summary>
    /// Component for storing and mainintaining a 2D cell array for a region
    /// </summary>
    public class RegionMap
    {
        readonly IEnumerable<Region> _regions;

        // Keep a grid of region data per cell in the region
        List<Region>[,] _regionMap;

        /// <summary>
        /// Gets an enumerable collection of regions for this cell location
        /// </summary>
        public IEnumerable<Region> this[int column, int row]
        {
            get { return _regionMap[column, row]; }
        }

        /// <summary>
        /// Returns the first region with the specified name
        /// </summary>
        public Region this[string regionName]
        {
            get
            {
                return _regions.First(region => region.Name == regionName);
            }
        }

        public RegionMap(IEnumerable<Region> regions, int width, int height)
        {
            _regions = regions;
            _regionMap = new List<Region>[width, height];

            // Initialize the region map array
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                    _regionMap[i, j] = new List<Region>();
            }

            // Iterate regions and initialize the map
            foreach (var region in regions)
            {
                foreach (var cell in region.Cells)
                {
                    if (_regionMap[cell.Column, cell.Row].Contains(region))
                        throw new Exception("Invalid Region construction - duplicate cell locations");

                    _regionMap[cell.Column, cell.Row].Add(region);
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
