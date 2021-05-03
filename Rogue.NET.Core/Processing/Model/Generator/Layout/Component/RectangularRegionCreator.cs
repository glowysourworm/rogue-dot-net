using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Model.Extension;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Construction;

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
        readonly ICellularAutomataRegionCreator _cellularAutomataRegionCreator;
        readonly IRandomSequenceGenerator _randomSequenceGenerator;

        [ImportingConstructor]
        public RectangularRegionCreator(ICellularAutomataRegionCreator cellularAutomataRegionCreator,
                                        IRandomSequenceGenerator randomSequenceGenerator)
        {
            _cellularAutomataRegionCreator = cellularAutomataRegionCreator;
            _randomSequenceGenerator = randomSequenceGenerator;
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
            // Cells within the intersection
            var intersection = new List<GridCellInfo>();

            // Cells within the intersection + the new region cells
            var join = new List<GridCellInfo>();

            // Procedure
            //
            // 1) Iterate grid within the boundary to find cells that have been CREATED
            // 2) Separate into two collections: 
            //      - One with all new cells for the boundary
            //      - One with all new cells in the intersection OR previously created cells
            // 3) Add padded tiles to the intersection collection RANDOMLY USING SEPARATION RATIO
            // 4) Use Cellular Automata rule to fill in the intersection + padded region
            // 5) Add cells created for the join
            //

            for (int i = boundary.Left; i <= boundary.Right; i++)
            {
                for (int j = boundary.Top; j <= boundary.Bottom; j++)
                {
                    // Intersection + Padding
                    if (grid[i, j] != null)
                        intersection.Add(grid[i, j]);

                    // New Region -> ADD CELLS TO GRID
                    else
                        grid[i, j] = new GridCellInfo(i, j);
                }
            }

            // PROCESS INTERSECTION
            if (intersection.Count > 0)
            {
                var paddedColumnLeft = (intersection.Min(cell => cell.Column) - padding).LowLimit(ModelConstants.LayoutGeneration.LayoutPadding);
                var paddedColumnRight = (intersection.Max(cell => cell.Column) + padding).HighLimit(grid.GetLength(0) - ModelConstants.LayoutGeneration.LayoutPadding);

                var paddedRowTop = (intersection.Min(cell => cell.Row) - padding).LowLimit(ModelConstants.LayoutGeneration.LayoutPadding);
                var paddedRowBottom = (intersection.Max(cell => cell.Row) + padding).HighLimit(grid.GetLength(1) - ModelConstants.LayoutGeneration.LayoutPadding);

                var paddedBoundary = new RegionBoundary(paddedColumnLeft,
                                                         paddedRowTop,
                                                         paddedColumnRight - paddedColumnLeft,
                                                         paddedRowBottom - paddedRowTop);

                _cellularAutomataRegionCreator.GenerateCells(grid, paddedBoundary, LayoutCellularAutomataType.FilledLess, 1 - separationRatio, true);
            }
        }
    }
}