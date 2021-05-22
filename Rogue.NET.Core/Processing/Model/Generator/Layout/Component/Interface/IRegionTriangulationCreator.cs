using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Construction;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface
{
    /// <summary>
    /// Component that creates a triangulation of the grid regions
    /// </summary>
    public interface IRegionTriangulationCreator
    {
        /// <summary>
        /// Creates triangulation of regions for the connection layer and stores the result in the layout container
        /// </summary>
        void CreateTriangulation(LayoutContainer container, LayoutTemplate template);
    }
}
