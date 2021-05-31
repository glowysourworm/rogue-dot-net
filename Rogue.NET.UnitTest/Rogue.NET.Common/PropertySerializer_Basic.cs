
using KellermanSoftware.CompareNetObjects;

using Moq;

using NUnit.Framework;

using Rogue.NET.Common.Serialization;
using Rogue.NET.Common.Utility;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Processing.Service;
using Rogue.NET.Core.Processing.Service.Cache;
using Rogue.NET.Core.Processing.Service.Cache.Interface;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.UnitTest.Extension;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Serialization;

namespace Rogue.NET.UnitTest
{
    public class PropertySerializer_Basic
    {
        IScenarioResourceService _scenarioResourceService;

        [SetUp]
        public void Setup()
        {
            TestInitialization.Initialize();

            var scenarioConfigurationCache = new ScenarioConfigurationCache();
            var scenarioCache = new Mock<IScenarioCache>();
            var svgCache = new Mock<ISvgCache>();
            var scenarioImageSourceFactory = new Mock<IScenarioImageSourceFactory>();

            // MOCK except for configuration cache
            _scenarioResourceService = new ScenarioResourceService(scenarioConfigurationCache, scenarioCache.Object, svgCache.Object, scenarioImageSourceFactory.Object);

            // Load configurations from embedded resources
            ScenarioConfigurationCache.Load();
        }

        [TearDown]
        public void Teardown()
        {
            TestInitialization.Cleanup();
        }

        [Test]
        public void ScenarioConfigurationSave()
        {
            var serializer = new RecursiveSerializer<ScenarioConfigurationContainer>();

            var manifestFileName = Path.Combine(TestParameters.DebugOutputDirectory, "Fighter." + ResourceConstants.ScenarioConfigurationExtension + ".manifest.xml");
            var fighterScenario = _scenarioResourceService.GetScenarioConfiguration("Fighter");

            ScenarioConfigurationContainer fighterScenarioResult1 = null;
            ScenarioConfigurationContainer fighterScenarioResult2 = null;

            var manifest1 = RunSerializer(fighterScenario, out fighterScenarioResult1);
            var manifest2 = RunSerializer(fighterScenarioResult1, out fighterScenarioResult2);
        }

        private SerializationManifest RunSerializer(ScenarioConfigurationContainer configuration, out ScenarioConfigurationContainer configurationResult)
        {
            var serializer = new RecursiveSerializer<ScenarioConfigurationContainer>();

            configurationResult = null;

            // Serialize
            using (var memoryStream = new MemoryStream())
            {
                try
                {
                    serializer.Serialize(memoryStream, configuration);
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }

                using (var deserializeStream = new MemoryStream(memoryStream.GetBuffer()))
                {
                    try
                    {
                        configurationResult = serializer.Deserialize(deserializeStream);
                    }
                    catch (Exception ex)
                    {
                        Assert.Fail(ex.Message);
                    }
                }

                return serializer.CreateManifest();
            }
        }
    }
}