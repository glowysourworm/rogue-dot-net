using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Construction;

using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Finishing.Interface
{
    public interface IWallFinisher
    {
        /// <summary>
        /// Creates wall outline for each region in the grid (using null cells) by checking 8-way adjacency.
        /// </summary>
        void CreateWalls(GridCellInfo[,] grid, bool createBorder);

        /// <summary>
        /// Creates doors where there are region cells adjacent to any corridor cells. The doors are created
        /// in the corridor.
        /// </summary>
        void CreateDoors(GridCellInfo[,] grid, IEnumerable<Region<GridCellInfo>> regions, IEnumerable<LayerInfo<GridCellInfo>> terrainLayers);
    }
}
