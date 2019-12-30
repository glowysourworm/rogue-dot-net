using Rogue.NET.Common.Collection;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Construction;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using Rogue.NET.Core.Processing.Model.Extension;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Rogue.NET.Core.Math.Geometry
{
    public abstract class DijkstraMapBase
    {
        /// <summary>
        /// Delegate used to fetch a cost for the specified column / row of the grid
        /// </summary>
        public delegate float DijkstraMapCostCallback(int column, int row);

        /// <summary>
        /// Delegate used to fetch a reference to the associated grid locator
        /// </summary>
        public delegate IGridLocator DijkstraMapLocatorCallback(int column, int row);

        /// <summary>
        /// Source location for the Dijkstra scan
        /// </summary>
        protected IGridLocator SourceLocation { get; private set; }

        /// <summary>
        /// Target locations fot the Dijkstra scan
        /// </summary>
        protected IEnumerable<IGridLocator> TargetLocations { get; private set; }

        /// <summary>
        /// Flag for N, S, E, W movement only
        /// </summary>
        protected bool ObeyCardinalMovement { get; private set; }

        /// <summary>
        /// 2D array of output values for Dijkstra's algorithm
        /// </summary>
        protected float[,] OutputMap { get; private set; }

        /// <summary>
        /// Constant that indictaes a region of the grid - typically used for a mask to prevent iterating
        /// back onto the regions.
        /// </summary>
        protected readonly static float RegionFeatureConstant = 10000;

        private readonly int _width;
        private readonly int _height;
        private readonly DijkstraMapCostCallback _costCallback;
        private readonly DijkstraMapLocatorCallback _locatorCallback;

        private bool _hasRun;

        const int CELL_MOVEMENT_COST = 1;
        const float MAP_MAX_VALUE = float.MaxValue;

        protected DijkstraMapBase(int width,
                                  int height,
                                  bool obeyCardinalMovement,
                                  IGridLocator sourceLocation,
                                  IEnumerable<IGridLocator> targetLocations,
                                  DijkstraMapCostCallback costCallback,
                                  DijkstraMapLocatorCallback locatorCallback)
        {
            _width = width;
            _height = height;

            _costCallback = costCallback;
            _locatorCallback = locatorCallback;

            this.ObeyCardinalMovement = obeyCardinalMovement;
            this.OutputMap = new float[width, height];
            this.SourceLocation = sourceLocation;
            this.TargetLocations = targetLocations;
        }

        protected void Run()
        {
            // Initialize the output map
            this.OutputMap.Iterate((column, row) =>
            {
                this.OutputMap[column, row] = ((column == this.SourceLocation.Column) && (row == this.SourceLocation.Row)) ? 0 : MAP_MAX_VALUE;
            });

            // Track visited elements AND queued elements (prevents a LOT of extra looking up on the queue)
            var visitedMap = new bool[_width, _height];

            // Track the frontier cells to check lowest cost next step
            var frontier = new BinarySearchTree<float, List<IGridLocator>>();

            // Track goal progress
            var goalDict = this.TargetLocations
                               .ToDictionary(location => location, location => false);

            // Process the first element
            var column = this.SourceLocation.Column;
            var row = this.SourceLocation.Row;

            // Iterate while any target not reached (AND) not visited
            while (!visitedMap[column, row] &&
                    goalDict.Any(element => !element.Value))
            {
                // Set current parameters
                var currentWeight = this.OutputMap[column, row];

                // Mark the element as visited
                visitedMap[column, row] = true;

                // Search cardinally adjacent elements (N,S,E,W)
                var north = row - 1 >= 0;
                var south = row + 1 < _height;
                var east = column + 1 < _width;
                var west = column - 1 >= 0;

                // Dijkstra Weight = Current Value + ("Change in Location Cost" + "Input Map Cost") 
                //
                // PROBLEM:         Negative gradient "costs" cause problems because they interrupt the
                //                 accumulated weight. Example: Walk-up-and-then-down a mountain. The
                //                 other side of the mountain will subtract off the accumulated cost of
                //                 climbing it.
                //
                // SOLUTION:       Use "Cost Map" instead of "Gradient Map"
                //

                // CARDINAL LOCATIONS
                if (north && !visitedMap[column, row - 1])
                {
                    UpdateOutputMap(frontier, currentWeight, column, row - 1);
                }

                if (south && !visitedMap[column, row + 1])
                {
                    UpdateOutputMap(frontier, currentWeight, column, row + 1);
                }

                if (east && !visitedMap[column + 1, row])
                {
                    UpdateOutputMap(frontier, currentWeight, column + 1, row);
                }

                if (west && !visitedMap[column - 1, row])
                {
                    UpdateOutputMap(frontier, currentWeight, column - 1, row);
                }

                // NON-CARDINAL LOCATIONS
                if (!this.ObeyCardinalMovement && north && east && !visitedMap[column + 1, row - 1])
                {
                    UpdateOutputMap(frontier, currentWeight, column + 1, row - 1);
                }

                if (!this.ObeyCardinalMovement && north && west && !visitedMap[column - 1, row - 1])
                {
                    UpdateOutputMap(frontier, currentWeight, column - 1, row - 1);
                }

                if (!this.ObeyCardinalMovement && south && east && !visitedMap[column + 1, row + 1])
                {
                    UpdateOutputMap(frontier, currentWeight, column + 1, row + 1);
                }

                if (!this.ObeyCardinalMovement && south && west && !visitedMap[column - 1, row + 1])
                {
                    UpdateOutputMap(frontier, currentWeight, column - 1, row + 1);
                }

                // Update goal dictionary
                for (int i = 0; i < goalDict.Count; i++)
                {
                    var element = goalDict.ElementAt(i);

                    if (element.Key.Column == column &&
                        element.Key.Row == row)
                        goalDict[element.Key] = true;
                }

                // Select next location from frontier queue - using the smallest weight
                if (frontier.Count > 0)
                {
                    // Lists in the frontier must have an entry
                    var nextCostList = frontier.Min();
                    var nextCost = frontier.MinKey();

                    // Get the first from the list
                    var nextNode = nextCostList.First();

                    // Maintain frontier list
                    nextCostList.RemoveAt(0);

                    if (nextCostList.Count == 0)
                        frontier.Remove(nextCost);

                    // Move to next location
                    column = nextNode.Column;
                    row = nextNode.Row;
                }
            }

            _hasRun = true;
        }

        private void UpdateOutputMap(BinarySearchTree<float, List<IGridLocator>> frontier, float currentWeight, int column, int row)
        {
            // Procedure
            //
            // 1) Get the existing (old) weight from the output map
            // 2) Calculate the new weight and update the output map
            // 3) Fetch the old / new weight lists from the frontier BST
            // 4) Update the old / new weight lists and the frontier
            //
            // NOTE*** The weight lists should be very small - so running the update should
            //         not depend heavily on the List<>.Contains(...) performance.
            //
            //         Also, the AVL binary search tree has O(log n) performance for inserts
            //         / removals / and searches.
            //

            // Pre-fetch the cost list for this frontier location
            var oldWeight = this.OutputMap[column, row];

            // Update the output map
            this.OutputMap[column, row] = System.Math.Min(this.OutputMap[column, row], currentWeight + _costCallback(column, row) + CELL_MOVEMENT_COST);

            // Update the frontier
            var newWeight = this.OutputMap[column, row];

            // Fetch locator for this location
            var locator = _locatorCallback(column, row);

            // UPDATE THE FRONTIER
            var oldWeightList = frontier.Search(oldWeight);
            var newWeightList = frontier.Search(newWeight);

            // Both weights are absent from the frontier
            if (oldWeightList == null &&
                newWeightList == null)
                frontier.Insert(newWeight, new List<IGridLocator>() { locator });

            // Old weight list exists; New weight list is absent
            else if (oldWeightList != null &&
                     newWeightList == null)
            {
                // Check for existing locator
                if (oldWeightList.Contains(locator))
                    oldWeightList.Remove(locator);

                // Remove unused node
                if (oldWeightList.Count == 0)
                    frontier.Remove(oldWeight);

                // Insert new node in the frontier
                frontier.Insert(newWeight, new List<IGridLocator>() { locator });
            }

            // Old weight is absent; New weight exists
            else if (oldWeightList == null &&
                     newWeightList != null)
            {
                // Locator doesn't exist in list
                if (!newWeightList.Contains(locator))
                    newWeightList.Add(locator);
            }

            // Both old and new weight lists exist
            else
            {
                // Check that they're different lists
                if (oldWeightList != newWeightList)
                {
                    // Check that old weight list has element removed
                    if (oldWeightList.Contains(locator))
                        oldWeightList.Remove(locator);

                    // Check that new weight list has element added
                    if (!newWeightList.Contains(locator))
                        newWeightList.Add(locator);
                }
            }
        }

        protected IEnumerable<IGridLocator> GeneratePath(IGridLocator targetLocation)
        {
            if (!_hasRun)
                throw new Exception("Must call Run() before generating a path DijkstraMapBase");

            if (!this.TargetLocations.Contains(targetLocation))
                throw new Exception("Requested target location not specified by the constructor DijkstraMapBase");

            var result = new List<IGridLocator>();

            var currentLocation = targetLocation;
            var goalLocation = this.SourceLocation;

            // Find the "easiest" route to the goal
            while (!currentLocation.Equals(goalLocation))
            {
                var column = currentLocation.Column;
                var row = currentLocation.Row;

                var north = row - 1 >= 0;
                var south = row + 1 < _height;
                var east = column + 1 < _width;
                var west = column - 1 >= 0;

                var lowestWeight = MAP_MAX_VALUE;
                var lowestWeightLocation = currentLocation;

                if (north && (this.OutputMap[column, row - 1] < lowestWeight))
                {
                    lowestWeightLocation = _locatorCallback(column, row - 1);
                    lowestWeight = this.OutputMap[column, row - 1];
                }

                if (south && (this.OutputMap[column, row + 1] < lowestWeight))
                {
                    lowestWeightLocation = _locatorCallback(column, row + 1);
                    lowestWeight = this.OutputMap[column, row + 1];
                }

                if (east && (this.OutputMap[column + 1, row] < lowestWeight))
                {
                    lowestWeightLocation = _locatorCallback(column + 1, row);
                    lowestWeight = this.OutputMap[column + 1, row];
                }

                if (west && (this.OutputMap[column - 1, row] < lowestWeight))
                {
                    lowestWeightLocation = _locatorCallback(column - 1, row);
                    lowestWeight = this.OutputMap[column - 1, row];
                }

                if (north && east && !this.ObeyCardinalMovement && (this.OutputMap[column + 1, row - 1] < lowestWeight))
                {
                    lowestWeightLocation = _locatorCallback(column + 1, row - 1);
                    lowestWeight = this.OutputMap[column + 1, row - 1];
                }

                if (north && west && !this.ObeyCardinalMovement && (this.OutputMap[column - 1, row - 1] < lowestWeight))
                {
                    lowestWeightLocation = _locatorCallback(column - 1, row - 1);
                    lowestWeight = this.OutputMap[column - 1, row - 1];
                }

                if (south && east && !this.ObeyCardinalMovement && (this.OutputMap[column + 1, row + 1] < lowestWeight))
                {
                    lowestWeightLocation = _locatorCallback(column + 1, row + 1);
                    lowestWeight = this.OutputMap[column + 1, row + 1];
                }

                if (south && west && !this.ObeyCardinalMovement && (this.OutputMap[column - 1, row + 1] < lowestWeight))
                {
                    lowestWeightLocation = _locatorCallback(column - 1, row + 1);
                    lowestWeight = this.OutputMap[column - 1, row + 1];
                }

                if (lowestWeight == double.MaxValue)
                    throw new Exception("Mishandled Dijkstra Map DijkstraMap.GeneratePath()");

                currentLocation = lowestWeightLocation;

                // Add this to the path
                if (!result.Any(location => location.Equals(lowestWeightLocation)) &&
                    !lowestWeightLocation.Equals(this.SourceLocation) &&
                    !lowestWeightLocation.Equals(targetLocation))
                    result.Add(lowestWeightLocation);

                else if (!lowestWeightLocation.Equals(this.SourceLocation) &&
                         !lowestWeightLocation.Equals(targetLocation))
                    throw new Exception("Loop in Dijkstra Map path finding");
            }

            return result;
        }

        // Use to help debug
        protected void OutputCSV(string directory, string filePrefix)
        {
            var inputMap = new float[_width, _height];

            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                    inputMap[i, j] = _costCallback(i, j);
            }

            OutputCSV(inputMap, Path.Combine(directory, filePrefix + "_input.csv"));
            OutputCSV(this.OutputMap, Path.Combine(directory, filePrefix + "_output.csv"));
        }

        private void OutputCSV(float[,] matrix, string fileName)
        {
            var builder = new StringBuilder();

            // Output by row CSV
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    if (matrix[i, j] != MAP_MAX_VALUE)
                        builder.Append(matrix[i, j].ToString("F3") + ", ");

                    else
                        builder.Append("MAX, ");
                }

                // Remove trailing comma
                builder.Remove(builder.Length - 1, 1);

                // Append return carriage
                builder.Append("\r\n");
            }

            File.WriteAllText(fileName, builder.ToString());
        }
    }

    public class DijkstraPathGenerator : DijkstraMapBase
    {
        /// <summary>
        /// Callback that allows setting properties of the embedded path cells
        /// </summary>
        public delegate void DijkstraEmbedPathCallback(GridCellInfo pathCell);

        readonly GridCellInfo[,] _grid;

        public DijkstraPathGenerator(GridCellInfo[,] grid, IEnumerable<Region<GridCellInfo>> avoidRegions, GridCellInfo sourceLocation, IEnumerable<GridCellInfo> targetLocations, bool obeyCardinalMovement)
             : base(grid.GetLength(0), grid.GetLength(1), obeyCardinalMovement, sourceLocation, targetLocations, new DijkstraMapCostCallback((column, row) =>
             {
                 if (avoidRegions.Any(region => region[column, row] != null))
                     return DijkstraMapBase.RegionFeatureConstant;

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

    public class DijkstraLevelGrid : DijkstraMapBase
    {
        public DijkstraLevelGrid(LayoutGrid grid, IGridLocator sourceLocation, IEnumerable<IGridLocator> targetLocations, bool obeyCardinalMovement)
             : base(grid.Bounds.Width, grid.Bounds.Height, obeyCardinalMovement, sourceLocation, targetLocations, new DijkstraMapCostCallback((column, row) =>
             {
                 if (grid[column, row] == null)
                     return 0;

                 else if (grid[column, row].IsWall)
                     return 0;

                 else
                     return DijkstraMapBase.RegionFeatureConstant;

             }), new DijkstraMapLocatorCallback((column, row) =>
             {
                 throw new NotImplementedException("DijkstraLevelGrid locator callback. Have to prevent iterating out into null cells");
             }))
        {
            Run();
        }
    }
}
