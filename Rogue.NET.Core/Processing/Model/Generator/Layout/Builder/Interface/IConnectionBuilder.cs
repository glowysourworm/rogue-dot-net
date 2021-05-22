using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Construction;

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
                              LayoutTemplate template);

        /// <summary>
        /// Creates maze regions in the empty space in the layout as NON-CORRIDOR ROOMS. DOES NOT COMPLETE 
        /// CONNECTIONS! MUST RE-GENERATE CONNECTION GRAPH; and call BuildConnections.
        /// </summary>
        public void CreateMazeRegionsInEmptySpace(LayoutContainer container,
                                                  LayoutTemplate template,
                                                  MazeType mazeType);
    }
}
