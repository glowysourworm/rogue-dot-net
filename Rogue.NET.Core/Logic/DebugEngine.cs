using Rogue.NET.Core.Logic.Content.Interface;
using Rogue.NET.Core.Logic.Interface;
using Rogue.NET.Core.Logic.Processing;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing.Factory.Interface;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioMessage;
using Rogue.NET.Core.Service.Interface;
using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Logic
{
    /// <summary>
    /// Component with modified routines to do things solely for the purpose of debugging and programming
    /// the scenario.
    /// </summary>
    [Export(typeof(IDebugEngine))]
    public class DebugEngine : IDebugEngine
    {
        readonly IModelService _modelService;
        readonly IContentEngine _contentEngine;
        readonly IScenarioMessageService _scenarioMessageService;
        readonly IPlayerProcessor _playerProcessor;
        readonly IRogueUpdateFactory _rogueUpdateFactory;

        [ImportingConstructor]
        public DebugEngine(
            IModelService modelService, 
            IContentEngine contentEngine, 
            IScenarioMessageService scenarioMessageService,
            IPlayerProcessor playerProcessor,
            IRogueUpdateFactory rogueUpdateFactory)
        {
            _modelService = modelService;
            _contentEngine = contentEngine;
            _scenarioMessageService = scenarioMessageService;
            _playerProcessor = playerProcessor;
            _rogueUpdateFactory = rogueUpdateFactory;
        }

        public event EventHandler<RogueUpdateEventArgs> RogueUpdateEvent;
        public event EventHandler<ILevelProcessingAction> LevelProcessingActionEvent;

        public void ApplyEndOfTurn()
        {
            throw new NotImplementedException();
        }

        public void GivePlayerExperience()
        {
            _modelService.Player.Experience += 10000;

            RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerAll, ""));
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

            RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerAll, ""));
        }

        public void RevealAll()
        {
            // TODO:  Should use a spell to call the spell engine for this. 
            foreach (var cell in _modelService.Level.Grid.GetCells())
                cell.IsRevealed = true;

            if (_modelService.Level.HasStairsDown)
                _modelService.Level.StairsDown.IsRevealed = true;

            if (_modelService.Level.HasStairsUp)
                _modelService.Level.StairsUp.IsRevealed = true;

            foreach (var consumable in _modelService.Level.Consumables)
                consumable.IsRevealed = true;

            foreach (var equipment in _modelService.Level.Equipment)
                equipment.IsRevealed = true;

            foreach (var scenarioObject in _modelService.Level.GetContents())
            {
                scenarioObject.IsHidden = false;
                scenarioObject.IsRevealed = true;
            }

            foreach (var consumable in _modelService.Level.Consumables.Where(x => x.SubType == ConsumableSubType.Food))
                consumable.IsRevealed = true;

            _modelService.UpdateVisibleLocations();
            _modelService.UpdateContents();
        }

        public void AdvanceToNextLevel()
        {
            RogueUpdateEvent(this, _rogueUpdateFactory.LevelChange(_modelService.Level.Number + 1, PlayerStartLocation.StairsUp));
        }

        public void SimulateAdvanceToNextLevel()
        {
            var player = _modelService.Player;
            var level = _modelService.Level;
            var template = _modelService.ScenarioConfiguration.DungeonTemplate.LayoutTemplates.First(x => x.Name == level.LayoutName);

            // *** Simulate Level
            //
            //  0) Calculate Path Length for level
            //  1) Generate Extra Enemies for that path length
            //  2) Defeat all enemies - grant items to player - advance skill learning
            //  3) Grant all items to player in level
            //  4) Advance player to stairs down
            //  5) Generate Hunger

            // Calculate Path Length
            var pathLength = template.GetPathLength();

            // Extra Enemies - for each step in the path length apply end of turn
            for (int i=0;i<pathLength;i++)
                _contentEngine.ApplyEndOfTurn();

            // Give all items and experience to the player and 
            // put player at exit
            foreach (var consumable in level.Consumables)
                player.Consumables.Add(consumable.Id, consumable);

            foreach (var equipment in level.Equipment)
                player.Equipment.Add(equipment.Id, equipment);

            for (int i = level.Enemies.Count() - 1; i >= 0; i--)
            {
                var enemy = level.Enemies.ElementAt(i);

                foreach (var equipment in enemy.Equipment)
                {
                    // Un-equip item before giving to the player
                    equipment.Value.IsEquipped = false;

                    player.Equipment.Add(equipment.Key, equipment.Value);
                }

                foreach (var consumable in enemy.Consumables)
                    player.Consumables.Add(consumable.Key, consumable.Value);

                // Calculate player gains
                _playerProcessor.CalculateEnemyDeathGains(_modelService.Player, enemy);

                //Set enemy identified
                _modelService.ScenarioEncyclopedia[enemy.RogueName].IsIdentified = true;
            }

            // REMOVE ALL CONTENTS
            for (int i = level.Consumables.Count() - 1; i >= 0; i--)
            {
                var consumable = level.Consumables.ElementAt(i);

                level.RemoveContent(consumable);
                RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.ContentRemove, consumable.Id));
            }

            for (int i = level.Equipment.Count() - 1; i >= 0; i--)
            {
                var equipment = level.Equipment.ElementAt(i);

                level.RemoveContent(equipment);
                RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.ContentRemove, equipment.Id));
            }

            for (int i = level.Enemies.Count() - 1; i >= 0; i--)
            {
                var enemy = level.Enemies.ElementAt(i);

                level.RemoveContent(enemy);
                RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.ContentRemove, enemy.Id));
            }

            if (level.HasStairsDown)
                player.Location = level.StairsDown.Location;

            // Generate Hunger
            player.Hunger += player.FoodUsagePerTurnBase * pathLength;

            // Queue update: TODO: Clean this up maybe? 
            _modelService.UpdateVisibleLocations();
            _modelService.UpdateContents();

            RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerAll, ""));
            RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.LayoutAll, ""));
            RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.ContentAll, ""));
        }
    }
}
