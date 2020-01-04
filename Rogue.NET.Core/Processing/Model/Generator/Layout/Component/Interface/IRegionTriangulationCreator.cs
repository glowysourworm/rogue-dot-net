using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;

using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface
{
    /// <summary>
    /// Component that creates a triangulation of the grid regions
    /// </summary>
    public interface IRegionTriangulationCreator
    {
        Graph CreateTriangulation<T>(IEnumerable<ConnectedRegion<T>> regions, LayoutTemplate template) where T : class, IGridLocator;

        Graph CreateDefaultTriangulation<T>(IEnumerable<ConnectedRegion<T>> regions) where T : class, IGridLocator;
    }
}
