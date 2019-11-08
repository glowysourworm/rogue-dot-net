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
        public static void CreateRectilinearRoutePoints(GridCellInfo[,] grid, GridLocation startPoint, GridLocation endPoint, bool yDirection)
        {
            //if ((startPoint.X == endPoint.X) && !yDirection)
            //    throw new Exception("Improper corridor specification NavigationTile.CreateRectilinearRoutePoints");

            //if ((startPoint.Y == endPoint.Y) && yDirection)
            //    throw new Exception("Improper corridor specification NavigationTile.CreateRectilinearRoutePoints");

            // Create follower for the loop
            var pointValue = yDirection ? startPoint.Row : startPoint.Column;

            // Get the total count for iteration
            var count = System.Math.Abs(yDirection ? (endPoint.Row - startPoint.Row) : (endPoint.Column - startPoint.Column)) + 1;

            // Get the incrementing the value during iteration
            var increment = (yDirection ? (endPoint.Row - startPoint.Row) : (endPoint.Column - startPoint.Column)) > 0 ? 1 : -1;

            for (int i = 0; i < count; i++)
            {
                GridLocation vertex;

                if (yDirection)
                    vertex = new GridLocation(startPoint.Column, pointValue);

                else
                    vertex = new GridLocation(pointValue, startPoint.Row);

                if (vertex.Column >= grid.GetLength(0) || vertex.Column < 0)
                    throw new Exception("Trying to create point outside the bounds of the navigation tile");

                if (vertex.Row > grid.GetLength(1) || vertex.Row < 0)
                    throw new Exception("Trying to create point outside the bounds of the navigation tile");

                // Create grid cell
                grid[vertex.Column, vertex.Row] = new GridCellInfo(vertex.Column, vertex.Row) { IsWall = false };

                pointValue += increment;
            }
        }
    }
}
