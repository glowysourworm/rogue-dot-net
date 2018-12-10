using Rogue.NET.Core.Logic.Algorithm.Interface;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Core.Logic.Interface;

using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Rogue.NET.Core.Logic.Static;
using Rogue.NET.Core.Model.Scenario.Content.Extension;
using System;

namespace Rogue.NET.Core.Logic.Algorithm
{
    /// <summary>
    /// Computes path via A* algorithm
    /// </summary>
    [Export(typeof(IPathFinder))]
    public class SimplePathFinder : IPathFinder
    {
        readonly IModelService _modelService;
        readonly ILayoutEngine _layoutEngine;

        [ImportingConstructor]
        public SimplePathFinder(IModelService modelService, ILayoutEngine layoutEngine)
        {
            _modelService = modelService;
            _layoutEngine = layoutEngine;
        }

        public CellPoint FindPath(CellPoint point1, CellPoint point2, double maxRadius)
        {
            // Initialize recurse A* algorithm with pathIdx = 1; and start point added to history
            return FindPath(maxRadius, new Dictionary<CellPoint, int>() { { point1, 0 } }, point1, point2, 1);
        }

        private CellPoint FindPath(double maxSeparation, 
                                   Dictionary<CellPoint, int> pathDictionary, 
                                   CellPoint start, 
                                   CellPoint end, 
                                   int pathIdx)
        {
            if (pathIdx > maxSeparation)
                return null;

            var grid = _modelService.Level.Grid;

            // Iterate over all round possible paths - add to end of dictionary so it's safe for recursion
            // Also cache the current dictionary count so that it doesn't change over recursion
            for (int i = 0; i < pathDictionary.Count; i++)
            {
                var pathElement = pathDictionary.ElementAt(i);

                // *** SHOULD BE ABLE TO SKIP ANYTHING WITH PREVIOUS PATH INDEX
                //     OR CURRENT INDEX (WAS JUST ADDED)
                if (pathElement.Value < pathIdx - 1 ||
                    pathElement.Value == pathIdx)
                    continue;

                // Gets a set of adjacent locations that aren't blocked
                var adjacentPathLocations = grid.GetAdjacentLocations(pathElement.Key)
                                                .Where(x => !_layoutEngine.IsPathToAdjacentCellBlocked(_modelService.Level, pathElement.Key, x, true));

                // First, Add ALL cells not in the path history
                foreach (var nextLocation in adjacentPathLocations)
                {
                    // Found the target point - so start backtracking
                    if (nextLocation == end)
                    {
                        var backtrackLocation = end;
                        var backtrackPathIdx = pathIdx;

                        while (backtrackLocation != start)
                        {
                            // Get adjacent cells for next backtracking point
                            // NOTE** Check for blocking from enemies only if path index != 0. (otherwise, would be the start point)
                            var nextBackTrackLocation = grid.GetAdjacentLocations(backtrackLocation)
                                                            .Where(x => pathDictionary.ContainsKey(x) &&
                                                                        pathDictionary[x] == backtrackPathIdx - 1 &&
                                                                        !_layoutEngine.IsPathToAdjacentCellBlocked(_modelService.Level, backtrackLocation, x, backtrackPathIdx - 1 != 0))
                                                            .FirstOrDefault();

                            // This catch should not happen; but have here as a safety clause
                            if (nextBackTrackLocation == null)
                                return null;

                            // Decrement the path index to find the previous entries on the history of paths
                            backtrackPathIdx--;

                            // If you've found the start, then return the next cell in the path
                            if (nextBackTrackLocation == start)
                                return backtrackLocation;

                            else
                                backtrackLocation = nextBackTrackLocation;
                        }
                    }

                    // Otherwise, add to the collection
                    if (!pathDictionary.ContainsKey(nextLocation))
                        pathDictionary.Add(nextLocation, pathIdx);
                }
            }

            // Iterate to collect path information to backtrack with - incrementing path index
            return FindPath(maxSeparation, pathDictionary, start, end, pathIdx + 1);
        }
    }
}
