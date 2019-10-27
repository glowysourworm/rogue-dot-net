using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using System;
using System.Collections.Generic;
using RegionModel = Rogue.NET.Core.Model.Scenario.Content.Layout.Region;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Region.Creator
{
    public static class CellularAutomataRegionCreator
    {
        static readonly IRandomSequenceGenerator _randomSequenceGenerator;

        // NOTE** Not used for all layout types. It was not required for certain types
        //        that had other padding involved (Rectangular Grid); or no padding (Maze).
        //
        //        MUST BE GREATER THAN OR EQUAL TO 2.
        private const int CELLULAR_AUTOMATA_PADDING = 2;
        private const int CELLULAR_AUTOMATA_ITERATIONS = 5;

        static CellularAutomataRegionCreator()
        {
            _randomSequenceGenerator = ServiceLocator.Current.GetInstance<IRandomSequenceGenerator>();
        }

        /// <summary>
        /// Creates a set of regions over the specified grid
        /// </summary>
        public static IEnumerable<RegionModel> CreateRegions(Cell[,] grid, bool filled, double fillRatio)
        {
            // Procedure
            //
            // 0) Create random cells in grid
            // 1) Iterate grid several times using smoothing rules
            // 2) Locate rooms and store them in an array

            var bounds = new RegionBoundary(new GridLocation(0, 0), grid.GetLength(0), grid.GetLength(1));

            // Create function to get count of adjacent empty cells
            //
            // NOTE*** CELLULAR_AUTOMATA_PADDING prevents out of bounds errors
            var emptyCellCountFunc = new Func<Cell[,], int, int, int, int, int>((gridArray, column, row, aggregator, distance) =>
            {
                // Use aggregator to add results from other distances
                var result = aggregator;

                if (distance == 0)
                    return result + (gridArray[column, row] == null ? 1 : 0);

                // Count Top and Bottom first
                for (int i = column - distance; i <= column + distance; i++)
                {
                    result += gridArray[i, row - distance] == null ? 1 : 0;
                    result += gridArray[i, row + distance] == null ? 1 : 0;
                }

                // Count Left and Right - excluding corners
                for (int j = (row - distance) + 1; j <= (row + distance) - 1; j++)
                {
                    result += gridArray[column - distance, j] == null ? 1 : 0;
                    result += gridArray[column + distance, j] == null ? 1 : 0;
                }

                return result;
            });

            // Iterate grid cells and fill them in randomly
            // 
            // Optimize:  Could cut down on the amount of iterations by using the fill ratio
            //            to give you a number of cells (to be chosen randomly)
            IterateCellularAutomata(grid, 1, (gridArray, currentCell, index) =>
            {
                // Fill Ratio => Empty Cell
                return _randomSequenceGenerator.Get() < fillRatio;
            });

            // Iterate grid to apply Filled Rule - Apply this for several iterations before smoothing
            if (filled)
            {
                IterateCellularAutomata(grid, CELLULAR_AUTOMATA_ITERATIONS, (gridArray, column, row) =>
                {
                    var empty0Count = emptyCellCountFunc(gridArray, column, row, 0, 0);
                    var empty1Count = emptyCellCountFunc(gridArray, column, row, empty0Count, 1);

                    if (empty1Count >= 5)
                        return true;

                    var empty2Count = emptyCellCountFunc(gridArray, column, row, empty1Count, 2);

                    return (empty2Count <= 7);
                });
            }

            // Iterate grid to apply Open Rule - This will smooth out the noise in the grid. Also,
            // it is used in conjunction with the Filled rule to fill out large open spaces after
            // the Filled rule is applied
            IterateCellularAutomata(grid, filled ? 1 : CELLULAR_AUTOMATA_ITERATIONS, (gridArray, column, row) =>
            {
                return (emptyCellCountFunc(gridArray, column, row, 0, 0) + emptyCellCountFunc(gridArray, column, row, 0, 1)) >= 5;
            });


            return grid.IdentifyRegions();
        }

        private static void IterateCellularAutomata(Cell[,] grid, int numberOfIterations, Func<Cell[,], int, int, bool> cellularAutomataRule)
        {
            for (int k = 0; k < numberOfIterations; k++)
            {
                for (int i = CELLULAR_AUTOMATA_PADDING; i < grid.GetLength(0) - CELLULAR_AUTOMATA_PADDING; i++)
                {
                    for (int j = CELLULAR_AUTOMATA_PADDING; j < grid.GetLength(1) - CELLULAR_AUTOMATA_PADDING; j++)
                    {
                        if (cellularAutomataRule(grid, i, j))
                            grid[i, j] = null;

                        else
                            grid[i, j] = new Cell(i, j, false);
                    }
                }
            }
        }
    }
}
