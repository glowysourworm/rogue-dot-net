using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Model.Algorithm.Interface;
using Rogue.NET.Core.Processing.Model.Extension;
using Rogue.NET.Core.Processing.Model.Static;
using Rogue.NET.Core.Processing.Service.Interface;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Algorithm
{
    /// <summary>
    /// Computes path via A* algorithm
    /// </summary>
    [Export(typeof(IPathFinder))]
    public class SimplePathFinder : IPathFinder
    {
        readonly IModelService _modelService;

        const int MAX_DEPTH = 50;

        protected class SimplePathFinderNode
        {
            public SimplePathFinderNode ParentNode { get; private set; }
            public GridLocation Location { get; private set; }
            public List<SimplePathFinderNode> ChildNodes { get; set; }
            public bool IsEndLocation { get; private set; }

            /// <summary>
            /// Use this constructor to generate a non-heuristic based node
            /// </summary>
            public SimplePathFinderNode(SimplePathFinderNode parentNode, GridLocation location, bool isEndLocation)
            {
                this.ParentNode = parentNode;
                this.Location = location;
                this.ChildNodes = new List<SimplePathFinderNode>();
                this.IsEndLocation = isEndLocation;
            }

            public override string ToString()
            {
                return this.Location?.ToString() ?? "";
            }
        }

        [ImportingConstructor]
        public SimplePathFinder(IModelService modelService)
        {
            _modelService = modelService;
        }

        public GridLocation FindCharacterNextPathLocation(GridLocation point1, GridLocation point2, CharacterAlignmentType alignmentType)
        {
            if (point1.Equals(point2))
                return point1;

            // Create Start Node
            var startNode = new SimplePathFinderNode(null, point1, false);

            // Recursively find End Node
            var endNode = FindPathRecurse(startNode, point2, alignmentType, 1);

            // Create list of path locations
            List<GridLocation> pathLocations;

            // Back-Track to find next location along path
            return BackTrack(startNode, endNode, false, out pathLocations);
        }

        public GridLocation FindNextPathLocation(GridLocation point1, GridLocation point2)
        {
            if (point1.Equals(point2))
                return point1;

            // Create Start Node
            var startNode = new SimplePathFinderNode(null, point1, false);

            // Recursively find End Node
            var endNode = FindPathRecurse(startNode, point2, CharacterAlignmentType.None, 1);

            // Create list of path locations
            List<GridLocation> pathLocations;

            // Back-Track to find next location along path
            return BackTrack(startNode, endNode, false, out pathLocations);
        }

        public IEnumerable<GridLocation> FindCharacterPath(GridLocation point1, GridLocation point2, CharacterAlignmentType alignmentType)
        {
            if (point1.Equals(point2))
                return new GridLocation[] { point1 };

            // Create Start Node
            var startNode = new SimplePathFinderNode(null, point1, false);

            // Recursively find End Node
            var endNode = FindPathRecurse(startNode, point2, alignmentType, 1);

            // Create list of path locations
            List<GridLocation> pathLocations;

            // Back-Track to find next location along path
            BackTrack(startNode, endNode, true, out pathLocations);

            return pathLocations;
        }

        public IEnumerable<GridLocation> FindPath(GridLocation point1, GridLocation point2)
        {
            // Trying Brian Walker's (Brogue) "Dijkstra Map"
            //
            // 1) Set the "goal" as point2 - weighting of zero
            //
            // 2) Iterate using Breadth First Search (flood fill)
            //
            // 3) "Walk Downhill" towards the goal
            //

            var grid = _modelService.Level.Grid;
            var sourceCell = grid[point1.Column, point1.Row];

            var frontier = new Queue<Cell>();
            var dijkstraMap = new Cell[grid.Bounds.CellWidth, grid.Bounds.CellHeight];

            // Initialize the frontier and Dijkstra Map
            sourceCell.DijkstraWeight = 0;
            frontier.Enqueue(sourceCell);
            dijkstraMap[sourceCell.Location.Column, sourceCell.Location.Row] = sourceCell;

            while (frontier.Count > 0)
            {
                // Dequeue cell 
                var testCell = frontier.Dequeue();

                // Get adjacent cells to test cell
                var adjacentGridCells = grid.GetAdjacentCells(testCell);

                // Iterate adjacent cells
                foreach (var cell in adjacentGridCells)
                {
                    // Check that cell isn't already in the Dijkstra Map
                    var existingCell = dijkstraMap[cell.Location.Column, cell.Location.Row];

                    // Skip cells that have been visited
                    if (existingCell != null)
                        continue;

                    // Calculate adjacent cells already in the dijkstra map
                    var adjacentDijkstraCells = dijkstraMap.GetAdjacentElements(cell.Location.Column, cell.Location.Row);

                    // Calculate the next Dijkstra Weight 
                    var dijkstraWeight = adjacentDijkstraCells.None() ? 1 : (adjacentDijkstraCells.Min(x => x.DijkstraWeight) + 1);

                    // Set the cell's weighting
                    cell.DijkstraWeight = dijkstraWeight;

                    // Add cell to the Dijkstra Map
                    dijkstraMap[cell.Location.Column, cell.Location.Row] = cell;

                    // Enqueue cell with the frontier
                    frontier.Enqueue(cell);
                }
            }

            // "Walk Downhill" to create the pathoo;    ;oo;
            var currentLocation = point2;
            var path = new List<GridLocation>();

            while (!currentLocation.Equals(point1))
            {
                // Add location to patharray 
                path.Add(currentLocation);

                // Get next "Downhill" location
                currentLocation = grid.GetAdjacentLocations(currentLocation)
                                      .MinBy(x => grid[x.Column, x.Row].DijkstraWeight);
            }

            // Add the first point
            path.Add(point1);

            // Reverse the path to put the cells in order of the path
            path.Reverse();

            return path;
        }

        // SAVE
        //public IEnumerable<GridLocation> FindPath(GridLocation point1, GridLocation point2)
        //{
        //    if (point1.Equals(point2))
        //        return new GridLocation[] { point1 };

        //    // Create Start Node
        //    var startNode = new SimplePathFinderNode(null, point1, false);

        //    // Recursively find End Node
        //    var endNode = FindPathRecurse(startNode, point2, true, CharacterAlignmentType.None, 1);

        //    // Create list of path locations
        //    List<GridLocation> pathLocations;

        //    // Back-Track to find next location along path
        //    BackTrack(startNode, endNode, true, out pathLocations);

        //    return pathLocations;
        //}

        /// <summary>
        /// Returns the next step along path towards the node - this will be one of the start node's child nodes (actual location)
        /// </summary>
        private GridLocation BackTrack(SimplePathFinderNode startNode, SimplePathFinderNode node, bool buildPath, out List<GridLocation> pathLocations)
        {
            // Create output list of path locations
            pathLocations = buildPath ? new List<GridLocation>() : null;

            // If the end location is found - backtrack to find the next point
            if (node != null &&
                node.ParentNode != null)
            {
                // Loop backwards to find the beginning of the path
                while (node.ParentNode != startNode)
                {
                    // Add node to path list
                    if (buildPath)
                        pathLocations.Add(node.Location);

                    // Back-track
                    node = node.ParentNode;
                }

                if (buildPath)
                {
                    // Add node (second node after start location)
                    pathLocations.Add(node.Location);

                    // Add start location to path
                    pathLocations.Add(startNode.Location);

                    // Reverse the direction to order them properly
                    pathLocations.Reverse();
                }

                // This will be the next path location
                return node.Location;
            }
            else
                return null;
        }

        private SimplePathFinderNode FindPathRecurse(SimplePathFinderNode currentNode, GridLocation endLocation, CharacterAlignmentType alignmentType, int depthIndex)
        {
            // Check recursion depth limit
            if (depthIndex >= MAX_DEPTH)
                return null;

            // Check for character path
            var isCharacter = alignmentType != CharacterAlignmentType.None;

            // Gets a set of adjacent locations that aren't blocked
            var orderedPathLocations = _modelService.Level
                                                    .Grid
                                                    .GetAdjacentLocations(currentNode.Location)
                // Check Pathing
                .Where(location => !_modelService.LayoutService.IsPathToAdjacentCellBlocked(currentNode.Location,
                                                                                             location,
                                                                                             isCharacter,
                                                                                             alignmentType))
                // Apply Heuristic
                .OrderBy(x => Metric.RoguianDistance(x, endLocation))
                .Actualize();

            // PROBLEM - NOT REMOVING VISITED CELLS FROM RECURSION

            // Recurse, Adding locations to the node tree
            foreach (var nextLocation in orderedPathLocations)
            {
                // Create new node
                var nextNode = new SimplePathFinderNode(currentNode, nextLocation, nextLocation.Equals(endLocation));

                // Adding location to the current node's list
                currentNode.ChildNodes.Add(nextNode);

                // FOUND THE END LOCATION - SO RETURN THE RESULT
                if (nextNode.IsEndLocation)
                    return nextNode;

                // Recurse next location 
                else
                {
                    // Try the path for this adjacent 
                    var resultNode = FindPathRecurse(nextNode, endLocation, alignmentType, depthIndex + 1);

                    if (resultNode != null)
                        return resultNode;
                }
            }

            // NO VIABLE PATH FOUND
            return null;
        }
    }
}
