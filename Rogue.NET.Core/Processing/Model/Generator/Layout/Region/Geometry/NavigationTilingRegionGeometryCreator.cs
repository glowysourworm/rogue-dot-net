using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Algorithm;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Region.Geometry
{
    /// <summary>
    /// Component that lays out rectangles - subdividing the negative space to create a mesh of
    /// other rectangles that tile the entire region. These can be used to triangulate corridors
    /// that pass through the negative space and avoid overlaps with the room or other terrain
    /// regions we wish to avoid.
    /// </summary>
    public static class NavigationTilingRegionGeometryCreator
    {
        /// <summary>
        /// Creates tiled mesh of connecting regions based on the input regions. This serves as a way to
        /// triangulate paths around the map. These are used to create corridors that don't overlaps the
        /// specified regions.
        /// </summary>
        public static NavigationTiling CreateRegionTiling(int regionWidth, int regionHeight, IEnumerable<RegionBoundary> regions)
        {
            // Initialize the tiling
            var tiling = new NavigationTiling(new NavigationTile(new VertexInt(0, 0), new VertexInt(regionWidth - 1, regionHeight - 1)));

            // Create the tiling by adding region tiles
            foreach (var boundary in regions)
            {
                var regionRectangle = new NavigationTile(new VertexInt(boundary.Left, boundary.Top),
                                                         new VertexInt(boundary.Right, boundary.Bottom));

                tiling.AddTile(regionRectangle);
            }

            // Decide how to create a route:  MST
            var tileCenters = tiling.RegionTiles
                                      .Union(tiling.ConnectingTiles)
                                      .Select(tile => new ReferencedVertex<NavigationTile>(tile, new Vertex(tile.Center)))
                                      .Actualize();

            // Calculate connection points before trying to calculate routes
            //
            // NOTE*** THE CONNECTION POINTS ARE USED TO CALCULATE GRAPH WEIGHTS
            //
            tiling.CalculateConnections();

            // Create MST using Prim's Algorithm
            // var minimumSpanningTree = GeometryUtility.PrimsMinimumSpanningTree(tileCenters, Metric.MetricType.Roguian);

            var navigationGraph = tiling.CreateMinimumSpanningTree();

            // var minimumSpanningTree = GeometryUtility.PrimsMinimumSpanningTree(navigationGraph.Vertices, Metric.MetricType.Roguian);

            // ROUTE:  Defined as a connection from one region tile to another. Create a unique route number for each route
            //
            var routeNumber = 0;

            // For each region pair - calculate a route using the MST
            foreach (var region1 in tiling.RegionTiles)
            {
                foreach (var region2 in tiling.RegionTiles)
                {
                    if (region1 == region2)
                        continue;

                    // TODO:TERRAIN - RETHINK HOW TO DEAL WITH OVERLAPPING REGIONS
                    if (region1.Intersects(region2))
                        continue;

                    // TODO:TERRAIN - THINK ABOUT HOW TO DEAL WITH THIS CASE (probably all overlapping region tiles)
                    if (region1.ConnectionPoints.Count() == 0 || region2.ConnectionPoints.Count() == 0)
                        continue;

                    // Use Breadth First Search through the graph to create a Dijkstra map from region1 -> region2
                    var dijkstraMap = GeometryUtility.BreadthFirstSearch(navigationGraph, region1, region2, Metric.MetricType.Roguian);

                    // Calculate the shortest path through the nagivation tiling
                    var routeTiles = dijkstraMap.GetShortestPath();

                    if (routeTiles.Count() < 2)
                        throw new Exception("Improperly formed shortest path");

                    // TODO:TERRAIN REMOVE THIS
                    // Double-Check to make sure that they're bordering
                    for (int i=0;i<routeTiles.Count() - 1;i++)
                    {
                        var tile1 = routeTiles.ElementAt(i);
                        var tile2 = routeTiles.ElementAt(i + 1);

                        if (!tile1.Borders(tile2) || tile1 == tile2)
                            throw new Exception("Improperly formed shortest path");
                    }

                    // Mark each of these tiles for routing 
                    foreach (var tile in routeTiles)
                    {
                        // Set route number for all involved connection points
                        tile.ConnectionPoints.Where(point => routeTiles.Contains(point.AdjacentTile))
                                             .ForEach(connectionPoint => connectionPoint.IncludeRouteNumber(routeNumber));
                    }

                    routeNumber++;
                }
            }

            // Finally, route the tiles in the MST
            tiling.RouteConnections(Enumerable.Range(0, routeNumber));

            return tiling;
        }
    }
}
