﻿using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Logic.Interface;
using Rogue.NET.Core.Logic.Processing;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Content.Doodad;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Service.Interface;
using System;
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
        readonly IAlterationEngine _alterationEngine;
        readonly ILayoutEngine _layoutEngine;
        readonly IDebugEngine _debugEngine;

        readonly IRogueEngine[] _rogueEngines;

        readonly IModelService _modelService;

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

        // Update Have Priority:  Process depending on the priority - with backend processed
        //                        AFTER all updates have been processed
        Queue<IRogueUpdate> _lowQueue;
        Queue<IRogueUpdate> _highQueue;
        Queue<IRogueUpdate> _criticalQueue;
        Queue<ILevelProcessingAction> _dataQueue;

        [ImportingConstructor]
        public ScenarioService(
            IScenarioEngine scenarioEngine,
            IContentEngine contentEngine, 
            IAlterationEngine alterationEngine,
            ILayoutEngine layoutEngine,
            IDebugEngine debugEngine,
            IModelService modelService)
        {
            _scenarioEngine = scenarioEngine;
            _contentEngine = contentEngine;
            _alterationEngine = alterationEngine;
            _layoutEngine = layoutEngine;
            _debugEngine = debugEngine;
            _modelService = modelService;            

            _rogueEngines = new IRogueEngine[] { _contentEngine, _layoutEngine, _scenarioEngine, _alterationEngine, _debugEngine };

            _lowQueue = new Queue<IRogueUpdate>();
            _highQueue = new Queue<IRogueUpdate>();
            _criticalQueue = new Queue<IRogueUpdate>();
            _dataQueue = new Queue<ILevelProcessingAction>();
            
            foreach (var engine in _rogueEngines)
            {
                // Updates
                engine.RogueUpdateEvent += (sender, args) =>
                {
                    switch (args.Priority)
                    {
                        case RogueUpdatePriority.Low:
                            _lowQueue.Enqueue(args.Update);
                            break;
                        case RogueUpdatePriority.High:
                            _highQueue.Enqueue(args.Update);
                            break;
                        case RogueUpdatePriority.Critical:
                            _criticalQueue.Enqueue(args.Update);
                            break;
                    }
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
                case LevelActionType.Attack:
                    {
                        _scenarioEngine.Attack(command.Direction);
                        nextAction = LevelContinuationAction.ProcessTurnNoRegeneration;
                    }
                    break;
                case LevelActionType.Throw:
                    {
                        nextAction = _scenarioEngine.Throw(command.Id);
                    }
                    break;
                case LevelActionType.ToggleDoor:
                    {
                        _layoutEngine.ToggleDoor(_modelService.Level.Grid, command.Direction, player.Location);
                        nextAction = LevelContinuationAction.ProcessTurnNoRegeneration;
                    }
                    break;
                case LevelActionType.Move:
                    {
                        var obj = _scenarioEngine.Move(command.Direction);
                        if (obj is Consumable || obj is Equipment)
                            _contentEngine.StepOnItem(player, (ItemBase)obj);
                        else if (obj is DoodadBase)
                            _contentEngine.StepOnDoodad(player, (DoodadBase)obj);
                        nextAction = LevelContinuationAction.ProcessTurn;
                    }
                    break;
                case LevelActionType.Search:
                    {
                        _layoutEngine.Search(_modelService.Level.Grid, _modelService.Player.Location);
                        nextAction = LevelContinuationAction.ProcessTurn;
                    }
                    break;
                case LevelActionType.Target:
                    {
                        _scenarioEngine.Target(command.Direction);
                        nextAction = LevelContinuationAction.DoNothing;
                    }
                    break;
                case LevelActionType.InvokeSkill:
                    {
                        nextAction = _scenarioEngine.InvokePlayerSkill();
                    }
                    break;
                case LevelActionType.InvokeDoodad:
                    {
                        nextAction = _scenarioEngine.InvokeDoodad();
                    }
                    break;
                case LevelActionType.Consume:
                    {
                        nextAction = _scenarioEngine.Consume(command.Id);
                    }
                    break;
                case LevelActionType.Drop:
                    {
                        _scenarioEngine.Drop(command.Id);
                        nextAction = LevelContinuationAction.ProcessTurn;
                    }
                    break;
                case LevelActionType.Fire:
                    {
                        nextAction = _scenarioEngine.Fire();
                    }
                    break;
                case LevelActionType.Equip:
                    {
                        if (_contentEngine.Equip(command.Id))
                            nextAction = LevelContinuationAction.ProcessTurn;
                    }
                    break;

#if DEBUG
                case LevelActionType.DebugSimulateNext:
                    {
                        _debugEngine.SimulateAdvanceToNextLevel();
                        nextAction = LevelContinuationAction.DoNothing;
                    }
                    break;
                case LevelActionType.DebugNext:
                    {
                        _debugEngine.AdvanceToNextLevel();
                        nextAction = LevelContinuationAction.DoNothing;
                    }
                    break;
                case LevelActionType.DebugIdentifyAll:
                    {
                        _debugEngine.IdentifyAll();
                        nextAction = LevelContinuationAction.DoNothing;
                    }
                    break;
                case LevelActionType.DebugExperience:
                    {
                        _debugEngine.GivePlayerExperience();
                        nextAction = LevelContinuationAction.DoNothing;
                    }
                    break;
                case LevelActionType.DebugRevealAll:
                    {
                        _debugEngine.RevealAll();
                        nextAction = LevelContinuationAction.ProcessTurn;
                    }
                    break;
#endif
            }

            // This method constitutes ONE TURN. The Player has made a move and the enemies
            // need to be allowed to react before any of the Player / Enemies stats are calculated.
            //
            // The End-Of-Turn methods also calculate visible for each character and any items / objects
            // in view of the Player.
            if (nextAction == LevelContinuationAction.ProcessTurn || 
                nextAction == LevelContinuationAction.ProcessTurnNoRegeneration)
                EndOfTurn(nextAction == LevelContinuationAction.ProcessTurn);
        }

        public void IssuePlayerCommand(IPlayerCommandAction command)
        {
            // Player commands don't involve level actions - so no need to check for altered states.
            switch (command.Type)
            {
                case PlayerActionType.AlterationEffect:
                    {
                        var effectCommand = command as IPlayerAlterationEffectCommandAction;

                        if (effectCommand.Effect is EquipmentEnhanceAlterationEffect)
                            _scenarioEngine.EnhanceEquipment(effectCommand.Effect as EquipmentEnhanceAlterationEffect, command.Id);

                        else
                            throw new Exception("Unknonw IPlayerAlterationEffectCommandAction.Effect");
                    }
                    break;
                case PlayerActionType.Uncurse:
                    _scenarioEngine.Uncurse(command.Id);
                    break;
                case PlayerActionType.Identify:
                    _scenarioEngine.Identify(command.Id);
                    break;
                case PlayerActionType.ActivateSkillSet:
                    _scenarioEngine.ToggleActiveSkillSet(command.Id, true);
                    break;
                case PlayerActionType.CycleSkillSet:
                    _scenarioEngine.CycleActiveSkillSet();
                    break;
                case PlayerActionType.SelectSkill:
                    _scenarioEngine.SelectSkill(command.Id);
                    break;
                case PlayerActionType.UnlockSkill:
                    _scenarioEngine.UnlockSkill(command.Id);
                    break;
                case PlayerActionType.PlayerAdvancement:
                    {
                        var advancementCommand = command as IPlayerAdvancementCommandAction;

                        _scenarioEngine.PlayerAdvancement(advancementCommand.Strength, 
                                                          advancementCommand.Agility,
                                                          advancementCommand.Intelligence,
                                                          advancementCommand.SkillPoints);
                    }
                    break;
                default:
                    break;
            }
        }

        public void IssuePlayerMultiItemCommand(IPlayerMultiItemCommandAction command)
        {
            // Player commands don't involve level actions - so no need to check for altered states.
            switch (command.Type)
            {
                case PlayerMultiItemActionType.AlterationEffect:
                    {
                        var effectCommand = command as IPlayerAlterationEffectMultiItemCommandAction;

                        if (effectCommand.Effect is TransmuteAlterationEffect)
                            _alterationEngine.ProcessTransmute(effectCommand.Effect as TransmuteAlterationEffect, command.ItemIds);

                        else
                            throw new Exception("Unknonw IPlayerAlterationEffectCommandAction.Effect");
                    }
                    break;
                default:
                    break;
            }
        }

        private void EndOfTurn(bool regenerate)
        {
            // Queue Enemy Reactions
            _contentEngine.CalculateEnemyReactions();


            _dataQueue.Enqueue(new LevelProcessingAction()
            {
                Type = regenerate ? LevelProcessingActionType.EndOfTurn  : 
                                    LevelProcessingActionType.EndOfTurnNoRegenerate
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
                    _rogueEngines.ForEach(engine => engine.ApplyEndOfTurn(true));
                    break;
                case LevelProcessingActionType.EndOfTurnNoRegenerate:
                    _rogueEngines.ForEach(engine => engine.ApplyEndOfTurn(false));
                    break;
                case LevelProcessingActionType.Reaction:
                    // Enemy not available (Reasons)
                    //
                    // *** Must be because enemy reaction was queued before it was removed.
                    //     Below are known causes
                    //
                    // 0) Enemy reacts twice before player turn while malign attribute effect 
                    //    causes their death.

                    // So, must check for the enemy to be available. The way to avoid this is
                    // to either do pruning of the queues; or to do full multi-threaded decoupling (lots of work).
                    if (_modelService.Level.Enemies.Any(x => x.Id == workItem.Actor.Id))
                        _contentEngine.ProcessEnemyReaction(_modelService.Level.Enemies.First(x => x.Id == workItem.Actor.Id));
                    break;
                case LevelProcessingActionType.CharacterAlteration:
                    _alterationEngine.Process(workItem.Actor, workItem.AlterationAffectedCharacters, workItem.Alteration);
                    break;
            }
            return true;
        }

        public void ClearQueues()
        {
            _lowQueue.Clear();
            _highQueue.Clear();
            _criticalQueue.Clear();
            _dataQueue.Clear();
        }

        public bool AnyUpdates(RogueUpdatePriority priority)
        {
            switch (priority)
            {
                case RogueUpdatePriority.Low:
                    return _lowQueue.Any();
                case RogueUpdatePriority.High:
                    return _highQueue.Any();
                case RogueUpdatePriority.Critical:
                    return _criticalQueue.Any();
                default:
                    throw new Exception("Unknown Rogue Priority");
            }
        }

        public IRogueUpdate DequeueUpdate(RogueUpdatePriority priority)
        {
            switch (priority)
            {
                case RogueUpdatePriority.Low:
                    return _lowQueue.Dequeue();
                case RogueUpdatePriority.High:
                    return _highQueue.Dequeue();
                case RogueUpdatePriority.Critical:
                    return _criticalQueue.Dequeue();
                default:
                    throw new Exception("Unknown Rogue Priority");
            }
        }
        #endregion
    }
}
