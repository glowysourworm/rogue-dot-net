using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Construction;

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
        readonly IRandomSequenceGenerator _randomSequenceGenerator;

        [ImportingConstructor]
        public RegionTriangulationCreator(IRandomSequenceGenerator randomSequenceGenerator)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
        }

        public GraphInfo<T> CreateTriangulation<T>(IEnumerable<Region<T>> regions, LayoutTemplate template) where T : class, IGridLocator
        {
            // Procedure
            //
            // 1) Create connection points for all pairs of regions (WARNING!!! THIS IS EXPENSIVE!)
            // 2) Save the connection points as an edge with reference regions
            // 3) Create a full Delaunay triangulation
            // 4) Remove all edges that are self-referencing
            // 5) Remove all but the shortest distance connection between two regions (USING A EUCILDEAN METRIC)
            //
            // The result will be a Delaunay triangulation of the Regions as a graph. This means
            // the shortest-distance connections are kept between the regions to create a graph of
            // the regions themselves representing the nodes.
            //
            // Finally, run this through an MST algorithm if desired and then choose the ratio of extra
            // corridors to keep.
            //

            // CAN BE EXPENSIVE - TRY AND OPTIMIZE
            var fullGraph = CreateFullGraph(regions);

            if (regions.Count() == 1)
                return fullGraph;

            // Create corridors between the MST and Delaunay using the fill ratio
            //
            // NOTE*** Connection points must only have an MST - because of the minimum size room (4 cells)
            //
            if (template.FillRatioCorridors > 0 &&
                template.ConnectionType != LayoutConnectionType.ConnectionPoints)
            {
                // NOTE*** Delaunay output graph is not fully connected; but the regions ARE fully connected.
                var delaunayGraph = CreateDelaunayTriangulation(fullGraph, regions);

                // The MST must be re-created from the regions. The expensive work is already finished
                //
                var mstGraph = CreateMinimumSpanningTree(delaunayGraph, regions);

                // Add corridors according to the fill ratio from the Delaunay graph
                var extraEdges = delaunayGraph.Edges
                                              .Where(delaunayEdge => !mstGraph.Edges.Any(mstEdge => delaunayEdge.IsEquivalent(mstEdge)))
                                              .Actualize();

                var extraConnections = delaunayGraph.Connections
                                                    .Where(connection => extraEdges.Any(edge => EdgeConnectionComparer(edge, connection)));

                // Calculate extra corridor count
                var extraCorridorCount = (int)(extraEdges.Count() * template.FillRatioCorridors);

                if (extraCorridorCount == 0)
                    return mstGraph;

                // Take random extra edges from extra edges
                var randomExtraEdges = _randomSequenceGenerator.GetDistinctRandomElements(extraEdges, extraCorridorCount);

                var finalConnections = new List<RegionConnectionInfo<T>>(mstGraph.Connections);
                var finalEdges = new List<GraphEdge>(mstGraph.Edges);

                foreach (var edge in randomExtraEdges)
                {
                    foreach (var connection in extraConnections.Where(connection => EdgeConnectionComparer(edge, connection)))
                    {
                        finalEdges.Add(edge);
                        finalConnections.Add(connection);
                    }
                }

                return new GraphInfo<T>(finalConnections, finalEdges);
            }
            else
                return CreateMinimumSpanningTree(fullGraph, regions);
        }

        public GraphInfo<T> CreateDefaultTriangulation<T>(IEnumerable<Region<T>> regions) where T : class, IGridLocator
        {
            if (regions.Count() != 1)
                throw new Exception("Trying to create default triangulation for non-default regions");

            return CreateFullGraph(regions);
        }

        /// <summary>
        /// Creates MST using Prim's Algorithm - which takes O(n log n)
        /// </summary>
        private GraphInfo<T> CreateMinimumSpanningTree<T>(GraphInfo<T> graph, IEnumerable<Region<T>> regions) where T : class, IGridLocator
        {
            if (regions.Count() < 1)
                throw new Exception("Trying to build MST with zero points");

            // NOTE*** The MST is being created on a graph of REGIONS. This will behave differently than a
            //         graph of vertices. So, the input graph should be the FULL GRAPH to avoid issues with
            //         creating an MST of REGIONS.
            //         

            // Procedure (Prim's Algorithm)
            //
            // 1) Start the tree with a single vertex
            // 2) Calculate edges of the graph that connect NEW points not yet in the tree (P)
            //    to the existing tree points (T)
            // 3) Choose the least distant edge and add that edge to the tree
            //

            var unusedRegions = new List<Region<T>>(regions);
            var usedRegions = new List<Region<T>>();
            var tree = new List<GraphEdge>();
            var treeConnections = new List<RegionConnectionInfo<T>>();

            while (usedRegions.Count < regions.Count())
            {
                // Initialize the tree
                if (usedRegions.Count == 0)
                {
                    // Add first vertex to the tree
                    usedRegions.Add(unusedRegions.First());

                    // Remove vertex from unused vertices
                    unusedRegions.RemoveAt(0);
                }

                else
                {
                    Region<T> nextRegion = null;
                    RegionConnectionInfo<T> nextConnection = null;
                    GraphEdge nextEdge = null;
                    double minDistance = double.MaxValue;

                    // Get the next edge that connects an UNUSED region to a USED region
                    foreach (var region1 in unusedRegions)
                    {
                        foreach (var region2 in usedRegions)
                        {
                            // Fetch connection points for the two regions
                            var connections = graph.Connections
                                                   .Where(x => (x.Vertex.ReferenceId == region1.Id &&
                                                                x.AdjacentVertex.ReferenceId == region2.Id) ||
                                                               (x.Vertex.ReferenceId == region2.Id &&
                                                                x.AdjacentVertex.ReferenceId == region1.Id));

                            // CONNECTION NOT GUARANTEED
                            if (!connections.Any())
                                continue;

                            var connection = connections.MinBy(x => x.EuclideanRenderedDistance);
                            var edge = graph.FindEdges(region1.Id, region2.Id)
                                            .Except(tree)
                                            .MinBy(edge => edge.Distance);

                            // EDGE NOT GUARANTEED
                            if (edge == null)
                                continue;

                            // KEEP THE SHORTEST EDGE TO THE EXISTING TREE
                            if (edge.Distance < minDistance)
                            {
                                nextEdge = edge;
                                nextRegion = region1;
                                minDistance = edge.Distance;
                                nextConnection = connection;
                            }
                        }
                    }

                    if (nextEdge == null)
                        throw new Exception("No edge found between regions Minimum Spanning Tree");

                    if (nextConnection == null)
                        throw new Exception("No connection found between regions Minimum Spanning Tree");

                    if (tree.Any(edge => edge.IsEquivalent(nextEdge)))
                        throw new Exception("Trying to add edge to tree that is already contained in the tree CreateMinimumSpanningTree<T>");

                    unusedRegions.Remove(nextRegion);
                    usedRegions.Add(nextRegion);

                    // Add next edge to the tree
                    tree.Add(nextEdge);
                    treeConnections.Add(nextConnection);
                }
            }

            return new GraphInfo<T>(treeConnections, tree);
        }

        /// <summary>
        /// Creates Delaunay triangulation using the Bowyer-Watson algorithm O(n log n). 
        /// </summary>
        private GraphInfo<T> CreateDelaunayTriangulation<T>(GraphInfo<T> fullGraph, IEnumerable<Region<T>> regions) where T : class, IGridLocator
        {
            if (fullGraph.Vertices.Count() < 3)
            {
                return fullGraph;
            }

            // NOTE*** The graph of regions is over the VERTICES of edge connections between two regions (NOT THE 
            //         REGIONS THEMSELVES). 
            //
            //         The result of this algorithm will be a Delaunay Triangulation - giving nearest neighbor graph
            //         relationships - between two VERTICES - which are from two separate REGIONS.
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
            var point1 = new GraphVertex(null, 0, 0, Metric.MetricType.Euclidean);
            var point2 = new GraphVertex(null, (int)((right * 2) + 1), 0, Metric.MetricType.Euclidean);
            var point3 = new GraphVertex(null, 0, (int)((bottom * 2) + 1), Metric.MetricType.Euclidean);

            // Initialize the mesh (the "super-triangle" is removed as part of the algorithm)
            //
            var superTriangle = new Triangle(point1, point2, point3);
            var triangles = new List<Triangle>();

            triangles.Add(superTriangle);

            // Add points: one-at-a-time
            //
            foreach (var graphVertex in fullGraph.Vertices)
            {
                // Find triangles in the mesh whose circum-circle contains the new point
                //
                // Remove those triangles from the mesh and return them
                //
                var badTriangles = triangles.Remove(triangle => triangle.CircumCircleContains(graphVertex));

                // Use edges from the polygon hole to create new triangles. This should be an "outline" of
                // the bad triangles. So, use all edges from the bad triangles except for shared edges.
                //
                foreach (var badTriangle in badTriangles)
                {
                    var otherBadTriangles = badTriangles.Except(new Triangle[] { badTriangle });

                    // Check Shared Edges 1 -> 2
                    if (!otherBadTriangles.Any(triangle => triangle.ContainsEdge(badTriangle.Point1, badTriangle.Point2)))
                        triangles.Add(new Triangle(badTriangle.Point1, badTriangle.Point2, graphVertex));

                    // 2 -> 3
                    if (!otherBadTriangles.Any(triangle => triangle.ContainsEdge(badTriangle.Point2, badTriangle.Point3)))
                        triangles.Add(new Triangle(badTriangle.Point2, badTriangle.Point3, graphVertex));

                    // 3 -> 1
                    if (!otherBadTriangles.Any(triangle => triangle.ContainsEdge(badTriangle.Point3, badTriangle.Point1)))
                        triangles.Add(new Triangle(badTriangle.Point3, badTriangle.Point1, graphVertex));
                }
            }

            // Create the delaunay graph using distinct edges
            var delaunayEdges = new List<GraphEdge>();

            foreach (var triangle in triangles)
            {
                // (Cleaning Up) Remove any edges shared with the "super-triangle" vertices
                //
                if (!(triangle.Point1.ReferenceId == null || triangle.Point2.ReferenceId == null) &&
                    !delaunayEdges.Any(edge => edge.IsEquivalent(triangle.Point1, triangle.Point2)))
                    delaunayEdges.Add(new GraphEdge(triangle.Point1, triangle.Point2));

                if (!(triangle.Point2.ReferenceId == null || triangle.Point3.ReferenceId == null) &&
                    !delaunayEdges.Any(edge => edge.IsEquivalent(triangle.Point2, triangle.Point3)))
                    delaunayEdges.Add(new GraphEdge(triangle.Point2, triangle.Point3));

                if (!(triangle.Point3.ReferenceId == null || triangle.Point1.ReferenceId == null) &&
                    !delaunayEdges.Any(edge => edge.IsEquivalent(triangle.Point3, triangle.Point1)))
                    delaunayEdges.Add(new GraphEdge(triangle.Point3, triangle.Point1));

                // Add vertices that are without edges (two "super-triangle" vertices)
                //if (triangle.Point1.ReferenceId != null && !delaunayGraph.Contains(triangle.Point1))
                //    delaunayGraph.AddVertex(triangle.Point1);

                //if (triangle.Point2.ReferenceId != null && !delaunayGraph.Contains(triangle.Point2))
                //    delaunayGraph.AddVertex(triangle.Point2);

                //if (triangle.Point3.ReferenceId != null && !delaunayGraph.Contains(triangle.Point3))
                //    delaunayGraph.AddVertex(triangle.Point3);
            }

            // (More Cleaning Up) Remove self referencing edges (BY REGION)
            delaunayEdges.Remove(edge => edge.Point1.ReferenceId == edge.Point2.ReferenceId);

            var delaunayConnections = new List<RegionConnectionInfo<T>>();

            // (Even More Cleaning Up) Keep only the minimum edge relating to a SINGLE connection between two region distinct regions
            //
            // ***NOTE  There may be no related edge to a connection from the FULL graph because it wasn't part of the Delaunay 
            //          triangulation for the VERTICES.
            //
            foreach (var region1 in regions)
            {
                foreach (var region2 in regions)
                {
                    if (region1 == region2)
                        continue;

                    // Fetch ACTUAL connection points for the two regions
                    var connection = fullGraph.Connections
                                              .Single(x => (x.Vertex.ReferenceId == region1.Id &&
                                                            x.AdjacentVertex.ReferenceId == region2.Id) ||
                                                           (x.Vertex.ReferenceId == region2.Id &&
                                                            x.AdjacentVertex.ReferenceId == region1.Id));

                    if (delaunayConnections.Contains(connection))
                        continue;

                    // Calculate the min distance edge for this DISTINCT connection
                    var edges = delaunayEdges.Where(edge => EdgeConnectionComparer(edge, connection))
                                             .ToList();

                    // BE SURE THERE'S AN EDGE FOR THIS CONNECTION
                    if (edges.Count == 0)
                        continue;

                    var minEdge = edges.MinBy(edge => edge.Distance);

                    // Using remove -> add to try and make this a little faster
                    delaunayEdges.Remove(edges);
                    delaunayEdges.Add(minEdge);

                    // Add connection to the final delaunay connections
                    delaunayConnections.Add(connection);
                }
            }

            // Return a new graph with modified connections and Delaunay edges
            return new GraphInfo<T>(delaunayConnections, delaunayEdges);
        }

        private GraphInfo<T> CreateFullGraph<T>(IEnumerable<Region<T>> regions) where T : class, IGridLocator
        {
            // For no edges - just create a graph with one vertex
            if (regions.Count() == 1)
            {
                var graph = new GraphInfo<T>(new RegionConnectionInfo<T>[] { });
                graph.AddVertex(new GraphVertex(regions.First().Id, 0, 0, Metric.MetricType.Euclidean));

                return graph;
            }

            // Create distinct region pairs 
            var connectionList = regions.Pairs(regions,
                                               region => region.Id,
                                               region => region.Id,
                                              (region1, region2) => CalculateConnection(region1, region2));

            // Create vertices for the graph -> filter duplicates
            var vertices = connectionList.SelectMany(connection =>
            {
                return new GraphVertex[] { connection.Vertex, connection.AdjacentVertex };

            }).DistinctWith((vertex1, vertex2) => vertex1.Equals(vertex2))
              .Actualize();

            // Create edges for the graph - ignoring ordering and self-referencing
            var edges = vertices.Pairs(vertices,
                                       vertex => vertex,
                                       vertex => vertex,
                                      (vertex1, vertex2) => new GraphEdge(vertex1, vertex2));

            return new GraphInfo<T>(connectionList, edges);
        }

        private RegionConnectionInfo<T> CalculateConnection<T>(Region<T> region1, Region<T> region2) where T : class, IGridLocator
        {
            // Finalized connections
            var connections = new Dictionary<string, RegionConnectionInfo<T>>();

            // Search related edges to look for candidates to know the minimum distance
            var candidateLocations = new List<Tuple<T, T, double>>();

            // Create rendered cell distance method for calculating euclidean distance between locations
            var renderedDistance = new Func<T, T, double>((location1, location2) =>
            {
                var dx = (location2.Column - location1.Column) * ModelConstants.CellWidth;
                var dy = (location2.Row - location1.Row) * ModelConstants.CellHeight;

                return System.Math.Sqrt((dx * dx) + (dy * dy));
            });

            // Candidate Locations
            //
            // - Want to minimize iterating the edge locations as much as possible - so,
            //   for non-overlapping boundaries, iterate just the exposed edge locations.
            //
            // - Otherwise, iterate all edge locations
            //

            // Left of -> Iterate the RIGHT edge
            if (region2.Boundary.IsLeftOf(region1.Boundary))
            {
                candidateLocations
                    .AddRange(region2.RightEdgeExposedLocations
                    .Cartesian(region1.LeftEdgeExposedLocations, (location2, location1) =>
                    {
                        return new Tuple<T, T, double>(location1, location2, renderedDistance(location1, location2));
                    }));
            }

            // Right of -> Iterate the LEFT edge
            if (region2.Boundary.IsRightOf(region1.Boundary))
            {
                candidateLocations
                    .AddRange(region2.LeftEdgeExposedLocations
                    .Cartesian(region1.RightEdgeExposedLocations, (location2, location1) =>
                    {
                        return new Tuple<T, T, double>(location1, location2, renderedDistance(location1, location2));
                    }));
            }

            // Above -> Iterate the BOTTOM edge
            if (region2.Boundary.IsAbove(region1.Boundary))
            {
                candidateLocations
                    .AddRange(region2.BottomEdgeExposedLocations
                    .Cartesian(region1.TopEdgeExposedLocations, (location2, location1) =>
                    {
                        return new Tuple<T, T, double>(location1, location2, renderedDistance(location1, location2));
                    }));
            }

            // Below -> Iterate the ABOVE edge
            if (region2.Boundary.IsBelow(region1.Boundary))
            {
                candidateLocations
                    .AddRange(region2.TopEdgeExposedLocations
                    .Cartesian(region1.BottomEdgeExposedLocations, (location2, location1) =>
                    {
                        return new Tuple<T, T, double>(location1, location2, renderedDistance(location1, location2));
                    }));
            }

            // OVERLAPPING BOUNDARIES
            if (candidateLocations.Count == 0)
            {
                // ALL EDGE LOCATIONS
                candidateLocations
                    .AddRange(region1.EdgeLocations
                    .Cartesian(region2.EdgeLocations, (location1, location2) =>
                    {
                        return new Tuple<T, T, double>(location1, location2, renderedDistance(location1, location2));
                    }));
            }

            // FILTER OUT NON-DISTINCT CONNECTIONS
            candidateLocations = candidateLocations.DistinctWith((tuple1, tuple2) =>
            {
                return (tuple1.Item1.Equals(tuple2.Item1) &&
                        tuple1.Item2.Equals(tuple2.Item2)) ||
                       (tuple1.Item1.Equals(tuple2.Item2) &&
                        tuple1.Item2.Equals(tuple2.Item1));
            }).ToList();

            // No candidates found
            if (candidateLocations.Count == 0)
                throw new Exception("No adjacent node connection found RegionTriangulatin.CalculateConnection");

            // Choose random element from the MINIMA BY DISTANCE of the candidates
            var tuple = _randomSequenceGenerator.GetRandomElement(candidateLocations.Minima(tuple => tuple.Item3));

            return new RegionConnectionInfo<T>()
            {
                AdjacentLocation = tuple.Item2,
                AdjacentVertex = new GraphVertex(region2.Id,
                                                 tuple.Item2.Column * ModelConstants.CellWidth,
                                                 tuple.Item2.Row * ModelConstants.CellHeight,
                                                 Metric.MetricType.Euclidean),
                Location = tuple.Item1,
                Vertex = new GraphVertex(region1.Id,
                                         tuple.Item1.Column * ModelConstants.CellWidth,
                                         tuple.Item1.Row * ModelConstants.CellHeight,
                                         Metric.MetricType.Euclidean),
                EuclideanRenderedDistance = tuple.Item3
            };
        }

        /// <summary>
        /// Compares equality of a GraphEdge and a RegionConnectionInfo 1) by region reference ID, 2) ignoring order
        /// </summary>
        private static bool EdgeConnectionComparer<T>(GraphEdge edge, RegionConnectionInfo<T> connection) where T : class, IGridLocator
        {
            return (edge.Point1.ReferenceId == connection.Vertex.ReferenceId &&
                    edge.Point2.ReferenceId == connection.AdjacentVertex.ReferenceId) ||
                   (edge.Point1.ReferenceId == connection.AdjacentVertex.ReferenceId &&
                    edge.Point2.ReferenceId == connection.Vertex.ReferenceId);
        }
    }
}
