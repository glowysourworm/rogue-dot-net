using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
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

        public bool[,] CreateMaze(int width, int height, 
                                  Region<GridCellInfo> region, 
                                  MazeType mazeType, 
                                  double wallRemovalRatio, 
                                  double horizontalVerticalBias)
        {
            return RecursiveBacktracker(width, height, region, new Region<GridCellInfo>[] { }, wallRemovalRatio, horizontalVerticalBias, mazeType);
        }

        public bool[,] CreateMaze(int width, int height, 
                                  Region<GridCellInfo> region, 
                                  IEnumerable<Region<GridCellInfo>> avoidRegions, 
                                  MazeType mazeType, 
                                  double wallRemovalRatio, 
                                  double horizontalVerticalBias)
        {
            return RecursiveBacktracker(width, height, region, avoidRegions, wallRemovalRatio, horizontalVerticalBias, mazeType);
        }

        bool[,] RecursiveBacktracker(int width,
                                     int height,
                                     Region<GridCellInfo> region,
                                     IEnumerable<Region<GridCellInfo>> avoidRegions, 
                                     double wallRemovalRatio, 
                                     double horizontalVerticalBias, 
                                     MazeType mazeType)
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

            // Create grid of locations REPRESENTING WALLS
            var grid = new GridLocation[width, height];

            // Pre-Condition:  Fill cells with "walls"
            grid.Iterate((column, row) => grid[column, row] = new GridLocation(column, row));

            // Choose random starting location
            var startingLocation = (IGridLocator)_randomSequenceGenerator.GetRandomElement(region.Locations);

            // Keep track of visited locations
            var visitedCells = new bool[grid.GetLength(0), grid.GetLength(1)];

            // Track from the starting location
            var currentLocation = startingLocation;

            // Initialize the history
            var history = new Stack<IGridLocator>();
            history.Push(currentLocation);

            // Set the first cell
            grid[currentLocation.Column, currentLocation.Row] = new GridLocation(currentLocation);
            visitedCells[currentLocation.Column, currentLocation.Row] = true;

            //Main loop - create the maze!
            while (history.Count > 0)
            {
                // Get all unvisited neighbor cells in the region that don't lie inside any of the avoid regions.
                //
                var adjacentLocations = grid.GetCardinalAdjacentElements(currentLocation.Column, currentLocation.Row)
                                            .Where(location => region[currentLocation] != null)
                                            .Where(location => avoidRegions.All(region => region[location] == null))
                                            .Where(location => !visitedCells[location.Column, location.Row])
                                            .Actualize();

                // Have tried all possibilities at this location - so back track
                if (adjacentLocations.None())
                {
                    // Back at the beginning - time to terminate loop
                    if (history.Count == 1)
                        break;

                    // Back-track one cell in the history and continue
                    else
                        currentLocation = history.Pop();
                }

                // Try directions randomly to continue iteration
                else
                {
                    // NOTE*** Be sure to use the method from the random sequence generator since
                    //         we're building the scenario
                    //
                    var nextLocation = GetCardinalAdjacentCellWithBias(adjacentLocations, currentLocation, horizontalVerticalBias);

                    // Track that the cell has been visited
                    //
                    visitedCells[nextLocation.Column, nextLocation.Row] = true;

                    // Run query to see whether this cell can be used in the maze
                    //
                    var viableLocation = mazeType == MazeType.Filled ? FiveAdjacentWallsDirectionalRule(currentLocation, nextLocation, grid.GetAdjacentElements(nextLocation.Column, nextLocation.Row))
                                                                     : CardinalAdjacentWallsRule(currentLocation, nextLocation, grid.GetCardinalAdjacentElements(nextLocation.Column, nextLocation.Row));

                    // If any neighbor cells CAN be visited - then push the current one on the stack
                    if (viableLocation)
                    {
                        // Remove the wall
                        grid[nextLocation.Column, nextLocation.Row] = null;

                        // Push on the stack
                        history.Push(nextLocation);

                        // Increment follower
                        currentLocation = nextLocation;
                    }
                }
            }

            // Scale [0, 1] wall removal ratio
            var safeRemovalRatio = ((MAZE_WALL_REMOVAL_HIGH - MAZE_WALL_REMOVAL_LOW) * wallRemovalRatio) + MAZE_WALL_REMOVAL_LOW;

            // Process wall removals 
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    // Wall was already removed
                    if (grid[i, j] == null)
                        continue;

                    // Check avoid regions
                    else if (avoidRegions.Any(region => region[i, j] != null))
                        continue;

                    // Check random wall removal ratio before proceeding
                    else if (_randomSequenceGenerator.Get() > safeRemovalRatio)
                        continue;

                    // Remove the wall
                    grid[i, j] = null;
                }
            }

            // RETURN A 2D ARRAY OF BOOLEANS AS THE RESULT
            return grid.GridCopy(location => location == null);
        }

        private IGridLocator GetCardinalAdjacentCellWithBias(IEnumerable<IGridLocator> cardinalAdjacentLocations, IGridLocator currentLocation, double horizontalVerticalBias)
        {
            var randomDraw = _randomSequenceGenerator.Get();

            // Used to limit bias
            var biasDivisor = 0.8;

            // Horizontal
            if (horizontalVerticalBias < 0.5)
            {
                var horizontalLocations = cardinalAdjacentLocations.Where(location =>
                {
                    var direction = GridCalculator.GetDirectionOfAdjacentLocation(currentLocation, location);

                    return direction == Compass.E || direction == Compass.W;
                });

                // Return biased cell
                if (_randomSequenceGenerator.Get() < ((0.5 - horizontalVerticalBias) / biasDivisor) &&
                    horizontalLocations.Any())
                    return _randomSequenceGenerator.GetRandomElement(horizontalLocations);

                // Return default
                else
                    return _randomSequenceGenerator.GetRandomElement(cardinalAdjacentLocations);
            }
            else
            {
                var verticalCells = cardinalAdjacentLocations.Where(location =>
                {
                    var direction = GridCalculator.GetDirectionOfAdjacentLocation(currentLocation, location);

                    return direction == Compass.N || direction == Compass.S;
                });

                // Return biased cell
                if (_randomSequenceGenerator.Get() < ((horizontalVerticalBias - 0.5) / biasDivisor) &&
                    verticalCells.Any())
                    return _randomSequenceGenerator.GetRandomElement(verticalCells);

                // Return default
                else
                    return _randomSequenceGenerator.GetRandomElement(cardinalAdjacentLocations);
            }
        }

        /// <summary>
        /// Returns true if all 4-way adjacent cells are walls (Except the current cell)
        /// </summary>
        private static bool CardinalAdjacentWallsRule(IGridLocator currentCell, IGridLocator nextLocation, IGridLocator[] nextCell8WayNeighbors)
        {
            return nextCell8WayNeighbors.Where(cell => cell != currentCell)
                                        .All(cell => cell != null);
        }

        /// <summary>
        /// Returns true if all 5 adjacent cells to the next cell are walls. This prevents neighboring off-diagonal walls that separate
        /// passages - making the maze look much more complete. 
        /// </summary>
        private static bool FiveAdjacentWallsRule(IGridLocator currentLocation, IGridLocator nextLocation, IGridLocator[] nextCell8WayNeighbors)
        {
            return nextCell8WayNeighbors.Where(location => location != currentLocation)
                                        .Count(location => location != null) >= 5;
        }

        /// <summary>
        /// Same as the FiveAdjacentWallsRule with the added condition that the 5 adjacent walls must be "in the direction of
        /// travel". Example:  Movement Direction = E. Then, the set { N, NE, E, SE, S } must ALL be walls.
        /// </summary>
        private static bool FiveAdjacentWallsDirectionalRule(IGridLocator currentLocation, IGridLocator nextLocation, IGridLocator[] nextCell8WayNeighbors)
        {
            var direction = GridCalculator.GetDirectionOfAdjacentLocation(currentLocation, nextLocation);
            var neighborWallDirections = nextCell8WayNeighbors.Where(location => location != null)
                                                              .Select(location => GridCalculator.GetDirectionOfAdjacentLocation(nextLocation, location))
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
