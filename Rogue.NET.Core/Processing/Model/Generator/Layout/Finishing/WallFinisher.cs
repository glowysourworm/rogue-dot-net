using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Finishing.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Finishing
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IWallFinisher))]
    public class WallFinisher : IWallFinisher
    {
        public WallFinisher()
        {

        }

        public void CreateWalls(GridCellInfo[,] grid)
        {
            var bounds = new RegionBoundary(new GridLocation(0, 0), grid.GetLength(0), grid.GetLength(1));
            var walls = new List<GridCellInfo>();

            // Iterate - leaving a padding of 1 on the edge to create cells for walls
            //           without stepping out of bounds
            //
            for (int i = 1; i < bounds.CellWidth - 1; i++)
            {
                for (int j = 1; j < bounds.CellHeight - 1; j++)
                {
                    // Locate a region or corridor
                    if (grid[i, j] == null)
                        continue;

                    // Check the surrounding grid for empty cells

                    // North wall
                    if (grid[i, j - 1] == null)
                        walls.Add(new GridCellInfo(new GridLocation(i, j - 1)) { IsWall = true });

                    // South wall
                    if (grid[i, j + 1] == null)
                        walls.Add(new GridCellInfo(new GridLocation(i, j + 1)) { IsWall = true });

                    // West wall
                    if (grid[i - 1, j] == null)
                        walls.Add(new GridCellInfo(new GridLocation(i - 1, j)) { IsWall = true });

                    // East wall
                    if (grid[i + 1, j] == null)
                        walls.Add(new GridCellInfo(new GridLocation(i + 1, j)) { IsWall = true });

                    // North-East wall
                    if (grid[i + 1, j - 1] == null)
                        walls.Add(new GridCellInfo(new GridLocation(i + 1, j - 1)) { IsWall = true });

                    // South-East wall
                    if (grid[i + 1, j + 1] == null)
                        walls.Add(new GridCellInfo(new GridLocation(i + 1, j + 1)) { IsWall = true });

                    // North-West wall
                    if (grid[i - 1, j - 1] == null)
                        walls.Add(new GridCellInfo(new GridLocation(i - 1, j - 1)) { IsWall = true });

                    // South-West wall
                    if (grid[i - 1, j + 1] == null)
                        walls.Add(new GridCellInfo(new GridLocation(i - 1, j + 1)) { IsWall = true });
                }
            }

            // Add wall cells to the grid
            foreach (var cell in walls)
                grid[cell.Location.Column, cell.Location.Row] = cell;
        }
    }
}
