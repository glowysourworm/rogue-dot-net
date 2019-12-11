using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using System.Collections.Generic;
using static Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface.IMazeRegionCreator;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface
{
    public interface IConnectionBuilder
    {
        /// <summary>
        /// Builds corridors between specified regions
        /// </summary>
        void BuildConnections(GridCellInfo[,] grid, IEnumerable<Region> regions, LayoutTemplate template);

        /// <summary>
        /// Generates connections avoiding the provided regions. This is typically used for connecting regions separated
        /// by terrain.
        /// </summary>
        void BuildConnectionsWithAvoidRegions(GridCellInfo[,] grid, IEnumerable<Region> regions, IEnumerable<Region> avoidRegions, LayoutTemplate template);
    }
}
