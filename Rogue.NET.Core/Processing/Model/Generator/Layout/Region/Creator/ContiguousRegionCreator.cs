using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System.Collections.Generic;
using RegionModel = Rogue.NET.Core.Model.Scenario.Content.Layout.Region;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Region.Creator
{
    public static class ContiguousRegionCreator
    {
        public static IEnumerable<RegionModel> CreateRegions(Cell[,] grid, IEnumerable<IEnumerable<RegionBoundary>> contiguousBoundaries, bool overwriteCells)
        {
            var regions = new List<RegionModel>();

            foreach (var boundaryGroup in contiguousBoundaries)
            {
                foreach (var boundary in boundaryGroup)
                {
                    // Create cells in rectangular region
                    //
                    regions.Add(GridUtility.CreateRectangularRegion(grid, boundary.Left, boundary.Top, boundary.CellWidth, boundary.CellHeight, false, true));
                }
            }

            return regions;
        }
    }
}
