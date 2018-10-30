using Rogue.NET.Core.Logic.Algorithm.Interface;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Core.Logic.Interface;

using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Logic.Algorithm
{
    /// <summary>
    /// Computes path via A* algorithm
    /// </summary>
    [Export(typeof(IPathFinder))]
    public class SimplePathFinder : IPathFinder
    {
        /// <summary>
        /// Controls the max level of the path tree that is created. Each iteration expands the search super-linearly.
        /// (Could be super-exponential.. can't remember)
        /// </summary>
        private const int MAX_CTR = 50;

        readonly IModelService _modelService;
        readonly ILayoutEngine _layoutEngine;

        [ImportingConstructor]
        public SimplePathFinder(IModelService modelService, ILayoutEngine layoutEngine)
        {
            _layoutEngine = layoutEngine;
        }

        #region Nested Classes
        protected class PathCell
        {
            public int PathIndex { get; private set; }
            public CellPoint CurrentCell { get; private set; }
            public CellPoint NextCell { get; private set; }

            public PathCell(int pathIdx, CellPoint currentCell, CellPoint nextCell)
            {
                this.PathIndex = pathIdx;
                this.CurrentCell = currentCell;
                this.NextCell = nextCell;
            }
            public override bool Equals(object obj)
            {
                return this.CurrentCell == null ? base.Equals(obj) : this.CurrentCell.Equals(obj);
            }
            public override int GetHashCode()
            {
                return this.CurrentCell == null ? base.GetHashCode() : this.CurrentCell.GetHashCode();
            }
            public override string ToString()
            {
                return this.CurrentCell.ToString();
            }
        }
        #endregion

        public CellPoint FindPath(CellPoint point1, CellPoint point2, double maxRadius)
        {
            return FindPath(maxRadius, new Dictionary<CellPoint, PathCell>(), point1, point1, point2, 0);
        }

        private CellPoint FindPath(double maxSeparation, 
                                   Dictionary<CellPoint, PathCell> pathDictionary, 
                                   CellPoint start, 
                                   CellPoint location, 
                                   CellPoint end, 
                                   int pathIdx)
        {
            if (pathIdx > maxSeparation)
                return null;

            var grid = _modelService.CurrentLevel.Grid;

            // Gets a set of adjacent locations that aren't blocked
            var adjacentPathLocations = _layoutEngine.GetAdjacentLocations(location)
                                                     .Where(x => !_layoutEngine.IsPathToAdjacentCellBlocked(location, x) &&
                                                                 !(_layoutEngine.EuclideanDistance(x, end) > maxSeparation))
                                                     .ToList();

            // Sort by distance from target
            adjacentPathLocations.Sort((point1, point2) =>
            {
                double dist1 = _layoutEngine.RoguianDistance(point1, end);
                double dist2 = _layoutEngine.RoguianDistance(point2, end);

                return dist1.CompareTo(dist2);
            });

            // Apply in decreasing order
            for (int i = adjacentPathLocations.Count - 1; i >= 0; i--)
            {
                var nextLocation = adjacentPathLocations[i];

                if (!pathDictionary.ContainsKey(nextLocation))
                    pathDictionary.Add(nextLocation, new PathCell(pathIdx, location, nextLocation));

                else
                    adjacentPathLocations.RemoveAt(i);

                // Found the end point
                if (nextLocation == end)
                {
                    //Backtrack to start location
                    var backTrackPoint = end;
                    while (backTrackPoint != start)
                    {
                        backTrackPoint = pathDictionary[backTrackPoint].CurrentCell;

                        if (backTrackPoint == start)
                            return pathDictionary[backTrackPoint].NextCell;
                    }
                }
            }

            // Iterate out to create tree of possible paths
            foreach (var nextLocation in adjacentPathLocations)
            {
                // Recursion only returns non-null if the end point is reached
                var recurseResult = FindPath(maxSeparation, pathDictionary, start, nextLocation, end, pathIdx + 1);

                if (recurseResult != null)
                    return recurseResult;
            }

            // Returning null indicates that nothing was reached; but this doesn't get returned unless all path
            // trees fail.
            return null;
        }
    }
}
