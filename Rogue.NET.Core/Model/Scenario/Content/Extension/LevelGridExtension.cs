using Rogue.NET.Core.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Rogue.NET.Core.Model.Scenario.Content.Extension
{
    public static class LevelGridExtension
    {
        /// <summary>
        /// (NOT EFFICIENT) Returns adjacent cells - nulls excluded
        /// </summary>
        public static IEnumerable<Cell> GetCardinalAdjacentCells(this Cell[,] grid, Cell cell)
        {
            var row = cell.Location.Row;
            var column = cell.Location.Column;

            var north = grid.Get(column, row - 1);
            var south = grid.Get(column, row + 1);
            var east = grid.Get(column - 1, row);
            var west = grid.Get(column + 1, row);

            return new List<Cell>() { north, south, east, west }.Where(x => x != null).ToList();
        }

        public static bool IsEdgeCell(this Cell[,] grid, Cell cell)
        {
            var column = cell.Location.Column;
            var row = cell.Location.Row;

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

        public static IEnumerable<GridLocation> GetAdjacentLocations(this LevelGrid grid, GridLocation location)
        {
            var n = grid[location.Column, location.Row - 1]?.Location ?? null;
            var s = grid[location.Column, location.Row + 1]?.Location ?? null;
            var e = grid[location.Column + 1, location.Row]?.Location ?? null;
            var w = grid[location.Column - 1, location.Row]?.Location ?? null;
            var ne = grid[location.Column + 1, location.Row - 1]?.Location ?? null;
            var nw = grid[location.Column - 1, location.Row - 1]?.Location ?? null;
            var se = grid[location.Column + 1, location.Row + 1]?.Location ?? null;
            var sw = grid[location.Column - 1, location.Row + 1]?.Location ?? null;

            var result = new List<GridLocation>() { n, s, e, w, ne, nw, se, sw };

            return result.Where(x => x != null);
        }

        public static Cell[] GetCardinalAdjacentCells(this LevelGrid grid, GridLocation location)
        {
            var n = grid[location.Column, location.Row - 1];
            var s = grid[location.Column, location.Row + 1];
            var e = grid[location.Column + 1, location.Row];
            var w = grid[location.Column - 1, location.Row];

            // Need this to be optimized for speed
            var count = 0;
            if (n != null) count++;
            if (s != null) count++;
            if (e != null) count++;
            if (w != null) count++;

            var result = new Cell[count];
            var index = 0;

            if (n != null) result[index++] = n;
            if (s != null) result[index++] = s;
            if (e != null) result[index++] = e;
            if (w != null) result[index++] = w;

            return result;

        }

        public static Compass GetDirectionOfAdjacentLocation(this LevelGrid grid, GridLocation location, GridLocation adjacentLocation)
        {
            var north = (adjacentLocation.Row - location.Row) == -1;
            var south = (adjacentLocation.Row - location.Row) == 1;
            var east = (adjacentLocation.Column - location.Column) == 1;
            var west = (adjacentLocation.Column - location.Column) == -1;

            if      (north && east) return Compass.NE;
            else if (north && west) return Compass.NW;
            else if (south && east) return Compass.SE;
            else if (south && west) return Compass.SW;
            else if (north) return Compass.N;
            else if (south) return Compass.S;
            else if (east) return Compass.E;
            else if (west) return Compass.W;
            else
                throw new Exception("Invalid adjacent cell GetDirectionOfAdjacentCell");
        }

        /// <summary>
        /// Returns first cell in the specified direction
        /// </summary>
        public static GridLocation GetPointInDirection(this LevelGrid grid, GridLocation location, Compass direction)
        {
            switch (direction)
            {
                case Compass.N: return grid[location.Column, location.Row - 1]?.Location ?? GridLocation.Empty;
                case Compass.S: return grid[location.Column, location.Row + 1]?.Location ?? GridLocation.Empty;
                case Compass.E: return grid[location.Column + 1, location.Row]?.Location ?? GridLocation.Empty;
                case Compass.W: return grid[location.Column - 1, location.Row]?.Location ?? GridLocation.Empty;
                case Compass.NE: return grid[location.Column + 1, location.Row - 1]?.Location ?? GridLocation.Empty;
                case Compass.NW: return grid[location.Column - 1, location.Row - 1]?.Location ?? GridLocation.Empty;
                case Compass.SW: return grid[location.Column - 1, location.Row + 1]?.Location ?? GridLocation.Empty;
                case Compass.SE: return grid[location.Column + 1, location.Row + 1]?.Location ?? GridLocation.Empty;
                case Compass.Null:
                default:
                    return location;
            }
        }

        /// <summary>
        /// Returns 1st of 2 off diagonal cells in the specified non-cardinal direction (Example: NE -> N cell)
        /// </summary>
        /// <param name="direction">NE, NW, SE, SW</param>
        public static Cell GetOffDiagonalCell1(this LevelGrid grid, GridLocation location, Compass direction, out Compass cardinalDirection1)
        {
            switch (direction)
            {
                case Compass.NE:
                case Compass.NW:
                    cardinalDirection1 = Compass.N;
                    return grid[location.Column, location.Row - 1];
                case Compass.SE:
                case Compass.SW:
                    cardinalDirection1 = Compass.S;
                    return grid[location.Column, location.Row + 1];
                default:
                    throw new Exception("Off-Diagonal directions don't include " + direction.ToString());
            }
        }

        /// <summary>
        /// Returns 2nd of 2 off diagonal cells in the specified non-cardinal direction (Example: NE -> E cell)
        /// </summary>
        /// <param name="direction">NE, NW, SE, SW</param>
        public static Cell GetOffDiagonalCell2(this LevelGrid grid, GridLocation location, Compass direction, out Compass cardinalDirection2)
        {
            switch (direction)
            {
                case Compass.NE:
                case Compass.SE:
                    cardinalDirection2 = Compass.E;
                    return grid[location.Column + 1, location.Row];
                case Compass.SW:
                case Compass.NW:
                    cardinalDirection2 = Compass.W;
                    return grid[location.Column - 1, location.Row];
                default:
                    throw new Exception("Off-Diagonal directions don't include " + direction.ToString());
            }
        }

        /// <summary>
        /// Returns 1st of 2 off diagonal cells in the specified non-cardinal direction (Example: NE -> N cell)
        /// </summary>
        /// <param name="direction">NE, NW, SE, SW</param>
        public static Cell GetOffDiagonalCell1(this Cell[,] grid, GridLocation location, Compass direction, out Compass cardinalDirection1)
        {
            switch (direction)
            {
                case Compass.NE:
                case Compass.NW:
                    cardinalDirection1 = Compass.N;
                    return grid.Get(location.Column, location.Row - 1);
                case Compass.SE:
                case Compass.SW:
                    cardinalDirection1 = Compass.S;
                    return grid.Get(location.Column, location.Row + 1);
                default:
                    throw new Exception("Off-Diagonal directions don't include " + direction.ToString());
            }
        }

        /// <summary>
        /// Returns 2nd of 2 off diagonal cells in the specified non-cardinal direction (Example: NE -> E cell)
        /// </summary>
        /// <param name="direction">NE, NW, SE, SW</param>
        public static Cell GetOffDiagonalCell2(this Cell[,] grid, GridLocation location, Compass direction, out Compass cardinalDirection2)
        {
            switch (direction)
            {
                case Compass.NE:
                case Compass.SE:
                    cardinalDirection2 = Compass.E;
                    return grid.Get(location.Column + 1, location.Row);
                case Compass.SW:
                case Compass.NW:
                    cardinalDirection2 = Compass.W;
                    return grid.Get(location.Column - 1, location.Row);
                default:
                    throw new Exception("Off-Diagonal directions don't include " + direction.ToString());
            }
        }

        public static Compass GetDirectionBetweenAdjacentPoints(GridLocation cell1, GridLocation cell2)
        {
            int deltaX = cell2.Column - cell1.Column;
            int deltaY = cell2.Row - cell1.Row;

            if (deltaX == -1)
            {
                switch (deltaY)
                {
                    case -1: return Compass.NW;
                    case 0: return Compass.W;
                    case 1: return Compass.SW;
                }
            }
            if (deltaX == 0)
            {
                switch (deltaY)
                {
                    case -1: return Compass.N;
                    case 0: return Compass.Null;
                    case 1: return Compass.S;
                }
            }
            if (deltaX == 1)
            {
                switch (deltaY)
                {
                    case -1: return Compass.NE;
                    case 0: return Compass.E;
                    case 1: return Compass.SE;
                }
            }
            return Compass.Null;
        }

        public static Compass GetOppositeDirection(Compass direction)
        {
            switch (direction)
            {
                case Compass.N:
                    return Compass.S;
                case Compass.S:
                    return Compass.N;
                case Compass.E:
                    return Compass.W;
                case Compass.W:
                    return Compass.E;
                case Compass.NE:
                    return Compass.SW;
                case Compass.NW:
                    return Compass.SE;
                case Compass.SE:
                    return Compass.SW;
                case Compass.SW:
                    return Compass.SE;
                default:
                    return Compass.Null;
            }
        }

        public static PointF TransformToPhysicalLayout(GridLocation p)
        {
            float x = (float)(ModelConstants.CellWidth * p.Column);
            float y = (float)(ModelConstants.CellHeight * p.Row);
            return new PointF(x, y);
        }
    }
}
