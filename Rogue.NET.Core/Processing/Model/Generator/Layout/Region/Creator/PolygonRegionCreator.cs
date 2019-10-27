using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System;
using System.Collections.Generic;
using RegionModel = Rogue.NET.Core.Model.Scenario.Content.Layout.Region;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Region.Creator
{
    public static class PolygonRegionCreator
    {
        public static RegionModel CreateRegion(Cell[,] grid, Polygon polygon, bool overwriteCells)
        {
            // Procedure
            //
            // 1) Iterate inside the boundary rectangle of the polygon
            // 2) Check to see if the point lies inside the polygon
            // 3) Create cells on the interior of the polygon
            //

            // Truncation of the polygon should expand one cell to include the captured cells
            //
            var left = (int)polygon.Boundary.Left;
            var right = (int)polygon.Boundary.Right;
            var top = (int)polygon.Boundary.Top;
            var bottom = (int)polygon.Boundary.Bottom;

            var regionBoundary = new RegionBoundary(new GridLocation(left, top), (right - left) + 1, (bottom - top) + 1);
            var regionCells = new List<GridLocation>();
            var edgeCells = new List<GridLocation>();

            for (int i = left; i < right + 1; i++)
            {
                for (int j = top; j < bottom + 1; j++)
                {
                    // Check to see whether point is on the edge of the polygon
                    //
                    var onEdge = false;
                    if (polygon.Contains(new Vertex(i, j), out onEdge) || onEdge)
                    {
                        // TODO:TERRAIN - Try and fix this if it's easy enough...
                        //
                        // PROBLEM WITH CELLS BEING ADDED TWICE IF THE POLYGON IS VERY
                        // "JAGGED". THIS MAY NOT BE EASY TO FIX IN THE MATH.
                        //
                        if (grid[i, j] != null && !overwriteCells)
                            throw new Exception("Trying to overwrite grid cell PolygonRegionCreator");

                        // Add cell to the grid
                        //
                        grid[i, j] = new Cell(i, j, false);

                        // TODO:TERRAIN - SHOULD NOT HAVE TO CHECK THESE LOOKUPS
                        //
                        if (onEdge)
                            edgeCells.Add(grid[i, j].Location);

                        // Store region data
                        //
                        regionCells.Add(grid[i, j].Location);
                    }
                }
            }

            return new RegionModel(regionCells.ToArray(), edgeCells.ToArray(), regionBoundary);
        }
    }
}
