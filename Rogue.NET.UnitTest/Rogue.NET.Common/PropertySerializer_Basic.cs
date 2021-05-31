
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

            CompareConfiguration(fighterScenario, fighterScenarioResult1);
            CompareConfiguration(fighterScenario, fighterScenarioResult2);
            CompareConfiguration(fighterScenarioResult1, fighterScenarioResult2);
        }

        // ROUGH COMPARISON!
        private void CompareConfiguration(ScenarioConfigurationContainer configuration1, ScenarioConfigurationContainer configuration2)
        {
            Assert.IsTrue(configuration1.AlterationCategories.Count == configuration2.AlterationCategories.Count);
            Assert.IsTrue(configuration1.AlteredCharacterStates.Count == configuration2.AlteredCharacterStates.Count);
            Assert.IsTrue(configuration1.AttackAttributes.Count == configuration2.AttackAttributes.Count);
            Assert.IsTrue(configuration1.ConsumableTemplates.Count == configuration2.ConsumableTemplates.Count);
            Assert.IsTrue(configuration1.DoodadTemplates.Count == configuration2.DoodadTemplates.Count);
            Assert.IsTrue(configuration1.EnemyTemplates.Count == configuration2.EnemyTemplates.Count);
            Assert.IsTrue(configuration1.EquipmentTemplates.Count == configuration2.EquipmentTemplates.Count);
            Assert.IsTrue(configuration1.FriendlyTemplates.Count == configuration2.FriendlyTemplates.Count);
            Assert.IsTrue(configuration1.LayoutTemplates.Count == configuration2.LayoutTemplates.Count);
            Assert.IsTrue(configuration1.PlayerTemplates.Count == configuration2.PlayerTemplates.Count);
            Assert.IsTrue(configuration1.SkillTemplates.Count == configuration2.SkillTemplates.Count);
            Assert.IsTrue(configuration1.TerrainLayers.Count == configuration2.TerrainLayers.Count);
            Assert.IsTrue(configuration1.SkillTemplates.Count == configuration2.SkillTemplates.Count);
            Assert.IsTrue(configuration1.SymbolPool.Count == configuration2.SymbolPool.Count);

            Assert.IsTrue(configuration1.ScenarioDesign.LevelDesigns.Count == configuration2.ScenarioDesign.LevelDesigns.Count);
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