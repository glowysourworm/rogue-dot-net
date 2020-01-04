using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Construction;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface
{
    public interface IRegionBuilder
    {
        /// <summary>
        /// Sets up regions of cells in the BASE layer, creates a triangulation of the layer to build
        /// the initial CONNECTION layer. Returns the layout container with the results.
        /// </summary>
        public LayoutContainer BuildRegions(LayoutTemplate template);

        /// <summary>
        /// Sets up regions of cells in the BASE layer, creates a triangulation of the layer to build
        /// the initial CONNECTION layer. Returns the layout container with the results.
        /// </summary>
        public LayoutContainer BuildDefaultLayout();
    }
}
