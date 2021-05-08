using Rogue.NET.Core.Model.Scenario.Content.Layout;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Algorithm
{
    public static class TraversalAlgorithm
    {
        /// <summary>
        /// Finds next connected region that hasn't been searched - using Breadth first search. Returns null if nothing is found.
        /// </summary>
        public static Region<GridLocation> FindNextSearchRegion(ConnectedLayerMap connectedLayer,
                                                                IEnumerable<Region<GridLocation>> searchableRegions, 
                                                                IEnumerable<Region<GridLocation>> searchedRegions,
                                                                Region<GridLocation> startingRegion)
        {
            if (!searchableRegions.Contains(startingRegion))
                throw new Exception("Starting region not in the searchable region collection TraversalAlgorithm.FindNextSearchRegion(...)");

            // Check to see if there are no regions left to search
            if (searchedRegions.Count() == searchableRegions.Count())
                return null;

            // Just a single region in the layout
            if (searchableRegions.Count() == 1)
                return startingRegion;

            // Starting region has not yet been searched
            if (!searchedRegions.Contains(startingRegion))
                return startingRegion;

            // Use Breadth First Search to scan the graph
            //
            var frontierQueue = new Queue<Region<GridLocation>>();
            var discoveredRegions = new Dictionary<Region<GridLocation>, Region<GridLocation>>();

            // Initialize the queue
            frontierQueue.Enqueue(startingRegion);

            while (frontierQueue.Any())
            {
                // Analyze the next region
                var region = frontierQueue.Dequeue();

                // If region hasn't been discovered - then add it
                if (!discoveredRegions.ContainsKey(region))
                    discoveredRegions.Add(region, region);

                // Find all connections and add them to the frontier
                foreach (var regionConnection in connectedLayer.Connections(region.Id))
                {
                    var connectedRegion = connectedLayer.Regions[regionConnection.AdjacentRegionId];

                    // Exclude non-searchable regions
                    if (!searchableRegions.Contains(connectedRegion))
                        continue;

                    if (!searchedRegions.Contains(connectedRegion))
                        return connectedRegion;

                    // O(1) check to see that the region hasn't been discovered
                    if (!discoveredRegions.ContainsKey(connectedRegion))
                        frontierQueue.Enqueue(connectedRegion);
                }
            }

            return null;
        }
    }
}
