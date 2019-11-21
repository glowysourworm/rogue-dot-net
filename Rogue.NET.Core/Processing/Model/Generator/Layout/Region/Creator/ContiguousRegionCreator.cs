using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System.Collections.Generic;
using RegionModel = Rogue.NET.Core.Model.Scenario.Content.Layout.Region;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Region.Creator
{
    public static class ContiguousRegionCreator
    {
        public static void CreateRegions(GridCellInfo[,] grid, IEnumerable<IEnumerable<RegionBoundary>> contiguousBoundaries, bool overwriteCells)
        {
            foreach (var boundaryGroup in contiguousBoundaries)
            {
                foreach (var boundary in boundaryGroup)
                {
                    // Create cells in rectangular region  (TODO:TERRAIN - PROVIDE REGION NAME)
                    //
                    GridUtility.GenerateCells(grid, boundary.Left, boundary.Top, boundary.CellWidth, boundary.CellHeight, true);
                }
            }
        }
    }
}
