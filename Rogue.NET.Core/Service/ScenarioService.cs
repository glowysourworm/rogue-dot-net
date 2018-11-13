using Rogue.NET.Core.Logic.Algorithm.Interface;
using Rogue.NET.Core.Logic.Interface;
using Rogue.NET.Core.Logic.Processing;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Doodad;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Service.Interface;

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Service
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioService))]
    public class ScenarioService : IScenarioService
    {
        readonly IScenarioEngine _scenarioEngine;
        readonly IContentEngine _contentEngine;
        readonly ILayoutEngine _layoutEngine;
        readonly ISpellEngine _spellEngine;

        readonly IModelService _modelService;
        readonly IRayTracer _rayTracer;

        // These queues are processed with priority for the UI. Processing is called from
        // the UI to dequeue next work-item (Animations (then) UI (then) Data)
        //
        // Example: 0) Player Moves (ILevelCommand issued)
        //          1) Model is updated for player move
        //          2) UI update is queued
        //          3) Enemy Reactions are queued (events bubble up from IContentEngine)
        //          4) UI queue processed for Player Move
        //          5) Data queue processed for one Enemy
        //          6) Enemy uses magic spell
        //          7) UI Animation is queued
        //          8) etc...

        // These Have Priority:  { Animation, Scenario, Splash, UI, Data }
        Queue<IAnimationUpdate> _animationQueue;
        Queue<IScenarioUpdate> _scenarioQueue;
        Queue<ISplashUpdate> _splashQueue;
        Queue<ILevelUpdate> _uiQueue;
        Queue<ILevelProcessingAction> _dataQueue;

        [ImportingConstructor]
        public ScenarioService(
            IScenarioEngine scenarioEngine,
            IContentEngine contentEngine, 
            ILayoutEngine layoutEngine,
            IModelService modelService,
            ISpellEngine spellEngine,
            IRayTracer rayTracer)
        {
            _scenarioEngine = scenarioEngine;
            _contentEngine = contentEngine;
            _layoutEngine = layoutEngine;
            _modelService = modelService;
            _spellEngine = spellEngine;
            _rayTracer = rayTracer;

            var rogueEngines = new IRogueEngine[] { _contentEngine, _layoutEngine, _scenarioEngine, _spellEngine };

            _animationQueue = new Queue<IAnimationUpdate>();
            _scenarioQueue = new Queue<IScenarioUpdate>();
            _splashQueue = new Queue<ISplashUpdate>();
            _uiQueue = new Queue<ILevelUpdate>();
            _dataQueue = new Queue<ILevelProcessingAction>();
            
            foreach (var engine in rogueEngines)
            {
                // Updates
                engine.AnimationUpdateEvent += (sender, update) =>
                {
                    _animationQueue.Enqueue(update);
                };
                engine.LevelUpdateEvent += (sender, update) =>
                {
                    _uiQueue.Enqueue(update);
                };
                engine.ScenarioUpdateEvent += (sender, update) =>
                {
                    _scenarioQueue.Enqueue(update);
                };
                engine.SplashUpdateEvent += (sender, update) =>
                {
                    _splashQueue.Enqueue(update);
                };

                // Actions
                engine.LevelProcessingActionEvent += (sender, action) =>
                {
                    _dataQueue.Enqueue(action);
                };
            }
        }

        public void IssueCommand(ILevelCommandAction command)
        {
            // Check for player altered states that cause automatic player actions
            var nextAction = _scenarioEngine.ProcessAlteredPlayerState();
            if (nextAction == LevelContinuationAction.ProcessTurn ||
                nextAction == LevelContinuationAction.ProcessTurnNoRegeneration)
            {
                EndOfTurn(nextAction == LevelContinuationAction.ProcessTurn);
                return;
            }

            var player = _modelService.Player;
            var level = _modelService.Level;

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
                case LevelAction.ToggleDoor:
                    {
                        _layoutEngine.ToggleDoor(_modelService.Level.Grid, command.Direction, player.Location);
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
                case LevelAction.Search:
                    {
                        _layoutEngine.Search(_modelService.Level.Grid, _modelService.Player.Location);
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

#if DEBUG
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

                        // Queue update: TODO: Clean this up maybe? 
                        _modelService.UpdateVisibleLocations();
                        _modelService.UpdateContents();
                        _uiQueue.Enqueue(new LevelUpdate() { LevelUpdateType = LevelUpdateType.ContentAll });
                        _uiQueue.Enqueue(new LevelUpdate() { LevelUpdateType = LevelUpdateType.LayoutAll });
                        _uiQueue.Enqueue(new LevelUpdate() { LevelUpdateType = LevelUpdateType.PlayerAll });
                    }
                    break;
                case LevelAction.DebugIdentifyAll:
                    {
                        foreach (var item in player.Inventory)
                            _scenarioEngine.Identify(item.Key);

                        _uiQueue.Enqueue(new LevelUpdate() { LevelUpdateType = LevelUpdateType.PlayerAll });
                    }
                    break;
                case LevelAction.DebugExperience:
                    {
                        player.Experience += 10000;
                        _uiQueue.Enqueue(new LevelUpdate() { LevelUpdateType = LevelUpdateType.PlayerAll });
                    }
                    break;
                case LevelAction.DebugSkillUp:
                    {
                        foreach (var skillSet in player.SkillSets)
                        {
                            if (skillSet.Level < player.SkillSets.Count)
                                skillSet.Level++;
                        }
                        _uiQueue.Enqueue(new LevelUpdate() { LevelUpdateType = LevelUpdateType.PlayerAll });
                    }
                    break;
#endif
            }

            if (nextAction == LevelContinuationAction.ProcessTurn || 
                nextAction == LevelContinuationAction.ProcessTurnNoRegeneration)
            {
                EndOfTurn(nextAction == LevelContinuationAction.ProcessTurn);
                return;
            }
        }

        private void EndOfTurn(bool regenerate)
        {
            _contentEngine.CalculateEnemyReactions();
            _dataQueue.Enqueue(new LevelProcessingAction()
            {
                Type = LevelProcessingActionType.EndOfTurn
            });
        }

        #region (public) Queue Methods

        public bool ProcessBackend()
        {
            if (!_dataQueue.Any())
                return false;

            var workItem = _dataQueue.Dequeue();
            switch (workItem.Type)
            {
                case LevelProcessingActionType.EndOfTurn:
                    _scenarioEngine.ProcessEndOfTurn(true);
                    break;
                case LevelProcessingActionType.EndOfTurnNoRegenerate:
                    _scenarioEngine.ProcessEndOfTurn(false);
                    break;
                case LevelProcessingActionType.EnemyReaction:
                    // If enemy not available then we've made a mistake in our processing logic. So, let it crash!
                    _contentEngine.ProcessEnemyReaction(_modelService.Level.Enemies.First(x => x.Id == workItem.CharacterId));
                    break;
                case LevelProcessingActionType.PlayerSpell:
                    _spellEngine.ProcessPlayerMagicSpell(workItem.PlayerSpell);
                    break;
            }
            return true;
        }

        public bool AnyLevelEvents()
        {
            return _uiQueue.Any();
        }
        public bool AnyAnimationEvents()
        {
            return _animationQueue.Any();
        }
        public bool AnyScenarioEvents()
        {
            return _scenarioQueue.Any();
        }
        public bool AnySplashEvents()
        {
            return _splashQueue.Any();
        }

        public IScenarioUpdate DequeueScenarioUpdate()
        {
            if (_scenarioQueue.Any())
                return _scenarioQueue.Dequeue();

            return null;
        }
        public ISplashUpdate DequeueSplashUpdate()
        {
            if (_splashQueue.Any())
                return _splashQueue.Dequeue();

            return null;
        }
        public ILevelUpdate DequeueLevelUpdate()
        {
            if (_uiQueue.Any())
                return _uiQueue.Dequeue();

            return null;
        }
        public IAnimationUpdate DequeueAnimationUpdate()
        {
            if (_animationQueue.Any())
                return _animationQueue.Dequeue();

            return null;
        }
        #endregion
    }
}
