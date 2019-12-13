using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Math.Geometry.Interface;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using Rogue.NET.Core.Processing.Model.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static Rogue.NET.Core.Math.Geometry.Metric;

namespace Rogue.NET.Core.Math.Algorithm
{
    /// <summary>
    /// Component that provides Delaunay triangulation calculation for connecting points
    /// </summary>
    public static class GeometryUtility
    {
        public static Graph<T> PrimsMinimumSpanningTree<T>(IEnumerable<Region<T>> regions, MetricType metricType) where T : class, IGridLocator
        {
            var pointsCount = regions.Count();   // O(n)

            if (pointsCount < 1)
                throw new Exception("Trying to build MST with zero points");

            // Procedure
            //
            // 1) Start the tree with a single vertex
            // 2) Calculate edges of the graph that connect NEW points (not yet in the tree)
            //    to the existing tree
            // 3) Choose the least distant point and add that edge to the tree
            //

            var result = new List<GraphEdge<T, Region<T>>>();
            var treeVertices = new List<Region<T>>();

            // This is a bit greedy; but our sets are small. So, should make this more efficient
            foreach (var region in regions)
            {
                // Point may be connected to the tree if it was calculated by the algorithm
                if (treeVertices.Any(vertex => vertex == region))
                    continue;

                // Initialize the tree
                else if (treeVertices.Count == 0)
                    treeVertices.Add(region);

                // Connect the point to the tree
                else
                {
                    // Calculate the least distant vertex IN the tree
                    var connection = treeVertices.MinBy(x => region.CalculateWeight(x, metricType));

                    result.Add(new GraphEdge<T, Region<T>>(new GraphVertex<T, Region<T>>(region, region.GetConnectionPoint(connection, metricType)), 
                                                new GraphVertex<T, Region<T>>(connection, region.GetAdjacentConnectionPoint(connection, metricType))));
                    treeVertices.Add(region);
                }
            }

            return new Graph<T>(result);
        }
    }
}
