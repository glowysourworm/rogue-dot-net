using Rogue.NET.Core.Event;
using Rogue.NET.Core.Logic.Interface;
using Rogue.NET.Core.Model.Common.Interface;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Doodad;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Service.Interface;

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Service
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioService))]
    public class ScenarioService : IScenarioService
    {
        readonly IScenarioEngine _scenarioEngine;
        readonly IContentEngine _contentEngine;
        readonly ILayoutEngine _layoutEngine;

        readonly IModelService _modelService;

        [ImportingConstructor]
        public ScenarioService(
            IScenarioEngine scenarioEngine,
            IContentEngine contentEngine, 
            ILayoutEngine layoutEngine,
            IModelService modelService)
        {
            _scenarioEngine = scenarioEngine;
            _contentEngine = contentEngine;
            _layoutEngine = layoutEngine;
            _modelService = modelService;
        }

        public void IssueCommand(ILevelCommand command)
        {
            // Check for player altered states that cause automatic player actions
            var nextAction = _scenarioEngine.ProcessAlteredPlayerState();
            if (nextAction == LevelContinuationAction.ProcessTurn || 
                nextAction == LevelContinuationAction.ProcessTurnNoRegeneration)
                ProcessDungeonTurn(nextAction == LevelContinuationAction.ProcessTurnNoRegeneration, true);

            var player = _modelService.Player;
            var level = _modelService.CurrentLevel;           

            nextAction = LevelContinuationAction.DoNothing;

            switch (command.Action)
            {
                case LevelAction.Attack:
                    {
                        _scenarioEngine.Attack(command.Direction);
                        nextAction = LevelContinuationAction.ProcessTurnNoRegeneration;
                    }
                    break;
                case LevelAction.Throw:
                    {
                        nextAction = _scenarioEngine.Throw(command.ScenarioObjectId);
                    }
                    break;
                case LevelAction.Close:
                    {
                        _layoutEngine.ToggleDoor(command.Direction, player.Location);
                        nextAction = LevelContinuationAction.ProcessTurnNoRegeneration;
                    }
                    break;
                case LevelAction.Move:
                    {
                        var obj = _scenarioEngine.Move(command.Direction);
                        if (obj is Consumable || obj is Equipment)
                            _contentEngine.StepOnItem(player, (ItemBase)obj);
                        else if (obj is DoodadBase)
                            _contentEngine.StepOnDoodad(player, (DoodadBase)obj);
                        nextAction = LevelContinuationAction.ProcessTurn;
                    }
                    break;
                case LevelAction.Open:
                    {
                        _layoutEngine.ToggleDoor(command.Direction, player.Location);
                        nextAction = LevelContinuationAction.ProcessTurnNoRegeneration;
                    }
                    break;
                case LevelAction.Search:
                    {
                        _layoutEngine.Search();
                        nextAction = LevelContinuationAction.ProcessTurn;
                    }
                    break;
                case LevelAction.Target:
                    {
                        _scenarioEngine.Target(command.Direction);
                        nextAction = LevelContinuationAction.DoNothing;
                    }
                    break;
                case LevelAction.InvokeSkill:
                    {
                        nextAction = _scenarioEngine.InvokePlayerSkill();
                    }
                    break;
                case LevelAction.InvokeDoodad:
                    {
                        nextAction = _scenarioEngine.InvokeDoodad();
                    }
                    break;
                case LevelAction.Consume:
                    {
                        nextAction = _scenarioEngine.Consume(command.ScenarioObjectId);
                    }
                    break;
                case LevelAction.Drop:
                    {
                        _scenarioEngine.Drop(command.ScenarioObjectId);
                        nextAction = LevelContinuationAction.ProcessTurn;
                    }
                    break;
                case LevelAction.Fire:
                    {
                        nextAction = _scenarioEngine.Fire();
                    }
                    break;
                case LevelAction.Equip:
                    {
                        if (_contentEngine.Equip(command.ScenarioObjectId))
                            nextAction = LevelContinuationAction.ProcessTurn;
                    }
                    break;
                case LevelAction.Enchant:
                    {
                        _scenarioEngine.Enchant(command.ScenarioObjectId);
                        nextAction = LevelContinuationAction.ProcessTurn;
                    }
                    break;
                case LevelAction.Identify:
                    {
                        _scenarioEngine.Identify(command.ScenarioObjectId);
                        nextAction = LevelContinuationAction.ProcessTurn;
                    }
                    break;
                case LevelAction.Uncurse:
                    {
                        _scenarioEngine.Uncurse(command.ScenarioObjectId);
                        nextAction = LevelContinuationAction.ProcessTurn;
                    }
                    break;
                case LevelAction.EmphasizeSkillDown:
                    {
                        _scenarioEngine.EmphasizeSkillDown(command.ScenarioObjectId);
                        nextAction = LevelContinuationAction.DoNothing;
                    }
                    break;
                case LevelAction.EmphasizeSkillUp:
                    {
                        _scenarioEngine.EmphasizeSkillUp(command.ScenarioObjectId);
                        nextAction = LevelContinuationAction.DoNothing;
                    }
                    break;
                case LevelAction.ActivateSkill:
                    {
                        _scenarioEngine.ToggleActiveSkill(command.ScenarioObjectId, true);
                        nextAction = LevelContinuationAction.ProcessTurn;
                    }
                    break;
                case LevelAction.DeactivateSkill:
                    {
                        _scenarioEngine.ToggleActiveSkill(command.ScenarioObjectId, false);
                        nextAction = LevelContinuationAction.DoNothing;
                    }
                    break;

                //TODO: Remove this
                case LevelAction.DebugNext:
                    {
                        //Give all items and experience to the player and 
                        //put player at exit
                        foreach (var consumable in level.Consumables)
                            player.Consumables.Add(consumable.Id, consumable);

                        foreach (var equipment in level.Equipment)
                            player.Equipment.Add(equipment.Id, equipment);

                        foreach (var enemy in level.Enemies)
                        {
                            player.Experience += enemy.ExperienceGiven;
                            foreach (var equipment in enemy.Equipment)
                                player.Equipment.Add(equipment.Key, equipment.Value);

                            foreach (var consumable in enemy.Consumables)
                                player.Consumables.Add(consumable.Key, consumable.Value);
                        }

                        for (int i = level.Consumables.Count() - 1; i >= 0; i--)
                            level.RemoveContent(level.Consumables.ElementAt(i));

                        for (int i = level.Equipment.Count() - 1; i >= 0; i--)
                            level.RemoveContent(level.Equipment.ElementAt(i));


                        for (int i = level.Enemies.Count() - 1; i >= 0; i--)
                            level.RemoveContent(level.Enemies.ElementAt(i));

                        if (level.HasStairsDown)
                            player.Location = level.StairsDown.Location;
                    }
                    break;
                //TODO: Remove this
                case LevelAction.DebugIdentifyAll:
                    {
                        foreach (var item in player.Inventory)
                            _scenarioEngine.Identify(item.Key);
                    }
                    break;
                case LevelAction.DebugExperience:
                    player.Experience += 10000;
                    break;
                case LevelAction.DebugSkillUp:
                    {
                        foreach (var skillSet in player.SkillSets)
                        {
                            if (skillSet.Level < player.SkillSets.Count)
                                skillSet.Level++;
                        }
                    }
                    break;
            }

            bool regenerate = nextAction == LevelContinuationAction.ProcessTurn;
            if (nextAction == LevelContinuationAction.ProcessTurn || nextAction == LevelContinuationAction.ProcessTurnNoRegeneration)
                ProcessDungeonTurn(regenerate, true);
        }

        private void ProcessDungeonTurn(bool regenerate, bool enemyReactions)
        {
            // TODO

            //Cell[] affected = _movementEngine.ProcessExploredCells();
            //_interactionEngine.ProcessTurn(_movementEngine, affected, regenerate, enemyReactions);

            //this.Level.UpdatePlayerAura(this.Player);
        }

        private void OnAnimationCompleted()
        {
            // TODO
            //_interactionEngine.OnAnimationCompleted(e);
            //ProcessDungeonTurn(true, true);
        }

        public void QueueLevelLoadRequest(int levelNumber, PlayerStartLocation startLocation)
        {
            throw new NotImplementedException();
        }

        public void QueueSplashScreenEvent(SplashEventType type)
        {
            throw new NotImplementedException();
        }
    }
}
