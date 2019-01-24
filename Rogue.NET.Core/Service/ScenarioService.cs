using Rogue.NET.Core.Logic.Algorithm.Interface;
using Rogue.NET.Core.Logic.Interface;
using Rogue.NET.Core.Logic.Processing;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration;
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
        readonly IReligionEngine _religionEngine;
        readonly IDebugEngine _debugEngine;

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
        Queue<IDialogUpdate> _dialogQueue;
        Queue<ILevelUpdate> _uiQueue;
        Queue<ILevelProcessingAction> _dataQueue;

        /// <summary>
        /// ***ATTACK ATTRIBUTES INTERCEPTED FROM THE DIALOG QUEUE AND STORED FOR A ONE-OFF
        /// IMBUE ALTERATION. 
        /// </summary>
        IEnumerable<AttackAttribute> _imbueAttackAttributes;

        [ImportingConstructor]
        public ScenarioService(
            IScenarioEngine scenarioEngine,
            IContentEngine contentEngine, 
            ILayoutEngine layoutEngine,
            IModelService modelService,
            IDebugEngine debugEngine,
            ISpellEngine spellEngine,
            IReligionEngine religionEngine,
            IRayTracer rayTracer)
        {
            _scenarioEngine = scenarioEngine;
            _contentEngine = contentEngine;
            _layoutEngine = layoutEngine;
            _modelService = modelService;
            _spellEngine = spellEngine;
            _religionEngine = religionEngine;
            _debugEngine = debugEngine;
            _rayTracer = rayTracer;

            var rogueEngines = new IRogueEngine[] { _contentEngine, _layoutEngine, _scenarioEngine, _spellEngine, _religionEngine, _debugEngine };

            _animationQueue = new Queue<IAnimationUpdate>();
            _scenarioQueue = new Queue<IScenarioUpdate>();
            _splashQueue = new Queue<ISplashUpdate>();
            _dialogQueue = new Queue<IDialogUpdate>();
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
                engine.DialogUpdateEvent += (sender, update) =>
                {
                    // ONE OFF FOR IMBUE ONLY!
                    if (update.Type == DialogEventType.ImbueArmor ||
                        update.Type == DialogEventType.ImbueWeapon)
                        _imbueAttackAttributes = update.ImbueAttackAttributes;

                    _dialogQueue.Enqueue(update);
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
                        nextAction = _scenarioEngine.Throw(command.ScenarioObjectId);
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
                        nextAction = _scenarioEngine.Consume(command.ScenarioObjectId);
                    }
                    break;
                case LevelActionType.Drop:
                    {
                        _scenarioEngine.Drop(command.ScenarioObjectId);
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
                        if (_contentEngine.Equip(command.ScenarioObjectId))
                            nextAction = LevelContinuationAction.ProcessTurn;
                    }
                    break;
                case LevelActionType.EnchantArmor:
                case LevelActionType.EnchantWeapon:
                    {
                        _scenarioEngine.Enchant(command.ScenarioObjectId);
                        nextAction = LevelContinuationAction.ProcessTurn;
                    }
                    break;
                case LevelActionType.ImbueArmor:
                    {
                        _scenarioEngine.ImbueArmor(command.ScenarioObjectId, _imbueAttackAttributes);
                        nextAction = LevelContinuationAction.ProcessTurn;

                        // *** SET TO NULL TO INDICATE USED
                        _imbueAttackAttributes = null;
                    }
                    break;
                case LevelActionType.ImbueWeapon:
                    {
                        _scenarioEngine.ImbueWeapon(command.ScenarioObjectId, _imbueAttackAttributes);
                        nextAction = LevelContinuationAction.ProcessTurn;

                        // *** SET TO NULL TO INDICATE USED
                        _imbueAttackAttributes = null;
                    }
                    break;
                case LevelActionType.Identify:
                    {
                        _scenarioEngine.Identify(command.ScenarioObjectId);
                        nextAction = LevelContinuationAction.ProcessTurn;
                    }
                    break;
                case LevelActionType.Uncurse:
                    {
                        _scenarioEngine.Uncurse(command.ScenarioObjectId);
                        nextAction = LevelContinuationAction.ProcessTurn;
                    }
                    break;
                case LevelActionType.ActivateSkillSet:
                    {
                        _scenarioEngine.ToggleActiveSkill(command.ScenarioObjectId, true);
                        nextAction = LevelContinuationAction.DoNothing;
                    }
                    break;
                case LevelActionType.CycleSkillSet:
                    {
                        _scenarioEngine.CycleActiveSkillSet();
                        nextAction = LevelContinuationAction.DoNothing;
                    }
                    break;
                case LevelActionType.ActivateSkill:
                    {
                        _scenarioEngine.ActivateSkill(command.ScenarioObjectId);
                        nextAction = LevelContinuationAction.DoNothing;
                    }
                    break;
                case LevelActionType.ChangeSkillLevelDown:
                    {
                        _scenarioEngine.ChangeSkillLevelUp(command.ScenarioObjectId);
                        nextAction = LevelContinuationAction.DoNothing;
                    }
                    break;
                case LevelActionType.ChangeSkillLevelUp:
                    {
                        _scenarioEngine.ChangeSkillLevelDown(command.ScenarioObjectId);
                        nextAction = LevelContinuationAction.DoNothing;
                    }
                    break;
                case LevelActionType.UnlockSkill:
                    {
                        _scenarioEngine.UnlockSkill(command.ScenarioObjectId);
                        nextAction = LevelContinuationAction.DoNothing;
                    }
                    break;
                case LevelActionType.RenounceReligion:
                    {
                        nextAction = _religionEngine.RenounceReligion(false);
                    }
                    break;

#if DEBUG
                case LevelActionType.DebugNext:
                    {
                        _debugEngine.SimulateAdvanceToNextLevel();
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
                Type = regenerate ? LevelProcessingActionType.EndOfTurn  : LevelProcessingActionType.EndOfTurnNoRegenerate
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
                    // Enemy not available (Reasons)
                    //
                    // *** Must be because enemy reaction was queued before it was removed.
                    //     Below are known causes
                    //
                    // 0) Enemy reacts twice before player turn while malign attribute effect 
                    //    causes their death.

                    // So, must check for the enemy to be available. The way to avoid this is
                    // to either do pruning of the queues; or to do full multi-threaded decoupling (lots of work).
                    if (_modelService.Level.Enemies.Any(x => x.Id == workItem.CharacterId))
                        _contentEngine.ProcessEnemyReaction(_modelService.Level.Enemies.First(x => x.Id == workItem.CharacterId));
                    break;
                case LevelProcessingActionType.PlayerSpell:
                    _spellEngine.ProcessPlayerMagicSpell(workItem.PlayerSpell);
                    break;
                case LevelProcessingActionType.EnemySpell:
                    _spellEngine.ProcessEnemyMagicSpell(workItem.Enemy, workItem.EnemySpell);
                    break;
            }
            return true;
        }

        public void ClearQueues()
        {
            _animationQueue.Clear();
            _dataQueue.Clear();
            _scenarioQueue.Clear();
            _splashQueue.Clear();
            _dialogQueue.Clear();
            _uiQueue.Clear();
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
        public bool AnyDialogEvents()
        {
            return _dialogQueue.Any();
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
        public IDialogUpdate DequeueDialogUpdate()
        {
            if (_dialogQueue.Any())
                return _dialogQueue.Dequeue();

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
