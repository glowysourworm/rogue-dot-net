using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Model.Content.Calculator;
using Rogue.NET.Core.Processing.Model.Extension;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Construction;
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

        public void CreateDoors(LayoutContainer container)
        {
            // Set door locations for each graph connection
            if (container.GetConnectionGraph().HasEdges())
            {
                foreach (var connection in container.GetConnectionGraph().GetConnections())
                {
                    //connection.DoorLocation.IsDoor = true;
                    //connection.AdjacentDoorLocation.IsDoor = true;
                }
            }

            /* OLD ROUTINE - PER REGION, ITERATE...
             * 
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
             */
        }

        public void CreateWalls(LayoutContainer container)
        {
            // Create new wall cells to add to the layout
            var walls = new List<GridCellInfo>();

            // Iterate - leaving a padding of 1 on the edge to create cells for walls
            //           without stepping out of bounds
            //
            for (int column = 1; column < container.Width - 1; column++)
            {
                for (int row = 1; row < container.Height - 1; row++)
                {
                    // Locate a region or corridor
                    if (container.Get(column, row) == null)
                        continue;

                    // Check the surrounding grid for empty cells

                    // North wall
                    if (container.Get(column, row - 1) == null)
                        walls.Add(new GridCellInfo(new GridLocation(column, row - 1)) { IsWall = true });

                    // South wall
                    if (container.Get(column, row + 1) == null)
                        walls.Add(new GridCellInfo(new GridLocation(column, row + 1)) { IsWall = true });

                    // West wall
                    if (container.Get(column - 1, row) == null)
                        walls.Add(new GridCellInfo(new GridLocation(column - 1, row)) { IsWall = true });

                    // East wall
                    if (container.Get(column + 1, row) == null)
                        walls.Add(new GridCellInfo(new GridLocation(column + 1, row)) { IsWall = true });

                    // North-East wall
                    if (container.Get(column + 1, row - 1) == null)
                        walls.Add(new GridCellInfo(new GridLocation(column + 1, row - 1)) { IsWall = true });

                    // South-East wall
                    if (container.Get(column + 1, row + 1) == null)
                        walls.Add(new GridCellInfo(new GridLocation(column + 1, row + 1)) { IsWall = true });

                    // North-West wall
                    if (container.Get(column - 1, row - 1) == null)
                        walls.Add(new GridCellInfo(new GridLocation(column - 1, row - 1)) { IsWall = true });

                    // South-West wall
                    if (container.Get(column - 1, row + 1) == null)
                        walls.Add(new GridCellInfo(new GridLocation(column - 1, row + 1)) { IsWall = true });
                }
            }

            // Add wall cells to the grid
            foreach (var cell in walls)
            {
                var existingCell = container.Get(cell.Column, cell.Row);
                if (existingCell == null)
                    container.AddLayout(cell.Location.Column, cell.Location.Row, cell);

                else
                    existingCell.IsWall = true;
            }
        }
    }
}
