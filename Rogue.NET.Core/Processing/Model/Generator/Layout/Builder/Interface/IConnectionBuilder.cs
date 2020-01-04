using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Construction;

using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface
{
    public interface IConnectionBuilder
    {
        /// <summary>
        /// Generates connections between the specified regions - returns the region triangulation graph along with
        /// modified regions (in the case that the grid is modified)
        /// </summary>
        void BuildConnections(LayoutContainer container,
                              LayoutTemplate template);

        /// <summary>
        /// Generates connections avoiding the provided regions. This is typically used for connecting regions separated
        /// by terrain. Returns the region triangulation graph.
        /// </summary>
        void BuildConnectionsWithAvoidRegions(LayoutContainer container,
                                               LayoutTemplate template,
                                               IEnumerable<Region<GridCellInfo>> avoidRegions);
    }
}
