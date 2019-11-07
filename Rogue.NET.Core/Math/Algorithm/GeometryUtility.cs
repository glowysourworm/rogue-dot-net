using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Math.Geometry.Interface;
using Rogue.NET.Core.Processing.Model.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using static Rogue.NET.Core.Math.Geometry.Metric;

namespace Rogue.NET.Core.Math.Algorithm
{
    /// <summary>
    /// Component that provides Delaunay triangulation calculation for connecting points
    /// </summary>
    public static class GeometryUtility
    {
        public static Graph<T> PrimsMinimumSpanningTree<T>(IEnumerable<ReferencedVertex<T>> referencedPoints, MetricType metricType) where T : class, IGraphWeightProvider<T>
        {
            var pointsCount = referencedPoints.Count();   // O(n)

            if (pointsCount < 1)
                throw new Exception("Trying to build MST with zero points");

            // Procedure
            //
            // 1) Start the tree with a single vertex
            // 2) Calculate edges of the graph that connect NEW points (not yet in the tree)
            //    to the existing tree
            // 3) Choose the least distant point and add that edge to the tree
            //

            var result = new List<ReferencedEdge<T>>();
            var treeVertices = new List<ReferencedVertex<T>>();

            // This is a bit greedy; but our sets are small. So, should make this more efficient
            foreach (var refPoint in referencedPoints)
            {
                // Point may be connected to the tree if it was calculated by the algorithm
                if (treeVertices.Any(vertex => vertex.Reference == refPoint.Reference))
                    continue;

                // Initialize the tree
                else if (treeVertices.Count == 0)
                    treeVertices.Add(refPoint);

                // Connect the point to the tree
                else
                {
                    // Calculate the least distant vertex IN the tree
                    var connection = treeVertices.MinBy(x => refPoint.Reference.CalculateWeight(x.Reference, metricType));

                    result.Add(new ReferencedEdge<T>(refPoint, connection));
                    treeVertices.Add(refPoint);
                }
            }

            return new Graph<T>(result);
        }

        /// <summary>
        /// Performs breadth first search on the specified mesh - using its edges to iterate from start to finish - and
        /// creating a Dijkstra map of the graph.
        /// </summary>
        /// <typeparam name="T">The reference object type for the mesh vertices</typeparam>
        public static DijkstraMap<T> BreadthFirstSearch<T>(Graph<T> graph, T startingNode, T endingNode, MetricType metricType) where T : class, IGraphWeightProvider<T>
        {
            // Calculate distinct vertex connections in the mesh
            var connections = graph.GetConnections();

            // Find the starting and ending vertices
            var startingVertex = connections.Keys.FirstOrDefault(vertex => vertex.Reference == startingNode);
            var endingVertex = connections.Keys.FirstOrDefault(vertex => vertex.Reference == endingNode);

            if (startingVertex == null || endingVertex == null)
                throw new Exception("Starting or ending vertices not found in the mesh BreadthFirstSearch");

            // Initialize the graph
            var dijkstraNodes = connections.Keys.Select(refVertex => new DijkstraMapNode<T>(refVertex.Reference, 
                                                                                            refVertex.Reference == startingNode ? 0.0 : double.MaxValue))
                                                .ToList();

            // Prepare for iterating
            var visitedVertices = new List<ReferencedVertex<T>>();
            var nodeQueue = new Queue<DijkstraMapNode<T>>();
            var firstNode = dijkstraNodes.First(node => node.Reference == startingNode);

            nodeQueue.Enqueue(firstNode);

            while (nodeQueue.Count > 0)
            {
                // Get the next node
                var currentNode = nodeQueue.Dequeue();

                // (Greedy - should probably integrate this with the mesh)
                var currentVertex = connections.Keys.First(vertex => vertex.Reference == currentNode.Reference);

                // Find connecting nodes and calculate the Dijkstra weight - queue the node if it has not already been visited
                foreach (var vertex in connections[currentVertex])
                {
                    // Calculate distance from current vertex
                    var distanceFromCurrent = currentVertex.Reference.CalculateWeight(vertex.Reference, metricType);

                    // Find the Dijkstra node for this (Greedy - should probably integrate this with the mesh)
                    var dijkstraNode = dijkstraNodes.FirstOrDefault(node => node.Reference == vertex.Reference);

                    // Update Dijkstra Weight - smaller of the current weight (or) the current node's weight + distance
                    dijkstraNode.DijkstraWeight = System.Math.Min(dijkstraNode.DijkstraWeight,
                                                                currentNode.DijkstraWeight + distanceFromCurrent);

                    // Connect nodes
                    if (!dijkstraNode.ConnectedNodes.Contains(currentNode))
                        dijkstraNode.AddNode(currentNode);

                    if (!currentNode.ConnectedNodes.Contains(dijkstraNode))
                        currentNode.AddNode(dijkstraNode);

                    // If vertex is not marked visited - queue it up
                    if (!visitedVertices.Contains(vertex))
                    {
                        // Mark visited
                        visitedVertices.Add(vertex);

                        // Add to queue
                        nodeQueue.Enqueue(dijkstraNode);
                    }
                }
            }

            return new DijkstraMap<T>(firstNode, dijkstraNodes.First(node => node.Reference == endingNode));
        }

        /// <summary>
        /// Creates a Dijkstra map from location1 to location2 using the provided weight (input) map.
        /// </summary>
        /// <param name="inputMap">Weight map (of the same grid dimensions) - relative to [0, 1] (but not necessarily [0, 1]) - to multiply the graph edges by when calculating the Dijkstra weight.</param>
        /// <returns>A weighted map of the path costs from location1 to location2</returns>
        public static double[,] CreateDijkstraMap(this double[,] inputMap, VertexInt location1, VertexInt location2)
        {
            // Initialize the Dijkstra Map
            var dijkstraMap = new double[inputMap.GetLength(0), inputMap.GetLength(1)];

            for (int i=0;i<dijkstraMap.GetLength(0);i++)
            {
                // Set to "infinity" except for the start location
                for (int j = 0; j < dijkstraMap.GetLength(1); j++)
                    dijkstraMap[i, j] = (i == location1.X && j == location1.Y) ? 0 : double.MaxValue;
            }

            // Track visited elements
            var visitedMap = new bool[inputMap.GetLength(0), inputMap.GetLength(1)];

            // Use stack to know what elements have been verified. Starting with test element - continue 
            // until all connected elements have been added to the resulting region.
            var stack = new Stack<VertexInt>(inputMap.GetLength(0) * inputMap.GetLength(1));

            // Process the first element
            stack.Push(location1);

            // NOTE*** Adding a variable weighting for a change in location (SHOULD BE SET TO 1.0)
            var tileMovementCost = 1.0;

            while (stack.Count > 0)
            {
                var currentLocation = stack.Pop();
                var column = currentLocation.X;
                var row = currentLocation.Y;
                var currentWeight = dijkstraMap[column, row];

                // Mark the element as visited
                visitedMap[column, row] = true;

                // Search cardinally adjacent elements (N,S,E,W)
                var north = row - 1 >= 0;
                var south = row + 1 < inputMap.GetLength(1);
                var east = column + 1 < inputMap.GetLength(0);
                var west = column - 1 >= 0;

                // Dijkstra Weight = Current Value + ("Change in Location Cost" + "Gradient Cost") 
                //                 = Current Value + (1 + Input Map Change)
                if (north)
                    dijkstraMap[column, row - 1] = System.Math.Min(dijkstraMap[column, row - 1], currentWeight + (inputMap[column, row - 1] - inputMap[column, row]) + tileMovementCost);

                if (south)
                    dijkstraMap[column, row + 1] = System.Math.Min(dijkstraMap[column, row + 1], currentWeight + (inputMap[column, row + 1] - inputMap[column, row]) + tileMovementCost);

                if (east)
                    dijkstraMap[column + 1, row] = System.Math.Min(dijkstraMap[column + 1, row], currentWeight + (inputMap[column + 1, row] - inputMap[column, row]) + tileMovementCost);

                if (west)
                    dijkstraMap[column - 1, row] = System.Math.Min(dijkstraMap[column - 1, row], currentWeight + (inputMap[column - 1, row] - inputMap[column, row]) + tileMovementCost);

                if (north && east)
                    dijkstraMap[column + 1, row - 1] = System.Math.Min(dijkstraMap[column + 1, row - 1], currentWeight + (inputMap[column + 1, row - 1] - inputMap[column, row]) + tileMovementCost);

                if (north && west)
                    dijkstraMap[column - 1, row - 1] = System.Math.Min(dijkstraMap[column - 1, row - 1], currentWeight + (inputMap[column - 1, row - 1] - inputMap[column, row]) + tileMovementCost);

                if (south && east)
                    dijkstraMap[column + 1, row + 1] = System.Math.Min(dijkstraMap[column + 1, row + 1], currentWeight + (inputMap[column + 1, row + 1] - inputMap[column, row]) + tileMovementCost);

                if (south && west)
                    dijkstraMap[column - 1, row + 1] = System.Math.Min(dijkstraMap[column - 1, row + 1], currentWeight + (inputMap[column - 1, row + 1] - inputMap[column, row]) + tileMovementCost);

                // Push cells onto the stack to be iterated
                if (north && !visitedMap[column, row - 1])
                    stack.Push(new VertexInt(column, row - 1));

                if (south && !visitedMap[column, row + 1])
                    stack.Push(new VertexInt(column, row + 1));

                if (east && !visitedMap[column + 1, row])
                    stack.Push(new VertexInt(column + 1, row));

                if (west && !visitedMap[column - 1, row])
                    stack.Push(new VertexInt(column - 1, row));

                if (north && east && !visitedMap[column + 1, row - 1])
                    stack.Push(new VertexInt(column + 1, row - 1));

                if (north && west && !visitedMap[column - 1, row - 1])
                    stack.Push(new VertexInt(column - 1, row - 1));

                if (south && east && !visitedMap[column + 1, row + 1])
                    stack.Push(new VertexInt(column + 1, row + 1));

                if (south && west && !visitedMap[column - 1, row + 1])
                    stack.Push(new VertexInt(column - 1, row + 1));
            }

            return dijkstraMap;
        }
    }
}
