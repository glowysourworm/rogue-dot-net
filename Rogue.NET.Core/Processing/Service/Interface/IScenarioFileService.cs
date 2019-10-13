using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Service.Interface
{
    public interface IScenarioFileService
    {
        void SaveConfiguration(string scenarioConfigurationName, ScenarioConfigurationContainer configuration);
        void EmbedConfiguration(ConfigResources configResource, ScenarioConfigurationContainer configuration);
        ScenarioConfigurationContainer OpenConfiguration(string scenarioConfigurationName);

        IEnumerable<ScenarioContainer> GetScenarios();
        
        ScenarioContainer OpenScenarioFile(string playerName);

        void SaveScenarioFile(ScenarioContainer scenario, string playerName);

        void DeleteScenario(string name);
    }
}
