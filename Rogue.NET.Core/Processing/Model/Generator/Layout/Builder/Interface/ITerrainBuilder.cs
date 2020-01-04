using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Construction;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface
{
    public interface ITerrainBuilder
    {
        /// <summary>
        /// Builds set of terrain layers based on the layout template and the provided grid. Returns true if terrain layout was successful. Failure
        /// happens typically when there's no unblocked regions of the grid left to create rooms from.
        /// </summary>
        bool BuildTerrain(LayoutContainer container, LayoutTemplate template);
    }
}
