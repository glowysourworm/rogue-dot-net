using Rogue.NET.Common.Extension;
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
        readonly LayoutGrid _layoutGrid;

        public DijkstraPathFinder(LayoutGrid layoutGrid,
                                  IGridLocator sourceLocation,
                                  IEnumerable<IGridLocator> targetLocations,
                                  DijkstraMapCostCallback costCallback)
             : base(layoutGrid.Bounds.Width,
                    layoutGrid.Bounds.Height,
                    false,
                    sourceLocation,
                    targetLocations,
                    costCallback,
             new DijkstraMapLocatorCallback((column, row) =>
             {
                 return layoutGrid[column, row].Location;
             }))
        {
            _layoutGrid = layoutGrid;

            Run();
        }

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
                // Get the next location towards the source - taking the smallest adjacent output
                nextLocation = this.OutputMap
                                   .GetAdjacentValueLocators(nextLocation.Column, nextLocation.Row)
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
