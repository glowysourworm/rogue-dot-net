using Rogue.NET.Core.Model.Scenario.Content.Layout.Construction;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface
{
    public interface IRegionBuilder
    {
        /// <summary>
        /// Sets up regions of cells in the grid based on the template and returns the 2D cell array for the layout
        /// </summary>
        public GridCellInfo[,] BuildRegions(LayoutTemplate template);

        /// <summary>
        /// Sets up a default region of cells in the grid for creating a default layout
        /// </summary>
        public GridCellInfo[,] BuildDefaultRegion();
    }
}
