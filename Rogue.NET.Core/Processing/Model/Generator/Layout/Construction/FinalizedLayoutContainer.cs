using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;

using System.Collections.Generic;

using static Rogue.NET.Core.Model.Scenario.Content.Layout.LayoutGrid;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Construction
{
    /// <summary>
    /// Simple container to retrieve finalized layout components
    /// </summary>
    public class FinalizedLayoutContainer
    {
        public RegionBoundary Bounds { get; set; }
        public GridCellInfo[,] Grid { get; set; }
        public IDictionary<LayoutLayer, IEnumerable<RegionInfo<GridLocation>>> RegionDict { get; set; }
        public IDictionary<TerrainLayerTemplate, IEnumerable<RegionInfo<GridLocation>>> TerrainDict { get; set; }
        public RegionGraphInfo<GridLocation> Graph { get; set; }
    }
}
