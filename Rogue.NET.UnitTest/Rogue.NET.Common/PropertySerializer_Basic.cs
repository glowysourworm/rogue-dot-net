
using Microsoft.Practices.ServiceLocation;

using NUnit.Framework;

using Rogue.NET.Common.Serialization;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Service.Interface;

using System;
using System.IO;
using System.Linq;

namespace Rogue.NET.UnitTest
{
    public class PropertySerializer_Basic
    {
        IScenarioResourceService _scenarioResourceService;
        IScenarioGenerator _scenarioGenerator;
        ILayoutGenerator _layoutGenerator;

        [SetUp]
        public void Setup()
        {
            TestInitialization.Initialize();

            _scenarioResourceService = ServiceLocator.Current.GetInstance<IScenarioResourceService>();
            _scenarioGenerator = ServiceLocator.Current.GetInstance<IScenarioGenerator>();
            _layoutGenerator = ServiceLocator.Current.GetInstance<ILayoutGenerator>();
        }

        [TearDown]
        public void Teardown()
        {
            TestInitialization.Cleanup();
        }

        [Test]
        public void ScenarioConfigurationSave()
        {
            var fighterScenario = _scenarioResourceService.GetScenarioConfiguration("Fighter");

            ScenarioConfigurationContainer fighterScenarioResult1 = null;
            ScenarioConfigurationContainer fighterScenarioResult2 = null;

            var manifest1 = RunSerializer(fighterScenario, out fighterScenarioResult1);
            var manifest2 = RunSerializer(fighterScenarioResult1, out fighterScenarioResult2);

            CompareConfiguration(fighterScenario, fighterScenarioResult1);
            CompareConfiguration(fighterScenario, fighterScenarioResult2);
            CompareConfiguration(fighterScenarioResult1, fighterScenarioResult2);
        }

        [Test]
        public void ScenarioSave()
        {
            var configuration = _scenarioResourceService.GetScenarioConfiguration("Fighter");
            // var scenario = _scenarioGenerator.CreateScenario(configuration, "Mr. Rogue", configuration.PlayerTemplates.First().Class, 1, false);
            var layout = _layoutGenerator.CreateLayout(configuration.LayoutTemplates.First());
            var serializer = new RecursiveSerializer<ScenarioContainer>();

            LayoutGrid layoutDeserialized = null;

            RunSerializer(layout, out layoutDeserialized);

            var manifest = serializer.CreateManifest();

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

        private SerializationManifest RunSerializer<T>(T theObject, out T theObjectResult)
        {
            var serializer = new RecursiveSerializer<T>();

            theObjectResult = default(T);

            // Serialize
            using (var memoryStream = new MemoryStream())
            {
                try
                {
                    serializer.Serialize(memoryStream, theObject);
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }

                using (var deserializeStream = new MemoryStream(memoryStream.GetBuffer()))
                {
                    try
                    {
                        theObjectResult = serializer.Deserialize(deserializeStream);
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