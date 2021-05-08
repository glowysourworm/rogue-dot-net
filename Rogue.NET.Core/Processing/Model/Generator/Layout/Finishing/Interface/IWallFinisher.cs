using Rogue.NET.Core.Processing.Model.Generator.Layout.Construction;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Finishing.Interface
{
    public interface IWallFinisher
    {
        /// <summary>
        /// Creates wall outline for each region in the grid (using null cells) by checking 8-way adjacency.
        /// </summary>
        void CreateWalls(LayoutContainer container);

        /// <summary>
        /// Creates doors where there are region cells adjacent to any corridor cells. The doors are created
        /// in the corridor.
        /// </summary>
        void CreateDoors(LayoutContainer container, GraphInfo<GridCellInfo> connectionGraph);
    }
}
