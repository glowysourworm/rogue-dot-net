using Rogue.NET.Core.Model.Scenario.Content.Layout;

using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Rogue.NET.Core.Model.Scenario.Content.Extension;
using Rogue.NET.Core.Processing.Model.Algorithm.Interface;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.Core.Processing.Model.Content.Interface;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Processing.Model.Static;
using Rogue.NET.Common.Extension;

namespace Rogue.NET.Core.Processing.Model.Algorithm
{
    /// <summary>
    /// Computes path via A* algorithm
    /// </summary>
    [Export(typeof(IPathFinder))]
    public class SimplePathFinder : IPathFinder
    {
        readonly IModelService _modelService;
        readonly ILayoutEngine _layoutEngine;

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
        public SimplePathFinder(IModelService modelService, ILayoutEngine layoutEngine)
        {
            _modelService = modelService;
            _layoutEngine = layoutEngine;
        }

        public GridLocation FindCharacterNextPathLocation(GridLocation point1, GridLocation point2, bool canOpenDoors, CharacterAlignmentType alignmentType)
        {
            if (point1.Equals(point2))
                return point1;

            // Create Start Node
            var startNode = new SimplePathFinderNode(null, point1, false);

            // Recursively find End Node
            var endNode = FindPathRecurse(startNode, point2, canOpenDoors, alignmentType, 1);

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
            var endNode = FindPathRecurse(startNode, point2, true, CharacterAlignmentType.None, 1);

            // Create list of path locations
            List<GridLocation> pathLocations;

            // Back-Track to find next location along path
            return BackTrack(startNode, endNode, false, out pathLocations);
        }

        public IEnumerable<GridLocation> FindCharacterPath(GridLocation point1, GridLocation point2, bool canOpenDoors, CharacterAlignmentType alignmentType)
        {
            if (point1.Equals(point2))
                return new GridLocation[] { point1 };

            // Create Start Node
            var startNode = new SimplePathFinderNode(null, point1, false);

            // Recursively find End Node
            var endNode = FindPathRecurse(startNode, point2, canOpenDoors, alignmentType, 1);

            // Create list of path locations
            List<GridLocation> pathLocations;

            // Back-Track to find next location along path
            BackTrack(startNode, endNode, true, out pathLocations);

            return pathLocations;
        }

        public IEnumerable<GridLocation> FindPath(GridLocation point1, GridLocation point2)
        {
            if (point1.Equals(point2))
                return new GridLocation[] { point1 };

            // Create Start Node
            var startNode = new SimplePathFinderNode(null, point1, false);

            // Recursively find End Node
            var endNode = FindPathRecurse(startNode, point2, true, CharacterAlignmentType.None, 1);

            // Create list of path locations
            List<GridLocation> pathLocations;

            // Back-Track to find next location along path
            BackTrack(startNode, endNode, true, out pathLocations);

            return pathLocations;
        }

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
                return GridLocation.Empty;
        }

        private SimplePathFinderNode FindPathRecurse(SimplePathFinderNode currentNode, GridLocation endLocation, bool canOpenDoors, CharacterAlignmentType alignmentType, int depthIndex)
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
                .Where(location => (isCharacter && // Character Path
                                    canOpenDoors) ? !_layoutEngine.IsPathToCellThroughWall(currentNode.Location,
                                                                                            location,
                                                                                            isCharacter,
                                                                                            alignmentType)

                                                   // Character or Non-Character
                                                  : !_layoutEngine.IsPathToAdjacentCellBlocked(currentNode.Location,
                                                                                             location,
                                                                                             isCharacter,
                                                                                             alignmentType))
                // Apply Heuristic
                .OrderBy(x => Calculator.RoguianDistance(x, endLocation))
                .Actualize();

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
                    var resultNode = FindPathRecurse(nextNode, endLocation, canOpenDoors, alignmentType, depthIndex + 1);

                    if (resultNode != null)
                        return resultNode;
                }
            }

            // NO VIABLE PATH FOUND
            return null;
        }
    }
}
