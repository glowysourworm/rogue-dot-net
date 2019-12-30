using Rogue.NET.Core.GameRouter.GameEvent.Backend.Enum;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Processing.Event.Backend.EventData.Factory.Interface;
using Rogue.NET.Core.Processing.Event.Backend.EventData.ScenarioMessage.Enum;
using Rogue.NET.Core.Processing.Model.Content.Calculator.Interface;
using Rogue.NET.Core.Processing.Model.Content.Interface;
using Rogue.NET.Core.Processing.Model.Static;
using Rogue.NET.Core.Processing.Service.Interface;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Content
{
    /// <summary>
    /// Component with modified routines to do things solely for the purpose of debugging and programming
    /// the scenario.
    /// </summary>
    [Export(typeof(IDebugProcessor))]
    public class DebugProcessor : BackendProcessor, IDebugProcessor
    {
        readonly IModelService _modelService;
        readonly IContentProcessor _contentProcessor;
        readonly IScenarioMessageService _scenarioMessageService;
        readonly IPlayerCalculator _playerCalculator;
        readonly IBackendEventDataFactory _backendEventDataFactory;

        [ImportingConstructor]
        public DebugProcessor(
            IModelService modelService,
            IContentProcessor contentProcessor,
            IScenarioMessageService scenarioMessageService,
            IPlayerCalculator playerCalculator,
            IBackendEventDataFactory backendEventDataFactory)
        {
            _modelService = modelService;
            _contentProcessor = contentProcessor;
            _scenarioMessageService = scenarioMessageService;
            _playerCalculator = playerCalculator;
            _backendEventDataFactory = backendEventDataFactory;
        }

        public void GivePlayerExperience()
        {
            var experience = RogueCalculator.CalculateExperienceNext(_modelService.Player.Level);

            _modelService.Player.Experience = experience;

            OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerAll, ""));
        }

        public void IdentifyAll()
        {
            foreach (var item in _modelService.Player.Inventory.Values)
            {
                var metaData = _modelService.ScenarioEncyclopedia[item.RogueName];

                metaData.IsIdentified = true;
                metaData.IsCurseIdentified = true;
                item.IsIdentified = true;

                _scenarioMessageService.Publish(ScenarioMessagePriority.Good, item.RogueName + " Identified");
            }

            OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerAll, ""));
        }

        public void RevealAll()
        {
            // TODO:  Should use a spell to call the alteration processor for this. 
            foreach (var location in _modelService.Level.Grid.FullMap.GetLocations())
                _modelService.Level.Grid[location.Column, location.Row].IsRevealed = true;

            if (_modelService.Level.HasStairsDown())
                _modelService.Level.GetStairsDown().IsRevealed = true;

            if (_modelService.Level.HasStairsUp())
                _modelService.Level.GetStairsUp().IsRevealed = true;

            foreach (var consumable in _modelService.Level.Consumables)
                consumable.IsRevealed = true;

            foreach (var equipment in _modelService.Level.Equipment)
                equipment.IsRevealed = true;

            foreach (var scenarioObject in _modelService.Level.AllContent)
            {
                scenarioObject.IsHidden = false;
                scenarioObject.IsRevealed = true;
            }

            foreach (var consumable in _modelService.Level.Consumables.Where(x => x.SubType == ConsumableSubType.Food))
                consumable.IsRevealed = true;

            OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.ContentAll, ""));
        }

        public void AdvanceToNextLevel()
        {
            OnScenarioEvent(_backendEventDataFactory.LevelChange(_modelService.Level.Parameters.Number + 1, PlayerStartLocation.StairsUp));
        }

        public void SimulateAdvanceToNextLevel()
        {
            var player = _modelService.Player;
            var level = _modelService.Level;

            // *** Simulate Level
            //
            //  0) Calculate Path Length for level
            //  1) Generate Extra Enemies for that path length
            //  2) Defeat all enemies - grant items to player - advance skill learning
            //  3) Grant all items to player in level
            //  4) Advance player to stairs down
            //  5) Generate Hunger

            // Calculate Path Length (TODO)
            var pathLength = 100; //template.GetPathLength();

            // Give all items and experience to the player and 
            // put player at exit
            foreach (var consumable in level.Consumables)
                player.Consumables.Add(consumable.Id, consumable);

            foreach (var equipment in level.Equipment)
                player.Equipment.Add(equipment.Id, equipment);

            for (int i = level.NonPlayerCharacters.Count() - 1; i >= 0; i--)
            {
                var character = level.NonPlayerCharacters.ElementAt(i);

                foreach (var equipment in character.Equipment)
                {
                    // Un-equip item before giving to the player
                    equipment.Value.IsEquipped = false;

                    player.Equipment.Add(equipment.Key, equipment.Value);
                }

                foreach (var consumable in character.Consumables)
                    player.Consumables.Add(consumable.Key, consumable.Value);

                // Calculate player gains
                if (character is Enemy)
                    _playerCalculator.CalculateEnemyDeathGains(_modelService.Player, character as Enemy);

                //Set enemy identified
                _modelService.ScenarioEncyclopedia[character.RogueName].IsIdentified = true;
            }

            // REMOVE ALL CONTENTS
            for (int i = level.Consumables.Count() - 1; i >= 0; i--)
            {
                var consumable = level.Consumables.ElementAt(i);

                level.RemoveContent(consumable.Id);
                OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.ContentRemove, consumable.Id));
            }

            for (int i = level.Equipment.Count() - 1; i >= 0; i--)
            {
                var equipment = level.Equipment.ElementAt(i);

                level.RemoveContent(equipment.Id);
                OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.ContentRemove, equipment.Id));
            }

            for (int i = level.NonPlayerCharacters.Count() - 1; i >= 0; i--)
            {
                var character = level.NonPlayerCharacters.ElementAt(i);

                level.RemoveContent(character.Id);
                OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.ContentRemove, character.Id));
            }

            if (level.HasStairsDown())
                player.Location = level.GetStairsDown().Location;

            // Generate Hunger
            player.Hunger += player.FoodUsagePerTurnBase * pathLength;

            // Queue update: TODO: Clean this up maybe? 
            _modelService.UpdateVisibility();

            OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerAll, ""));
            OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.LayoutAll, ""));
            OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.ContentAll, ""));
        }
    }
}
