using Rogue.NET.Common.Collection;
using Rogue.NET.Core.Math.Algorithm.Interface;
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
        /// <summary>
        /// Runs Dijkstra's algorithm on an IGraph using the specified type. NOTE*** Type T must support
        /// reference lookup. (Used for list comparison)
        /// </summary>
        public static GraphTraversal<TNode, TEdge> Run<TNode, TEdge>(IGraph<TNode, TEdge> graph, TNode source, TNode destination) where TNode : IGraphNode
                                                                                                                                  where TEdge : IGraphEdge<TNode>
        {
            var visitedDict = new Dictionary<TNode, double>();
            var outputDict = new Dictionary<TNode, double>();

            // Initialize the cost dictionary
            foreach (var vertex in graph.Vertices)
            {
                if (ReferenceEquals(vertex, source))
                    outputDict.Add(vertex, 0);

                else
                    outputDict.Add(vertex, double.MaxValue);
            }

            // Track the frontier to check the lowest cost next step towards the destination
            var frontier = new BinarySearchTree<double, List<TNode>>();

            var currentVertex = source;

            // Iterate while target not reached (AND) not visited
            while (!visitedDict.ContainsKey(currentVertex) &&
                   !ReferenceEquals(currentVertex, destination))
            {
                // Fetch the current weight for this vertex
                var currentWeight = outputDict[currentVertex];

                // Add the new vertex with a max value weight
                visitedDict.Add(currentVertex, currentWeight);

                // Iterate edges connected to the current vertex
                foreach (var edge in graph.GetAdjacentEdges(currentVertex))
                {
                    var otherVertex = ReferenceEquals(edge.Node, currentVertex) ? edge.AdjacentNode : edge.Node;

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
            if (!ReferenceEquals(currentVertex, destination))
                return null;

            else
            {
                // Fetch the final route from the output by tracing the vertices back to the source by lowest weight
                var currentWeight = double.MaxValue;
                var currentNode = destination;
                var nextNode = destination;

                var finalRoute = new List<TEdge>();
                

                do
                {
                    var finalEdge = default(TEdge);

                    // Find lowest weight edge towards source
                    foreach (var edge in graph.GetAdjacentEdges(currentNode))
                    {
                        var otherNode = ReferenceEquals(edge.Node, currentNode) ? edge.AdjacentNode : edge.Node;

                        if (outputDict[otherNode] < currentWeight)
                        {
                            currentWeight = outputDict[otherNode];
                            nextNode = otherNode;
                            finalEdge = edge;
                        }
                    }

                    if (!ReferenceEquals(finalEdge, default(TEdge)))
                        finalRoute.Add(finalEdge);

                    else
                        throw new System.Exception("Invalid route found:  DijkstrasAlgorithm.Run");

                    // Update the follower
                    currentNode = nextNode;

                } while (!ReferenceEquals(currentNode, source));

                return new GraphTraversal<TNode, TEdge>(source, destination, finalRoute);
            }
        }

        private static void UpdateOutput<T>(Dictionary<T, double> outputDict,
                                            BinarySearchTree<double, List<T>> frontier,
                                            IGraphEdge<T> edge,
                                            T adjacentVertex,
                                            double currentWeight) where T : IGraphNode
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
            outputDict[adjacentVertex] = System.Math.Min(outputDict[adjacentVertex], currentWeight + edge.Weight);

            // Update the frontier
            var newWeight = outputDict[adjacentVertex];

            // UPDATE THE FRONTIER
            var oldWeightList = frontier.Search(oldWeight);
            var newWeightList = frontier.Search(newWeight);

            // Both weights are absent from the frontier
            if (oldWeightList == null &&
                newWeightList == null)
                frontier.Insert(newWeight, new List<T>() { adjacentVertex });

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
                frontier.Insert(newWeight, new List<T>() { adjacentVertex });
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
