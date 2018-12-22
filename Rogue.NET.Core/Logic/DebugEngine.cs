using Rogue.NET.Core.Logic.Content.Interface;
using Rogue.NET.Core.Logic.Interface;
using Rogue.NET.Core.Logic.Processing;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.ScenarioMessage;
using Rogue.NET.Core.Service.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Logic
{
    [Export(typeof(IDebugEngine))]
    public class DebugEngine : IDebugEngine
    {
        readonly IModelService _modelService;
        readonly IContentEngine _contentEngine;
        readonly IScenarioMessageService _scenarioMessageService;
        readonly IPlayerProcessor _playerProcessor;

        [ImportingConstructor]
        public DebugEngine(
            IModelService modelService, 
            IContentEngine contentEngine, 
            IScenarioMessageService scenarioMessageService,
            IPlayerProcessor playerProcessor)
        {
            _modelService = modelService;
            _contentEngine = contentEngine;           
            _scenarioMessageService = scenarioMessageService;
            _playerProcessor = playerProcessor;
        }

        public event EventHandler<IScenarioUpdate> ScenarioUpdateEvent;
        public event EventHandler<ISplashUpdate> SplashUpdateEvent;
        public event EventHandler<IDialogUpdate> DialogUpdateEvent;
        public event EventHandler<ILevelUpdate> LevelUpdateEvent;
        public event EventHandler<IAnimationUpdate> AnimationUpdateEvent;
        public event EventHandler<ILevelProcessingAction> LevelProcessingActionEvent;

        public void ApplyEndOfTurn()
        {
            throw new NotImplementedException();
        }

        public void GivePlayerExperience()
        {
            _modelService.Player.Experience += 10000;

            LevelUpdateEvent(this, new LevelUpdate() { LevelUpdateType = LevelUpdateType.PlayerAll });
        }

        public void GivePlayerSkillExperience()
        {
            foreach (var skillSet in _modelService.Player.SkillSets)
            {
                if (skillSet.Level < _modelService.Player.SkillSets.Count)
                    skillSet.Level++;
            }

            LevelUpdateEvent(this, new LevelUpdate() { LevelUpdateType = LevelUpdateType.PlayerAll });
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
                

            LevelUpdateEvent(this, new LevelUpdate() { LevelUpdateType = LevelUpdateType.PlayerAll });
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

            foreach (var enemy in level.Enemies)
            {
                foreach (var equipment in enemy.Equipment)
                {
                    // Un-equip item before giving to the player
                    equipment.Value.IsEquipped = false;

                    player.Equipment.Add(equipment.Key, equipment.Value);
                }

                foreach (var consumable in enemy.Consumables)
                    player.Consumables.Add(consumable.Key, consumable.Value);

                // Calculate player gains
                _playerProcessor.CalculateEnemyDeathGains(player, enemy);
            }

            for (int i = level.Consumables.Count() - 1; i >= 0; i--)
                level.RemoveContent(level.Consumables.ElementAt(i));

            for (int i = level.Equipment.Count() - 1; i >= 0; i--)
                level.RemoveContent(level.Equipment.ElementAt(i));

            for (int i = level.Enemies.Count() - 1; i >= 0; i--)
                level.RemoveContent(level.Enemies.ElementAt(i));

            if (level.HasStairsDown)
                player.Location = level.StairsDown.Location;

            // Generate Hunger
            player.Hunger += player.FoodUsagePerTurnBase * pathLength;

            // Queue update: TODO: Clean this up maybe? 
            _modelService.UpdateVisibleLocations();
            _modelService.UpdateContents();

            LevelUpdateEvent(this, new LevelUpdate() { LevelUpdateType = LevelUpdateType.ContentAll });
            LevelUpdateEvent(this, new LevelUpdate() { LevelUpdateType = LevelUpdateType.LayoutAll });
            LevelUpdateEvent(this, new LevelUpdate() { LevelUpdateType = LevelUpdateType.PlayerAll });
        }
    }
}
