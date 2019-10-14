using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration;

using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Service.Cache.Interface
{
    public interface IScenarioConfigurationCache
    {
        IEnumerable<string> EmbeddedConfigurations { get; }
        IEnumerable<string> UserConfigurations { get; }
        IEnumerable<ScenarioConfigurationInfo> GetScenarioConfigurationInfos();
        ScenarioImage GetRandomSmileyCharacter(bool eliminateChoice);
        ScenarioConfigurationContainer GetConfiguration(string configurationName);
        void SaveConfiguration(ScenarioConfigurationContainer configuration);
        void EmbedConfiguration(ScenarioConfigurationContainer configuration);
    }
}
