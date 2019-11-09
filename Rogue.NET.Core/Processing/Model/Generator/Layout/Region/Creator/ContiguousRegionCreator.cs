﻿using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System.Collections.Generic;
using RegionModel = Rogue.NET.Core.Model.Scenario.Content.Layout.Region;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Region.Creator
{
    public static class ContiguousRegionCreator
    {
        public static IEnumerable<RegionModel> CreateRegions(GridCellInfo[,] grid, IEnumerable<IEnumerable<RegionBoundary>> contiguousBoundaries, bool overwriteCells)
        {
            var regions = new List<RegionModel>();

            foreach (var boundaryGroup in contiguousBoundaries)
            {
                foreach (var boundary in boundaryGroup)
                {
                    // Create cells in rectangular region  (TODO:TERRAIN - PROVIDE REGION NAME)
                    //
                    regions.Add(GridUtility.CreateRectangularRegion("Region " + System.Guid.NewGuid().ToString(), grid, boundary.Left, boundary.Top, boundary.CellWidth, boundary.CellHeight, true));
                }
            }

            return regions;
        }
    }
}