using Rogue.NET.Core.Math.Algorithm;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using System;
using System.Collections.Generic;
using RegionModel = Rogue.NET.Core.Model.Scenario.Content.Layout.Region;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Region.Creator
{
    public static class PerlinNoiseRegionCreator
    {
        public static IEnumerable<RegionModel> CreateRegions(Cell[,] grid, LayoutTemplate template, bool overwriteCells)
        {
            // Frequency Range:  Probably best between [0.01, 0.5]
            //
            NoiseGenerator.GeneratePerlinNoise(grid.GetLength(0), grid.GetLength(1), 0.08, (column, row, value) =>
            {
                if (value < 0)
                {
                    var cell = grid[column, row];

                    if (cell == null)
                        cell = new Cell(new GridLocation(column, row), false);

                    else if (!overwriteCells)
                        throw new Exception("Trying to overwrite grid cell PerlinNoiseRegionCreator");

                    grid[column, row] = cell;
                }

                return value;
            });

            return grid.IdentifyRegions();
        }
    }
}
