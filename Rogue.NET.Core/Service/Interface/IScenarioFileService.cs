using Rogue.NET.Core.IO;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Service.Interface
{
    public interface IScenarioFileService
    {
        void SaveConfiguration(string scenarioConfigurationName, ScenarioConfigurationContainer configuration);
        void EmbedConfiguration(ConfigResources configResource, ScenarioConfigurationContainer configuration);
        ScenarioConfigurationContainer OpenConfiguration(string scenarioConfigurationFile);

        IDictionary<string, ScenarioFileHeader> GetScenarioHeaders();
        
        ScenarioFile OpenScenarioFile(string playerName);
        void SaveScenarioFile(ScenarioFile scenarioFile, string playerName);
        void DeleteScenario(string name);
    }
}
