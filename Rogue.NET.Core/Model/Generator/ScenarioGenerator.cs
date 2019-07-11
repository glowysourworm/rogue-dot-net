using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.ScenarioConfiguration;

using System.ComponentModel.Composition;
using System.Linq;

using Prism.Events;

namespace Rogue.NET.Core.Model.Generator
{
    [Export(typeof(IScenarioGenerator))]
    public class ScenarioGenerator : IScenarioGenerator
    {
        readonly IEventAggregator _eventAggregator;
        readonly ILayoutGenerator _layoutGenerator;
        readonly IContentGenerator _contentGenerator;
        readonly ICharacterGenerator _characterGenerator;
        readonly IReligionGenerator _religionGenerator;
        readonly IAttackAttributeGenerator _attackAttributeGenerator;
        readonly IScenarioMetaDataGenerator _scenarioMetaDataGenerator;
        readonly IRandomSequenceGenerator _randomSequenceGenerator;

        [ImportingConstructor]
        public ScenarioGenerator(
            IEventAggregator eventAggregator,
            ILayoutGenerator layoutGenerator,
            IContentGenerator contentGenerator,
            ICharacterGenerator characterGenerator,
            IReligionGenerator religionGenerator,
            IAttackAttributeGenerator attackAttributeGenerator,
            IScenarioMetaDataGenerator scenarioMetaDataGenerator,
            IRandomSequenceGenerator randomSequenceGenerator)
        {
            _eventAggregator = eventAggregator;
            _layoutGenerator = layoutGenerator;
            _contentGenerator = contentGenerator;
            _characterGenerator = characterGenerator;
            _religionGenerator = religionGenerator;
            _attackAttributeGenerator = attackAttributeGenerator;
            _scenarioMetaDataGenerator = scenarioMetaDataGenerator;
            _randomSequenceGenerator = randomSequenceGenerator;
        }

        public ScenarioContainer CreateScenario(ScenarioConfigurationContainer configuration, string religionName, int seed, bool survivorMode)
        {
            ScenarioContainer scenario = new ScenarioContainer();

            // Reseed the Random number generator
            _randomSequenceGenerator.Reseed(seed);

            // Generate Attack Attributes
            scenario.AttackAttributes = configuration.AttackAttributes.Select(x => _attackAttributeGenerator.GenerateAttackAttribute(x))
                                                                      .ToDictionary(x => x.RogueName, x => x);

            // Generate Religions
            scenario.Religions = configuration.Religions.Select(x => _religionGenerator.GenerateReligion(x, configuration.SkillTemplates))
                                                        .ToDictionary(x => x.RogueName, x => x);

            // Generate Player
            scenario.Player = _characterGenerator.GeneratePlayer(configuration.PlayerTemplate, religionName, scenario.Religions.Values, scenario.AttackAttributes.Values);

            var levels = _layoutGenerator.CreateDungeonLayouts(configuration);

            scenario.LoadedLevels = _contentGenerator.CreateContents(levels, configuration, scenario.Religions.Values, scenario.AttackAttributes.Values, survivorMode).ToList();

            //Load Encyclopedia Rogue-Tanica (Consumables)
            foreach (var template in configuration.ConsumableTemplates)
                scenario.ScenarioEncyclopedia.Add(template.Name, _scenarioMetaDataGenerator.CreateScenarioMetaData(template));

            //Load Encyclopedia Rogue-Tanica (Equipment)
            foreach (var template in configuration.EquipmentTemplates)
                scenario.ScenarioEncyclopedia.Add(template.Name, _scenarioMetaDataGenerator.CreateScenarioMetaData(template));

            //Load Encyclopedia Rogue-Tanica (Enemies)
            foreach (var template in configuration.EnemyTemplates)
                scenario.ScenarioEncyclopedia.Add(template.Name, _scenarioMetaDataGenerator.CreateScenarioMetaData(template));

            //Load Encyclopedia Rogue-Tanica (Doodads)
            foreach (var template in configuration.DoodadTemplates)
                scenario.ScenarioEncyclopedia.Add(template.Name, _scenarioMetaDataGenerator.CreateScenarioMetaData(template));

            //Load Encyclopedia Rogue-Tanica (Skill Sets)
            foreach (var template in configuration.SkillTemplates)
                scenario.ScenarioEncyclopedia.Add(template.Name, _scenarioMetaDataGenerator.CreateScenarioMetaData(template));

            //Load Encyclopedia Rogue-Tanica (Alterations - NOT SHOWN IN UI)
            foreach (var template in configuration.SkillTemplates.SelectMany(x => x.Skills.Select(z => z.Alteration)))
                scenario.ScenarioEncyclopedia.Add(template.Name, _scenarioMetaDataGenerator.CreateScenarioMetaData(template));

            //Load Encyclopedia Rogue-Tanica (Religions)
            foreach (var template in configuration.Religions)
                scenario.ScenarioEncyclopedia.Add(template.Name, _scenarioMetaDataGenerator.CreateScenarioMetaData(template));

            //Load Encyclopedia Rogue-Tanica (Normal Doodads)
            scenario.ScenarioEncyclopedia.Add(ModelConstants.DoodadSavePointRogueName, _scenarioMetaDataGenerator.CreateScenarioMetaData(DoodadNormalType.SavePoint));
            scenario.ScenarioEncyclopedia.Add(ModelConstants.DoodadStairsDownRogueName, _scenarioMetaDataGenerator.CreateScenarioMetaData(DoodadNormalType.StairsDown));
            scenario.ScenarioEncyclopedia.Add(ModelConstants.DoodadStairsUpRogueName, _scenarioMetaDataGenerator.CreateScenarioMetaData(DoodadNormalType.StairsUp));
            scenario.ScenarioEncyclopedia.Add(ModelConstants.DoodadTeleporterARogueName, _scenarioMetaDataGenerator.CreateScenarioMetaData(DoodadNormalType.Teleport1));
            scenario.ScenarioEncyclopedia.Add(ModelConstants.DoodadTeleporterBRogueName, _scenarioMetaDataGenerator.CreateScenarioMetaData(DoodadNormalType.Teleport2));
            scenario.ScenarioEncyclopedia.Add(ModelConstants.DoodadTeleporterRandomRogueName, _scenarioMetaDataGenerator.CreateScenarioMetaData(DoodadNormalType.TeleportRandom));

            //Identify player skills / equipment / consumables / and identified religions
            foreach (var skillSet in scenario.Player.SkillSets)
            {
                scenario.ScenarioEncyclopedia[skillSet.RogueName].IsIdentified = true;

                // TODO:SKILLSET
                // Also setup skill set IsLearned flag based on player level 
                // skillSet.IsLearned = (scenario.Player.Level >= skillSet.LevelLearned);

                // Also setup skill IsLearned flag based on player level
                foreach (var skill in skillSet.Skills)
                    skill.IsLearned = (scenario.Player.Level >= skill.LevelRequirement);
            }

            foreach (var equipment in scenario.Player.Equipment.Values)
            {
                scenario.ScenarioEncyclopedia[equipment.RogueName].IsIdentified = true;
                equipment.IsIdentified = true;
            }

            foreach (var consumable in scenario.Player.Consumables.Values)
            {
                scenario.ScenarioEncyclopedia[consumable.RogueName].IsIdentified = true;
                consumable.IsIdentified = true;
            }

            foreach (var template in configuration.Religions)
            {
                scenario.ScenarioEncyclopedia[template.Name].IsIdentified = template.IsIdentified;
            }

            return scenario;
        }
    }
}
