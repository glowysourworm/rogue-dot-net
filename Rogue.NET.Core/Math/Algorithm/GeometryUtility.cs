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

        /// <summary>
        /// Creates Delaunay triangulation using the Bowyer-Watson algorithm O(n log n). Returns the single edge list (one
        /// edge maximum between points in the mesh)
        /// </summary>
        public static Triangulation<T> Triangulate<T>(IEnumerable<ReferencedVertex<T>> referencedPoints) where T : class
        {
            if (referencedPoints.Count() < 3)
                throw new Exception("Trying to create triangulation with less than 3 points");

            // Procedure
            //
            // 0) Create "super-triangle" that encompasses all the points (the mesh)
            // 1) Add points one-at-a-time to the mesh
            // 2) Find the affected triangles in the mesh
            //      - Point lies inside the circumcircle of the triangle
            //
            // 3) For each edge in each affected triangle
            //      - If edge is not shared by any other affected triangle, Then add to "polygon hole"
            //
            // 4) Remove bad triangles from mesh
            // 5) For each edge in "polygon hole"
            //      - Form new triangle with edge and the added point
            //      - Add this triangle to the mesh
            //
            // 6) For each triangle in the mesh
            //      - If triangle contains a vertex from the original "super-triangle", 
            //        Then remove the triangle from the mesh
            //

            // 0) Create "super-triangle" by using the bounding rectangle for the points inscribed inside of a triangle
            //

            // Construct fake references for the "super-triangle"
            var superTriangleReference1 = typeof(T).Construct<T>();
            var superTriangleReference2 = typeof(T).Construct<T>();
            var superTriangleReference3 = typeof(T).Construct<T>();
            var bounds = new Rectangle(referencedPoints.Select(refPoint => refPoint.Vertex));
            var point1 = new ReferencedVertex<T>(superTriangleReference1, new Vertex(0, 0));
            var point2 = new ReferencedVertex<T>(superTriangleReference2, new Vertex((bounds.BottomRight.X * 2) + 1, 0));
            var point3 = new ReferencedVertex<T>(superTriangleReference3, new Vertex(0, (bounds.BottomRight.Y * 2) + 1));

            // Initialize the mesh (the "super-triangle" is removed as part of the algorithm)
            var mesh = new Triangulation<T>();
            mesh.AddTriangle(new ReferencedTriangle<T>(point1, point2, point3));

            // 1) Add points: one-at-a-time
            //
            foreach (var refPoint in referencedPoints)
            {
                // 2) Find triangles in the mesh whose circum-circle contains the new point
                //
                var badTriangles = mesh.Triangles
                                       .Where(triangle => triangle.CircumCircleContains(refPoint.Vertex))
                                       .Actualize();

                // 3) Choose distinct triangles that don't share an edge
                //
                var distinctBadTriangles = badTriangles.DistinctWith((t1, t2) => t1.SharesEdgeWith(t2))
                                                       .Actualize();

                // 4) Remove the "bad triangles" from the mesh
                //
                foreach (var triangle in badTriangles)
                    mesh.RemoveTriangle(triangle);

                // 5) Create new triangles from the "distinct bad triangles" with each edge of those + the point
                //
                foreach (var edge in distinctBadTriangles.SelectMany(triangle => triangle.Edges))
                    mesh.AddTriangle(new ReferencedTriangle<T>(edge.Point1, edge.Point2, refPoint));
            }

            // 6) (Cleaning Up) Remove any triangles with the "super-triangle" vertices
            //
            foreach (var triangle in mesh.Triangles.Where(x => x.Vertices.Any(refVertex => refVertex.Reference == point1.Reference ||
                                                                                           refVertex.Reference == point2.Reference ||
                                                                                           refVertex.Reference == point3.Reference)).Actualize())
            {
                mesh.RemoveTriangle(triangle);
            }

            return mesh;
        }

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
        /// Computes the Graham Scan of a set of points which returns an ordered subset that constitutes the 
        /// Convex Hull. This is O(n log n). The result is ordered from the first point to the last point - with
        /// the closing edge included.
        /// </summary>
        public static Polygon GrahamScan(IEnumerable<Vertex> points)
        {
            if (points.Count() < 3)
                throw new Exception("Trying to compute convex hull with less than 3 points");

            // Procedure
            //
            // 0) Find the point with coordinate closest to the origin
            //      - Using one other the other dimensions to break a tie
            //
            // 1) Choose that point and label it "P"
            //
            // 2) Sort the points in increasing angle counter-clockwise (in cartesian coordinates) around "P"
            //
            // 3) Iterate the resulting sequence of points
            //      - Consider the current point + previous two
            //
            //      - If they're oriented clockwise (TODO: CHECK SIGN FOR UI COORDINATES)
            //        Then, the last point is NOT part of the convex hull and is in the interior
            //      
            //      - If the point lied on the interior, then discard it and re-create the current
            //        triplet of points with the next point in the sequence filling in for the one
            //        that was on the interior.
            //
            // 

            // Order by Y-Coordinate O(n)
            var orderedPoints = points.OrderBy(point => point.Y)
                                      .Actualize();

            // Get all points with this Y-Coordinate
            var firstPoints = points.Where(point => point.Y == orderedPoints.First().Y);

            // Select the first point
            var firstPoint = firstPoints.MinBy(point => point.X);
            var firstVector = new Vector(firstPoint.X, firstPoint.Y);

            // Order points by their dot product with the first point "P" (a*b = ab cos (theta))
            //
            var angleOrderedPoints = points.Except(new Vertex[] { firstPoint })
                                           .Select(vertex => new Vector(vertex.X, vertex.Y))

                                           // Use dot product with the subtracted vector to see math steps...
                                           //
                                           .OrderByDescending(vector => vector.Subtract(firstVector).X / vector.Subtract(firstVector).Magnitude)
                                           .Select(vector => new Vertex(vector.X, vector.Y))
                                           .ToList();

            // Iterate - removing points that lie in the interior and putting the result on a stack
            //
            var resultStack = new Stack<Vertex>();
            resultStack.Push(firstPoint);

            Vertex segmentPoint1 = firstPoint;
            Vertex segmentPoint2 = new Vertex(0, 0);
            Vertex segmentPoint3 = new Vertex(0, 0);

            // NOTE*** Adding the first point to the end of the ordered list so that it can be used to check 
            //         the preceding point
            //
            angleOrderedPoints.Add(firstPoint);

            for (int i = 0; i < angleOrderedPoints.Count(); i++)
            {
                // First point becomes the second point of the segment-in-question
                if (i == 0)
                    segmentPoint2 = angleOrderedPoints[i];

                // Current point used to check for orientation (of the segment)
                else
                {
                    segmentPoint3 = angleOrderedPoints[i];

                    // Calculate the orientation using the R^3 cross product
                    var orientation = Vertex.Orientation(segmentPoint1, segmentPoint2, segmentPoint3);

                    // DON'T KNOW ABOUT THIS PART - CHECK THE ORIENTATION.
                    //
                    // I think - in UI coordinates - this would imply an angle "sweeping from the x-to-y axis"
                    // so, it would look clockwise - which would mean the 2nd segment point is in the INTERIOR
                    //
                    // Update:  "sweeping from x-to-y" produces a positive orientation. However, so does our
                    //          coordinate ordering. Therefore, the convex hull will be ordered in a positive
                    //          orientation. This corresponds to increasing theta values around point "P". 
                    //
                    if (orientation < 0)
                    {
                        // Just increment the segment points and continue
                        segmentPoint2 = segmentPoint3;
                    }

                    // Go ahead and throw out collinear case
                    else if (orientation == 0)
                    {
                        segmentPoint2 = segmentPoint3;
                    }

                    // Found another point on the hull. So, add the 2nd segment point to the hull
                    //
                    else
                    {
                        // Push 2nd point on the stack
                        resultStack.Push(segmentPoint2);

                        // Increment followers
                        segmentPoint1 = segmentPoint2;
                        segmentPoint2 = segmentPoint3;
                    }
                }
            }

            return new Polygon(resultStack);
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
