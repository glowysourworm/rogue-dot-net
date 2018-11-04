using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Generator;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Core.Service;
using Rogue.NET.Core.Model.Enums;

using ExpressMapper;
using Moq;
using Prism.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;
using System.Threading.Tasks;

namespace Rogue.NET.UnitTest.Core.Model.ScenarioConfiguration
{
    [TestClass]
    public class SerliazationTest
    {
        IScenarioResourceService _scenarioResourceService;

        Mock<IEventAggregator> _eventAggregatorMock;

        [TestInitialize]
        public void Initialize()
        {
            _eventAggregatorMock = new Mock<IEventAggregator>();
            _scenarioResourceService = new ScenarioResourceService(_eventAggregatorMock.Object);

            Mapper.Compile();
        }

        private IScenarioGenerator CreateScenarioGenerator(int seed)
        {

            var scenarioResourceService = new ScenarioResourceService(_eventAggregatorMock.Object);
            var randomSequenceGenerator = new RandomSequenceGenerator();
            randomSequenceGenerator.Reseed(seed);
            var layoutGenerator = new LayoutGenerator(randomSequenceGenerator);
            var attackAttributeGenerator = new AttackAttributeGenerator(randomSequenceGenerator);
            var spellGenerator = new SpellGenerator();
            var skillSetGenerator = new SkillSetGenerator(spellGenerator);
            var behaviorGenerator = new BehaviorGenerator(spellGenerator);
            var scenarioMetaDataGenerator = new ScenarioMetaDataGenerator();
            var itemGenerator = new ItemGenerator(
                randomSequenceGenerator,
                attackAttributeGenerator,
                spellGenerator,
                skillSetGenerator);
            var characterGenerator = new CharacterGenerator(
                randomSequenceGenerator,
                attackAttributeGenerator,
                skillSetGenerator,
                behaviorGenerator,
                itemGenerator);
            var doodadGenerator = new DoodadGenerator(spellGenerator);
            var contentGenerator = new ContentGenerator(
                randomSequenceGenerator,
                characterGenerator,
                doodadGenerator,
                itemGenerator);
            var scenarioGenerator = new Rogue.NET.Core.Model.Generator.ScenarioGenerator(
                _eventAggregatorMock.Object,
                layoutGenerator,
                contentGenerator,
                characterGenerator,
                scenarioMetaDataGenerator,
                randomSequenceGenerator);

            return scenarioGenerator;
        }

        [TestMethod]
        public void DeserializeConfiguration()
        {
            var configurationFromModel = _scenarioResourceService.GetEmbeddedScenarioConfiguration(ConfigResources.Fighter);

            Assert.IsNotNull(configurationFromModel);
        }

        [TestMethod]
        public void CreateScenarioFromConfiguration()
        {
            var configuration = _scenarioResourceService.GetEmbeddedScenarioConfiguration(Rogue.NET.Core.Model.Enums.ConfigResources.Fighter);

            Assert.IsNotNull(configuration);

            var scenario = CreateScenarioGenerator(1234).CreateScenario(configuration, 1234, false);

            Assert.IsNotNull(scenario);
            Assert.IsTrue(scenario.LoadedLevels.Count == configuration.DungeonTemplate.NumberOfLevels);
        }
        [TestMethod]
        public void Create10000ScenarioFromConfiguration()
        {
            var configuration = _scenarioResourceService.GetEmbeddedScenarioConfiguration(Rogue.NET.Core.Model.Enums.ConfigResources.Fighter);

            Assert.IsNotNull(configuration);

            Parallel.ForEach<int>(Enumerable.Range(1, 100), (seed) =>
            {
                var scenarioGenerator = CreateScenarioGenerator(seed);

                var scenario = scenarioGenerator.CreateScenario(configuration, seed, false);

                Assert.IsNotNull(scenario);
                Assert.IsTrue(scenario.LoadedLevels.Count == configuration.DungeonTemplate.NumberOfLevels);
            });
        }
    }
}
