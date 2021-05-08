using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Construction;

using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface
{
    /// <summary>
    /// Component that creates a triangulation of the grid regions
    /// </summary>
    public interface IRegionTriangulationCreator
    {
        GraphInfo<T> CreateTriangulation<T>(IEnumerable<Region<T>> regions, LayoutTemplate template) where T : class, IGridLocator;
    }
}
