using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Region.Geometry
{
    /// <summary>
    /// Component that lays out rectangles - subdividing the negative space to create a mesh of
    /// other rectangles that tile the entire region. These can be used to triangulate corridors
    /// that pass through the negative space and avoid overlaps with the room or other terrain
    /// regions we wish to avoid.
    /// </summary>
    public static class TiledMeshRegionGeometryCreator
    {
        /// <summary>
        /// Creates tiled mesh of connecting regions based on the input regions. This serves as a way to
        /// triangulate paths around the map. These are used to create corridors that don't overlaps the
        /// specified regions. The grid width / height are the level grid dimensions.
        /// </summary>
        public static IEnumerable<RegionBoundary> CreateConnectingRegions(int gridWidth, int gridHeight, IEnumerable<RegionBoundary> regions)
        {
            var tiling = new RectangularTiling(new RectangleInt(new VertexInt(0, 0), new VertexInt(gridWidth - 1, gridHeight - 1)));

            foreach (var boundary in regions)
            {
                var regionRectangle = new RectangleInt(new VertexInt(boundary.Left, boundary.Top),
                                                       new VertexInt(boundary.Right, boundary.Bottom));

                tiling.AddRegionRectangle(regionRectangle);
            }

            return tiling.ConnectingRectangles.Select(rectangle =>
            {
                return new RegionBoundary(new GridLocation(rectangle.Left, rectangle.Top), rectangle.Width, rectangle.Height);
            }).Actualize();
        }
    }
}
