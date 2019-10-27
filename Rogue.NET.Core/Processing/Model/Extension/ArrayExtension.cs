using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Region;
using System;

namespace Rogue.NET.Core.Processing.Model.Extension
{
    public static class ArrayExtension
    {
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
        /// Returns true if any adjacent cells are default(T)
        /// </summary>
        public static bool IsEdgeCell<T>(this T[,] grid, int column, int row) where T : class
        {
            var north = grid.Get(column, row - 1);
            var south = grid.Get(column, row + 1);
            var east = grid.Get(column + 1, row);
            var west = grid.Get(column - 1, row);
            var northEast = grid.Get(column + 1, row - 1);
            var northWest = grid.Get(column - 1, row - 1);
            var southEast = grid.Get(column + 1, row + 1);
            var southWest = grid.Get(column - 1, row + 1);

            return north == null ||
                   south == null ||
                   east == null ||
                   west == null ||
                   northEast == null ||
                   northWest == null ||
                   southEast == null ||
                   southWest == null;
        }

        /// <summary>
        /// Returns 1st of 2 off diagonal elements in the specified non-cardinal direction (Example: NE -> N element)
        /// </summary>
        /// <param name="direction">NE, NW, SE, SW</param>
        public static T GetOffDiagonalCell1<T>(this T[,] grid, int column, int row, Compass direction, out Compass cardinalDirection1)
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
        /// Returns 2nd of 2 off diagonal cells in the specified non-cardinal direction (Example: NE -> E cell)
        /// </summary>
        /// <param name="direction">NE, NW, SE, SW</param>
        public static T GetOffDiagonalCell2<T>(this T[,] grid, int column, int row, Compass direction, out Compass cardinalDirection2)
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
        /// Calculates whether adjacent element is connected by accessible path - This will check for non-default elements
        /// at the off-diagonal locations
        /// </summary>
        public static bool IsAdjacentElementConnected<T>(this T[,] grid, int column1, int row1, int column2, int row2) where T : class
        {
            var direction = GridUtility.GetDirectionBetweenAdjacentPoints(column1, row1, column2, row2);

            Compass cardinalDirection = Compass.Null;

            switch (direction)
            {
                case Compass.N:
                case Compass.S:
                case Compass.E:
                case Compass.W:
                    return true;

                case Compass.NW:
                case Compass.NE:
                case Compass.SE:
                case Compass.SW:
                    return grid.GetOffDiagonalCell1(column1, row1, direction, out cardinalDirection) != null ||
                           grid.GetOffDiagonalCell2(column1, row1, direction, out cardinalDirection) != null;
                default:
                    return false;
            }
        }
    }
}
