using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Construction;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface
{
    public interface IRegionBuilder
    {
        /// <summary>
        /// Sets up regions of cells in the layout grid
        /// </summary>
        public GridCellInfo[,] BuildRegions(LayoutTemplate template);

        /// <summary>
        /// Sets up regions of cells in the layout grid
        /// </summary>
        public GridCellInfo[,] BuildDefaultLayout(LayoutTemplate template);
    }
}
