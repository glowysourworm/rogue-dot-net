using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Construction;
using Rogue.NET.Core.Processing.Model.Extension;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface;

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Component
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IRectangularRegionCreator))]
    public class RectangularRegionCreator : IRectangularRegionCreator
    {
        public RectangularRegionCreator()
        {

        }

        public void CreateCells(GridCellInfo[,] grid, RegionBoundary boundary, bool overwrite)
        {
            for (int i = boundary.Left; i <= boundary.Right; i++)
            {
                for (int j = boundary.Top; j <= boundary.Bottom; j++)
                {
                    if (grid[i, j] != null && !overwrite)
                        throw new Exception("Trying to overwrite existing grid cell RectangularRegionCreator");

                    grid[i, j] = new GridCellInfo(i, j);
                }
            }
        }

        public void CreateCellsXOR(GridCellInfo[,] grid, RegionBoundary boundary, int padding, double separationRatio)
        {
            var locations = new List<GridCellInfo>();
            var allLocations = new List<GridCellInfo>();
            var regionDetected = false;

            for (int i = boundary.Left; i <= boundary.Right; i++)
            {
                for (int j = boundary.Top; j <= boundary.Bottom; j++)
                {
                    if (grid[i, j] != null)
                    {
                        regionDetected = true;
                        continue;
                    }

                    var cell = new GridCellInfo(i, j);

                    // Be sure that all adjacent elements are null (padding + 1 gives a minimum of one cell between boundaries)
                    if (grid.GetElementsNearUnsafe(i, j, padding + 1).All(cell => cell == null))
                        locations.Add(cell);

                    else
                        regionDetected = true;

                    // Keep a list of all cells in case the number doesn't exceed the threshold
                    allLocations.Add(cell);
                }
            }

            // Number of valid locations exceeds the threshold
            if (locations.Count >= (allLocations.Count * (1 - separationRatio)))
            {
                // Now, safely add the cells to the grid
                foreach (var cell in locations)
                    grid[cell.Location.Column, cell.Location.Row] = cell;
            }
            else
            {
                foreach (var cell in allLocations)
                    grid[cell.Location.Column, cell.Location.Row] = cell;
            }
        }
    }
}