using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Content.Doodad;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Processing.Action;
using Rogue.NET.Core.Processing.Action.Enum;
using Rogue.NET.Core.Processing.Command.Backend.CommandData;
using Rogue.NET.Core.Processing.Event.Backend.EventData;
using Rogue.NET.Core.Processing.Model.Content;
using Rogue.NET.Core.Processing.Model.Content.Interface;
using Rogue.NET.Core.Processing.Service.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Processing.Service
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

        readonly BackendEngine[] _rogueEngines;

        readonly IModelService _modelService;

        // These queues are processed with priority for the UI. Processing is called from
        // the UI to dequeue next work-item (Animations (then) UI (then) Data)
        //
        // Example: 0) Player Moves (LevelCommand issued)
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
        Queue<LevelEventData> _levelEventDataQueue;
        Queue<TargetRequestEventData> _targetRequestEventDataQueue;
        Queue<DialogEventData> _dialogEventDataQueue;
        Queue<AnimationEventData> _animationEventDataQueue;
        Queue<ProjectileAnimationEventData> _projectileAnimationEventDataQueue;
        Queue<ScenarioEventData> _scenarioEventDataQueue;
        Queue<LevelProcessingAction> _backendQueue;

        // NO EVENT AGGREGATOR! This service is meant to process by method calls ONLY.
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

            _rogueEngines = new BackendEngine[] { _contentEngine as BackendEngine,
                                                _layoutEngine as BackendEngine,
                                                _scenarioEngine as BackendEngine,
                                                _alterationEngine as BackendEngine,
                                                _debugEngine as BackendEngine };

            _levelEventDataQueue = new Queue<LevelEventData>();
            _targetRequestEventDataQueue = new Queue<TargetRequestEventData>();
            _dialogEventDataQueue = new Queue<DialogEventData>();
            _animationEventDataQueue = new Queue<AnimationEventData>();
            _projectileAnimationEventDataQueue = new Queue<ProjectileAnimationEventData>();
            _scenarioEventDataQueue = new Queue<ScenarioEventData>();
            _backendQueue = new Queue<LevelProcessingAction>();
            
            foreach (var engine in _rogueEngines)
            {
                engine.AnimationEvent += (eventData) =>
                {
                    _animationEventDataQueue.Enqueue(eventData);
                };
                engine.ProjectileAnimationEvent += (eventData) =>
                {
                    _projectileAnimationEventDataQueue.Enqueue(eventData);
                };
                engine.DialogEvent += (eventData) =>
                {
                    _dialogEventDataQueue.Enqueue(eventData);
                };
                engine.LevelEvent += (eventData) =>
                {
                    _levelEventDataQueue.Enqueue(eventData);
                };
                engine.TargetRequestEvent += (eventData) =>
                {
                    _targetRequestEventDataQueue.Enqueue(eventData);
                };
                engine.LevelProcessingActionEvent += (eventData) =>
                {
                    _backendQueue.Enqueue(eventData);
                };
                engine.ScenarioEvent += (eventData) =>
                {
                    _scenarioEventDataQueue.Enqueue(eventData);
                };
            }
        }

        public void IssueCommand(LevelCommandData commandData)
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

            switch (commandData.LevelAction)
            {
                case LevelCommandType.Attack:
                    {
                        _scenarioEngine.Attack(commandData.Direction);
                        nextAction = LevelContinuationAction.ProcessTurnNoRegeneration;
                    }
                    break;
                case LevelCommandType.Throw:
                    {
                        nextAction = _scenarioEngine.Throw(commandData.Id);
                    }
                    break;
                case LevelCommandType.ToggleDoor:
                    {
                        _layoutEngine.ToggleDoor(commandData.Direction, player.Location);
                        nextAction = LevelContinuationAction.ProcessTurnNoRegeneration;
                    }
                    break;
                case LevelCommandType.Move:
                    {
                        var obj = _scenarioEngine.Move(commandData.Direction);
                        if (obj is Consumable || obj is Equipment)
                            _contentEngine.StepOnItem(player, (ItemBase)obj);
                        else if (obj is DoodadBase)
                            _contentEngine.StepOnDoodad(player, (DoodadBase)obj);
                        nextAction = LevelContinuationAction.ProcessTurn;
                    }
                    break;
                case LevelCommandType.Search:
                    {
                        _layoutEngine.Search(_modelService.Player.Location);
                        nextAction = LevelContinuationAction.ProcessTurn;
                    }
                    break;
                case LevelCommandType.InvokeSkill:
                    {
                        nextAction = _scenarioEngine.InvokePlayerSkill();
                    }
                    break;
                case LevelCommandType.InvokeDoodad:
                    {
                        nextAction = _scenarioEngine.InvokeDoodad();
                    }
                    break;
                case LevelCommandType.Consume:
                    {
                        nextAction = _scenarioEngine.Consume(commandData.Id);
                    }
                    break;
                case LevelCommandType.Drop:
                    {
                        _scenarioEngine.Drop(commandData.Id);
                        nextAction = LevelContinuationAction.ProcessTurn;
                    }
                    break;
                case LevelCommandType.Fire:
                    {
                        nextAction = _scenarioEngine.Fire();
                    }
                    break;
                case LevelCommandType.Equip:
                    {
                        if (_contentEngine.Equip(commandData.Id))
                            nextAction = LevelContinuationAction.ProcessTurn;
                    }
                    break;

#if DEBUG
                case LevelCommandType.DebugSimulateNext:
                    {
                        _debugEngine.SimulateAdvanceToNextLevel();
                        nextAction = LevelContinuationAction.DoNothing;
                    }
                    break;
                case LevelCommandType.DebugNext:
                    {
                        _debugEngine.AdvanceToNextLevel();
                        nextAction = LevelContinuationAction.DoNothing;
                    }
                    break;
                case LevelCommandType.DebugIdentifyAll:
                    {
                        _debugEngine.IdentifyAll();
                        nextAction = LevelContinuationAction.DoNothing;
                    }
                    break;
                case LevelCommandType.DebugExperience:
                    {
                        _debugEngine.GivePlayerExperience();
                        nextAction = LevelContinuationAction.DoNothing;
                    }
                    break;
                case LevelCommandType.DebugRevealAll:
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

        // PLAYER COMMAND: Something that typically originates from some UI action.
        public void IssuePlayerCommand(PlayerCommandData command)
        {
            // Player commands don't involve level actions - so no need to check for altered states.
            switch (command.Type)
            {
                case PlayerCommandType.AlterationEffect:
                    {
                        var effectCommand = command as PlayerAlterationEffectCommandData;

                        if (effectCommand.Effect is EquipmentEnhanceAlterationEffect)
                            _scenarioEngine.EnhanceEquipment(effectCommand.Effect as EquipmentEnhanceAlterationEffect, command.Id);

                        else
                            throw new Exception("Unknonw IPlayerAlterationEffectCommandAction.Effect");
                    }
                    break;
                case PlayerCommandType.Uncurse:
                    _scenarioEngine.Uncurse(command.Id);
                    break;
                case PlayerCommandType.Identify:
                    _scenarioEngine.Identify(command.Id);
                    break;
                case PlayerCommandType.ActivateSkillSet:
                    _scenarioEngine.ToggleActiveSkillSet(command.Id, true);
                    break;
                case PlayerCommandType.CycleSkillSet:
                    _scenarioEngine.CycleActiveSkillSet();
                    break;
                case PlayerCommandType.SelectSkill:
                    _scenarioEngine.SelectSkill(command.Id);
                    break;
                case PlayerCommandType.UnlockSkill:
                    _scenarioEngine.UnlockSkill(command.Id);
                    break;
                case PlayerCommandType.PlayerAdvancement:
                    {
                        var advancementCommand = command as PlayerAdvancementCommandData;

                        _scenarioEngine.PlayerAdvancement(advancementCommand.Hp,
                                                          advancementCommand.Stamina,
                                                          advancementCommand.Strength, 
                                                          advancementCommand.Agility,
                                                          advancementCommand.Intelligence,
                                                          advancementCommand.SkillPoints);
                    }
                    break;
                default:
                    break;
            }
        }

        public void IssuePlayerMultiItemCommand(PlayerMultiItemCommandData command)
        {
            // Player commands don't involve level actions - so no need to check for altered states.
            switch (command.Type)
            {
                case PlayerMultiItemActionType.AlterationEffect:
                    {
                        var effectCommand = command as PlayerAlterationEffectMultiItemCommandData;

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
            _contentEngine.CalculateCharacterReactions();


            _backendQueue.Enqueue(new LevelProcessingAction()
            {
                Type = regenerate ? LevelProcessingActionType.EndOfTurn  : 
                                    LevelProcessingActionType.EndOfTurnNoRegenerate
            });
        }

        #region (public) Queue Methods

        public bool ProcessBackend()
        {
            if (!_backendQueue.Any())
                return false;

            var workItem = _backendQueue.Dequeue();
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
                    if (_modelService.Level.NonPlayerCharacters.Any(x => x.Id == workItem.Actor.Id))
                        _contentEngine.ProcessCharacterReaction(_modelService.Level.NonPlayerCharacters.First(x => x.Id == workItem.Actor.Id));
                    break;
                case LevelProcessingActionType.CharacterAlteration:
                    _alterationEngine.Process(workItem.Actor, workItem.AlterationAffectedCharacters, workItem.Alteration);
                    break;
            }
            return true;
        }

        public void ClearQueues()
        {
            _animationEventDataQueue.Clear();
            _projectileAnimationEventDataQueue.Clear();
            _backendQueue.Clear();
            _dialogEventDataQueue.Clear();
            _levelEventDataQueue.Clear();
            _targetRequestEventDataQueue.Clear();
            _scenarioEventDataQueue.Clear();
        }

        public AnimationEventData DequeueAnimationEventData()
        {
            return _animationEventDataQueue.Any() ? _animationEventDataQueue.Dequeue() : null;
        }

        public ProjectileAnimationEventData DequeueProjectileAnimationEventData()
        {
            return _projectileAnimationEventDataQueue.Any() ? _projectileAnimationEventDataQueue.Dequeue() : null;
        }

        public LevelEventData DequeueLevelEventData()
        {
            return _levelEventDataQueue.Any() ? _levelEventDataQueue.Dequeue() : null;
        }

        public TargetRequestEventData DequeueTargetRequestEventData()
        {
            return _targetRequestEventDataQueue.Any() ? _targetRequestEventDataQueue.Dequeue() : null;
        }

        public DialogEventData DequeueDialogEventData()
        {
            return _dialogEventDataQueue.Any() ? _dialogEventDataQueue.Dequeue() : null;
        }

        public ScenarioEventData DequeueScenarioEventData()
        {
            return _scenarioEventDataQueue.Any() ? _scenarioEventDataQueue.Dequeue() : null;
        }
        #endregion
    }
}
