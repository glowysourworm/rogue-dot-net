using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Generator.Interface
{
    public interface IContentGenerator
    {
        IEnumerable<Level> CreateContents(
                IEnumerable<Level> levels,
                ScenarioConfigurationContainer configurationContainer,
                IEnumerable<Religion> religions,
                IEnumerable<AttackAttribute> scenarioAttributes,
                bool survivorMode);
    }
}
