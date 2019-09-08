using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Processing.IO;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Service.Interface
{
    public interface IScenarioFileService
    {
        void SaveConfiguration(string scenarioConfigurationName, ScenarioConfigurationContainer configuration);
        void EmbedConfiguration(ConfigResources configResource, ScenarioConfigurationContainer configuration);
        ScenarioConfigurationContainer OpenConfiguration(string scenarioConfigurationName);

        IDictionary<string, ScenarioFileHeader> GetScenarioHeaders();
        
        ScenarioFile OpenScenarioFile(string playerName);
        void SaveScenarioFile(ScenarioFile scenarioFile, string playerName);
        void DeleteScenario(string name);
    }
}
