using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System.Collections.Generic;
using static Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface.IMazeRegionCreator;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface
{
    public interface IConnectionBuilder
    {
        /// <summary>
        /// Identifies regions and builds rectangular corridors between them. THIS SHOULD BE USED FOR RECTANGULAR ROOM REGIONS ONLY
        /// </summary>
        IEnumerable<Region> BuildRectilinearCorridors(GridCellInfo[,] grid);

        /// <summary>
        /// Identifies regions and builds linear corridors between them
        /// </summary>
        IEnumerable<Region> BuildCorridors(GridCellInfo[,] grid);

        /// <summary>
        /// Identifies regions and builds mazes into the negative space. It then connects the regions to the mazes.
        /// </summary>
        IEnumerable<Region> BuildMazeCorridors(GridCellInfo[,] grid, MazeType mazeType, double wallRemovalRatio, double horizontalVerticalBias);

        /// <summary>
        /// Identifies regions and builds mandatory connection points into the grid locations to connect them together
        /// </summary>
        IEnumerable<Region> BuildConnectionPoints(GridCellInfo[,] grid);
    }
}
