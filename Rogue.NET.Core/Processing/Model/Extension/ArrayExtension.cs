using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using Rogue.NET.Core.Processing.Model.Content.Calculator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Extension
{
    public static class ArrayExtension
    {
        /// <summary>
        /// Callback used for grid processing - provides the column, row, and value at that index
        /// </summary>
        /// <typeparam name="T">Type parameter for grid (2D array) to iterate</typeparam>
        /// <param name="column">The column of iteration (1st dimension index)</param>
        /// <param name="row">The row of iteration (2nd dimension index)</param>
        public delegate void GridCallback<T>(int column, int row);

        /// <summary>
        /// Iterates the grid entirely with the provided callback
        /// </summary>
        public static void Iterate<T>(this T[,] grid, GridCallback<T> callback)
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                    callback(i, j);
            }
        }

        /// <summary>
        /// Performs a boundary-safe indexing operation to get the cell from the grid. Returns default(T) if out-of-bounds
        /// or no cell was at that location.
        /// </summary>
        public static T Get<T>(this T[,] grid, int column, int row)
        {
            if (column < 0 ||
                column >= grid.GetLength(0) ||
                row < 0 ||
                row >= grid.GetLength(1))
                return default(T);

            return grid[column, row];
        }

        /// <summary>
        /// Special "Get" method to check the specified boundary for the sub-grid within the specified grid. This is used for region grids
        /// that have the same dimensions as their parent. Returns null if outside the region OR grid boundaries.
        /// </summary>
        public static T GetFrom<T>(this T[,] grid, int regionColumn, int regionRow, int regionWidth, int regionHeight, int column, int row)
        {
            // Check region boundaries
            if (column < regionColumn || column >= ((regionColumn + regionWidth) - 1))
                return default(T);

            if (row < regionRow || row >= ((regionRow + regionHeight) - 1))
                return default(T);

            // Check parent grid boundaries
            return Get(grid, column, row);
        }

        /// <summary>
        /// Returns 4-way adjacent cells - nulls excluded
        /// </summary>
        public static T[] GetCardinalAdjacentElements<T>(this T[,] grid, int column, int row) where T : class
        {
            var n = grid.Get(column, row - 1);
            var s = grid.Get(column, row + 1);
            var e = grid.Get(column + 1, row);
            var w = grid.Get(column - 1, row);

            // Need this to be optimized for speed
            var count = 0;
            if (n != null) count++;
            if (s != null) count++;
            if (e != null) count++;
            if (w != null) count++;

            var result = new T[count];
            var index = 0;

            if (n != null) result[index++] = n;
            if (s != null) result[index++] = s;
            if (e != null) result[index++] = e;
            if (w != null) result[index++] = w;

            return result;
        }

        /// <summary>
        /// Returns 8-way adjacent cells - stripping out nulls
        /// </summary>
        public static T[] GetAdjacentElements<T>(this T[,] grid, int column, int row) where T : class
        {
            var n = grid.Get(column, row - 1);
            var s = grid.Get(column, row + 1);
            var e = grid.Get(column + 1, row);
            var w = grid.Get(column - 1, row);
            var ne = grid.Get(column + 1, row - 1);
            var nw = grid.Get(column - 1, row - 1);
            var se = grid.Get(column + 1, row + 1);
            var sw = grid.Get(column - 1, row + 1);

            // Need this to be optimized for speed
            var count = 0;
            if (n != null) count++;
            if (s != null) count++;
            if (e != null) count++;
            if (w != null) count++;
            if (ne != null) count++;
            if (nw != null) count++;
            if (se != null) count++;
            if (sw != null) count++;

            var result = new T[count];
            var index = 0;

            if (n != null) result[index++] = n;
            if (s != null) result[index++] = s;
            if (e != null) result[index++] = e;
            if (w != null) result[index++] = w;
            if (ne != null) result[index++] = ne;
            if (nw != null) result[index++] = nw;
            if (se != null) result[index++] = se;
            if (sw != null) result[index++] = sw;

            return result;
        }

        /// <summary>
        /// Returns adjacent elements that are reachable by a cardinal one. Example:  NW is connected if it is
        /// non-null; and N is also non-null.
        /// </summary>
        public static T[] GetAdjacentElementsWithCardinalConnection<T>(this T[,] grid, int column, int row) where T : class
        {
            var n = grid.Get(column, row - 1);
            var s = grid.Get(column, row + 1);
            var e = grid.Get(column + 1, row);
            var w = grid.Get(column - 1, row);
            var ne = grid.Get(column + 1, row - 1);
            var nw = grid.Get(column - 1, row - 1);
            var se = grid.Get(column + 1, row + 1);
            var sw = grid.Get(column - 1, row + 1);

            var count = 0;

            if (n != null) count++;
            if (s != null) count++;
            if (e != null) count++;
            if (w != null) count++;
            if (ne != null && (n != null || e != null)) count++;
            if (nw != null && (n != null || w != null)) count++;
            if (se != null && (s != null || e != null)) count++;
            if (sw != null && (s != null || w != null)) count++;

            var result = new T[count];
            var index = 0;

            if (n != null) result[index++] = n;
            if (s != null) result[index++] = s;
            if (e != null) result[index++] = e;
            if (w != null) result[index++] = w;
            if (ne != null && (n != null || e != null)) result[index++] = ne;
            if (nw != null && (n != null || w != null)) result[index++] = nw;
            if (se != null && (s != null || e != null)) result[index++] = se;
            if (sw != null && (s != null || w != null)) result[index++] = sw;

            return result;
        }

        /// <summary>
        /// Returns all elements within the specified distance from the given location (using the Roguian metric). Method does
        /// not strip out null elements; but checks boundaries to prevent exceptions.
        /// </summary>
        public static T[] GetElementsNearUnsafe<T>(this T[,] grid, int column, int row, int distance) where T : class
        {
            // Calculate the array result length
            var sumSequence = 0;
            for (int i = 1; i <= distance; i++)
                sumSequence += i;

            // Circumference of a "circle" of "radius = distance" is 8 * distance. So, Area = Sum of Circumferences from 1 -> distance
            //
            var result = new T[8 * sumSequence];
            var index = 0;

            for (int k = 1; k <= distance; k++)
            {
                // Iterate top and bottom
                for (int i = column - k; i <= column + k; i++)
                {
                    // Top
                    result[index++] = grid.Get(i, row - k);

                    // Bottom
                    result[index++] = grid.Get(i, row + k);
                }

                // Iterate left and right (minus the corners)
                for (int j = row - k + 1; j <= row + k - 1; j++)
                {
                    // Left 
                    result[index++] = grid.Get(column - k, j);

                    // Right
                    result[index++] = grid.Get(column + k, j);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns 8-way adjacent cells - leaving nulls; but checking boundaries to prevent exceptions. 
        /// NOTE*** This will return null references for possible element positions ONLY.
        /// </summary>
        public static T[] GetAdjacentElementsUnsafe<T>(this T[,] grid, int column, int row) where T : class
        {
            var n = grid.Get(column, row - 1);
            var s = grid.Get(column, row + 1);
            var e = grid.Get(column + 1, row);
            var w = grid.Get(column - 1, row);
            var ne = grid.Get(column + 1, row - 1);
            var nw = grid.Get(column - 1, row - 1);
            var se = grid.Get(column + 1, row + 1);
            var sw = grid.Get(column - 1, row + 1);

            // NW Corner
            if (row - 1 < 0 &&
                column - 1 < 0)
                return new T[] { s, e, se };

            // NE Corner
            if (row - 1 < 0 &&
                column + 1 >= grid.GetLength(0))
                return new T[] { s, w, sw };

            // SE Corner
            if (row + 1 >= grid.GetLength(1) &&
                column + 1 >= grid.GetLength(0))
                return new T[] { n, w, nw };

            // SW Corner
            if (row + 1 >= grid.GetLength(1) &&
                column - 1 < 0)
                return new T[] { n, e, ne };

            // N Boundary
            if (row - 1 < 0)
                return new T[] { s, e, w, se, sw };

            // S Boundary
            if (row + 1 >= grid.GetLength(1))
                return new T[] { n, e, w, ne, nw };

            // E Boundary
            if (column + 1 >= grid.GetLength(0))
                return new T[] { n, s, w, nw, sw };

            // W Boundary
            if (column - 1 < 0)
                return new T[] { n, s, e, ne, se };

            return new T[] { n, s, e, w, ne, nw, se, sw };
        }     

        /// <summary>
        /// Returns 1st of 2 off diagonal elements in the specified non-cardinal direction (Example: NE -> N element)
        /// </summary>
        /// <param name="direction">NE, NW, SE, SW</param>
        public static T GetOffDiagonalElement1<T>(this T[,] grid, int column, int row, Compass direction, out Compass cardinalDirection1)
        {
            switch (direction)
            {
                case Compass.NE:
                case Compass.NW:
                    cardinalDirection1 = Compass.N;
                    return grid.Get(column, row - 1);
                case Compass.SE:
                case Compass.SW:
                    cardinalDirection1 = Compass.S;
                    return grid.Get(column, row + 1);
                default:
                    throw new Exception("Off-Diagonal directions don't include " + direction.ToString());
            }
        }

        /// <summary>
        /// Returns 2nd of 2 off diagonal elements in the specified non-cardinal direction (Example: NE -> E element)
        /// </summary>
        /// <param name="direction">NE, NW, SE, SW</param>
        public static T GetOffDiagonalElement2<T>(this T[,] grid, int column, int row, Compass direction, out Compass cardinalDirection2)
        {
            switch (direction)
            {
                case Compass.NE:
                case Compass.SE:
                    cardinalDirection2 = Compass.E;
                    return grid.Get(column + 1, row);
                case Compass.SW:
                case Compass.NW:
                    cardinalDirection2 = Compass.W;
                    return grid.Get(column - 1, row);
                default:
                    throw new Exception("Off-Diagonal directions don't include " + direction.ToString());
            }
        }

        /// <summary>
        /// Returns 4-way adjacent cell values
        /// </summary>
        public static T[] GetCardinalAdjacentValues<T>(this T[,] grid, int column, int row) where T : struct
        {
            var n = grid.Get(column, row - 1);
            var s = grid.Get(column, row + 1);
            var e = grid.Get(column + 1, row);
            var w = grid.Get(column - 1, row);

            // Need this to be optimized for speed
            var count = 0;
            var north = (row - 1 >= 0);
            var south = (row + 1 < grid.GetLength(1));
            var east = (column + 1 < grid.GetLength(0));
            var west = (column - 1 >= 0);

            if (north) count++;
            if (south) count++;
            if (east) count++;
            if (west) count++;

            var result = new T[count];
            var index = 0;

            if (north) result[index++] = n;
            if (south) result[index++] = s;
            if (east) result[index++] = e;
            if (west) result[index++] = w;

            return result;
        }

        /// <summary>
        /// Returns 8-way adjacent cell values
        /// </summary>
        public static T[] GetAdjacentValues<T>(this T[,] grid, int column, int row) where T : struct
        {
            var n = grid.Get(column, row - 1);
            var s = grid.Get(column, row + 1);
            var e = grid.Get(column + 1, row);
            var w = grid.Get(column - 1, row);
            var ne = grid.Get(column + 1, row - 1);
            var nw = grid.Get(column - 1, row - 1);
            var se = grid.Get(column + 1, row + 1);
            var sw = grid.Get(column - 1, row + 1);

            var north = (row - 1 >= 0);
            var south = (row + 1 < grid.GetLength(1));
            var east = (column + 1 < grid.GetLength(0));
            var west = (column - 1 >= 0);

            // Need this to be optimized for speed
            var count = 0;
            if (north) count++;
            if (south) count++;
            if (east) count++;
            if (west) count++;
            if (north && east) count++;
            if (north && west) count++;
            if (south && east) count++;
            if (south && west) count++;

            var result = new T[count];
            var index = 0;

            if (north) result[index++] = n;
            if (south) result[index++] = s;
            if (east) result[index++] = e;
            if (west) result[index++] = w;
            if (north && east) result[index++] = ne;
            if (north && west) result[index++] = nw;
            if (south && east) result[index++] = se;
            if (south && west) result[index++] = sw;

            return result;
        }

        /// <summary>
        /// Returns adjacent element in the direction specified
        /// </summary>
        /// <typeparam name="T">Type of grid element</typeparam>
        /// <param name="grid">The input 2D array</param>
        /// <param name="column">The column of the location</param>
        /// <param name="row">The row of the location</param>
        /// <param name="direction">The direction to look adjacent to specified location</param>
        public static T GetElementInDirection<T>(this T[,] grid, int column, int row, Compass direction)
        {
            switch (direction)
            {
                case Compass.N:
                    return grid.Get(column, row - 1);
                case Compass.S:
                    return grid.Get(column, row + 1);
                case Compass.E:
                    return grid.Get(column + 1, row);
                case Compass.W:
                    return grid.Get(column - 1, row);
                case Compass.NW:
                    return grid.Get(column - 1, row - 1);
                case Compass.NE:
                    return grid.Get(column + 1, row - 1);
                case Compass.SE:
                    return grid.Get(column + 1, row + 1);
                case Compass.SW:
                    return grid.Get(column - 1, row + 1);
                case Compass.Null:
                default:
                    throw new Exception("Unhandled direction ArrayExtension.GetElementInDirection");
            }
        }

        /// <summary>
        /// Returns true if any adjacent elements are positive with respect to the provided predicate OR are 
        /// out of bounds OR are null.
        /// </summary>
        public static bool IsEdgeElement<T>(this T[,] grid, int column, int row, Func<T, bool> predicate) where T : class
        {
            var north = grid.Get(column, row - 1);
            var south = grid.Get(column, row + 1);
            var east = grid.Get(column + 1, row);
            var west = grid.Get(column - 1, row);
            var northEast = grid.Get(column + 1, row - 1);
            var northWest = grid.Get(column - 1, row - 1);
            var southEast = grid.Get(column + 1, row + 1);
            var southWest = grid.Get(column - 1, row + 1);

            return north == null || predicate(north) ||
                   south == null || predicate(south) ||
                   east == null || predicate(east) ||
                   west == null || predicate(west) ||
                   northEast == null || predicate(northEast) ||
                   northWest == null || predicate(northWest) ||
                   southEast == null || predicate(southEast) ||
                   southWest == null || predicate(southWest);
        }

        /// <summary>
        /// Identifies regions using Breadth First Search (Flood Fill) algorithm with the specified (positive) predicate.
        /// </summary>
        public static IEnumerable<Region<T>> IdentifyRegions<T>(this T[,] grid, Func<T, bool> predicate) where T : class, IGridLocator
        {
            // Procedure
            //
            // 0) Iterate cells
            // 1) First cell that's non-empty AND not part of an existing region
            //    use flood fill to find connected cells

            // Collect region data to pass to level grid constructor
            var regionGrids = new List<T[,]>();
            var regions = new List<Region<T>>();

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
                            // Keep track of region grids
                            T[,] regionGrid;

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
        public static Region<T> FloodFill<T>(this T[,] grid, int column, int row, Func<T, bool> predicate, out T[,] regionGrid) where T : class, IGridLocator
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
            if (IsEdgeElement(grid, column, row, element => !predicate(element)))
                edgeLocations.Add(firstElement);

            // Track the region boundary
            var top = int.MaxValue;
            var bottom = int.MinValue;
            var left = int.MaxValue;
            var right = int.MinValue;

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
                        if (IsEdgeElement(grid, location.Column, location.Row, element => !predicate(element)))
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

            // Assign region data to new region
            return new Region<T>(regionLocations.ToArray(), edgeLocations.ToArray(), boundary, parentBoundary);
        }

        /// <summary>
        /// Returns elements of the 2D array that match the given predicate
        /// </summary>
        public static IEnumerable<T> Where<T>(this T[,] grid, Func<T, bool> predicate)
        {
            var list = new List<T>();

            grid.Iterate((column, row) =>
            {
                if (predicate(grid[column, row]))
                    list.Add(grid[column, row]);
            });

            return list;
        }
    }
}
