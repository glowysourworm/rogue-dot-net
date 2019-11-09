﻿using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Model.Extension;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Region.Creator
{
    public static class MazeRegionCreator
    {
        static readonly IRandomSequenceGenerator _randomSequenceGenerator;

        static MazeRegionCreator()
        {
            _randomSequenceGenerator = ServiceLocator.Current.GetInstance<IRandomSequenceGenerator>();
        }

        /// <summary>
        /// Creates maze inside the specified grid by removing walls from the 2D cell array. Assumed that the entire
        /// grid is filled with wall cells.
        /// </summary>
        public static void CreateMaze(GridCellInfo[,] grid, int numberWallRemovals)
        {
            // Choose random starting cell
            var randomColumn = _randomSequenceGenerator.Get(0, grid.GetLength(0));
            var randomRow = _randomSequenceGenerator.Get(0, grid.GetLength(1));

            RecursiveBacktracker(grid, grid[randomColumn, randomRow], numberWallRemovals);
        }

        /// <summary>
        /// Creates maze inside the specified grid by removing walls from the 2D cell array. Assumed that the 
        /// starting cell is a wall.
        /// </summary>
        public static void CreateMaze(GridCellInfo[,] grid, int startingColumn, int startingRow, int numberWallRemovals)
        {
            RecursiveBacktracker(grid, grid[startingColumn, startingRow], numberWallRemovals);
        }

        private static void RecursiveBacktracker(GridCellInfo[,] grid, GridCellInfo startingCell, int numberWallRemovals)
        {
            // Pre-Condition: All cells in the region must be filled with walls
            //
            // Procedure:  Region grid is a sub-grid of the whole grid. This has been initialized to the 
            //             parameters given { column, row, width, height }.
            //
            // 1) Choose random cell in the region (THIS IS BY DESIGN)
            // 2) Initialize the recursive back-tracking algorithm
            //      - Begin history with the chosen cell
            //      - While history > 0:
            //          - Choose N, S, E, W (randomly)
            //          - Chosen Current Cell:
            //              - Check to see if cell is viable (there's no surrounding cells that have been removed)
            //              - Remove the wall, push it on the stack, mark visited
            //
            //          - If all adjacent cells visited
            //              - Remove the most recent cell
            //              - Back-track one and continue
            //
            // 3) When the loop completes - the history will be empty
            // 4) Remove (at random) some of the walls to make it a little easier
            //

            // Keep track of visited cells
            var visitedCells = new bool[grid.GetLength(0), grid.GetLength(1)];

            // Track from the starting cell
            var currentCell = startingCell;

            // Initialize the history
            var history = new Stack<GridCellInfo>();
            history.Push(currentCell);

            // Set the first cell
            currentCell.IsWall = false;
            visitedCells[currentCell.Location.Column, currentCell.Location.Row] = true;

            //Main loop - create the maze!
            while (history.Count > 0)
            {
                // Get all unvisited neighbor cells - whose neighbor cells haven't been visited yet
                //
                var adjacentCells = grid.GetCardinalAdjacentElements(currentCell.Location.Column, currentCell.Location.Row)
                                        .Where(cell => cell.IsWall)
                                        .Where(cell => !visitedCells[cell.Location.Column, cell.Location.Row])
                                        .Actualize();

                // Have tried all possibilities at this location - so back track
                if (adjacentCells.None())
                {
                    // Back at the beginning - time to terminate loop
                    if (history.Count == 1)
                        break;

                    // Back-track one cell in the history and continue
                    else
                        currentCell = history.Pop();
                }

                // Try directions randomly to continue iteration
                else
                {
                    // NOTE*** Be sure to use the method from the random sequence generator since
                    //         we're building the scenario
                    //
                    var nextCell = _randomSequenceGenerator.GetRandomElement(adjacentCells);

                    // Track that the cell has been visited
                    //
                    visitedCells[nextCell.Location.Column, nextCell.Location.Row] = true;

                    // Run query to see whether this cell can be used in the maze
                    //
                    var viableCell = FiveAdjacentWallsDirectionalRule(currentCell, nextCell, grid.GetAdjacentElements(nextCell.Location.Column, nextCell.Location.Row));

                    // If any neighbor cells CAN be visited - then push the current one on the stack
                    if (viableCell)
                    {
                        // Remove the wall
                        nextCell.IsWall = false;

                        // Push on the stack
                        history.Push(nextCell);

                        // Increment follower
                        currentCell = nextCell;
                    }
                }
            }

            // Process wall removals
            //for (int i = 0; i < numberWallRemovals; i++)
            //{
            //    // Choose random cell in the region
            //    var column = _randomSequenceGenerator.Get(regionColumn, regionColumn + regionWidth);
            //    var row = _randomSequenceGenerator.Get(regionRow, regionRow + regionHeight);

            //    // Remove the wall setting
            //    grid[column, row].SetWall(false);
            //}
        }

        /// <summary>
        /// Returns true if all 4-way adjacent cells are walls (Except the current cell)
        /// </summary>
        private static bool CardinalAdjacentWallsRule(GridCellInfo currentCell, GridCellInfo nextCell, GridCellInfo[] nextCell8WayNeighbors)
        {
            return nextCell8WayNeighbors.Where(cell => cell != currentCell)
                                        .Where(cell => GridUtility.IsCardinalDirection(GridUtility.GetDirectionOfAdjacentLocation(nextCell.Location, cell.Location)))
                                        .All(cell => cell.IsWall);
        }

        /// <summary>
        /// Returns true if all 5 adjacent cells to the next cell are walls. This prevents neighboring off-diagonal walls that separate
        /// passages - making the maze look much more complete. 
        /// </summary>
        private static bool FiveAdjacentWallsRule(GridCellInfo currentCell, GridCellInfo nextCell, GridCellInfo[] nextCell8WayNeighbors)
        {
            return nextCell8WayNeighbors.Where(cell => cell != currentCell)
                                        .Count(cell => cell.IsWall) >= 5;
        }

        /// <summary>
        /// Same as the FiveAdjacentWallsRule with the added condition that the 5 adjacent walls must be "in the direction of
        /// travel". Example:  Movement Direction = E. Then, the set { N, NE, E, SE, S } must ALL be walls.
        /// </summary>
        private static bool FiveAdjacentWallsDirectionalRule(GridCellInfo currentCell, GridCellInfo nextCell, GridCellInfo[] nextCell8WayNeighbors)
        {
            var direction = GridUtility.GetDirectionOfAdjacentLocation(currentCell.Location, nextCell.Location);
            var neighborWallDirections = nextCell8WayNeighbors.Where(cell => cell.IsWall)
                                                              .Select(cell => GridUtility.GetDirectionOfAdjacentLocation(nextCell.Location, cell.Location))
                                                              .Actualize();

            // Check directional flag (OR'ed) to make sure ALL 5 cells are accounted for
            //
            switch (direction)
            {
                case Compass.N:
                    return neighborWallDirections.Count(x => x.Has(Compass.W | Compass.NW | Compass.N | Compass.NE | Compass.E)) >= 5;
                case Compass.S:
                    return neighborWallDirections.Count(x => x.Has(Compass.W | Compass.SW | Compass.S | Compass.SE | Compass.E)) >= 5;
                case Compass.E:
                    return neighborWallDirections.Count(x => x.Has(Compass.N | Compass.NE | Compass.E | Compass.SE | Compass.S)) >= 5;
                case Compass.W:
                    return neighborWallDirections.Count(x => x.Has(Compass.N | Compass.NW | Compass.W | Compass.SW | Compass.S)) >= 5;
                case Compass.NW:
                case Compass.NE:
                case Compass.SE:
                case Compass.SW:
                case Compass.Null:
                default:
                    throw new Exception("Invalid maze generation movement");
            }
        }
    }
}