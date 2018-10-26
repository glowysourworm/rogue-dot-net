using Rogue.NET.Common;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Generator;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Core.Service;
using Rogue.NET.Model;

using ExpressMapper;
using Moq;
using Prism.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;

using OldScenarioConfiguration = Rogue.NET.Model.ScenarioConfiguration;
using NewScenarioConfiguration = Rogue.NET.Core.Model.ScenarioConfiguration.ScenarioConfigurationContainer;
using System.Threading.Tasks;

namespace Rogue.NET.UnitTest.Core.Model.ScenarioConfiguration
{
    [TestClass]
    public class SerliazationTest
    {
        IScenarioResourceService _scenarioResourceService;

        [TestInitialize]
        public void Initialize()
        {
            _scenarioResourceService = new ScenarioResourceService();

            // Mapper configuration to map Rogue.NET.Model -> Rogue.NET.Core
            Mapper.RegisterCustom<Rogue.NET.Model.ProbabilityEquipmentTemplate, Rogue.NET.Core.Model.ScenarioConfiguration.Content.ProbabilityEquipmentTemplate>((src) =>
            {
                return new NET.Core.Model.ScenarioConfiguration.Content.ProbabilityEquipmentTemplate()
                {
                    EquipOnStartup = src.EquipOnStartup,
                    GenerationProbability = src.GenerationProbability,
                    Guid = src.Guid,
                    Name = src.Name,
                    TheTemplate = Mapper.Map<Rogue.NET.Model.EquipmentTemplate, Rogue.NET.Core.Model.ScenarioConfiguration.Content.EquipmentTemplate>((EquipmentTemplate)src.TheTemplate)                    
                };
            });
            Mapper.RegisterCustom<Rogue.NET.Model.ProbabilityConsumableTemplate, Rogue.NET.Core.Model.ScenarioConfiguration.Content.ProbabilityConsumableTemplate>((src) =>
            {
                return new NET.Core.Model.ScenarioConfiguration.Content.ProbabilityConsumableTemplate()
                {
                    GenerationProbability = src.GenerationProbability,
                    Guid = src.Guid,
                    Name = src.Name,
                    TheTemplate = Mapper.Map<Rogue.NET.Model.ConsumableTemplate, Rogue.NET.Core.Model.ScenarioConfiguration.Content.ConsumableTemplate>((ConsumableTemplate)src.TheTemplate)
                };
            });

            Mapper.Compile();
        }

        private IScenarioGenerator CreateScenarioGenerator(int seed)
        {
            var eventAggregatorMock = new Mock<IEventAggregator>();
            var scenarioResourceService = new ScenarioResourceService();
            var randomSequenceGenerator = new RandomSequenceGenerator(seed);
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
            var textService = new TextService();
            var scenarioGenerator = new Rogue.NET.Core.Model.Generator.ScenarioGenerator(
                eventAggregatorMock.Object,
                layoutGenerator,
                contentGenerator,
                characterGenerator,
                scenarioMetaDataGenerator,
                textService);

            return scenarioGenerator;
        }

        [TestMethod]
        public void DeserializeAndMapConfiguration()
        {
            var configurationFromModel = ResourceManager.GetEmbeddedScenarioConfiguration(ConfigResources.Fighter);

            var newScenarioConfiguration = Mapper.Map<OldScenarioConfiguration, NewScenarioConfiguration>(configurationFromModel);

            Assert.IsNotNull(newScenarioConfiguration);

            // TODO - Make this relative...
            _scenarioResourceService.SaveConfig(@"C:\Backup\_Source\Git\rogue-dot-net\Rogue.NET.Core\Resource\Configuration\Fighter.rdns", newScenarioConfiguration);
        }

        [TestMethod]
        public void CreateScenarioFromConfiguration()
        {
            var configuration = _scenarioResourceService.GetEmbeddedScenarioConfiguration(Rogue.NET.Core.Model.Enums.ConfigResources.Fighter);

            Assert.IsNotNull(configuration);

            var scenario = CreateScenarioGenerator(1234).CreateScenario(configuration, false);

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

                var scenario = scenarioGenerator.CreateScenario(configuration, false);

                Assert.IsNotNull(scenario);
                Assert.IsTrue(scenario.LoadedLevels.Count == configuration.DungeonTemplate.NumberOfLevels);
            });
        }
    }
}
