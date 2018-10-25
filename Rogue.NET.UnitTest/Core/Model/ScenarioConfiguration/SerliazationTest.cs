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

using OldScenarioConfiguration = Rogue.NET.Model.ScenarioConfiguration;
using NewScenarioConfiguration = Rogue.NET.Core.Model.ScenarioConfiguration.ScenarioConfigurationContainer;


namespace Rogue.NET.UnitTest.Core.Model.ScenarioConfiguration
{
    [TestClass]
    public class SerliazationTest
    {
        IScenarioResourceService _scenarioResourceService;
        IScenarioGenerator _scenarioGenerator;
        ILayoutGenerator _layoutGenerator;
        ISpellGenerator _spellGenerator;
        IContentGenerator _contentGenerator;
        ICharacterGenerator _characterGenerator;
        ISkillSetGenerator _skillSetGenerator;
        IBehaviorGenerator _behaviorGenerator;
        IItemGenerator _itemGenerator;
        IDoodadGenerator _doodadGenerator;
        IAttackAttributeGenerator _attackAttributeGenerator;
        IScenarioMetaDataGenerator _scenarioMetaDataGenerator;
        ITextService _textService;
        IRandomSequenceGenerator _randomSequenceGenerator;

        Mock<IEventAggregator> _eventAggregatorMock;

        [TestInitialize]
        public void Initialize()
        {
            _eventAggregatorMock = new Mock<IEventAggregator>();
            _scenarioResourceService = new ScenarioResourceService();
            _randomSequenceGenerator = new RandomSequenceGenerator(1234);
            _layoutGenerator = new LayoutGenerator(_randomSequenceGenerator);
            _attackAttributeGenerator = new AttackAttributeGenerator(_randomSequenceGenerator);
            _spellGenerator = new SpellGenerator();
            _skillSetGenerator = new SkillSetGenerator(_spellGenerator);
            _behaviorGenerator = new BehaviorGenerator(_spellGenerator);
            _scenarioMetaDataGenerator = new ScenarioMetaDataGenerator();
            _itemGenerator = new ItemGenerator(
                _randomSequenceGenerator,
                _attackAttributeGenerator,
                _spellGenerator,
                _skillSetGenerator);
            _characterGenerator = new CharacterGenerator(
                _randomSequenceGenerator,
                _attackAttributeGenerator,
                _skillSetGenerator,
                _behaviorGenerator,
                _itemGenerator);
            _doodadGenerator = new DoodadGenerator(_spellGenerator);
            _contentGenerator = new ContentGenerator(
                _randomSequenceGenerator,
                _characterGenerator,
                _doodadGenerator,
                _itemGenerator);
            _textService = new TextService();
            _scenarioGenerator = new Rogue.NET.Core.Model.Generator.ScenarioGenerator(
                _eventAggregatorMock.Object,
                _layoutGenerator,
                _contentGenerator,
                _characterGenerator,
                _scenarioMetaDataGenerator,
                _textService);

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

            var scenario = _scenarioGenerator.CreateScenario(configuration, 1234, false);

            Assert.IsNotNull(scenario);
            Assert.IsTrue(scenario.LoadedLevels.Count == configuration.DungeonTemplate.NumberOfLevels);
        }
    }
}
