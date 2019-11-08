using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Model.Extension;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using RegionModel = Rogue.NET.Core.Model.Scenario.Content.Layout.Region;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Region
{
    /// <summary>
    /// Provides methods for iterating regions of the grid and filling out region boundaries
    /// </summary>
    public static class GridUtility
    {
        public static RegionModel CreateRectangularRegion(Cell[,] grid, RegionBoundary boundary, bool cellsAreWalls, bool overwriteCells)
        {
            return CreateRectangularRegion(grid, boundary.Left, boundary.Top, boundary.CellWidth, boundary.CellHeight, cellsAreWalls, overwriteCells);
        }

        /// <summary>
        /// Creates rectangular region inside of the provided cell 2D array with the specified parameters. The cells INSIDE
        /// the region MUST BE UN-INITIALIZED (NULL).
        /// </summary>
        public static RegionModel CreateRectangularRegion(Cell[,] grid, int column, int row, int width, int height, bool cellsAreWalls, bool overwriteCells)
        {
            var regionBounds = new RegionBoundary(new GridLocation(column, row), width, height);
            var regionCells = new List<GridLocation>();
            var edgeCells = new List<GridLocation>();

            // Create cells to fill the region
            for (int regionCol = column; regionCol < column + width; regionCol++)
            {
                for (int regionRow = row; regionRow < row + height; regionRow++)
                {
                    var cell = grid[regionCol, regionRow];

                    if (cell != null && !overwriteCells)
                        throw new Exception("Trying to over-write region cell");

                    // Calculate if the cell is on the edge 
                    var isEdge = (regionCol == column ||
                                  regionCol == ((width + column) - 1) ||
                                  regionRow == row ||
                                  regionRow == ((row + height) - 1));

                    cell = new Cell(regionCol, regionRow, cellsAreWalls);

                    // SET THE CELL IN THE GRID
                    grid[regionCol, regionRow] = cell;

                    // Store region scells 
                    regionCells.Add(cell.Location);

                    // Store edge cells for region data
                    if (isEdge)
                        edgeCells.Add(cell.Location);
                }
            }

            return new RegionModel(regionCells.ToArray(), edgeCells.ToArray(), regionBounds);
        }

        /// <summary>
        /// Identifies regions using Breadth First Search (Flood Fill) algorithm
        /// </summary>
        public static IEnumerable<RegionModel> IdentifyRegions(this Cell[,] grid)
        {
            // Locate regions and assign them inside the LevelGrid
            //
            // 0) Iterate cells
            // 1) First cell that's non-empty AND not part of an existing region
            // 2) Use flood fill to find connected cells
            //

            // Collect region data to pass to level grid constructor
            var regions = new List<RegionModel>();

            // Collect cell data on new regions to know what locations have been found
            // to be in one of the regions (during iteration)
            var regionGrids = new List<Cell[,]>();

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j] != null &&
                       !grid[i, j].IsWall &&
                        regionGrids.All(region => region[i, j] == null))
                    {
                        // Use flood fill to locate all region cells
                        //
                        Cell[,] regionGrid = null;
                        RegionModel region = null;

                        // Success -> Save the region data
                        if (grid.FloodFill(grid[i, j].Location, out region, out regionGrid))
                        {
                            regions.Add(region);
                            regionGrids.Add(regionGrid);
                        }
                    }
                }
            }

            return regions;
        }

        /// <summary>
        /// Applied Breadth First Search to try and identify a region at the given test location. Returns true if any region
        /// cells were found.
        /// </summary>
        public static bool FloodFill(this Cell[,] grid, GridLocation testLocation, out RegionModel region, out Cell[,] regionGrid)
        {
            var bounds = new RegionBoundary(new GridLocation(0, 0), grid.GetLength(0), grid.GetLength(1));

            // Region Data
            regionGrid = new Cell[grid.GetLength(0), grid.GetLength(1)];
            var regionCells = new List<GridLocation>();
            var edgeCells = new List<GridLocation>();
            var regionBounds = new RegionBoundary(testLocation, 1, 1);

            // Use queue to know what cells have been verified. Starting with test cell - continue 
            // until all connected cells have been added to the resulting region.
            var resultQueue = new Queue<Cell>(bounds.CellWidth * bounds.CellHeight);

            // Process the first cell
            var testCell = grid[testLocation.Column, testLocation.Row];
            resultQueue.Enqueue(testCell);
            regionCells.Add(testCell.Location);
            regionGrid[testLocation.Column, testLocation.Row] = testCell;

            // Check for edge-of-region cells or edge of grid cells
            if (GridUtility.IsEdgeCell(grid, testCell.Location.Column, testCell.Location.Row))
                edgeCells.Add(testCell.Location);

            while (resultQueue.Count > 0)
            {
                var regionCell = resultQueue.Dequeue();

                // Search cardinally adjacent cells (N,S,E,W)
                foreach (var cell in grid.GetCardinalAdjacentElements(regionCell.Location.Column, regionCell.Location.Row))
                {
                    // Find connected cells that are not yet part of the region
                    if (GridUtility.IsAdjacentCellConnected(grid, regionCell.Location, cell.Location) &&
                        regionGrid[cell.Location.Column, cell.Location.Row] == null)
                    {
                        // Add cell to region immediately to prevent extra cells on queue
                        regionGrid[cell.Location.Column, cell.Location.Row] = cell;

                        // Add cell also to region data
                        regionCells.Add(cell.Location);

                        // Determine whether cell is an edge cell
                        if (GridUtility.IsEdgeCell(grid, cell.Location.Column, cell.Location.Row))
                            edgeCells.Add(cell.Location);

                        // Re-calculate boundary
                        regionBounds.Expand(cell.Location);

                        // Push cell onto the queue to be iterated
                        resultQueue.Enqueue(cell);
                    }
                }
            }

            // Assign region data to new region
            region = new RegionModel(regionCells.ToArray(), edgeCells.ToArray(), regionBounds);

            return true;
        }

        public static Compass GetDirectionOfAdjacentLocation(GridLocation location, GridLocation adjacentLocation)
        {
            var north = (adjacentLocation.Row - location.Row) == -1;
            var south = (adjacentLocation.Row - location.Row) == 1;
            var east = (adjacentLocation.Column - location.Column) == 1;
            var west = (adjacentLocation.Column - location.Column) == -1;

            if (north && east) return Compass.NE;
            else if (north && west) return Compass.NW;
            else if (south && east) return Compass.SE;
            else if (south && west) return Compass.SW;
            else if (north) return Compass.N;
            else if (south) return Compass.S;
            else if (east) return Compass.E;
            else if (west) return Compass.W;
            else
                throw new Exception("Invalid adjacent cell GetDirectionOfAdjacentLocation");
        }

        public static PointF TransformToPhysicalLayout(GridLocation p)
        {
            float x = (float)(ModelConstants.CellWidth * p.Column);
            float y = (float)(ModelConstants.CellHeight * p.Row);
            return new PointF(x, y);
        }

        /// <summary>
        /// Gets the direction between two adjacent points
        /// </summary>
        public static Compass GetDirectionBetweenAdjacentPoints(int column1, int row1, int column2, int row2)
        {
            int deltaX = column2 - column1;
            int deltaY = row2 - row1;

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

        /// <summary>
        /// Returns the direction opposite to the provided one
        /// </summary>
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

        /// <summary>
        /// Calculates whether adjacent element is connected by accessible path - This will check for non-default elements
        /// at the off-diagonal locations
        /// </summary>
        public static bool IsAdjacentCellConnected(this Cell[,] grid, GridLocation location, GridLocation adjacentLocation)
        {
            var direction = GridUtility.GetDirectionOfAdjacentLocation(location, adjacentLocation);

            Compass cardinalDirection = Compass.Null;

            switch (direction)
            {
                case Compass.N:
                case Compass.S:
                case Compass.E:
                case Compass.W:
                    return grid[adjacentLocation.Column, adjacentLocation.Row] != null &&
                          !grid[adjacentLocation.Column, adjacentLocation.Row].IsWall;

                case Compass.NW:
                case Compass.NE:
                case Compass.SE:
                case Compass.SW:
                    {
                        var diag1 = grid.GetOffDiagonalElement1(location.Column, location.Row, direction, out cardinalDirection);
                        var diag2 = grid.GetOffDiagonalElement2(location.Column, location.Row, direction, out cardinalDirection);

                        return (diag1 != null && !diag1.IsWall) || 
                               (diag2 != null && !diag2.IsWall);
                    }
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns true if any adjacent cells are null or walls
        /// </summary>
        public static bool IsEdgeCell(Cell[,] grid, int column, int row)
        {
            var north = grid.Get(column, row - 1);
            var south = grid.Get(column, row + 1);
            var east = grid.Get(column + 1, row);
            var west = grid.Get(column - 1, row);
            var northEast = grid.Get(column + 1, row - 1);
            var northWest = grid.Get(column - 1, row - 1);
            var southEast = grid.Get(column + 1, row + 1);
            var southWest = grid.Get(column - 1, row + 1);

            return (north == null || north.IsWall) ||
                   (south == null || south.IsWall) ||
                   (east == null || east.IsWall) ||
                   (west == null || west.IsWall) ||
                   (northEast == null || northEast.IsWall) ||
                   (northWest == null || northWest.IsWall) ||
                   (southEast == null || southEast.IsWall) ||
                   (southWest == null || southWest.IsWall);
        }

        public static bool IsCardinalDirection(Compass direction)
        {
            switch (direction)
            {
                case Compass.N:
                case Compass.S:
                case Compass.E:
                case Compass.W:
                    return true;

                case Compass.Null:
                case Compass.NW:
                case Compass.NE:
                case Compass.SE:
                case Compass.SW:
                default:
                    return false;
            }
        }
    }
}
