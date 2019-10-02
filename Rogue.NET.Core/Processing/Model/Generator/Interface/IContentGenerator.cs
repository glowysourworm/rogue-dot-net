using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Design;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Generator.Interface
{
    public interface IContentGenerator
    {
        IEnumerable<Level> CreateContents(
                IEnumerable<Level> levels,
                IDictionary<Level, LevelBranchTemplate> selectedBranches,
                IDictionary<Level, LayoutGenerationTemplate> selectedLayouts,
                bool survivorMode);
    }
}
