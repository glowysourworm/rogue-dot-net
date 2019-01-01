using Antmicro.Migrant;
using Antmicro.Migrant.Customization;
using Rogue.NET.Common.Utility;
using Rogue.NET.Core.IO;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Service.Interface;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;

namespace Rogue.NET.Core.Service
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

            if (File.Exists(file))
                File.Delete(file);

            using (var stream = File.OpenWrite(file))
            {
                var serializer = new Serializer(
                    new Settings(Method.Generated,
                                 Method.Generated,
                                 VersionToleranceLevel.AllowFieldRemoval | VersionToleranceLevel.AllowFieldAddition | VersionToleranceLevel.AllowInheritanceChainChange,
                                 false, false,
                                 true, true, false,
                                 ReferencePreservation.Preserve));

                serializer.Serialize(configuration, stream);
            }
        }
        public void EmbedConfiguration(ConfigResources configResource, ScenarioConfigurationContainer configuration)
        {
            var file = Path.Combine(ResourceConstants.EmbeddedScenarioDirectory, configResource.ToString()) + "." + 
                                    ResourceConstants.ScenarioConfigurationExtension;

            if (File.Exists(file))
                File.Delete(file);

            using (var stream = File.OpenWrite(file))
            {
                var serializer = new Serializer(
                    new Settings(Method.Generated,
                                 Method.Generated,
                                 VersionToleranceLevel.AllowFieldRemoval | VersionToleranceLevel.AllowFieldAddition | VersionToleranceLevel.AllowInheritanceChainChange,
                                 false, false,
                                 true, true, false,
                                 ReferencePreservation.Preserve));

                serializer.Serialize(configuration, stream);
            }
        }
        public ScenarioConfigurationContainer OpenConfiguration(string scenarioConfigurationName)
        {
            var path = Path.Combine(ResourceConstants.ScenarioDirectory, scenarioConfigurationName + "." + ResourceConstants.ScenarioConfigurationExtension);

            using (var stream = File.OpenRead(path))
            {
                var serializer = new Serializer(
                    new Settings(Method.Generated,
                                 Method.Generated,
                                 VersionToleranceLevel.AllowFieldRemoval | VersionToleranceLevel.AllowFieldAddition | VersionToleranceLevel.AllowInheritanceChainChange,
                                 false, false,
                                 true, true, false,
                                 ReferencePreservation.Preserve));

                return serializer.Deserialize<ScenarioConfigurationContainer>(stream);
            }
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
