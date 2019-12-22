using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface
{
    /// <summary>
    /// Component that creates a triangulation of the grid regions
    /// </summary>
    public interface IRegionTriangulationCreator
    {
        Graph<Region<T>> CreateTriangulation<T>(IEnumerable<Region<T>> regions, LayoutTemplate template) where T : class, IGridLocator;
    }
}
