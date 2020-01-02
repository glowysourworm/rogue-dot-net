using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Construction;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;

using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface
{
    public interface IConnectionBuilder
    {
        /// <summary>
        /// Generates connections between the specified regions - returns the region triangulation graph
        /// </summary>
        Graph BuildConnections(GridCellInfo[,] grid, IEnumerable<ConnectedRegion<GridCellInfo>> regions, LayoutTemplate template);

        /// <summary>
        /// Generates connections avoiding the provided regions. This is typically used for connecting regions separated
        /// by terrain. Returns the region triangulation graph.
        /// </summary>
        Graph BuildConnectionsWithAvoidRegions(GridCellInfo[,] grid, IEnumerable<ConnectedRegion<GridCellInfo>> regions, IEnumerable<Region<GridCellInfo>> avoidRegions, LayoutTemplate template);
    }
}
