using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using Rogue.NET.Core.Service.Interface;

using System;
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
        readonly IScenarioMetaDataGenerator _scenarioMetaDataGenerator;
        readonly ITextService _textService;

        public ScenarioGenerator(
            IEventAggregator eventAggregator,
            ILayoutGenerator layoutGenerator,
            IContentGenerator contentGenerator,
            ICharacterGenerator characterGenerator,
            IScenarioMetaDataGenerator scenarioMetaDataGenerator,
            ITextService textService)
        {
            _eventAggregator = eventAggregator;
            _layoutGenerator = layoutGenerator;
            _contentGenerator = contentGenerator;
            _characterGenerator = characterGenerator;
            _scenarioMetaDataGenerator = scenarioMetaDataGenerator;
            _textService = textService;
        }

        public ScenarioContainer CreateScenario(ScenarioConfigurationContainer configuration, bool survivorMode)
        {
            ScenarioContainer scenario = new ScenarioContainer();

            //Generate Dungeon
            scenario.Player1 = _characterGenerator.GeneratePlayer(configuration.PlayerTemplate);
            scenario.Player1.AttributeEmphasis = AttributeEmphasis.Agility;

            var levels = _layoutGenerator.CreateDungeonLayouts(configuration);

            scenario.LoadedLevels = _contentGenerator.CreateContents(levels, configuration, survivorMode).ToList();

            // TODO
            //PublishLoadingMessage("Initializing Scenario Meta Data", 99);

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

            //Load Encyclopedia Rogue-Tanica (Normal Doodads)
            foreach (var type in Enum.GetValues(typeof(DoodadNormalType)).Cast<DoodadNormalType>())
                scenario.ScenarioEncyclopedia.Add(_textService.CamelCaseToTitleCase(type.ToString()), _scenarioMetaDataGenerator.CreateScenarioMetaData(type));

            //progressUpdate("Adding 'other' items...", 99);

            //Identify player skills / equipment / consumables
            foreach (var skillSet in scenario.Player1.SkillSets)
            {
                if (skillSet.LevelLearned <= scenario.Player1.Level)
                {
                    scenario.ScenarioEncyclopedia[skillSet.RogueName].IsIdentified = true;
                }
            }

            foreach (var equipment in scenario.Player1.Equipment.Values)
            {
                scenario.ScenarioEncyclopedia[equipment.RogueName].IsIdentified = true;
                equipment.IsIdentified = true;
            }

            foreach (var consumable in scenario.Player1.Consumables.Values)
            {
                scenario.ScenarioEncyclopedia[consumable.RogueName].IsIdentified = true;
                consumable.IsIdentified = true;
            }
            return scenario;
        }
        public ScenarioContainer CreateDebugScenario(ScenarioConfigurationContainer configuration)
        {
            var layoutTemplate = new LayoutTemplate();
            layoutTemplate.GenerationRate = 1;
            layoutTemplate.HiddenDoorProbability = 0;
            layoutTemplate.Level = new Range<int>(0, 0, 100, 100);
            layoutTemplate.Name = "Debug Level";
            layoutTemplate.NumberRoomCols = 3;
            layoutTemplate.NumberRoomRows = 3;
            layoutTemplate.RoomDivCellHeight = 20;
            layoutTemplate.RoomDivCellWidth = 20;
            layoutTemplate.Type = LayoutType.Normal;

            configuration.DungeonTemplate.LayoutTemplates.Clear();
            configuration.DungeonTemplate.LayoutTemplates.Add(layoutTemplate);

            foreach (var template in configuration.ConsumableTemplates)
            {
                template.Level = new Range<int>(0, 0, 100, 100);
                template.GenerationRate = 3;
            }
            foreach (var template in configuration.EquipmentTemplates)
            {
                template.Level = new Range<int>(0, 0, 100, 100);
                template.GenerationRate = 3;
            }
            foreach (var template in configuration.EnemyTemplates)
            {
                template.Level = new Range<int>(0, 0, 100, 100);
                template.GenerationRate = 3;
            }
            foreach (var template in configuration.DoodadTemplates)
            {
                template.Level = new Range<int>(0, 0, 100, 100);
                template.GenerationRate = 3;
            }

            var identifyScroll = configuration.ConsumableTemplates.First(z => z.Name.Contains("Identify"));
            for (int i = 0; i < 20; i++)
                configuration.PlayerTemplate.StartingConsumables.Add(
                    new ProbabilityConsumableTemplate()
                    {
                        TheTemplate = identifyScroll,
                        GenerationProbability = 1
                    });

            return CreateScenario(configuration, false);
        }
    }
}
