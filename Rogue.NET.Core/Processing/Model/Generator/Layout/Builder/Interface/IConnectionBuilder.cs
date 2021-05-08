using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Construction;

using System.Collections.Generic;

using static Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface.IMazeRegionCreator;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface
{
    public interface IConnectionBuilder
    {
        /// <summary>
        /// Generates connections between the specified regions - uses impassable terrain regions from the
        /// LayoutContainer to build around ("Avoid Regions"). NOTE*** For maze connection types - must call
        /// CreateMazeCorridors first manually. Then, the graph of regions to triangulate must be calculated.
        /// </summary>
        void BuildConnections(LayoutContainer container,
                              LayoutTemplate template,
                              GraphInfo<GridCellInfo> regionGraph);

        /// <summary>
        /// Creates maze CORRIDORS in the empty space in the layout. DOES NOT COMPLETE CONNECTIONS! MUST RE-GENERATE 
        /// CONNECTION GRAPH; and call ConnectUsingShortestPath.
        /// </summary>
        public void CreateMazeCorridors(LayoutContainer container,
                                        LayoutTemplate template,
                                        MazeType mazeType);

        /// <summary>
        /// Completes the layout connections using the provided connection graph
        /// </summary>
        void ConnectUsingShortestPath(LayoutContainer container, GraphInfo<GridCellInfo> connectionGraph);
    }
}
