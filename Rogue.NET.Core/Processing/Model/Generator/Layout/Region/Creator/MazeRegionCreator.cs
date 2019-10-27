using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Model.Extension;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
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
        /// Creates maze inside the specified region by removing walls from the 2D cell array.
        /// </summary>
        public static void CreateMaze(Cell[,] grid, RegionBoundary boundary, int numberWallRemovals)
        {
            // TODO:TERRAIN - Create regions "based on the template" (just make some up, or the whole grid)
            //              
            //              - Fill each region with walls to initialize the grid
            //
            //              - Use the recursive back-tracker to "punch-out" walls and create the maze
            //

            // Procedure
            //
            // 0) Initialize the grid (region grid) with wall cells
            // 1) Perform Recursive Backtracking Algorithm on the grid (region grid)
            // 2) Remove walls from the grid (region grid) to make it easier
            //

            RecursiveBacktracker(grid, boundary, numberWallRemovals);
        }

        private static void RecursiveBacktracker(Cell[,] grid, RegionBoundary boundary, int numberWallRemovals)
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

            var regionColumn = boundary.Left;
            var regionRow = boundary.Top;
            var regionWidth = boundary.CellWidth;
            var regionHeight = boundary.CellHeight;

            // Keep track of visited cells
            var visitedCells = new bool[grid.GetLength(0), grid.GetLength(1)];

            // Choose random starting cell
            var randomColumn = _randomSequenceGenerator.Get(regionColumn, regionColumn + regionWidth);
            var randomRow = _randomSequenceGenerator.Get(regionRow, regionRow + regionHeight);
            var currentCell = grid[randomColumn, randomRow];

            // Initialize the history
            var history = new Stack<Cell>();
            history.Push(currentCell);

            // Set the first cell
            currentCell.SetWall(false);
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
                    var viableCell = grid.GetCardinalAdjacentElements(nextCell.Location.Column, nextCell.Location.Row)
                                         .Where(cell => cell != currentCell)
                                         .All(cell => cell.IsWall);

                    // If any neighbor cells CAN be visited - then push the current one on the stack
                    if (viableCell)
                    {
                        // Remove the wall
                        nextCell.SetWall(false);

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
    }
}
