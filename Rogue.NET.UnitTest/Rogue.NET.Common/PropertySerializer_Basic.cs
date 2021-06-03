
using KellermanSoftware.CompareNetObjects;

using Microsoft.Practices.ServiceLocation;

using NUnit.Framework;

using Rogue.NET.Common.Serialization;
using Rogue.NET.Common.Serialization.Manifest;
using Rogue.NET.Common.Utility;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Service.Interface;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

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
            var scenario = _scenarioResourceService.GetScenarioConfiguration(ConfigResources.Adventurer.ToString());

            ScenarioConfigurationContainer scenarioResult1 = null;
            ScenarioConfigurationContainer scenarioResult2 = null;

            var manifest1 = RunSerializer(scenario, out scenarioResult1);
            var manifest2 = RunSerializer(scenarioResult1, out scenarioResult2);

            var comparison = new CompareLogic(new ComparisonConfig()
            {
                MaxDifferences = int.MaxValue
            });

            var result1 = comparison.Compare(manifest1.SerializerOutput, manifest1.DeserializerOutput);
            var result2 = comparison.Compare(manifest2.SerializerOutput, manifest2.DeserializerOutput);

            Assert.IsTrue(result1.AreEqual);
            Assert.IsTrue(result2.AreEqual);

            CompareConfiguration(scenario, scenarioResult1);
            CompareConfiguration(scenario, scenarioResult2);
            CompareConfiguration(scenarioResult1, scenarioResult2);
        }

        [Test]
        public void LayoutSave()
        {
            var configuration = _scenarioResourceService.GetScenarioConfiguration(ConfigResources.Adventurer.ToString());
            var layout = _layoutGenerator.CreateLayout(configuration.LayoutTemplates.First());

            LayoutGrid layoutDeserialized1 = null;
            LayoutGrid layoutDeserialized2 = null;

            RunSerializer(layout, out layoutDeserialized1);
            RunSerializer(layoutDeserialized1, out layoutDeserialized2);

            CompareLayoutGrid(layout, layoutDeserialized1);
            CompareLayoutGrid(layoutDeserialized1, layoutDeserialized2);
            CompareLayoutGrid(layoutDeserialized2, layout);
        }


        [Test]
        public void ScenarioSave()
        {
            var configuration = _scenarioResourceService.GetScenarioConfiguration(ConfigResources.Adventurer.ToString());
            var scenario = _scenarioGenerator.CreateScenario(configuration, "Test Scenario", configuration.PlayerTemplates.First().Name, 1, false);

            ScenarioContainer scenarioDeserialized1 = null;
            ScenarioContainer scenarioDeserialized2 = null;

            RunSerializer(scenario, out scenarioDeserialized1);
            RunSerializer(scenarioDeserialized1, out scenarioDeserialized2);
        }

        // ROUGH COMPARISON!
        private void CompareLayoutGrid(LayoutGrid grid1, LayoutGrid grid2)
        {
            Assert.IsTrue(grid1.Bounds.Equals(grid2.Bounds));

            CompareLayerMap(grid1.ConnectionMap, grid2.ConnectionMap);
            CompareLayerMap(grid1.CorridorMap, grid2.CorridorMap);
            CompareLayerMap(grid1.FullNoTerrainSupportMap, grid2.FullNoTerrainSupportMap);
            CompareLayerMap(grid1.PlacementMap, grid2.PlacementMap);
            CompareLayerMap(grid1.RoomMap, grid2.RoomMap);
            CompareLayerMap(grid1.TerrainSupportMap, grid2.TerrainSupportMap);
            CompareLayerMap(grid1.WalkableMap, grid2.WalkableMap);
            CompareLayerMap(grid1.WallMap, grid2.WallMap);

            foreach (var terrainMap1 in grid1.TerrainMaps)
            {
                var terrainMap2 = grid2.TerrainMaps.FirstOrDefault(map => map.Name == terrainMap1.Name);

                CompareLayerMap(terrainMap1, terrainMap2);
            }
        }
        private void CompareLayerMap(ILayerMap map1, ILayerMap map2)
        {
            Assert.IsTrue(map1.Boundary.Equals(map2.Boundary));
            Assert.IsTrue(map1.Name.Equals(map2.Name));
            Assert.IsTrue(map1.ParentBoundary.Equals(map2.ParentBoundary));

            foreach (var region1 in map1.Regions)
            {
                var region2 = map2.Regions.FirstOrDefault(region => region.Id == region1.Id);

                Assert.IsNotNull(region2);
                Assert.AreEqual(region1.GetHashCode(), region2.GetHashCode());
            }
        }
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
                    OutputManifestDiff(serializer.CreateDifferenceList(), "serialize-diff");

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
                        OutputManifestDiff(serializer.CreateDifferenceList(), "deserialize-diff");

                        Assert.Fail(ex.Message);
                    }
                }

                return serializer.CreateManifest();
            }
        }

        private void OutputManifestDiff(List<SerializedNodeDifference> diff, string name)
        {
            var fileName = Path.Combine(ResourceConstants.GetPath(ResourceConstants.ResourcePaths.DebugOutputDirectory), name + ".xml");

            using (var stream = File.OpenWrite(fileName))
            {
                var serializer = new XmlSerializer(typeof(List<SerializedNodeDifference>));

                serializer.Serialize(stream, diff);
            }
        }

        private void OutputManifest(SerializationManifest manifest, string name)
        {
            var fileName = Path.Combine(ResourceConstants.GetPath(ResourceConstants.ResourcePaths.DebugOutputDirectory), name + ".xml");

            using (var stream = File.OpenWrite(fileName))
            {
                var serializer = new XmlSerializer(typeof(SerializationManifest));

                serializer.Serialize(stream, manifest);
            }
        }
    }
}