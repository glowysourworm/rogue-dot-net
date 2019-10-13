using Rogue.NET.Common.Utility;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Processing.Service.Interface;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;

namespace Rogue.NET.Core.Processing.Service
{
    [Export(typeof(IScenarioFileService))]
    public class ScenarioFileService : IScenarioFileService
    {
        public ScenarioFileService()
        {
        }

        public void SaveConfiguration(string scenarioConfigurationName, ScenarioConfigurationContainer configuration)
        {
            var file = Path.Combine(ResourceConstants.ScenarioDirectory, scenarioConfigurationName) +
                           "." + ResourceConstants.ScenarioConfigurationExtension;

            File.WriteAllBytes(file, BinarySerializer.Serialize(configuration));
        }
        public void EmbedConfiguration(ConfigResources configResource, ScenarioConfigurationContainer configuration)
        {
            var file = Path.Combine(ResourceConstants.EmbeddedScenarioDirectory, configResource.ToString()) + "." +
                                    ResourceConstants.ScenarioConfigurationExtension;

            File.WriteAllBytes(file, BinarySerializer.Serialize(configuration));
        }
        public ScenarioConfigurationContainer OpenConfiguration(string scenarioConfigurationName)
        {
            var path = Path.Combine(ResourceConstants.ScenarioDirectory, scenarioConfigurationName + "." + ResourceConstants.ScenarioConfigurationExtension);

            return (ScenarioConfigurationContainer)BinarySerializer.Deserialize(File.ReadAllBytes(path));
        }
        public void DeleteScenario(string name)
        {
            var path = Path.Combine(ResourceConstants.SavedGameDirectory, name + "." + ResourceConstants.ScenarioExtension);

            if (File.Exists(path))
                File.Delete(path);
        }

        public IEnumerable<ScenarioContainer> GetScenarios()
        {
            var scenarioFiles = Directory.GetFiles(ResourceConstants.SavedGameDirectory);
            var scenarios = new List<ScenarioContainer>();

            foreach (var file in scenarioFiles)
            {
                scenarios.Add((ScenarioContainer)BinarySerializer.DeserializeFromFile(file));
            }

            return scenarios;
        }

        public ScenarioContainer OpenScenarioFile(string playerName)
        {
            return (ScenarioContainer)BinarySerializer.DeserializeFromFile(ResourceConstants.SavedGameDirectory + "\\" + playerName + "." + ResourceConstants.ScenarioExtension);
        }

        public void SaveScenarioFile(ScenarioContainer scenario, string playerName)
        {
            BinarySerializer.SerializeToFile(ResourceConstants.SavedGameDirectory + "\\" + playerName + "." + ResourceConstants.ScenarioExtension, scenario);
        }
    }
}
