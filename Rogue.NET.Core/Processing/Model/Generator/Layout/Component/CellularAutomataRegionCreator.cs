using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface;
using System;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Component
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(ICellularAutomataRegionCreator))]
    public class CellularAutomataRegionCreator : ICellularAutomataRegionCreator
    {
        readonly IRandomSequenceGenerator _randomSequenceGenerator;

        // NOTE**  MUST BE GREATER THAN OR EQUAL TO 2.
        private const int CELLULAR_AUTOMATA_PADDING = 2;
        private const int CELLULAR_AUTOMATA_ITERATIONS = 5;

        // Scales [0, 1] fill ratio to a safe scale
        private const double CELLULAR_AUTOMATA_FILL_LOW = 0.4;
        private const double CELLULAR_AUTOMATA_FILL_HIGH = 0.5;

        [ImportingConstructor]
        public CellularAutomataRegionCreator(IRandomSequenceGenerator randomSequenceGenerator)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
        }

        public void GenerateCells(GridCellInfo[,] grid, RegionBoundary boundary, LayoutCellularAutomataType type, double fillRatio, bool overwrite)
        {
            // Procedure
            //
            // 0) Create random cells in grid
            // 1) Iterate grid several times using smoothing rules
            // 2) Locate rooms and store them in an array

            // Maps the fill ratio to a safe scale [0, 1] -> [0.4, 0.5]
            var scaledFillRatio = ((CELLULAR_AUTOMATA_FILL_HIGH - CELLULAR_AUTOMATA_FILL_LOW) * fillRatio) + CELLULAR_AUTOMATA_FILL_LOW;
            var filled = type == LayoutCellularAutomataType.Filled;

            // Create function to get count of adjacent empty cells
            //
            // NOTE*** CELLULAR_AUTOMATA_PADDING prevents out of bounds errors
            var emptyCellCountFunc = new Func<GridCellInfo[,], int, int, int, int, int>((gridArray, column, row, aggregator, distance) =>
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
            IterateCellularAutomata(grid, boundary, 1, (gridArray, column, row) =>
            {
                if (grid[column, row] != null && !overwrite)
                    throw new Exception("Trying to overwrite existing grid cell CellularAutomataRegionCreator");

                // Fill Ratio => Empty Cell
                return _randomSequenceGenerator.Get() < scaledFillRatio;
            });

            // Iterate grid to apply Filled Rule - Apply this for several iterations before smoothing
            if (filled)
            {
                IterateCellularAutomata(grid, boundary, CELLULAR_AUTOMATA_ITERATIONS, (gridArray, column, row) =>
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
            IterateCellularAutomata(grid, boundary, filled ? 1 : CELLULAR_AUTOMATA_ITERATIONS, (gridArray, column, row) =>
            {
                return (emptyCellCountFunc(gridArray, column, row, 0, 0) + emptyCellCountFunc(gridArray, column, row, 0, 1)) >= 5;
            });
        }



        private void IterateCellularAutomata(GridCellInfo[,] grid, RegionBoundary boundary, int numberOfIterations, Func<GridCellInfo[,], int, int, bool> cellularAutomataRule)
        {
            for (int k = 0; k < numberOfIterations; k++)
            {
                for (int i = boundary.Left + CELLULAR_AUTOMATA_PADDING; i <= boundary.Right - CELLULAR_AUTOMATA_PADDING; i++)
                {
                    for (int j = boundary.Top + CELLULAR_AUTOMATA_PADDING; j <= boundary.Bottom - CELLULAR_AUTOMATA_PADDING; j++)
                    {
                        if (cellularAutomataRule(grid, i, j))
                            grid[i, j] = null;

                        else
                            grid[i, j] = new GridCellInfo(i, j);
                    }
                }
            }
        }
    }
}
