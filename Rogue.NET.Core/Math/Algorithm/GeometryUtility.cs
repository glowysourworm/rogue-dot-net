using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Math.Geometry.Interface;
using Rogue.NET.Core.Processing.Model.Static;
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
        // TODO:TERRAIN Make this a global tolerance
        readonly static double CELL_CONVERSION_TOLERANCE = 1e-10;

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

        private static double CalculateDistance(Vertex vertex1, Vertex vertex2, MetricType metricType)
        {
            if (System.Math.Abs(vertex1.X - (int)vertex1.X) > CELL_CONVERSION_TOLERANCE)
                throw new Exception("Cell Conversion Tolerance Exceeded");

            if (System.Math.Abs(vertex1.Y - (int)vertex1.Y) > CELL_CONVERSION_TOLERANCE)
                throw new Exception("Cell Conversion Tolerance Exceeded");

            if (System.Math.Abs(vertex2.X - (int)vertex2.X) > CELL_CONVERSION_TOLERANCE)
                throw new Exception("Cell Conversion Tolerance Exceeded");

            if (System.Math.Abs(vertex2.Y - (int)vertex2.Y) > CELL_CONVERSION_TOLERANCE)
                throw new Exception("Cell Conversion Tolerance Exceeded");

            switch (metricType)
            {
                case MetricType.Roguian:
                    return Metric.RoguianDistance(vertex1, vertex2);
                case MetricType.Euclidean:
                    return vertex2.Subtract(vertex1).Magnitude;
                default:
                    throw new Exception("Unhandled metric type GeometryUtility.PrimsMinimumSpanningTree");
            }
        }
    }
}
