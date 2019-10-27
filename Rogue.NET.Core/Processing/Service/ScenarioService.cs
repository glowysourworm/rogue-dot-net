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
        readonly IScenarioProcessor _scenarioProcessor;
        readonly IContentProcessor _contentProcessor;
        readonly IAlterationProcessor _alterationProcessor;
        readonly IDebugProcessor _debugProcessor;

        readonly BackendProcessor[] _rogueProcessors;

        readonly IModelService _modelService;

        // These queues are processed with priority for the UI. Processing is called from
        // the UI to dequeue next work-item (Animations (then) UI (then) Data)
        //
        // Example: 0) Player Moves (LevelCommand issued)
        //          1) Model is updated for player move
        //          2) UI update is queued
        //          3) Enemy Reactions are queued (events bubble up from IContentProcessor)
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
            IScenarioProcessor scenarioProcessor,
            IContentProcessor contentProcessor, 
            IAlterationProcessor alterationProcessor,
            IDebugProcessor debugProcessor,
            IModelService modelService)
        {
            _scenarioProcessor = scenarioProcessor;
            _contentProcessor = contentProcessor;
            _alterationProcessor = alterationProcessor;
            _debugProcessor = debugProcessor;
            _modelService = modelService;

            _rogueProcessors = new BackendProcessor[] { _contentProcessor as BackendProcessor,
                                                        _scenarioProcessor as BackendProcessor,
                                                        _alterationProcessor as BackendProcessor,
                                                        _debugProcessor as BackendProcessor };

            _levelEventDataQueue = new Queue<LevelEventData>();
            _targetRequestEventDataQueue = new Queue<TargetRequestEventData>();
            _dialogEventDataQueue = new Queue<DialogEventData>();
            _animationEventDataQueue = new Queue<AnimationEventData>();
            _projectileAnimationEventDataQueue = new Queue<ProjectileAnimationEventData>();
            _scenarioEventDataQueue = new Queue<ScenarioEventData>();
            _backendQueue = new Queue<LevelProcessingAction>();
            
            foreach (var processor in _rogueProcessors)
            {
                processor.AnimationEvent += (eventData) =>
                {
                    _animationEventDataQueue.Enqueue(eventData);
                };
                processor.ProjectileAnimationEvent += (eventData) =>
                {
                    _projectileAnimationEventDataQueue.Enqueue(eventData);
                };
                processor.DialogEvent += (eventData) =>
                {
                    _dialogEventDataQueue.Enqueue(eventData);
                };
                processor.LevelEvent += (eventData) =>
                {
                    _levelEventDataQueue.Enqueue(eventData);
                };
                processor.TargetRequestEvent += (eventData) =>
                {
                    _targetRequestEventDataQueue.Enqueue(eventData);
                };
                processor.LevelProcessingActionEvent += (eventData) =>
                {
                    _backendQueue.Enqueue(eventData);
                };
                processor.ScenarioEvent += (eventData) =>
                {
                    _scenarioEventDataQueue.Enqueue(eventData);
                };
            }
        }

        public void IssueCommand(LevelCommandData commandData)
        {
            // Check for player altered states that cause automatic player actions
            var nextAction = _scenarioProcessor.ProcessAlteredPlayerState();
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
                        _scenarioProcessor.Attack(commandData.Direction);
                        nextAction = LevelContinuationAction.ProcessTurnNoRegeneration;
                    }
                    break;
                case LevelCommandType.Throw:
                    {
                        nextAction = _scenarioProcessor.Throw(commandData.Id);
                    }
                    break;
                case LevelCommandType.ToggleDoor:
                    {
                        //_scenarioProcessor.ToggleDoor(commandData.Direction, player.Location);
                        //nextAction = LevelContinuationAction.ProcessTurnNoRegeneration;
                    }
                    break;
                case LevelCommandType.Move:
                    {
                        var obj = _scenarioProcessor.Move(commandData.Direction);
                        if (obj is Consumable || obj is Equipment)
                            _contentProcessor.StepOnItem(player, (ItemBase)obj);
                        else if (obj is DoodadBase)
                            _contentProcessor.StepOnDoodad(player, (DoodadBase)obj);
                        nextAction = LevelContinuationAction.ProcessTurn;
                    }
                    break;
                case LevelCommandType.Search:
                    {
                        _scenarioProcessor.Search();
                        nextAction = LevelContinuationAction.ProcessTurn;
                    }
                    break;
                case LevelCommandType.InvokeSkill:
                    {
                        nextAction = _scenarioProcessor.InvokePlayerSkill();
                    }
                    break;
                case LevelCommandType.InvokeDoodad:
                    {
                        nextAction = _scenarioProcessor.InvokeDoodad();
                    }
                    break;
                case LevelCommandType.Consume:
                    {
                        nextAction = _scenarioProcessor.Consume(commandData.Id);
                    }
                    break;
                case LevelCommandType.Drop:
                    {
                        _scenarioProcessor.Drop(commandData.Id);
                        nextAction = LevelContinuationAction.ProcessTurn;
                    }
                    break;
                case LevelCommandType.Fire:
                    {
                        nextAction = _scenarioProcessor.Fire();
                    }
                    break;
                case LevelCommandType.Equip:
                    {
                        if (_contentProcessor.Equip(commandData.Id))
                            nextAction = LevelContinuationAction.ProcessTurn;
                    }
                    break;

#if DEBUG
                case LevelCommandType.DebugSimulateNext:
                    {
                        _debugProcessor.SimulateAdvanceToNextLevel();
                        nextAction = LevelContinuationAction.DoNothing;
                    }
                    break;
                case LevelCommandType.DebugNext:
                    {
                        _debugProcessor.AdvanceToNextLevel();
                        nextAction = LevelContinuationAction.DoNothing;
                    }
                    break;
                case LevelCommandType.DebugIdentifyAll:
                    {
                        _debugProcessor.IdentifyAll();
                        nextAction = LevelContinuationAction.DoNothing;
                    }
                    break;
                case LevelCommandType.DebugExperience:
                    {
                        _debugProcessor.GivePlayerExperience();
                        nextAction = LevelContinuationAction.DoNothing;
                    }
                    break;
                case LevelCommandType.DebugRevealAll:
                    {
                        _debugProcessor.RevealAll();
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
                            _scenarioProcessor.EnhanceEquipment(effectCommand.Effect as EquipmentEnhanceAlterationEffect, command.Id);

                        else
                            throw new Exception("Unknonw IPlayerAlterationEffectCommandAction.Effect");
                    }
                    break;
                case PlayerCommandType.Uncurse:
                    _scenarioProcessor.Uncurse(command.Id);
                    break;
                case PlayerCommandType.Identify:
                    _scenarioProcessor.Identify(command.Id);
                    break;
                case PlayerCommandType.ActivateSkillSet:
                    _scenarioProcessor.ToggleActiveSkillSet(command.Id, true);
                    break;
                case PlayerCommandType.CycleSkillSet:
                    _scenarioProcessor.CycleActiveSkillSet();
                    break;
                case PlayerCommandType.SelectSkill:
                    _scenarioProcessor.SelectSkill(command.Id);
                    break;
                case PlayerCommandType.UnlockSkill:
                    _scenarioProcessor.UnlockSkill(command.Id);
                    break;
                case PlayerCommandType.PlayerAdvancement:
                    {
                        var advancementCommand = command as PlayerAdvancementCommandData;

                        _scenarioProcessor.PlayerAdvancement(advancementCommand.Hp,
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
                            _alterationProcessor.ProcessTransmute(effectCommand.Effect as TransmuteAlterationEffect, command.ItemIds);

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
            _contentProcessor.CalculateCharacterReactions();


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
                    _rogueProcessors.ForEach(processor => processor.ApplyEndOfTurn(true));
                    break;
                case LevelProcessingActionType.EndOfTurnNoRegenerate:
                    _rogueProcessors.ForEach(processor => processor.ApplyEndOfTurn(false));
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
                        _contentProcessor.ProcessCharacterReaction(_modelService.Level.NonPlayerCharacters.First(x => x.Id == workItem.Actor.Id));
                    break;
                case LevelProcessingActionType.CharacterAlteration:
                    _alterationProcessor.Process(workItem.Actor, workItem.AlterationAffectedCharacters, workItem.Alteration);
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
