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
            foreach (var tile in tiling.ConnectingTiles.Where(x => x.IsMarkedForRouting))
            {
                foreach (var vertex in tile.ConnectionRoute.ConnectionPathway)
                {
                    if (grid[vertex.X, vertex.Y] != null)
                        throw new Exception("Trying to create corridor in existing cell TilingCorridorRegionConnector");

                    grid[vertex.X, vertex.Y] = new Cell(vertex.X, vertex.Y, false);
                }
            }
        }
    }
}
