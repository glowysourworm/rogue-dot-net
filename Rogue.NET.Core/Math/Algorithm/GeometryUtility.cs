using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Math.Algorithm
{
    /// <summary>
    /// Component that provides Delaunay triangulation calculation for connecting points
    /// </summary>
    public static class GeometryUtility
    {
        /// <summary>
        /// Creates triangulation using the Bowyer-Watson algorithm O(n log n). Returns the single edge list (one
        /// edge maximum between points in the mesh)
        /// </summary>
        public static Mesh BowyerWatson(IEnumerable<Vertex> points)
        {
            if (points.Count() < 3)
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
            var bounds = new Rectangle(points);
            var point1 = new Vertex(0, 0);
            var point2 = new Vertex((bounds.BottomRight.X * 2) + 1, 0);
            var point3 = new Vertex(0, (bounds.BottomRight.Y * 2) + 1);

            // Initialize the mesh (the "super-triangle" is removed as part of the algorithm)
            var mesh = new Mesh();
            mesh.AddTriangle(new Triangle(point1, point2, point3));

            // 1) Add points: one-at-a-time
            //
            foreach (var point in points)
            {
                // 2) Find triangles in the mesh whose circum-circle contains the new point
                //
                var badTriangles = mesh.Triangles
                                       .Where(triangle => triangle.CircumCircleContains(point))
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
                    mesh.AddTriangle(new Triangle(edge.Point1, edge.Point2, point));
            }

            // 6) (Cleaning Up) Remove any triangles with the "super-triangle" vertices
            //
            foreach (var triangle in mesh.Triangles.Where(x => x.Vertices.Any(vertex => vertex == point1 ||
                                                                                        vertex == point2 ||
                                                                                        vertex == point3)).Actualize())
            {
                mesh.RemoveTriangle(triangle);
            }

            return mesh;
        }

        public static IEnumerable<Edge> PrimsMinimumSpanningTree(IEnumerable<Vertex> points)
        {
            var pointsCount = points.Count();   // O(n)

            if (pointsCount < 1)
                throw new Exception("Trying to build MST with zero points");

            // Procedure
            //
            // 1) Start the tree with a single vertex
            // 2) Calculate edges of the graph that connect NEW points (not yet in the tree)
            //    to the existing tree
            // 3) Choose the least distant point and add that edge to the tree
            //

            var result = new List<Edge>();
            var treeVertices = new List<Vertex>();

            // This is a bit greedy; but our sets are small. So, should make this more efficient
            foreach (var point in points)
            {
                // Point may be connected to the tree if it was calculated by the algorithm
                if (treeVertices.Contains(point))
                    continue;

                // Initialize the tree
                else if (treeVertices.Count == 0)
                    treeVertices.Add(point);

                // Connect the point to the tree
                else
                {
                    // Calculate the least distant vertex IN the tree
                    var connection = treeVertices.MinBy(x => x.Subtract(point).Magnitude);

                    result.Add(new Edge(point, connection));
                    treeVertices.Add(point);
                }
            }

            return result;
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
    }
}
