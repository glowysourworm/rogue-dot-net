using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using System.Collections.Generic;
using static Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface.IMazeRegionCreator;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface
{
    public interface IConnectionBuilder
    {
        /// <summary>
        /// Identifies regions and builds corridors between them
        /// </summary>
        IEnumerable<Region> BuildConnections(GridCellInfo[,] grid, LayoutTemplate template);
    }
}
