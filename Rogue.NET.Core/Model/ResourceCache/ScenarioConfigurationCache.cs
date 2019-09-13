using Rogue.NET.Common.Utility;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ResourceCache.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;

namespace Rogue.NET.Core.Model.ResourceCache
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioConfigurationCache))]
    public class ScenarioConfigurationCache : IScenarioConfigurationCache
    {
        static IList<ScenarioConfigurationContainer> _embeddedScenarioConfigurations;

        /// <summary>
        /// Statically - preloaded configuration resources
        /// </summary>
        public IEnumerable<ScenarioConfigurationContainer> EmbeddedConfigurations
        {
            get { return _embeddedScenarioConfigurations; }
        }

        public ScenarioConfigurationCache()
        {

        }

        public static void Load()
        {
            _embeddedScenarioConfigurations = new List<ScenarioConfigurationContainer>();

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
        }
    }
}
