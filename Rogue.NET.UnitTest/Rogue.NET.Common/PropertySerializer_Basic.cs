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

using System;
using System.IO;
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

            var fileName = Path.Combine(TestParameters.DebugOutputDirectory, "Fighter." + ResourceConstants.ScenarioConfigurationExtension);
            var manifestFileName = Path.Combine(TestParameters.DebugOutputDirectory, "Fighter." + ResourceConstants.ScenarioConfigurationExtension + ".manifest");
            var fighterScenarioBefore = _scenarioResourceService.GetScenarioConfiguration("Fighter");
            ScenarioConfigurationContainer fighterScenarioAfter = null;

            // Serialize
            using (var memoryStream = new MemoryStream())
            {
                try
                {
                    serializer.Serialize(memoryStream, fighterScenarioBefore);
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }

                // Render the buffer
                var buffer = memoryStream.GetBuffer();

                // Compress
                // buffer = ZipEncoder.Compress(buffer);

                File.WriteAllBytes(fileName, buffer);

                // Validate using manifest
                // var manifest = serializer.GetSerializationManifest();
            }

            // Deserialize
            using (var fileStream = File.OpenRead(fileName))
            {
                try
                {
                    fighterScenarioAfter = serializer.Deserialize(fileStream);
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }

            var compareLogic = new CompareLogic();
            var result = compareLogic.Compare(fighterScenarioAfter, fighterScenarioBefore);

            Assert.IsTrue(result.AreEqual);
        }
    }
}