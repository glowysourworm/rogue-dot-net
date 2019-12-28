using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration.Design;

namespace Rogue.NET.Core.Processing.Model.Generator.Interface
{
    public interface IContentGenerator
    {
        Level CreateContents(Level level,
                             LevelBranchTemplate branchTemplate,
                             LayoutGenerationTemplate layoutTemplate,
                             ScenarioEncyclopedia encyclopedia,
                             bool lastLevel,
                             bool survivorMode);
    }
}
