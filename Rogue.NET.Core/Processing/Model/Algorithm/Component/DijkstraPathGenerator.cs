using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Construction;

using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Algorithm.Component
{
    public class DijkstraPathGenerator : DijkstraMapBase
    {
        /// <summary>
        /// Callback that allows setting properties of the embedded path cells
        /// </summary>
        public delegate void DijkstraEmbedPathCallback(GridCellInfo pathCell);

        readonly GridCellInfo[,] _grid;

        public DijkstraPathGenerator(GridCellInfo[,] grid,
                                     IEnumerable<Region<GridCellInfo>> avoidRegions,
                                     GridCellInfo sourceLocation,
                                     IEnumerable<GridCellInfo> targetLocations,
                                     bool obeyCardinalMovement)
             : base(grid.GetLength(0),
                    grid.GetLength(1),
                    obeyCardinalMovement,
                    sourceLocation,
                    targetLocations,
                    new DijkstraMapCostCallback((column1, row1, column2, row2) =>
                    {
                        // MOVEMENT COST BASED ON THE DESTINATION CELL
                        //
                        if (avoidRegions.Any(region => region[column2, row2] != null))
                            return DijkstraMapBase.MapCostAvoid;

                        else
                            return 0;

                    }), new DijkstraMapLocatorCallback((column, row) =>
                    {
                        // ALLOCATE NEW GRID CELLS FOR CREATING NEW PATHS
                        return grid[column, row] ?? new GridCellInfo(column, row);
                    }))
        {
            _grid = grid;
        }

        public void EmbedPaths(DijkstraEmbedPathCallback callback)
        {
            Run();

            // Create paths for each target
            foreach (var targetLocation in this.TargetLocations)
            {
                foreach (var cell in GeneratePath(targetLocation))
                {
                    // Allow setting properties on cells from the new path
                    callback(cell as GridCellInfo);

                    // Embed the cell
                    _grid[cell.Column, cell.Row] = cell as GridCellInfo;
                }
            }
        }
    }
}
