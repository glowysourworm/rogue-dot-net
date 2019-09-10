﻿using Rogue.NET.Common.Utility;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Processing.IO;
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
        public IDictionary<string, ScenarioFileHeader> GetScenarioHeaders()
        {
            var scenarioFiles = Directory.GetFiles(ResourceConstants.SavedGameDirectory);
            var scenarioHeaders = new Dictionary<string, ScenarioFileHeader>();
            foreach (var file in scenarioFiles)
            {
                var header = ScenarioFile.OpenHeader(File.ReadAllBytes(file));
                var name = Path.GetFileNameWithoutExtension(file);
                if (header != null)
                    scenarioHeaders.Add(name, header);
            }

            return scenarioHeaders;
        }
        public void SaveScenarioFile(ScenarioFile scenarioFile, string playerName)
        {
            var buffer = scenarioFile.Save();

            File.WriteAllBytes(ResourceConstants.SavedGameDirectory + "\\" + playerName + "." + ResourceConstants.ScenarioExtension, buffer);
        }
        public ScenarioFile OpenScenarioFile(string playerName)
        {
            var buffer = File.ReadAllBytes(ResourceConstants.SavedGameDirectory + "\\" + playerName + "." + ResourceConstants.ScenarioExtension);

            return ScenarioFile.Open(buffer);
        }
        public void DeleteScenario(string name)
        {
            var path = Path.Combine(ResourceConstants.SavedGameDirectory, name + "." + ResourceConstants.ScenarioExtension);

            if (File.Exists(path))
                File.Delete(path);
        }
    }
}