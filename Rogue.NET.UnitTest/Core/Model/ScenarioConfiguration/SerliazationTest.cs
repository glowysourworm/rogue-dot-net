using Rogue.NET.Core.Model.Enums;

using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;
using System.Threading.Tasks;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.Core.Processing.Service;
using Rogue.NET.Core.Processing.Model.Generator;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Model.ResourceCache.Interface;

namespace Rogue.NET.UnitTest.Core.Model.ScenarioConfiguration
{
    [TestClass]
    public class SerliazationTest
    {
        IScenarioResourceService _scenarioResourceService;

        Mock<IScenarioFileService> _scenarioFileServiceMock;
        Mock<IScenarioConfigurationCache> _scenarioConfigurationCacheMock;
        Mock<IRogueEventAggregator> _eventAggregatorMock;

        [TestInitialize]
        public void Initialize()
        {
            _eventAggregatorMock = new Mock<IRogueEventAggregator>();
            _scenarioFileServiceMock = new Mock<IScenarioFileService>();
            _scenarioConfigurationCacheMock = new Mock<IScenarioConfigurationCache>();
            _scenarioResourceService = new ScenarioResourceService(_scenarioFileServiceMock.Object, _scenarioConfigurationCacheMock.Object);
        }

        private IScenarioGenerator CreateScenarioGenerator(int seed)
        {

            var animationGenerator = new AnimationGenerator();
            var scenarioResourceService = new ScenarioResourceService(_scenarioFileServiceMock.Object, _scenarioConfigurationCacheMock.Object);
            var randomSequenceGenerator = new RandomSequenceGenerator();
            randomSequenceGenerator.Reseed(seed);
            var layoutGenerator = new LayoutGenerator(randomSequenceGenerator);
            var attackAttributeGenerator = new AttackAttributeGenerator(randomSequenceGenerator);
            var skillGenerator = new SkillGenerator();
            var skillSetGenerator = new SkillSetGenerator(skillGenerator);
            var behaviorGenerator = new BehaviorGenerator();
            var scenarioMetaDataGenerator = new ScenarioMetaDataGenerator();
            var alteredStateGenerator = new AlteredStateGenerator();
            var itemGenerator = new ItemGenerator(
                randomSequenceGenerator,
                attackAttributeGenerator,
                animationGenerator,
                skillSetGenerator);
            var alterationGenerator = new AlterationGenerator(randomSequenceGenerator, attackAttributeGenerator, alteredStateGenerator, animationGenerator, itemGenerator);
            var characterGenerator = new CharacterGenerator(
                randomSequenceGenerator,
                attackAttributeGenerator,
                skillSetGenerator,
                behaviorGenerator,
                itemGenerator,
                animationGenerator,
                alterationGenerator);
            var doodadGenerator = new DoodadGenerator();
            var contentGenerator = new ContentGenerator(
                randomSequenceGenerator,
                characterGenerator,
                doodadGenerator,
                itemGenerator);
            var scenarioGenerator = new Rogue.NET.Core.Processing.Model.Generator.ScenarioGenerator(
                _eventAggregatorMock.Object,
                layoutGenerator,
                contentGenerator,
                characterGenerator,
                attackAttributeGenerator,
                scenarioMetaDataGenerator,
                randomSequenceGenerator);

            return scenarioGenerator;
        }

        [TestMethod]
        public void DeserializeConfiguration()
        {
            var configurationFromModel = _scenarioResourceService.GetScenarioConfiguration(ConfigResources.Fighter);

            Assert.IsNotNull(configurationFromModel);
        }

        [TestMethod]
        public void CreateScenarioFromConfiguration()
        {
            var configuration = _scenarioResourceService.GetScenarioConfiguration(Rogue.NET.Core.Model.Enums.ConfigResources.Fighter);

            Assert.IsNotNull(configuration);

            var scenario = CreateScenarioGenerator(1234).CreateScenario(configuration, "", 1234, false);

            Assert.IsNotNull(scenario);
            Assert.IsTrue(scenario.LoadedLevels.Count == configuration.DungeonTemplate.NumberOfLevels);
        }
        [TestMethod]
        public void Create10000ScenarioFromConfiguration()
        {
            var configuration = _scenarioResourceService.GetScenarioConfiguration(Rogue.NET.Core.Model.Enums.ConfigResources.Fighter);

            Assert.IsNotNull(configuration);

            Parallel.ForEach<int>(Enumerable.Range(1, 100), (seed) =>
            {
                var scenarioGenerator = CreateScenarioGenerator(seed);

                var scenario = scenarioGenerator.CreateScenario(configuration, "", seed, false);

                Assert.IsNotNull(scenario);
                Assert.IsTrue(scenario.LoadedLevels.Count == configuration.DungeonTemplate.NumberOfLevels);
            });
        }
    }
}
