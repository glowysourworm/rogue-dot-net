using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using Rogue.NET.Core.Processing.Model.Extension;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Construction;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Algorithm
{
    public static class GridAlgorithm
    {
        /// <summary>
        /// Removes regions of elements that PASS the provided invalidPredicate by setting them to null. Also, returns the
        /// VALID regions.
        /// </summary>
        public static IEnumerable<RegionInfo<T>> RemoveInvalidRegions<T>(this T[,] grid,
                                                                         string regionBaseName,
                                                                         Func<T, bool> regionIdentifier,
                                                                         Func<RegionInfo<T>, bool> invalidPredicate) where T : class, IGridLocator
        {
            // Identify regions in the grid
            var regions = grid.ConstructRegions(regionBaseName, regionIdentifier, location => location);

            // Select invalid regions
            var invalidRegions = regions.Where(region => invalidPredicate(region));

            // Select valid regions
            var validRegions = regions.Except(invalidRegions).Actualize();

            // Remove elements for this region
            foreach (var region in invalidRegions)
            {
                // Nullify the invalid region elements
                foreach (var location in region.Locations)
                    grid[location.Column, location.Row] = null;
            }

            return validRegions;
        }

        /// <summary>
        /// Constructs 4-way contiguous regions of the grid where the elements pass the provided predicate. 
        /// </summary>
        public static IEnumerable<RegionInfo<TResult>> ConstructRegions<T, TResult>(this T[,] grid, 
                                                                                    string baseName, 
                                                                                    Func<T, bool> predicate, 
                                                                                    Func<T, TResult> selector) where T : class, IGridLocator
                                                                                                               where TResult : class, IGridLocator
                                                                                                                            
        {
            var regionConstructors = IdentifyRegions(grid, baseName, predicate, selector);

            return regionConstructors.Select(info => new RegionInfo<TResult>(info.Id, 
                                                                             info.Locations, 
                                                                             info.EdgeLocations, 
                                                                             info.Boundary, 
                                                                             info.ParentBoundary)).Actualize();
        }

        /// <summary>
        /// Identifies regions using Breadth First Search (Flood Fill) algorithm with the specified (positive) predicate.
        /// </summary>
        public static IEnumerable<RegionConstructorInfo<TResult>> IdentifyRegions<T, TResult>(T[,] grid, 
                                                                                              string baseName, 
                                                                                              Func<T, bool> predicate,
                                                                                              Func<T, TResult> selector) where T : class, IGridLocator
                                                                                                                         where TResult : class, IGridLocator
        {
            // Procedure
            //
            // 0) Iterate cells
            // 1) First cell that's non-empty AND not part of an existing region
            //    use flood fill to find connected cells

            // Collect region data to pass to level grid constructor
            var regions = new List<RegionConstructorInfo<TResult>>();
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
                            var regionName = baseName + " " + (regions.Count + 1).ToString();
                            var region = FloodFill(grid, i, j, regionName, predicate, selector, out regionGrid);

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
        public static RegionConstructorInfo<TResult> FloodFill<T, TResult>(T[,] grid, 
                                                                           int column, int row, string id, 
                                                                           Func<T, bool> predicate, 
                                                                           Func<T, TResult> selector, 
                                                                           out T[,] regionGrid) where T : class, IGridLocator
                                                                                                where TResult : class, IGridLocator
        {
            // Region Data
            regionGrid = new T[grid.GetLength(0), grid.GetLength(1)];
            var regionLocations = new List<TResult>();
            var edgeLocations = new List<TResult>();

            // Use queue to know what locations have been verified. Starting with test location - continue 
            // until all connected cells have been added to the resulting region using the predicate.
            var resultQueue = new Queue<TResult>(grid.GetLength(0) * grid.GetLength(1));

            // Process the first location
            var firstElement = grid[column, row];
            var firstResult = selector(firstElement);
            resultQueue.Enqueue(firstResult);
            regionLocations.Add(firstResult);
            regionGrid[column, row] = firstElement;

            // Check for edge-of-region location or edge of grid cells
            if (grid.IsEdgeElement(column, row, element => !predicate(element)))
                edgeLocations.Add(firstResult);

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
                        var resultLocation = selector(location);

                        // Add cell to region immediately to prevent extra cells on queue
                        regionGrid[location.Column, location.Row] = location;

                        // Add cell also to region data
                        regionLocations.Add(resultLocation);

                        // Determine whether cell is an edge cell
                        if (grid.IsEdgeElement(location.Column, location.Row, element => !predicate(element)))
                            edgeLocations.Add(resultLocation);

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
                        resultQueue.Enqueue(resultLocation);
                    }
                }
            }

            // Calculate boundaries for the region
            var parentBoundary = new RegionBoundary(0, 0, grid.GetLength(0), grid.GetLength(1));
            var boundary = new RegionBoundary(left, top, right - left + 1, bottom - top + 1);

            // Assign region data to new region info
            //
            return new RegionConstructorInfo<TResult>(id, regionLocations.ToArray(), edgeLocations.ToArray(), boundary, parentBoundary);
        }

        /// <summary>
        /// Scans the supplied grid for (overlapping) rectangular regions whose elements pass the given predicate. These are packaged
        /// into a collection.
        /// </summary>
        public static IEnumerable<Region<TResult>> ScanRectanglarRegions<T, TResult>(this T[,] grid, 
                                                                                     int rectangleWidth, 
                                                                                     int rectangleHeight, 
                                                                                     Func<T, bool> predicate,
                                                                                     Func<T, TResult> selector) where TResult : class, IGridLocator
        {
            // TODO: OPTIMIZE BY SKIPPING SECTIONS THAT AREN'T GOING TO PASS THE PREDICATE (OR) BY SCANNING RECTANGLE-BY-RECTANGLE
            //       INSTEAD OF CELL-BY-CELL, RECTANGLE-BY-RECTANGLE
            var regions = new List<Region<TResult>>();

            // Run a single scane of the grid to locate regions
            for (int i = 0; i < grid.GetLength(0) - rectangleWidth; i++)
            {
                for (int j = 0; j < grid.GetLength(1) - rectangleHeight; j++)
                {
                    var locations = new List<TResult>();
                    var edgeLocations = new List<TResult>();
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
                                locations.Add(selector(grid[column, row]));

                                // Edge locations
                                if (column == i ||
                                    column == (i + rectangleWidth - 1) ||
                                    row == j ||
                                    row == (j + rectangleHeight - 1))
                                    edgeLocations.Add(selector(grid[column, row]));

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
                        regions.Add(new Region<TResult>(System.Guid.NewGuid().ToString(),
                                                        locations.ToArray(),
                                                        edgeLocations.ToArray(),
                                                        new RegionBoundary(left, top, right - left + 1, bottom - top + 1),
                                                        new RegionBoundary(0, 0, grid.GetLength(0), grid.GetLength(1))));
                }
            }

            return regions;
        }
    }
}
