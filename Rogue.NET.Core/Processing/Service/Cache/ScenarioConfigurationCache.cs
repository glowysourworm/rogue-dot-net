using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Utility;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Processing.Service.Cache.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media;

namespace Rogue.NET.Core.Processing.Service.Cache
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioConfigurationCache))]
    public class ScenarioConfigurationCache : IScenarioConfigurationCache
    {
        readonly static List<ScenarioConfigurationContainer> _embeddedScenarioConfigurations;
        readonly static List<ScenarioConfigurationContainer> _userScenarioConfigurations;

        /// <summary>
        /// Statically - preloaded configuration resources
        /// </summary>
        public IEnumerable<ScenarioConfigurationContainer> EmbeddedConfigurations
        {
            get { return _embeddedScenarioConfigurations; }
        }

        /// <summary>
        /// Created by the user - stored on disk in scenarios folder
        /// </summary>
        public IEnumerable<ScenarioConfigurationContainer> UserConfigurations
        {
            get { return _userScenarioConfigurations; }
        }

        static ScenarioConfigurationCache()
        {
            _embeddedScenarioConfigurations = new List<ScenarioConfigurationContainer>();
            _userScenarioConfigurations = new List<ScenarioConfigurationContainer>();
        }
        public ScenarioConfigurationCache()
        {

        }

        public void SaveConfiguration(ScenarioConfigurationContainer configuration)
        {
            if (!_userScenarioConfigurations.Contains(configuration))
                _userScenarioConfigurations.Add(configuration);

            var file = Path.Combine(ResourceConstants.ScenarioDirectory, configuration.ScenarioDesign.Name) + "." + 
                                    ResourceConstants.ScenarioConfigurationExtension;

            File.WriteAllBytes(file, BinarySerializer.Serialize(configuration));
        }
        public void EmbedConfiguration(ScenarioConfigurationContainer configuration)
        {
            if (!_embeddedScenarioConfigurations.Contains(configuration))
                _embeddedScenarioConfigurations.Add(configuration);

            var file = Path.Combine(ResourceConstants.EmbeddedScenarioDirectory, configuration.ScenarioDesign.Name) + "." +
                                    ResourceConstants.ScenarioConfigurationExtension;

            File.WriteAllBytes(file, BinarySerializer.Serialize(configuration));
        }
        public ScenarioImage GetRandomSmileyCharacter()
        {
            var smileySymbols = _embeddedScenarioConfigurations.Union(_userScenarioConfigurations)
                                                                .SelectMany(x => x.PlayerTemplates)
                                                                .Where(x => x.SymbolDetails.SymbolType == SymbolType.Smiley)
                                                                .Select(x => x.SymbolDetails);

            return smileySymbols.Any() ? new ScenarioImage(smileySymbols.PickRandom()) : new ScenarioImage()
            {
                SymbolType = SymbolType.Smiley,
                SmileyBodyColor = Colors.Yellow.ToString(),
                SmileyLineColor = Colors.Black.ToString(),
                SmileyExpression = SmileyExpression.Happy
            };
        }
        public static void Load()
        {
            // Embedded Scenario Configurations
            _embeddedScenarioConfigurations.Clear();

            foreach (var configResource in Enum.GetValues(typeof(ConfigResources)))
            {
                var name = configResource.ToString();
                var assembly = Assembly.GetAssembly(typeof(ZipEncoder));
                var location = "Rogue.NET.Common.Resource.Configuration." + name.ToString() + "." + ResourceConstants.ScenarioConfigurationExtension;
                using (var stream = assembly.GetManifestResourceStream(location))
                {
                    var memoryStream = new MemoryStream();
                    stream.CopyTo(memoryStream);

                    var configuration = (ScenarioConfigurationContainer)BinarySerializer.Deserialize(memoryStream.GetBuffer());

                    _embeddedScenarioConfigurations.Add(configuration);
                }
            }

            // User Scenario Configurations (On Disk)
            _userScenarioConfigurations.Clear();

            foreach (var file in Directory.GetFiles(ResourceConstants.ScenarioDirectory)
                                          .Where(x => x.EndsWith("." + ResourceConstants.ScenarioConfigurationExtension)))
            {
                _userScenarioConfigurations.Add((ScenarioConfigurationContainer)BinarySerializer.DeserializeFromFile(file));
            }
        }
    }
}
