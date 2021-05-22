using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Algorithm;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Construction;

using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Builder
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(ISymmetricBuilder))]
    public class SymmetricBuilder : ISymmetricBuilder
    {
        readonly IRegionBuilder _regionBuilder;

        [ImportingConstructor]
        public SymmetricBuilder(IRegionBuilder regionBuilder)
        {
            _regionBuilder = regionBuilder;
        }

        public GridCellInfo[,] CreateSymmetricLayout(LayoutTemplate template)
        {
            // Procedure
            //
            // 0) Generate the BASE region layer
            // 1) Execute the grid folding - copy over cell information
            //

            // Step 0 - Base + Initial Connection Layer
            var grid = _regionBuilder.BuildRegions(template);

            // Step 1 - Grid Folding
            ExecuteLayoutFolding(grid, template);

            return grid;
        }

        private void ExecuteLayoutFolding(GridCellInfo[,] grid, LayoutTemplate template)
        {
            switch (template.SymmetryType)
            {
                case LayoutSymmetryType.LeftRight:
                    {
                        // E Half
                        for (int i = 0; i < grid.GetLength(0) / 2; i++)
                        {
                            var mirrorColumn = grid.GetLength(0) - i - 1;

                            for (int j = 0; j < grid.GetLength(1); j++)
                            {
                                // E -> W
                                if (grid[i, j] != null)
                                    grid[mirrorColumn, j]=  new GridCellInfo(mirrorColumn, j);

                                // W -> E
                                else if (grid[mirrorColumn, j] != null)
                                    grid[i, j] =  new GridCellInfo(i, j);
                            }
                        }
                    }
                    break;
                case LayoutSymmetryType.Quadrant:
                    {
                        // NE, SE, SW Quadrants
                        for (int i = 0; i < grid.GetLength(0) / 2; i++)
                        {
                            var mirrorColumn = grid.GetLength(0) - i - 1;

                            for (int j = 0; j < grid.GetLength(1) / 2; j++)
                            {
                                var mirrorRow = grid.GetLength(1) - j - 1;

                                GridCellInfo[] cells = new GridCellInfo[4];

                                // Find cell to mirror - start with NW

                                // NW
                                if (grid[i, j] != null)
                                    cells[0] = grid[i, j];

                                // NE
                                else if (grid[mirrorColumn, j] != null)
                                    cells[1] = grid[mirrorColumn, j];

                                // SE
                                else if (grid[mirrorColumn, mirrorRow] != null)
                                    cells[2] = grid[mirrorColumn, mirrorRow];

                                // SW
                                else if (grid[i, mirrorRow] != null)
                                    cells[3] = grid[i, mirrorRow];

                                var cell = cells.FirstOrDefault(x => x != null);

                                // Mirror cell over to other quadrants
                                if (cell != null)
                                {
                                    grid[i, j] = new GridCellInfo(i, j);

                                    grid[mirrorColumn, j] = new GridCellInfo(mirrorColumn, j);

                                    grid[i, mirrorRow] = new GridCellInfo(i, mirrorRow);

                                    grid[mirrorColumn, mirrorRow] = new GridCellInfo(mirrorColumn, mirrorRow);
                                }
                            }
                        }
                    }
                    break;
                default:
                    throw new Exception("Unhandled symmetry type SymmetryBuilder");
            }
        }
    }
}
