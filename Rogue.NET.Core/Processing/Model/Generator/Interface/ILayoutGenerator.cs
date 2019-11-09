using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;

namespace Rogue.NET.Core.Processing.Model.Generator.Interface
{
    public interface ILayoutGenerator
    {
        LevelGrid CreateLayout(LayoutTemplate template);
    }
}
