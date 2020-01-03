using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Utility;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using Rogue.NET.Core.Processing.Model.Extension;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Algorithm.Component
{
    public class DijkstraPathFinder : DijkstraMapBase
    {
        /// <summary>
        /// Returns true if the path is blocked for movement
        /// </summary>
        public delegate bool DijkstraPathFinderBlockedCallback(int column1, int row1, int column2, int row2);

        readonly LayoutGrid _layoutGrid;
        readonly DijkstraPathFinderBlockedCallback _blockedCallback;

        public DijkstraPathFinder(LayoutGrid layoutGrid,
                                  IGridLocator sourceLocation,
                                  IEnumerable<IGridLocator> targetLocations,
                                  DijkstraPathFinderBlockedCallback blockedCallback)
             : base(layoutGrid.Bounds.Width,
                    layoutGrid.Bounds.Height,
                    false,
                    sourceLocation,
                    targetLocations,
                    new DijkstraMapCostCallback((column1, row1, column2, row2) =>
                    {
                        return blockedCallback(column1, row1, column2, row2) ? DijkstraMapBase.MapCostInfinity : 0;
                    }),
                    new DijkstraMapLocatorCallback((column, row) =>
                    {
                        if (layoutGrid[column, row] == null)
                        {
                            int foo = 43;
                        }

                        return layoutGrid[column, row].Location;
                    }))
        {
            _layoutGrid = layoutGrid;
            _blockedCallback = blockedCallback;
        }

        /// <summary>
        /// Resets / re-runs the algorithm to calculate a new output
        /// </summary>
        public void Reset(GridLocation sourceLocation, IEnumerable<GridLocation> targetLocations)
        {
            base.Initialize(sourceLocation, targetLocations);
        }

        public new void Run()
        {
            base.Run();
        }

        /// <summary>
        /// Returns the total cost of movement from the source to the specified target calculated by
        /// Dijkstra's algorithm.
        /// </summary>
        public float GetMovementCost(IGridLocator targetLocation)
        {
            if (!this.TargetLocations.Any(location => location.Column == targetLocation.Column &&
                                                      location.Row == targetLocation.Row))
                throw new Exception("Trying to get Dijkstra path for non-routed location");

            return this.OutputMap[targetLocation.Column, targetLocation.Row];
        }

        /// <summary>
        /// Gets the next step in the path towards the specified target
        /// </summary>
        public GridLocation GetNextPathLocation(IGridLocator targetLocation)
        {
            if (!this.TargetLocations.Any(location => location.Column == targetLocation.Column &&
                                                      location.Row == targetLocation.Row))
                throw new Exception("Trying to get Dijkstra path for non-routed location");

            var nextOutput = this.OutputMap[targetLocation.Column, targetLocation.Row];
            var nextLocation = targetLocation;
            var resultLocation = targetLocation;

            // Backtrack until the source is reached
            while (!nextLocation.Equals(this.SourceLocation))
            {
                // Get the next location towards the source - taking the smallest adjacent output that isn't blocked
                nextLocation = this.OutputMap
                                   .GetAdjacentValueLocators(nextLocation.Column, nextLocation.Row)
                                   .Where(location => !_blockedCallback(nextLocation.Column, nextLocation.Row, location.Column, location.Row))
                                   .MinBy(location => this.OutputMap[location.Column, location.Row]);

                nextOutput = this.OutputMap[nextLocation.Column, nextLocation.Row];

                // Set the follower if the source location isn't reached
                if (!nextLocation.Equals(this.SourceLocation))
                    resultLocation = nextLocation;
            }

            return _layoutGrid[resultLocation].Location;
        }
    }
}
