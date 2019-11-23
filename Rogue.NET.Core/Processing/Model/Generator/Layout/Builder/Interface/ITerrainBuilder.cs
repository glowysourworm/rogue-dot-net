using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface
{
    public interface ITerrainBuilder
    {
        /// <summary>
        /// Builds set of terrain layers based on the layout template and the provided grid and feature map (input map of features for
        /// Dijkstra's algorithm)
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="template"></param>
        /// <param name="roomLayer"></param>
        /// <returns></returns>
        IEnumerable<LayerInfo> BuildTerrain(GridCellInfo[,] grid, LayoutTemplate template, out LayerInfo roomLayer);
    }
}
