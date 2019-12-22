using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Utility;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface;

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Component
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IRegionTriangulationCreator))]
    public class RegionTriangulationCreator : IRegionTriangulationCreator
    {
        public RegionTriangulationCreator()
        {

        }

        public Graph<Region<T>> CreateTriangulation<T>(IEnumerable<Region<T>> regions, LayoutTemplate template) where T : class, IGridLocator
        {
            // Procedure
            //
            // 1) Create connection points for all pairs of regions (WARNING!!! THIS IS EXPENSIVE!)
            // 2) Save the connection points as an edge with reference regions
            // 3) Create a full Delaunay triangulation
            // 4) Remove all edges that are self-referencing
            // 5) Remove all but the shortest distance connection between two regions
            //
            // The result will be a Delaunay triangulation of the Regions as a graph. This means
            // the shortest-distance connections are kept between the regions to create a graph of
            // the regions themselves representing the nodes.
            //
            // Finally, run this through an MST algorithm if desired and then choose the ratio of extra
            // corridors to keep.
            //

            // CAN BE EXPENSIVE - TRY AND OPTIMIZE
            var graph = CreateFullGraph(regions);

            if (regions.Count() == 1)
                return graph;

            // TODO:TERRAIN - REMOVE THIS. Testing performance of full output
            var delaunayGraph = CreateDelaunayTriangulation(graph, regions);

            //var minimumSpanningGraph = CreateMinimumSpanningTree(delaunayGraph);

            return delaunayGraph;
        }

        private Graph<Region<T>> CreateFullGraph<T>(IEnumerable<Region<T>> regions) where T : class, IGridLocator
        {
            var graph = new Graph<Region<T>>();

            // For no edges - just create a graph with one vertex
            if (regions.Count() == 1)
            {
                graph.AddVertex(new GraphVertex<Region<T>>(regions.First(), 0, 0));

                return graph;
            }

            var regionList = new List<Region<T>>(regions);

            // Create connection points
            for (int i = 0; i < regionList.Count; i++)
            {
                // Iterate starting at the current region to avoid duplicating connections
                for (int j = i; j < regionList.Count; j++)
                {
                    var region1 = regionList[i];
                    var region2 = regionList[j];

                    // Don't add self-referential connections
                    if (region1 == region2)
                        continue;

                    var distance = region1.CalculateConnection(region2, Metric.MetricType.Euclidean);

                    var location1 = region1.GetConnectionPoint(region2, Metric.MetricType.Euclidean);
                    var location2 = region1.GetAdjacentConnectionPoint(region2, Metric.MetricType.Euclidean);

                    // Duplicate vertices are unlikely; but possible. So, have to check
                    // for existing vertices
                    //
                    if (!graph.Vertices.Any(vertex => vertex.Reference == region1 &&
                                                      vertex.Column == location1.Column &&
                                                      vertex.Row == location1.Row))
                        graph.AddVertex(new GraphVertex<Region<T>>(region1, location1.Column, location1.Row));

                    if (!graph.Vertices.Any(vertex => vertex.Reference == region2 &&
                                                      vertex.Column == location2.Column &&
                                                      vertex.Row == location2.Row))
                        graph.AddVertex(new GraphVertex<Region<T>>(region2, location2.Column, location2.Row));
                }
            }

            return graph;
        }

        /// <summary>
        /// Creates MST using Prim's Algorithm - which takes O(n log n)
        /// </summary>
        private Graph<Region<T>> CreateMinimumSpanningTree<T>(Graph<Region<T>> delaunayGraph) where T : class, IGridLocator
        {
            var regionCount = delaunayGraph.Vertices.Count();   // O(n)

            if (regionCount < 1)
                throw new Exception("Trying to build MST with zero points");

            // Procedure
            //
            // 1) Start the tree with a single vertex
            // 2) Calculate edges of the graph that connect NEW points not yet in the tree (P)
            //    to the existing tree points (T)
            // 3) Choose the least distant edge and add that edge to the tree
            //

            var result = new List<GraphEdge<Region<T>>>();
            var unusedVertices = new List<GraphVertex<Region<T>>>(delaunayGraph.Vertices);

            var tree = new Graph<Region<T>>();

            while (tree.Vertices.Count() < regionCount)
            {
                // Initialize the tree
                if (tree.Vertices.Count() == 0)
                {
                    // Add first vertex to the tree
                    tree.AddVertex(unusedVertices.First());

                    // Remove vertex from unused vertices
                    unusedVertices.Remove(unusedVertices.First());
                }

                else
                {
                    // Get the next edge that connects an UNUSED vertex to a USED vertex (in the tree)
                    var nextEdges = unusedVertices.SelectMany(unusedVertex => delaunayGraph[unusedVertex])
                                                 .Where(edge =>
                                                 {
                                                     return tree.Contains(edge.Point1) ||
                                                            tree.Contains(edge.Point2);
                                                 })
                                                 .Actualize();

                    var nextEdge = nextEdges.MinBy(edge =>
                                            {
                                                return edge.Point1
                                                        .Reference
                                                        .CalculateConnection(edge.Point2.Reference, Metric.MetricType.Euclidean);
                                            });

                    if (tree.Contains(nextEdge.Point1) &&
                        tree.Contains(nextEdge.Point2))
                        throw new Exception("Trying to add edge to tree that is already contained in the tree CreateMinimumSpanningTree<T>");

                    // Mark used / unused vertices
                    if (!tree.Contains(nextEdge.Point1))
                        unusedVertices.Remove(nextEdge.Point1);

                    if (!tree.Contains(nextEdge.Point2))
                        unusedVertices.Remove(nextEdge.Point2);

                    // Add next edge to the tree
                    tree.AddEdge(nextEdge);
                }
            }

            return tree;
        }

        /// <summary>
        /// Creates Delaunay triangulation using the Bowyer-Watson algorithm O(n log n). Returns the single edge list (one
        /// edge maximum between points in the mesh)
        /// </summary>
        private Graph<Region<T>> CreateDelaunayTriangulation<T>(Graph<Region<T>> fullGraph, IEnumerable<Region<T>> regions) where T : class, IGridLocator
        {
            if (fullGraph.Vertices.Count() < 3)
            {
                return fullGraph;
            }

            // NOTE*** The graph of regions is over the vertices of edge connections between two regions. These
            //         differ depending on the two regions - because nearest neighbor locations are chosen for
            //         the graph connection. 
            //
            //         This means that the Delaunay triangulation will have extra edges from region-to-region as
            //         well as self-referencing edges (region references itself). These have to be filtered one 
            //         more time to create a "Delaunay Triangulation" of the regions - thought of as graph nodes.
            //          

            // Procedure - https://en.wikipedia.org/wiki/Bowyer%E2%80%93Watson_algorithm
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
            // 7) (Cleaning Up) Remove self-referential edges and keep only the lowest-weight edge between
            //                  two regions (NOT VERTICES)
            //

            // 0) Create "super-triangle" by using the bounding rectangle for the points inscribed inside of a triangle
            //

            // Create an enclosing rectangle for the points
            var top = double.MaxValue;
            var bottom = double.MinValue;
            var left = double.MaxValue;
            var right = double.MinValue;

            foreach (var vertex in fullGraph.Vertices)
            {
                if (vertex.Row < top)
                    top = vertex.Row;

                if (vertex.Row > bottom)
                    bottom = vertex.Row;

                if (vertex.Column < left)
                    left = vertex.Column;

                if (vertex.Column > right)
                    right = vertex.Column;
            }

            // NOTE*** NULL VERTEX REFERENCE USED TO IDENTIFY SUPER TRIANGLE
            var point1 = new GraphVertex<Region<T>>(null, 0, 0);
            var point2 = new GraphVertex<Region<T>>(null, (int)((right * 2) + 1), 0);
            var point3 = new GraphVertex<Region<T>>(null, 0, (int)((bottom * 2) + 1));

            // Initialize the mesh (the "super-triangle" is removed as part of the algorithm)
            //
            var superTriangle = new Triangle<Region<T>>(point1, point2, point3);
            var triangles = new List<Triangle<Region<T>>>();

            triangles.Add(superTriangle);

            // Add points: one-at-a-time
            //
            foreach (var graphVertex in fullGraph.Vertices)
            {
                // Find triangles in the mesh whose circum-circle contains the new point
                //
                // Remove those triangles from the mesh and return them
                //
                var badTriangles = triangles.Filter(triangle => triangle.CircumCircleContains(graphVertex));

                // Use edges from the polygon hole to create new triangles. This should be an "outline" of
                // the bad triangles. So, use all edges from the bad triangles except for shared edges.
                //
                foreach (var badTriangle in badTriangles)
                {
                    var otherBadTriangles = badTriangles.Except(new Triangle<Region<T>>[] { badTriangle });

                    // Check Shared Edges 1 -> 2
                    if (!otherBadTriangles.Any(triangle => triangle.ContainsEdge(badTriangle.Point1, badTriangle.Point2)))
                        triangles.Add(new Triangle<Region<T>>(badTriangle.Point1, badTriangle.Point2, graphVertex));

                    // 2 -> 3
                    if (!otherBadTriangles.Any(triangle => triangle.ContainsEdge(badTriangle.Point2, badTriangle.Point3)))
                        triangles.Add(new Triangle<Region<T>>(badTriangle.Point2, badTriangle.Point3, graphVertex));

                    // 3 -> 1
                    if (!otherBadTriangles.Any(triangle => triangle.ContainsEdge(badTriangle.Point3, badTriangle.Point1)))
                        triangles.Add(new Triangle<Region<T>>(badTriangle.Point3, badTriangle.Point1, graphVertex));
                }
            }

            // Create the delaunay graph using distinct edges
            var delaunayGraph = new Graph<Region<T>>();
            
            foreach (var triangle in triangles)
            {
                // (Cleaning Up) Remove any edges shared with the "super-triangle" vertices
                //
                if (!(triangle.Point1.Reference == null || triangle.Point2.Reference == null) &&                    
                    !delaunayGraph.Edges.Any(edge => edge.Equals(triangle.Point1, triangle.Point2)))
                     delaunayGraph.AddEdge(new GraphEdge<Region<T>>(triangle.Point1, triangle.Point2));

                if (!(triangle.Point2.Reference == null || triangle.Point3.Reference == null) &&
                    !delaunayGraph.Edges.Any(edge => edge.Equals(triangle.Point2, triangle.Point3)))
                     delaunayGraph.AddEdge(new GraphEdge<Region<T>>(triangle.Point2, triangle.Point3));

                if (!(triangle.Point3.Reference == null || triangle.Point1.Reference == null) &&
                    !delaunayGraph.Edges.Any(edge => edge.Equals(triangle.Point3, triangle.Point1)))
                     delaunayGraph.AddEdge(new GraphEdge<Region<T>>(triangle.Point3, triangle.Point1));

                // Add vertices that are without edges (two "super-triangle" vertices)
                if (triangle.Point1.Reference != null && !delaunayGraph.Contains(triangle.Point1))
                    delaunayGraph.AddVertex(triangle.Point1);

                if (triangle.Point2.Reference != null && !delaunayGraph.Contains(triangle.Point2))
                    delaunayGraph.AddVertex(triangle.Point2);

                if (triangle.Point3.Reference != null && !delaunayGraph.Contains(triangle.Point3))
                    delaunayGraph.AddVertex(triangle.Point3);
            }

            // (More Cleaning Up) Remove self referencing edges
            delaunayGraph.FilterByEdge(edge => edge.Point1.Reference == edge.Point2.Reference);

            // delaunayGraph.OutputCSV(ResourceConstants.DijkstraOutputDirectory, "delaunay_before");

            // (Even More Cleaning Up) Keep only the edges involving the proper region connection points
            foreach (var region1 in regions)
            {
                foreach (var region2 in regions)
                {
                    if (region1 == region2)
                        continue;

                    // Fetch ACTUAL connection points for the two regions
                    var actualConnection1 = region1.GetConnectionPoint(region2, Metric.MetricType.Euclidean);
                    var actualConnection2 = region1.GetAdjacentConnectionPoint(region2, Metric.MetricType.Euclidean);

                    // Keep only the edge with the proper connection points
                    foreach (var edge in delaunayGraph.FindEdges(region1, region2))
                    {
                        var criteria1 = (edge.Point1.Column == actualConnection1.Column &&
                                         edge.Point1.Row == actualConnection1.Row) &&
                                        (edge.Point2.Column == actualConnection2.Column &&
                                         edge.Point2.Row == actualConnection2.Row);

                        var criteria2 = (edge.Point1.Column == actualConnection2.Column &&
                                         edge.Point1.Row == actualConnection2.Row) &&
                                        (edge.Point2.Column == actualConnection1.Column &&
                                         edge.Point2.Row == actualConnection1.Row);

                        // Edge must contain both actual connection points
                        if (criteria1 || criteria2)
                            continue;

                        // Edge is a superfluous connection between the two regions
                        delaunayGraph.Remove(edge);
                    }
                }
            }

            // Finally, filter any vertices that aren't being used; but be sure that there is at least one
            // per region
            delaunayGraph.FilterByVertex(vertex => !delaunayGraph[vertex].Any());

            // delaunayGraph.OutputCSV(ResourceConstants.DijkstraOutputDirectory, "delaunay_after");

            // Validate the graph
            if (delaunayGraph.Vertices.Any(vertex => delaunayGraph[vertex].Count() == 0))
                throw new Exception("Unconnected node detected in Delaunay triangulation");

            if (delaunayGraph.Edges.Any(edge => edge.Point1.Reference == edge.Point2.Reference))
                throw new Exception("Self-referential full graph created RegionTriangulationCreator");

            return delaunayGraph;
        }
    }
}
