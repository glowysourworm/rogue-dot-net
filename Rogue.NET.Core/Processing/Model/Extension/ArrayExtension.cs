using Rogue.NET.Common.Utility;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using Rogue.NET.Core.Processing.Model.Content.Calculator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Rogue.NET.Core.Processing.Model.Extension
{
    public static class ArrayExtension
    {
        /// <summary>
        /// Callback used for grid processing - provides the column, and row
        /// </summary>
        /// <typeparam name="T">Type parameter for grid (2D array) to iterate</typeparam>
        /// <param name="column">The column of iteration (1st dimension index)</param>
        /// <param name="row">The row of iteration (2nd dimension index)</param>
        public delegate void GridCallback<T>(int column, int row);

        /// <summary>
        /// Predicate used for grid processing - provides column, row
        /// </summary>
        /// <typeparam name="T">Type parameter for grid (2D array) to iterate</typeparam>
        /// <param name="value">The value if the cell at this location</param>
        /// <returns>True if the predicate is to pass for this location</returns>
        public delegate bool GridPredicate<T>(T value);

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
        /// Iterates - starting at the provided column and row - outwards to the specified radius - checking the grid
        /// boundary as it goes.
        /// </summary>
        public static void IterateAround<T>(this T[,] grid, int column, int row, int maxRadius, GridPredicate<T> continuationPredicate)
        {
            for (int radius = 0; radius <= maxRadius; radius++)
            {
                // CENTER
                if (radius == 0)
                {
                    if (!continuationPredicate(grid[column, row]))
                        return;

                    continue;
                }

                var left = System.Math.Max(0, column - radius);
                var right = System.Math.Min(grid.GetLength(0), column + radius);

                // Iterate in a box around the center - using the radius
                for (int columnIndex = left; columnIndex <= right; columnIndex++)
                {
                    var top = System.Math.Max(0, row - radius);
                    var bottom = System.Math.Min(grid.GetLength(1), row + radius);

                    // LEFT EDGE (or) RIGHT EDGE
                    if (columnIndex == left || columnIndex == right)
                    {
                        for (int rowIndex = top; rowIndex <= bottom; rowIndex++)
                        {
                            if (!continuationPredicate(grid[columnIndex, rowIndex]))
                                return;
                        }
                    }
                    // TOP cell (and) BOTTOM cell
                    else
                    {
                        if (!continuationPredicate(grid[columnIndex, top]))
                            return;

                        if (!continuationPredicate(grid[columnIndex, bottom]))
                            return;
                    }
                }
            }
        }

        /// <summary>
        /// Creates default IGridLocator (GridLocation) for a given column and row
        /// </summary>
        public static IGridLocator CreateLocator<T>(this T[,] grid, int column, int row)
        {
            return new GridLocation(column, row);
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
        /// Returns 4-way adjacent cells - leaving nulls; but checking boundaries to prevent exceptions. 
        /// NOTE*** This will return null references for possible element positions ONLY.
        /// </summary>
        public static T[] GetCardinalAdjacentElementsUnsafe<T>(this T[,] grid, int column, int row) where T : class
        {
            var n = grid.Get(column, row - 1);
            var s = grid.Get(column, row + 1);
            var e = grid.Get(column + 1, row);
            var w = grid.Get(column - 1, row);

            return new T[] { n, s, e, w };
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
        /// Returns locators for the adjacent elements whose values are non-null; and in bounds
        /// </summary>
        public static IGridLocator[] GetAdjacentElementLocators<T>(this T[,] grid, int column, int row) where T : class
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

            var result = new IGridLocator[count];
            var index = 0;

            if (n != null) result[index++] = grid.CreateLocator(column, row - 1);
            if (s != null) result[index++] = grid.CreateLocator(column, row + 1);
            if (e != null) result[index++] = grid.CreateLocator(column + 1, row);
            if (w != null) result[index++] = grid.CreateLocator(column - 1, row);
            if (ne != null) result[index++] = grid.CreateLocator(column + 1, row - 1);
            if (nw != null) result[index++] = grid.CreateLocator(column - 1, row - 1);
            if (se != null) result[index++] = grid.CreateLocator(column + 1, row + 1);
            if (sw != null) result[index++] = grid.CreateLocator(column - 1, row + 1);

            return result;
        }

        /// <summary>
        /// Returns locators for the adjacent elements whose values are non-null; and in bounds
        /// </summary>
        public static IGridLocator[] GetCardinalAdjacentElementLocators<T>(this T[,] grid, int column, int row) where T : class
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

            var result = new IGridLocator[count];
            var index = 0;

            if (n != null) result[index++] = grid.CreateLocator(column, row - 1);
            if (s != null) result[index++] = grid.CreateLocator(column, row + 1);
            if (e != null) result[index++] = grid.CreateLocator(column + 1, row);
            if (w != null) result[index++] = grid.CreateLocator(column - 1, row);

            return result;
        }

        /// <summary>
        /// Returns locators for 8-way adjacent values that are in bounds
        /// </summary>
        public static IGridLocator[] GetAdjacentValueLocators<T>(this T[,] grid, int column, int row) where T : struct
        {
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

            var result = new IGridLocator[count];
            var index = 0;

            if (north) result[index++] = grid.CreateLocator(column, row - 1);
            if (south) result[index++] = grid.CreateLocator(column, row + 1);
            if (east) result[index++] = grid.CreateLocator(column + 1, row);
            if (west) result[index++] = grid.CreateLocator(column - 1, row);
            if (north && east) result[index++] = grid.CreateLocator(column + 1, row - 1);
            if (north && west) result[index++] = grid.CreateLocator(column - 1, row - 1);
            if (south && east) result[index++] = grid.CreateLocator(column + 1, row + 1);
            if (south && west) result[index++] = grid.CreateLocator(column - 1, row + 1);

            return result;
        }

        /// <summary>
        /// Returns locators for 4-way adjacent values that are in bounds
        /// </summary>
        public static IGridLocator[] GetCardinalAdjacentValueLocators<T>(this T[,] grid, int column, int row) where T : struct
        {
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

            var result = new IGridLocator[count];
            var index = 0;

            if (north) result[index++] = grid.CreateLocator(column, row - 1);
            if (south) result[index++] = grid.CreateLocator(column, row + 1);
            if (east) result[index++] = grid.CreateLocator(column + 1, row);
            if (west) result[index++] = grid.CreateLocator(column - 1, row);

            return result;
        }

        /// <summary>
        /// Retusn cardinally adjacent elements that pass the given predicate - stripping out nulls
        /// </summary>
        public static IGridLocator[] GetCardinalAdjacentLocatorsFor<T>(this T[,] grid, int column, int row, GridPredicate<T> predicate) where T : class
        {
            var n = grid.Get(column, row - 1);
            var s = grid.Get(column, row + 1);
            var e = grid.Get(column + 1, row);
            var w = grid.Get(column - 1, row);

            // Need this to be optimized for speed
            var count = 0;
            if (n != null && predicate(n)) count++;
            if (s != null && predicate(s)) count++;
            if (e != null && predicate(e)) count++;
            if (w != null && predicate(w)) count++;

            var result = new IGridLocator[count];
            var index = 0;

            if (n != null && predicate(n)) result[index++] = grid.CreateLocator(column, row - 1);
            if (s != null && predicate(s)) result[index++] = grid.CreateLocator(column, row + 1);
            if (e != null && predicate(e)) result[index++] = grid.CreateLocator(column + 1, row);
            if (w != null && predicate(w)) result[index++] = grid.CreateLocator(column - 1, row);

            return result;
        }

        /// <summary>
        /// Retusn cardinally adjacent elements that pass the given predicate - leaving in null elements
        /// </summary>
        public static IGridLocator[] GetCardinalAdjacentLocatorsForUnsafe<T>(this T[,] grid, int column, int row, GridPredicate<T> predicate) where T : class
        {
            var n = grid.Get(column, row - 1);
            var s = grid.Get(column, row + 1);
            var e = grid.Get(column + 1, row);
            var w = grid.Get(column - 1, row);

            // Need this to be optimized for speed
            var count = 0;
            if (predicate(n)) count++;
            if (predicate(s)) count++;
            if (predicate(e)) count++;
            if (predicate(w)) count++;

            var result = new IGridLocator[count];
            var index = 0;

            if (predicate(n)) result[index++] = grid.CreateLocator(column, row - 1);
            if (predicate(s)) result[index++] = grid.CreateLocator(column, row + 1);
            if (predicate(e)) result[index++] = grid.CreateLocator(column + 1, row);
            if (predicate(w)) result[index++] = grid.CreateLocator(column - 1, row);

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

        /// <summary>
        /// Copies over references from the primary grid into a new grid of the same dimensions using the provided copier
        /// </summary>
        public static TResult[,] GridCopy<T, TResult>(this T[,] grid, Func<T, TResult> copier)
        {
            var copy = new TResult[grid.GetLength(0), grid.GetLength(1)];

            grid.Iterate((column, row) => copy[column, row] = copier(grid[column, row]));

            return copy;
        }

        /// <summary>
        /// Outputs a CSV file to the Dijkstra's map directory (overwrites existing files)
        /// </summary>
        /// <typeparam name="T">Uses ToString() representation</typeparam>
        /// <param name="grid">Grid to output</param>
        /// <param name="name">Name of file (used as prefix)</param>
        public static void OutputDebug<T>(this T[,] grid, string name)
        {
            var fileName = Path.Combine(ResourceConstants.DebugOutputDirectory, name + "_output.csv");

            var builder = new StringBuilder();

            // Output by row CSV
            for (int row = 0; row < grid.GetLength(1); row++)
            {
                for (int column = 0; column < grid.GetLength(0); column++)
                {
                    builder.Append(grid[column, row].ToString() + ", ");
                }

                // Remove trailing comma
                builder.Remove(builder.Length - 1, 1);

                // Append return carriage
                builder.Append("\r\n");
            }

            File.WriteAllText(fileName, builder.ToString());
        }
    }
}
