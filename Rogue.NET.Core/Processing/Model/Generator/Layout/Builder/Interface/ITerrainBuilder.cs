using Rogue.NET.Common.Collection;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Construction;

using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface
{
    /// <summary>
    /// (STATEFUL!!!) Represents a component that works with the LayoutContainer to generate terrain.
    /// </summary>
    public interface ITerrainBuilder
    {
        /// <summary>
        /// Should be called before other methods - initializes the set of layers to build
        /// </summary>
        void Initialize(int width, int height, IEnumerable<TerrainLayerGenerationTemplate> layers);

        /// <summary>
        /// Constructs regions from the initialized layers in their current state using the LayoutContainer to reference the
        /// current state of the layout grid. THIS CAN CREATE TERRAIN SUPPORT CELLS IN THE LAYOUT - BUT DOES NOT MODIFY THE
        /// TOPOLOGY OF THE ROOM CONNECTIONS.
        /// </summary>
        void AddTerrainSupport(LayoutContainer container);

        /// <summary>
        /// Completes the terrain regions based on the FULL layer. ELIMINATES ISLANDS FROM THE TERRAIN
        /// AND LAYOUT GRIDS. DOES NOT MODIFY THE TOPOLOGY OF THE ROOM CONNECTIONS.
        /// </summary>
        SimpleDictionary<TerrainLayerTemplate, IEnumerable<RegionInfo<GridLocation>>> FinalizeTerrainRegions(LayoutContainer container);

        /// <summary>
        /// Queries the original terrain grids to see whether there is impassable terrain at this location
        /// </summary>
        bool AnyImpassableTerrain(int column, int row);

        /// <summary>
        /// Queries the original terrain grids to see whether there is terrain at this location
        /// </summary>
        bool AnyTerrain(int column, int row);
    }
}
