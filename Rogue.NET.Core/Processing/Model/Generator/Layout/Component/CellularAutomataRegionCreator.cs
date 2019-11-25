using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Model.Extension;
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
        private const int CELLULAR_AUTOMATA_ITERATIONS = 5;
        private const int CELLULAR_AUTOMATA_OPEN_PADDING = 1;
        private const int CELLULAR_AUTOMATA_FILLED_PADDING = 1;

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
            var filled = type == LayoutCellularAutomataType.FilledLess || type == LayoutCellularAutomataType.FilledMore;

            // Iterate grid cells and fill them in randomly
            // 
            
            Iterate(grid, boundary, 1, CELLULAR_AUTOMATA_FILLED_PADDING, (grid, boundary, column, row) =>
            {
                if (grid[column, row] != null && !overwrite)
                    throw new Exception("Trying to overwrite existing grid cell CellularAutomataRegionCreator");

                // Fill Ratio => Empty Cell
                return _randomSequenceGenerator.Get() < scaledFillRatio;
            });

            // Iterate grid to apply Filled Rule - Apply this for several iterations before smoothing
            switch (type)
            {
                case LayoutCellularAutomataType.Open:
                    Iterate(grid, boundary, CELLULAR_AUTOMATA_ITERATIONS, CELLULAR_AUTOMATA_FILLED_PADDING, CellularAutomataOpenRule);
                    break;
                case LayoutCellularAutomataType.FilledLess:
                    {
                        Iterate(grid, boundary, CELLULAR_AUTOMATA_ITERATIONS, CELLULAR_AUTOMATA_FILLED_PADDING, CellularAutomataFilledLessRule);

                        // Iterate grid to apply Open Rule - This will smooth out the noise in the grid. Also,
                        // it is used in conjunction with the Filled rule to fill out large open spaces after
                        // the Filled rule is applied
                        Iterate(grid, boundary, 1, CELLULAR_AUTOMATA_FILLED_PADDING, CellularAutomataOpenRule);
                    }
                    break;
                case LayoutCellularAutomataType.FilledMore:
                    {
                        Iterate(grid, boundary, CELLULAR_AUTOMATA_ITERATIONS, CELLULAR_AUTOMATA_FILLED_PADDING, CellularAutomataFilledMoreRule);

                        // Iterate grid to apply Open Rule - This will smooth out the noise in the grid. Also,
                        // it is used in conjunction with the Filled rule to fill out large open spaces after
                        // the Filled rule is applied
                        Iterate(grid, boundary, 1, CELLULAR_AUTOMATA_FILLED_PADDING, CellularAutomataOpenRule);
                    }
                    break;
                default:
                    throw new Exception("Unhandled cellular automata type");
            }           
        }

        public void RunSmoothingIteration(GridCellInfo[,] grid, RegionBoundary boundary, double roughnessRatio)
        {
            //// Apply roughness
            //if (roughnessRatio > 0)
            //{
            //    Iterate(grid, boundary, 1, 0, (grid, boundary, column, row) =>
            //    {
            //        // Fill Ratio => Empty Cell
            //        return _randomSequenceGenerator.Get() < roughnessRatio;
            //    });
            //}

            Iterate(grid, boundary, 1, CELLULAR_AUTOMATA_OPEN_PADDING, CellularAutomataFilledLessRule);
        }

        private bool CellularAutomataOpenRule(GridCellInfo[,] grid, RegionBoundary boundary, int column, int row)
        {
            if (boundary.CellWidth < 3 ||
                boundary.CellHeight < 3)
                throw new ArgumentException("Trying to iterate cellular automata with too small of a boundary");

            return (Count(grid, boundary, column, row, 0) + Count(grid, boundary, column, row, 1)) >= 5;
        }

        private bool CellularAutomataFilledMoreRule(GridCellInfo[,] grid, RegionBoundary boundary, int column, int row)
        {
            var empty0Count = Count(grid, boundary, column, row, 0);
            var empty1Count = Count(grid, boundary, column, row, 1);

            if ((empty0Count + empty1Count) >= 5)
                return true;

            var empty2Count = Count(grid, boundary, column, row, 2);

            return (empty0Count + empty1Count + empty2Count) <= 7;
        }

        private bool CellularAutomataFilledLessRule(GridCellInfo[,] grid, RegionBoundary boundary, int column, int row)
        {
            var empty0Count = Count(grid, boundary, column, row, 0);
            var empty1Count = Count(grid, boundary, column, row, 1);

            if ((empty0Count + empty1Count) >= 5)
                return true;

            var empty2Count = Count(grid, boundary, column, row, 2);

            return (empty0Count + empty1Count + empty2Count) <= 6;
        }

        private int Count(GridCellInfo[,] grid, RegionBoundary boundary, int column, int row, int distance)
        {
            var count = 0;

            if (distance == 0)
                return count + (grid[column, row] == null ? 1 : 0);

            // Count Top and Bottom first
            for (int i = column - distance; i <= column + distance; i++)
            {
                // Avoid region boundaries
                if (i < boundary.Left ||
                    i > boundary.Right ||
                    row - distance < boundary.Top ||
                    row + distance > boundary.Bottom)
                    continue;

                count += grid[i, row - distance] == null ? 1 : 0;
                count += grid[i, row + distance] == null ? 1 : 0;
            }

            // Count Left and Right - excluding corners
            for (int j = (row - distance) + 1; j <= (row + distance) - 1; j++)
            {
                // Avoid region boundaries
                if (column - distance < boundary.Left ||
                    column + distance > boundary.Right ||
                    j < boundary.Top ||
                    j > boundary.Bottom)
                    continue;

                count += grid[column - distance, j] == null ? 1 : 0;
                count += grid[column + distance, j] == null ? 1 : 0;
            }

            return count;
        }

        private void Iterate(GridCellInfo[,] grid, RegionBoundary boundary, int numberOfIterations, int padding, Func<GridCellInfo[,], RegionBoundary, int, int, bool> fillRule)
        {
            for (int k = 0; k < numberOfIterations; k++)
            {
                for (int i = boundary.Left + padding; i <= boundary.Right - padding; i++)
                {
                    for (int j = boundary.Top + padding; j <= boundary.Bottom - padding; j++)
                    {
                        if (fillRule(grid, boundary, i, j))
                            grid[i, j] = null;

                        else
                            grid[i, j] = new GridCellInfo(i, j);
                    }
                }
            }
        }
    }
}
