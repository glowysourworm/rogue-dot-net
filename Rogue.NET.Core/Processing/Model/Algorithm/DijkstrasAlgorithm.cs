using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Geometry;

using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Math.Algorithm
{
    /// <summary>
    /// Implementation of Dijkstra's Algorithm for the Graph
    /// </summary>
    public static class DijkstrasAlgorithm
    {
        public static GraphTraversal Run(Graph graph, GraphVertex source, GraphVertex destination)
        {
            var visitedDict = new Dictionary<GraphVertex, double>();
            var outputDict = new Dictionary<GraphVertex, double>();

            // Initialize the cost dictionary
            foreach (var vertex in graph.Vertices)
            {
                if (vertex.Equals(source))
                    outputDict.Add(vertex, 0);

                else
                    outputDict.Add(vertex, double.MaxValue);
            }

            // Track the frontier to check the lowest cost next step towards the destination
            var frontier = new BinarySearchTree<double, List<GraphVertex>>();

            var currentVertex = source;

            // Iterate while target not reached (AND) not visited
            while (!visitedDict.ContainsKey(currentVertex) &&
                   !currentVertex.Equals(destination))
            {
                // Fetch the current weight for this vertex
                var currentWeight = outputDict[currentVertex];

                // Add the new vertex with a max value weight
                visitedDict.Add(currentVertex, currentWeight);

                // Iterate edges connected to the current vertex
                foreach (var edge in graph[currentVertex])
                {
                    var otherVertex = edge.Point1.Equals(currentVertex) ? edge.Point2 : edge.Point1;

                    // Not yet visited - CAN MODIFY OUTPUT VALUE
                    if (!visitedDict.ContainsKey(otherVertex))
                    {
                        UpdateOutput(outputDict, frontier, edge, otherVertex, currentWeight);
                    }
                }

                // Select next location from frontier queue - using the smallest weight
                if (frontier.Count > 0)
                {
                    // Lists in the frontier must have an entry
                    var nextCostList = frontier.Min();
                    var nextCost = frontier.MinKey();

                    // Get the first from the list
                    currentVertex = nextCostList.First();

                    // Maintain frontier list
                    nextCostList.RemoveAt(0);

                    if (nextCostList.Count == 0)
                        frontier.Remove(nextCost);
                }
            }

            // Create the final graph traversal
            if (!currentVertex.Equals(destination))
                return null;

            else
            {
                // Fetch the final route from the output by ordering the vertices by weight
                var finalWeight = outputDict[destination];
                var finalRoute = outputDict.Where(element => element.Value <= finalWeight)
                                           .OrderBy(element => element.Value)
                                           .Select(element => element.Key)
                                           .Actualize();

                return new GraphTraversal(source, destination, finalRoute);
            }
        }

        private static void UpdateOutput(Dictionary<GraphVertex, double> outputDict,
                                         BinarySearchTree<double, List<GraphVertex>> frontier,
                                         GraphEdge edge,
                                         GraphVertex adjacentVertex,
                                         double currentWeight)
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
            var oldWeight = outputDict[adjacentVertex];

            // Update the output map
            outputDict[adjacentVertex] = System.Math.Min(outputDict[adjacentVertex], currentWeight + edge.Distance);

            // Update the frontier
            var newWeight = outputDict[adjacentVertex];

            // UPDATE THE FRONTIER
            var oldWeightList = frontier.Search(oldWeight);
            var newWeightList = frontier.Search(newWeight);

            // Both weights are absent from the frontier
            if (oldWeightList == null &&
                newWeightList == null)
                frontier.Insert(newWeight, new List<GraphVertex>() { adjacentVertex });

            // Old weight list exists; New weight list is absent
            else if (oldWeightList != null &&
                     newWeightList == null)
            {
                // Check for existing locator
                if (oldWeightList.Contains(adjacentVertex))
                    oldWeightList.Remove(adjacentVertex);

                // Remove unused node
                if (oldWeightList.Count == 0)
                    frontier.Remove(oldWeight);

                // Insert new node in the frontier
                frontier.Insert(newWeight, new List<GraphVertex>() { adjacentVertex });
            }

            // Old weight is absent; New weight exists
            else if (oldWeightList == null &&
                     newWeightList != null)
            {
                // Locator doesn't exist in list
                if (!newWeightList.Contains(adjacentVertex))
                    newWeightList.Add(adjacentVertex);
            }

            // Both old and new weight lists exist
            else
            {
                // Check that they're different lists
                if (oldWeightList != newWeightList)
                {
                    // Check that old weight list has element removed
                    if (oldWeightList.Contains(adjacentVertex))
                        oldWeightList.Remove(adjacentVertex);

                    // Check that new weight list has element added
                    if (!newWeightList.Contains(adjacentVertex))
                        newWeightList.Add(adjacentVertex);
                }
            }
        }
    }
}
