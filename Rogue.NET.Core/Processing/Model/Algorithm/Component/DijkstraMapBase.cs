using Rogue.NET.Common.Collection;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using Rogue.NET.Core.Processing.Model.Extension;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Rogue.NET.Core.Processing.Model.Algorithm.Component
{
    public abstract class DijkstraMapBase
    {
        /// <summary>
        /// Delegate used to fetch a cost for the specified column / row of the grid
        /// </summary>
        public delegate float DijkstraMapCostCallback(int column1, int row1, int column2, int row2);

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
        /// Constant that indictaes a region of the grid that is to be AVOIDED; but CROSSABLE.
        /// </summary>
        public readonly static float MapCostAvoid = 2;

        /// <summary>
        /// Constant that indicates a region of the grid that is to be HIGHLY AVOIDED; but CROSSABLE.
        /// </summary>
        public readonly static float MapCostHighAvoid = 10;

        /// <summary>
        /// Constant that indicates that a region is off-limits to the Dijkstra map.
        /// </summary>
        public readonly static float MapCostInfinity = float.PositiveInfinity;

        private readonly int _width;
        private readonly int _height;
        private readonly DijkstraMapCostCallback _costCallback;
        private readonly DijkstraMapLocatorCallback _locatorCallback;

        // Visited locations on the map
        private bool[,] _visitedMap;

        // Frontier BST for the map
        BinarySearchTree<float, Dictionary<IGridLocator, IGridLocator>> _frontier;
        
        // Cell movement cost
        const int CELL_MOVEMENT_COST = 1;

        bool _initialized = false;
        bool _finished = false;

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

            _visitedMap = new bool[width, height];
            _frontier = new BinarySearchTree<float, Dictionary<IGridLocator, IGridLocator>>();
        }

        protected void Initialize(IGridLocator sourceLocation, IEnumerable<IGridLocator> targetLocations)
        {
            this.SourceLocation = sourceLocation;
            this.TargetLocations = targetLocations;

            // Clear out the frontier
            _frontier.Clear();

            // Initialize the private maps
            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    // Initialize output map
                    this.OutputMap[i, j] = ((i == this.SourceLocation.Column) && (j == this.SourceLocation.Row)) ? 0 : MapCostInfinity;

                    // Initialize visited map
                    _visitedMap[i, j] = false;
                }
            }

            _initialized = true;
        }

        /// <summary>
        /// Runs Dijkstra's algorithm ONCE. WILL NOT RE-RUN.
        /// </summary>
        protected void Run()
        {
            if (!_initialized)
                throw new Exception("Must call Initialize() before Run() DijkstraMapBase");

            if (_finished)
                return;

            // Track goal progress
            var goalDict = this.TargetLocations
                               .ToDictionary(location => location, location => false);

            // Process the first element
            var column = this.SourceLocation.Column;
            var row = this.SourceLocation.Row;

            // Iterate while any target not reached (AND) not visited
            while (!_visitedMap[column, row] &&
                    goalDict.Any(element => !element.Value))
            {
                // Set current parameters
                var currentWeight = this.OutputMap[column, row];

                // Mark the element as visited
                _visitedMap[column, row] = true;

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
                if (north && !_visitedMap[column, row - 1])
                {
                    UpdateOutputMap(currentWeight, column, row - 1, column, row);
                }

                if (south && !_visitedMap[column, row + 1])
                {
                    UpdateOutputMap(currentWeight, column, row + 1, column, row);
                }

                if (east && !_visitedMap[column + 1, row])
                {
                    UpdateOutputMap(currentWeight, column + 1, row, column, row);
                }

                if (west && !_visitedMap[column - 1, row])
                {
                    UpdateOutputMap(currentWeight, column - 1, row, column, row);
                }

                // NON-CARDINAL LOCATIONS
                if (!this.ObeyCardinalMovement && north && east && !_visitedMap[column + 1, row - 1])
                {
                    UpdateOutputMap(currentWeight, column + 1, row - 1, column, row);
                }

                if (!this.ObeyCardinalMovement && north && west && !_visitedMap[column - 1, row - 1])
                {
                    UpdateOutputMap(currentWeight, column - 1, row - 1, column, row);
                }

                if (!this.ObeyCardinalMovement && south && east && !_visitedMap[column + 1, row + 1])
                {
                    UpdateOutputMap(currentWeight, column + 1, row + 1, column, row);
                }

                if (!this.ObeyCardinalMovement && south && west && !_visitedMap[column - 1, row + 1])
                {
                    UpdateOutputMap(currentWeight, column - 1, row + 1, column, row);
                }

                // Update goal dictionary
                var locator = _locatorCallback(column, row);

                // O(1)
                if (goalDict.ContainsKey(locator))
                    goalDict[locator] = true;

                // Select next location from frontier queue - using the smallest weight
                if (_frontier.Count > 0)
                {
                    // Lists in the frontier must have an entry
                    var nextCostDict = _frontier.Min();
                    var nextCost = _frontier.MinKey();

                    // Get the first from the dictionary
                    var nextNode = nextCostDict.First();

                    // Maintain frontier hash
                    nextCostDict.Remove(nextNode.Key);

                    if (nextCostDict.Count == 0)
                        _frontier.Remove(nextCost);

                    // Move to next location
                    column = nextNode.Key.Column;
                    row = nextNode.Key.Row;
                }
            }

            _finished = true;
        }

        private void UpdateOutputMap(float currentWeight, int destColumn, int destRow, int sourceColumn, int sourceRow)
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
            var oldWeight = this.OutputMap[destColumn, destRow];

            // Update the output map
            this.OutputMap[destColumn, destRow] = System.Math.Min(this.OutputMap[destColumn, destRow],
                                                                                currentWeight + _costCallback(sourceColumn, sourceRow, destColumn, destRow) + CELL_MOVEMENT_COST);

            // Update the frontier
            var newWeight = this.OutputMap[destColumn, destRow];

            // Fetch locator for this location
            var locator = _locatorCallback(destColumn, destRow);

            // UPDATE THE FRONTIER
            var oldWeightList = _frontier.Search(oldWeight);
            var newWeightList = _frontier.Search(newWeight);

            // Both weights are absent from the frontier
            if (oldWeightList == null &&
                newWeightList == null)
                _frontier.Insert(newWeight, new Dictionary<IGridLocator, IGridLocator>() { { locator, locator } });

            // Old weight list exists; New weight list is absent
            else if (oldWeightList != null &&
                     newWeightList == null)
            {
                // Check for existing locator
                if (oldWeightList.ContainsKey(locator))
                    oldWeightList.Remove(locator);

                // Remove unused node
                if (oldWeightList.Count == 0)
                    _frontier.Remove(oldWeight);

                // Insert new node in the frontier
                _frontier.Insert(newWeight, new Dictionary<IGridLocator, IGridLocator>() { { locator, locator } });
            }

            // Old weight is absent; New weight exists
            else if (oldWeightList == null &&
                     newWeightList != null)
            {
                // Locator doesn't exist in list
                if (!newWeightList.ContainsKey(locator))
                    newWeightList.Add(locator, locator);
            }

            // Both old and new weight lists exist
            else
            {
                // Check that they're different lists
                if (oldWeightList != newWeightList)
                {
                    // Check that old weight list has element removed
                    if (oldWeightList.ContainsKey(locator))
                        oldWeightList.Remove(locator);

                    // Check that new weight list has element added
                    if (!newWeightList.ContainsKey(locator))
                        newWeightList.Add(locator, locator);
                }
            }
        }

        protected IEnumerable<IGridLocator> GeneratePath(IGridLocator targetLocation)
        {
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

                var lowestWeight = MapCostInfinity;
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

                if (lowestWeight == DijkstraMapBase.MapCostInfinity)
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
    }
}
