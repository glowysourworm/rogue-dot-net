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
                                                                         IEnumerable<ConnectedRegion<GridLocation>> searchedRegions,
                                                                         ConnectedRegion<GridLocation> startingRegion)
        {
            // Check to see if there are no regions left to search
            if (searchedRegions.Count() == connectedLayer.Regions.Count())
                return null;

            if (connectedLayer.Regions.Count() == 1)
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
                foreach (var connectedRegion in connectedLayer.Connections(startingRegion.Id))
                {
                    if (connectedRegion == startingRegion)
                        throw new Exception("Improperly initialized connections TraversalAlgorithm.FindNextSearchRegion");

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
