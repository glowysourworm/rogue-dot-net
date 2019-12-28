using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Generator.Interface
{
    public interface ILayoutGenerator
    {
        LayoutGrid CreateLayout(LayoutTemplate template);
    }
}
