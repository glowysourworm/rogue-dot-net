using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Model.Content.Calculator;
using Rogue.NET.Core.Processing.Model.Extension;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Construction;

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

using static Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface.IMazeRegionCreator;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Component
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IMazeRegionCreator))]
    public class MazeRegionCreator : IMazeRegionCreator
    {
        readonly IRandomSequenceGenerator _randomSequenceGenerator;

        // Scales [0, 1] fill ratio to a safe scale
        private const double MAZE_WALL_REMOVAL_LOW = 0.0;
        private const double MAZE_WALL_REMOVAL_HIGH = 0.5;

        [ImportingConstructor]
        public MazeRegionCreator(IRandomSequenceGenerator randomSequenceGenerator)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
        }

        public void CreateCells(GridCellInfo[,] grid, RegionBoundary boundary, MazeType mazeType, double wallRemovalRatio, double horizontalVerticalBias, bool overwrite)
        {
            // Pre-Condition:  Fill cells with walls
            for (int i = boundary.Left; i <= boundary.Right; i++)
            {
                for (int j = boundary.Top; j <= boundary.Bottom; j++)
                {
                    if (grid[i, j] != null && !overwrite)
                        throw new Exception("Trying to overwrite existing cell MazeRegionCreator");

                    grid[i, j] = new GridCellInfo(i, j) { IsWall = true };
                }
            }

            // Choose random starting location
            var startingColumn = _randomSequenceGenerator.Get(boundary.Left, boundary.Right + 1);
            var startingRow = _randomSequenceGenerator.Get(boundary.Top, boundary.Bottom + 1);

            RecursiveBacktracker(grid, new Region<GridCellInfo>[] { }, grid[startingColumn, startingRow].Location, wallRemovalRatio, horizontalVerticalBias, mazeType);
        }

        public void CreateCellsStartingAt(GridCellInfo[,] grid, IEnumerable<Region<GridCellInfo>> avoidRegions, GridLocation startingLocation, MazeType mazeType, double wallRemovalRatio, double horizontalVerticalBias)
        {
            if (grid[startingLocation.Column, startingLocation.Row] == null)
                throw new ArgumentException("Invalid starting location MazeRegionCreator.CreateCellsStartingAt");

            RecursiveBacktracker(grid, avoidRegions, startingLocation, wallRemovalRatio, horizontalVerticalBias, mazeType);
        }

        private void RecursiveBacktracker(GridCellInfo[,] grid, IEnumerable<Region<GridCellInfo>> avoidRegions, GridLocation startingLocation, double wallRemovalRatio, double horizontalVerticalBias, MazeType mazeType)
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
            var currentCell = grid[startingLocation.Column, startingLocation.Row];

            // Initialize the history
            var history = new Stack<GridCellInfo>();
            history.Push(currentCell);

            // Set the first cell
            currentCell.IsWall = false;
            visitedCells[currentCell.Location.Column, currentCell.Location.Row] = true;

            //Main loop - create the maze!
            while (history.Count > 0)
            {
                // Get all unvisited neighbor cells - whose neighbor cells haven't been visited yet - and whose cells
                // DON'T lie inside any of the avoid regions.
                //
                var adjacentCells = grid.GetCardinalAdjacentElements(currentCell.Location.Column, currentCell.Location.Row)
                                        .Where(cell => avoidRegions.All(region => region[cell.Location.Column, cell.Location.Row] == null))
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
                    var nextCell = GetCardinalAdjacentCellWithBias(adjacentCells, currentCell, horizontalVerticalBias);

                    // Track that the cell has been visited
                    //
                    visitedCells[nextCell.Location.Column, nextCell.Location.Row] = true;

                    // Run query to see whether this cell can be used in the maze
                    //
                    var viableCell = mazeType == MazeType.Filled ? FiveAdjacentWallsDirectionalRule(currentCell, nextCell, grid.GetAdjacentElements(nextCell.Location.Column, nextCell.Location.Row))
                                                                 : CardinalAdjacentWallsRule(currentCell, nextCell, grid.GetCardinalAdjacentElements(nextCell.Location.Column, nextCell.Location.Row));

                    // If any neighbor cells CAN be visited - then push the current one on the stack
                    if (viableCell)
                    {
                        // Remove the wall
                        nextCell.IsWall = false;

                        // Set as corridor
                        nextCell.IsCorridor = true;

                        // Push on the stack
                        history.Push(nextCell);

                        // Increment follower
                        currentCell = nextCell;
                    }
                }
            }

            // For Rectangular Region Mazes - can support wall removal
            //if (mazeBoundary != null)
            //{

            // Scale [0, 1] wall removal ratio
            var safeRemovalRatio = ((MAZE_WALL_REMOVAL_HIGH - MAZE_WALL_REMOVAL_LOW) * wallRemovalRatio) + MAZE_WALL_REMOVAL_LOW;

            // Process wall removals 
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j] == null)
                        continue;

                    // Wall was already removed
                    else if (!grid[i, j].IsWall)
                        continue;

                    // Check avoid regions
                    else if (avoidRegions.Any(region => region[i, j] != null))
                        continue;

                    // Check random wall removal ratio before proceeding
                    else if (_randomSequenceGenerator.Get() > safeRemovalRatio)
                        continue;

                    // Remove the wall setting
                    grid[i, j].IsWall = false;

                    // Set as corridor
                    grid[i, j].IsCorridor = true;
                }
            }
        }

        private GridCellInfo GetCardinalAdjacentCellWithBias(IEnumerable<GridCellInfo> cardinalAdjacentCells, GridCellInfo currentCell, double horizontalVerticalBias)
        {
            var randomDraw = _randomSequenceGenerator.Get();

            // Used to limit bias
            var biasDivisor = 0.8;

            // Horizontal
            if (horizontalVerticalBias < 0.5)
            {
                var horizontalCells = cardinalAdjacentCells.Where(cell =>
                {
                    var direction = GridCalculator.GetDirectionOfAdjacentLocation(currentCell.Location, cell.Location);

                    return direction == Compass.E || direction == Compass.W;
                });

                // Return biased cell
                if (_randomSequenceGenerator.Get() < ((0.5 - horizontalVerticalBias) / biasDivisor) &&
                    horizontalCells.Any())
                    return _randomSequenceGenerator.GetRandomElement(horizontalCells);

                // Return default
                else
                    return _randomSequenceGenerator.GetRandomElement(cardinalAdjacentCells);
            }
            else
            {
                var verticalCells = cardinalAdjacentCells.Where(cell =>
                {
                    var direction = GridCalculator.GetDirectionOfAdjacentLocation(currentCell.Location, cell.Location);

                    return direction == Compass.N || direction == Compass.S;
                });

                // Return biased cell
                if (_randomSequenceGenerator.Get() < ((horizontalVerticalBias - 0.5) / biasDivisor) &&
                    verticalCells.Any())
                    return _randomSequenceGenerator.GetRandomElement(verticalCells);

                // Return default
                else
                    return _randomSequenceGenerator.GetRandomElement(cardinalAdjacentCells);
            }
        }

        /// <summary>
        /// Returns true if all 4-way adjacent cells are walls (Except the current cell)
        /// </summary>
        private static bool CardinalAdjacentWallsRule(GridCellInfo currentCell, GridCellInfo nextCell, GridCellInfo[] nextCell8WayNeighbors)
        {
            return nextCell8WayNeighbors.Where(cell => cell != currentCell)
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
            var direction = GridCalculator.GetDirectionOfAdjacentLocation(currentCell.Location, nextCell.Location);
            var neighborWallDirections = nextCell8WayNeighbors.Where(cell => cell.IsWall)
                                                              .Select(cell => GridCalculator.GetDirectionOfAdjacentLocation(nextCell.Location, cell.Location))
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
