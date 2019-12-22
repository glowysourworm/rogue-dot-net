using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Model.Content.Calculator;
using Rogue.NET.Core.Processing.Model.Extension;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Finishing.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Finishing
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IWallFinisher))]
    public class WallFinisher : IWallFinisher
    {
        public WallFinisher()
        {

        }

        public void CreateDoors(GridCellInfo[,] grid, IEnumerable<Region<GridCellInfo>> regions, IEnumerable<LayerInfo> terrainLayers)
        {
            foreach (var region in regions)
            {
                // Iterate edge locations
                foreach (var location in region.EdgeLocations)
                {
                    // Look for locations with a 4-way adjacent corridor - NOT in ANY region
                    foreach (var adjacentCorridor in grid.GetCardinalAdjacentElements(location.Column, location.Row)
                                                         .Where(cell => cell.IsCorridor &&
                                                                       !cell.IsDoor &&
                                                                       !regions.Any(otherRegion => otherRegion[cell.Column, cell.Row] != null)))
                    {
                        // Rules
                        //
                        // 1) Must have two directly adjacent walls to the door (on either side)
                        // 2) Must have a walkable cell opposite the door

                        var direction = GridCalculator.GetDirectionOfAdjacentLocation(location, adjacentCorridor);
                        GridCellInfo oppositeDoorCell = null;
                        GridCellInfo wallCell1 = null;
                        GridCellInfo wallCell2 = null;
                        
                        // Check for the adjacent walls
                        switch (direction)
                        {
                            case Compass.N:
                            case Compass.S:
                                {
                                    wallCell1 = grid.Get(adjacentCorridor.Column - 1, adjacentCorridor.Row);
                                    wallCell2 = grid.Get(adjacentCorridor.Column + 1, adjacentCorridor.Row);
                                }
                                break;
                            case Compass.E:
                            case Compass.W:
                                {
                                    wallCell1 = grid.Get(adjacentCorridor.Column, adjacentCorridor.Row - 1);
                                    wallCell2 = grid.Get(adjacentCorridor.Column, adjacentCorridor.Row + 1);
                                }
                                break;
                            default:
                                throw new Exception("Unhandled door adjacent location");
                        }

                        // Check for walkable cell opposite the door                        
                        switch (direction)
                        {
                            case Compass.N:
                                {
                                    oppositeDoorCell = grid.Get(adjacentCorridor.Column, adjacentCorridor.Row - 1);
                                }
                                break;
                            case Compass.S:
                                {
                                    oppositeDoorCell = grid.Get(adjacentCorridor.Column, adjacentCorridor.Row + 1);
                                }
                                break;
                            case Compass.E:
                                {
                                    oppositeDoorCell = grid.Get(adjacentCorridor.Column + 1, adjacentCorridor.Row);
                                }
                                break;
                            case Compass.W:
                                {
                                    oppositeDoorCell = grid.Get(adjacentCorridor.Column - 1, adjacentCorridor.Row);
                                }
                                break;
                            default:
                                throw new Exception("Unhandled door adjacent location");
                        }

                        adjacentCorridor.IsDoor = wallCell1 != null &&
                                                  wallCell2 != null &&
                                                  wallCell1.IsWall &&
                                                  wallCell2.IsWall &&
                                                  oppositeDoorCell != null &&
                                                 !oppositeDoorCell.IsWall &&
                                                 !terrainLayers.Any(layer => !layer.IsPassable && layer[oppositeDoorCell.Column, oppositeDoorCell.Row] != null);
                    }
                }
            }
        }

        public void CreateWalls(GridCellInfo[,] grid, bool createBorder)
        {
            var bounds = new RegionBoundary(new GridLocation(0, 0), grid.GetLength(0), grid.GetLength(1));
            var walls = new List<GridCellInfo>();

            // Iterate - leaving a padding of 1 on the edge to create cells for walls
            //           without stepping out of bounds
            //
            for (int i = 1; i < bounds.Width - 1; i++)
            {
                for (int j = 1; j < bounds.Height - 1; j++)
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

            // Optionally, create border
            if (createBorder)
            {
                for (int i=0;i<grid.GetLength(0);i++)
                {
                    grid[i, 0] = new GridCellInfo(i, 0) { IsWall = true };
                    grid[i, grid.GetLength(1) - 1] = new GridCellInfo(i, grid.GetLength(1) - 1) { IsWall = true };
                }

                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    grid[0, j] = new GridCellInfo(0, j) { IsWall = true };
                    grid[grid.GetLength(0) - 1, j] = new GridCellInfo(grid.GetLength(0) - 1, j) { IsWall = true };
                }
            }
        }
    }
}
