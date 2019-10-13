using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration;

using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Service.Cache.Interface
{
    public interface IScenarioConfigurationCache
    {
        IEnumerable<ScenarioConfigurationContainer> EmbeddedConfigurations { get; }
        IEnumerable<ScenarioConfigurationContainer> UserConfigurations { get; }
        ScenarioImage GetRandomSmileyCharacter();
        void SaveConfiguration(ScenarioConfigurationContainer configuration);
        void EmbedConfiguration(ScenarioConfigurationContainer configuration);
    }
}
