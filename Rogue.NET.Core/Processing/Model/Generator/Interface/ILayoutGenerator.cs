using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Design;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Generator.Interface
{
    public interface ILayoutGenerator
    {
        IEnumerable<Level> CreateLayouts(IEnumerable<string> levelBranchNames, IEnumerable<LayoutGenerationTemplate> layoutTemplates);
    }
}
