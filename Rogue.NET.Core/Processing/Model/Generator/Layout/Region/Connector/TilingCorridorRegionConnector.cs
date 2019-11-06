using Rogue.NET.Core.Math.Algorithm;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Region.Connector
{
    public static class TilingCorridorRegionConnector
    {
        /// <summary>
        /// Connects regions specified by the tiling - which contains pre-computed routes for cells
        /// </summary>
        public static void ConnectRegionTiling(Cell[,] grid, NavigationTiling tiling)
        {
            // Search through marked tiles - which have pre-computed routes
            foreach (var tile in tiling.ConnectingTiles.Where(x => x.ConnectionPoints
                                                                    .SelectMany(point => point.RouteNumbers)
                                                                    .Any()))
            {
                foreach (var vertex in tile.ConnectionRoute.ConnectionPathway)
                {
                    //if (grid[vertex.X, vertex.Y] != null)
                    //    throw new Exception("Trying to create corridor in existing cell TilingCorridorRegionConnector");

                    grid[vertex.X, vertex.Y] = new Cell(vertex.X, vertex.Y, true);
                }
            }
        }

        public static void CreateRectilinearRoutePoints(Cell[,] grid, VertexInt startPoint, VertexInt endPoint, bool yDirection)
        {
            //if ((startPoint.X == endPoint.X) && !yDirection)
            //    throw new Exception("Improper corridor specification NavigationTile.CreateRectilinearRoutePoints");

            //if ((startPoint.Y == endPoint.Y) && yDirection)
            //    throw new Exception("Improper corridor specification NavigationTile.CreateRectilinearRoutePoints");

            // Create follower for the loop
            var pointValue = yDirection ? startPoint.Y : startPoint.X;

            // Get the total count for iteration
            var count = System.Math.Abs(yDirection ? (endPoint.Y - startPoint.Y) : (endPoint.X - startPoint.X)) + 1;

            // Get the incrementing the value during iteration
            var increment = (yDirection ? (endPoint.Y - startPoint.Y) : (endPoint.X - startPoint.X)) > 0 ? 1 : -1;

            for (int i = 0; i < count; i++)
            {
                VertexInt vertex;

                if (yDirection)
                    vertex = new VertexInt(startPoint.X, pointValue);

                else
                    vertex = new VertexInt(pointValue, startPoint.Y);

                if (vertex.X >= grid.GetLength(0) || vertex.X < 0)
                    throw new Exception("Trying to create point outside the bounds of the navigation tile");

                if (vertex.Y > grid.GetLength(1) || vertex.Y < 0)
                    throw new Exception("Trying to create point outside the bounds of the navigation tile");

                // Create grid cell
                grid[vertex.X, vertex.Y] = new Cell(vertex.X, vertex.Y, false);

                pointValue += increment;
            }
        }
    }
}
