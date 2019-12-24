using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Design;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Generator.Interface
{
    public interface IContentGenerator
    {
        Level CreateContents(Level level, 
                             LevelBranchTemplate branchTemplate, 
                             LayoutGenerationTemplate layoutTemplate, 
                             ScenarioEncyclopedia encyclopedia,
                             Graph<Region<GridLocation>> transporterGraph,
                             bool lastLevel, 
                             bool survivorMode);
    }
}
