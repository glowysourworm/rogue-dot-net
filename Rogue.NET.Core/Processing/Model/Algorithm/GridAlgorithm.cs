using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Construction;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using Rogue.NET.Core.Processing.Model.Extension;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Algorithm
{
    public static class GridAlgorithm
    {
        public static IEnumerable<Region<T>> ConstructRegions<T>(this T[,] grid, Func<T, bool> predicate) where T : class, IGridLocator
        {
            var regionConstructors = IdentifyRegions(grid, predicate);

            return regionConstructors.Select(info => new Region<T>(info.Locations, info.EdgeLocations, info.Boundary, info.ParentBoundary))
                                     .Actualize();
        }

        public static IEnumerable<ConnectedRegion<T>> ConstructConnectedRegions<T>(this T[,] grid, Func<T, bool> predicate) where T : class, IGridLocator
        {
            var regionConstructors = IdentifyRegions(grid, predicate);

            return regionConstructors.Select(info => new ConnectedRegion<T>(info.Locations, info.EdgeLocations, info.Boundary, info.ParentBoundary))
                                     .Actualize();
        }

        /// <summary>
        /// Identifies regions using Breadth First Search (Flood Fill) algorithm with the specified (positive) predicate.
        /// </summary>
        public static IEnumerable<RegionConstructorInfo<T>> IdentifyRegions<T>(T[,] grid, Func<T, bool> predicate) where T : class, IGridLocator
        {
            // Procedure
            //
            // 0) Iterate cells
            // 1) First cell that's non-empty AND not part of an existing region
            //    use flood fill to find connected cells

            // Collect region data to pass to level grid constructor
            var regions = new List<RegionConstructorInfo<T>>();
            var regionGrids = new List<T[,]>();

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j] != null &&
                        predicate(grid[i, j]))
                    {
                        // First, identify cell (room) regions
                        if (regionGrids.All(regionGrid => regionGrid[i, j] == null))
                        {
                            // Keep track of grids for fast checking 
                            T[,] regionGrid = null;

                            // Use flood fill to locate all region cells
                            var region = FloodFill(grid, i, j, predicate, out regionGrid);

                            regions.Add(region);
                            regionGrids.Add(regionGrid);
                        }
                    }
                }
            }

            return regions;
        }

        /// <summary>
        /// Applied Breadth First Search to try and identify a region at the given test location using the specified (positive) predicate. 
        /// Returns instantiated region with the results.
        /// </summary>
        public static RegionConstructorInfo<T> FloodFill<T>(T[,] grid, int column, int row, Func<T, bool> predicate, out T[,] regionGrid) where T : class, IGridLocator
        {
            // Region Data
            regionGrid = new T[grid.GetLength(0), grid.GetLength(1)];
            var regionLocations = new List<T>();
            var edgeLocations = new List<T>();

            // Use queue to know what locations have been verified. Starting with test location - continue 
            // until all connected cells have been added to the resulting region using the predicate.
            var resultQueue = new Queue<T>(grid.GetLength(0) * grid.GetLength(1));

            // Process the first location
            var firstElement = grid[column, row];
            resultQueue.Enqueue(firstElement);
            regionLocations.Add(firstElement);
            regionGrid[column, row] = firstElement;

            // Check for edge-of-region location or edge of grid cells
            if (grid.IsEdgeElement(column, row, element => !predicate(element)))
                edgeLocations.Add(firstElement);

            // Track the region boundary
            var top = firstElement.Row;
            var bottom = firstElement.Row;
            var left = firstElement.Column;
            var right = firstElement.Column;

            while (resultQueue.Count > 0)
            {
                var regionLocation = resultQueue.Dequeue();

                // Search cardinally adjacent cells (N,S,E,W)
                foreach (var location in grid.GetCardinalAdjacentElements(regionLocation.Column, regionLocation.Row))
                {
                    // CONNECTED AS LONG AS THERE IS A PASSING LOCATION CARDINALLY ADJACENT
                    if (grid[location.Column, location.Row] != null &&
                        regionGrid[location.Column, location.Row] == null &&
                        predicate(location))
                    {
                        // Add cell to region immediately to prevent extra cells on queue
                        regionGrid[location.Column, location.Row] = location;

                        // Add cell also to region data
                        regionLocations.Add(location);

                        // Determine whether cell is an edge cell
                        if (grid.IsEdgeElement(location.Column, location.Row, element => !predicate(element)))
                            edgeLocations.Add(location);

                        // Expand the region boundary
                        if (location.Column < left)
                            left = location.Column;

                        if (location.Column > right)
                            right = location.Column;

                        if (location.Row < top)
                            top = location.Row;

                        if (location.Row > bottom)
                            bottom = location.Row;

                        // Push cell onto the queue to be iterated
                        resultQueue.Enqueue(location);
                    }
                }
            }

            // Calculate boundaries for the region
            var parentBoundary = new RegionBoundary(0, 0, grid.GetLength(0), grid.GetLength(1));
            var boundary = new RegionBoundary(left, top, right - left + 1, bottom - top + 1);

            // Assign region data to new region info
            //
            return new RegionConstructorInfo<T>(regionLocations.ToArray(), edgeLocations.ToArray(), boundary, parentBoundary);
        }

        /// <summary>
        /// Scans the supplied grid for (overlapping) rectangular regions whose elements pass the given predicate. These are packaged
        /// into a collection.
        /// </summary>
        public static IEnumerable<Region<T>> ScanRectanglarRegions<T>(this T[,] grid, int rectangleWidth, int rectangleHeight, Func<T, bool> predicate) where T : class, IGridLocator
        {
            // TODO: OPTIMIZE BY SKIPPING SECTIONS THAT AREN'T GOING TO PASS THE PREDICATE (OR) BY SCANNING RECTANGLE-BY-RECTANGLE
            //       INSTEAD OF CELL-BY-CELL, RECTANGLE-BY-RECTANGLE
            var regions = new List<Region<T>>();

            // Run a single scane of the grid to locate regions
            for (int i = 0; i < grid.GetLength(0) - rectangleWidth; i++)
            {
                for (int j = 0; j < grid.GetLength(1) - rectangleHeight; j++)
                {
                    var locations = new List<T>();
                    var edgeLocations = new List<T>();
                    var top = int.MaxValue;
                    var bottom = int.MinValue;
                    var left = int.MaxValue;
                    var right = int.MinValue;
                    var predicateFailed = false;

                    // Run sub-scan over the current rectangle block
                    for (int column = i; column < i + rectangleWidth && !predicateFailed; column++)
                    {
                        for (int row = j; row < j + rectangleHeight && !predicateFailed; row++)
                        {
                            // Passes predicate test
                            if (grid[column, row] != null &&
                                predicate(grid[column, row]))
                            {
                                locations.Add(grid[column, row]);

                                // Edge locations
                                if (column == i ||
                                    column == (i + rectangleWidth - 1) ||
                                    row == j ||
                                    row == (j + rectangleHeight - 1))
                                    edgeLocations.Add(grid[column, row]);

                                // Expand the region boundary
                                if (column < left)
                                    left = column;

                                if (column > right)
                                    right = column;

                                if (row < top)
                                    top = row;

                                if (row > bottom)
                                    bottom = row;
                            }

                            else
                                predicateFailed = true;
                        }
                    }

                    if (!predicateFailed)
                        regions.Add(new Region<T>(locations.ToArray(),
                                                  edgeLocations.ToArray(),
                                                  new RegionBoundary(left, top, right - left + 1, bottom - top + 1),
                                                  new RegionBoundary(0, 0, grid.GetLength(0), grid.GetLength(1))));
                }
            }

            return regions;
        }
    }
}
