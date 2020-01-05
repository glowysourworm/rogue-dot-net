using Rogue.NET.Core.Model.Scenario.Content.Layout;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Algorithm
{
    public static class TraversalAlgorithm
    {
        /// <summary>
        /// Finds next connected region that hasn't been searched - using Breadth first search.
        /// </summary>
        public static ConnectedRegion<GridLocation> FindNextSearchRegion(ConnectedLayerMap connectedLayer,
                                                                         IEnumerable<ConnectedRegion<GridLocation>> searchableRegions, 
                                                                         IEnumerable<ConnectedRegion<GridLocation>> searchedRegions,
                                                                         ConnectedRegion<GridLocation> startingRegion)
        {
            if (!searchableRegions.Contains(startingRegion))
                throw new Exception("Starting region not in the searchable region collection TraversalAlgorithm.FindNextSearchRegion(...)");

            // Check to see if there are no regions left to search
            if (searchedRegions.Count() == searchableRegions.Count())
                return null;

            if (searchableRegions.Count() == 1)
                return startingRegion;

            // Use Breadth First Search to scan the graph
            //
            var frontierQueue = new Queue<ConnectedRegion<GridLocation>>();
            var discoveredRegions = new Dictionary<ConnectedRegion<GridLocation>, ConnectedRegion<GridLocation>>();

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
                foreach (var connectedRegion in connectedLayer.Connections(region.Id))
                {
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

            return startingRegion;
        }
    }
}
