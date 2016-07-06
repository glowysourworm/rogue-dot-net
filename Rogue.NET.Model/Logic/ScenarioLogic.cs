using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Rogue.NET.Scenario.Model;
using Rogue.NET.Common.Collections;
using Rogue.NET.Common;
using Rogue.NET.Model.Scenario;
using Microsoft.Practices.Prism.Events;
using Rogue.NET.Model.Events;
using Rogue.NET.Common.Events.Scenario;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Prism.PubSubEvents;

namespace Rogue.NET.Model.Logic
{
    /// <summary>
    /// Calls to Interaction / Movement engine to process manipulation on
    /// Level data. Enum sent back determines further action by Level Module. String
    /// Sent back is forwarded to dialog.
    /// </summary>
    public class ScenarioLogic : LogicBase
    {
        private MovementLogic _movementEngine = null;
        private InteractionLogic _interactionEngine = null;

        public ScenarioLogic(IUnityContainer unityContainer, IEventAggregator eventAggregator) : base(eventAggregator, unityContainer)
        {
            _movementEngine = unityContainer.Resolve<MovementLogic>();
            _interactionEngine = unityContainer.Resolve<InteractionLogic>();
        }
        protected override void OnLevelCommand(LevelCommandArgs e)
        {
            if (this.Player.States.Any(z => z != CharacterStateType.Normal))
            {
                //Sleeping
                if (this.Player.States.Any(z => z == CharacterStateType.Sleeping))
                {
                    PublishScenarioMessage(this.Player.RogueName + " is asleep!");
                    ProcessDungeonTurn(true, true);
                    return;
                }

                //Paralyzed
                else if (this.Player.States.Any(z => z == CharacterStateType.Paralyzed))
                {
                    PublishScenarioMessage(this.Player.RogueName + " is paralyzed!");
                    ProcessDungeonTurn(true, true);
                    return;
                }

                //Confused
                else if (this.Player.States.Any(z => z == CharacterStateType.Confused))
                {
                    PublishScenarioMessage(this.Player.RogueName + " is confused!");
                    ScenarioObject obj = _movementEngine.MoveRandom();
                    if (obj is Consumable || obj is Equipment)
                        _interactionEngine.StepOnItem(this.Player, (Item)obj);
                    else if (obj is Doodad)
                        _interactionEngine.StepOnDoodad(this.Player, (Doodad)obj);

                    ProcessDungeonTurn(true, true);
                    return;
                }
            }

            LevelContinuationAction nextAction = LevelContinuationAction.DoNothing;
            switch (e.Action)
            {
                case LevelAction.Attack:
                    {
                        _interactionEngine.Attack(e.Direction);
                        nextAction = LevelContinuationAction.ProcessTurnNoRegeneration;
                    }
                    break;
                case LevelAction.Throw:
                    {
                        nextAction = _interactionEngine.Throw(e.ItemId);
                    }
                    break;
                case LevelAction.Close:
                    {
                        _interactionEngine.ToggleDoor(e.Direction, this.Player.Location);
                        nextAction = LevelContinuationAction.ProcessTurnNoRegeneration;
                    }
                    break;
                case LevelAction.Move:
                    {
                        ScenarioObject obj = _movementEngine.Move(e.Direction);
                        if (obj is Consumable || obj is Equipment)
                            _interactionEngine.StepOnItem(this.Player, (Item)obj);
                        else if (obj is Doodad)
                            _interactionEngine.StepOnDoodad(this.Player, (Doodad)obj);
                        nextAction = LevelContinuationAction.ProcessTurn;
                    }
                    break;
                case LevelAction.Open:
                    {
                        _interactionEngine.ToggleDoor(e.Direction, this.Player.Location);
                        nextAction = LevelContinuationAction.ProcessTurnNoRegeneration;
                    }
                    break;
                case LevelAction.Search:
                    {
                        _movementEngine.Search();
                        nextAction = LevelContinuationAction.ProcessTurn;
                    }
                    break;
                case LevelAction.Target:
                    {
                        _interactionEngine.Target(e.Direction);
                        nextAction = LevelContinuationAction.DoNothing;
                    }
                    break;
                case LevelAction.InvokeSkill:
                    {
                        nextAction = _interactionEngine.InvokePlayerSkill();
                    }
                    break;
                case LevelAction.InvokeDoodad:
                    {
                        nextAction = _interactionEngine.InvokeDoodad();
                    }
                    break;
                case LevelAction.Consume:
                    {
                        nextAction = _interactionEngine.Consume(e.ItemId);
                    }
                    break;
                case LevelAction.Drop:
                    {
                        _interactionEngine.Drop(e.ItemId);
                        nextAction = LevelContinuationAction.ProcessTurn;
                    }
                    break;
                case LevelAction.Fire:
                    {
                        nextAction = _interactionEngine.Fire();
                    }
                    break;
                case LevelAction.Equip:
                    {
                        _interactionEngine.Equip(e.ItemId);
                        nextAction = LevelContinuationAction.ProcessTurn;
                    }
                    break;
                case LevelAction.Enchant:
                    {
                        _interactionEngine.Enchant(e.ItemId);
                        nextAction = LevelContinuationAction.ProcessTurn;
                    }
                    break;
                case LevelAction.Identify:
                    {
                        _interactionEngine.Identify(e.ItemId);
                        nextAction = LevelContinuationAction.ProcessTurn;
                    }
                    break;
                case LevelAction.Uncurse:
                    {
                        _interactionEngine.Uncurse(e.ItemId);
                        nextAction = LevelContinuationAction.ProcessTurn;
                    }
                    break;
                case LevelAction.EmphasizeSkillDown:
                    {
                        _interactionEngine.EmphasizeSkillDown(e.ItemId);
                        nextAction = LevelContinuationAction.DoNothing;
                    }
                    break;
                case LevelAction.EmphasizeSkillUp:
                    {
                        _interactionEngine.EmphasizeSkillUp(e.ItemId);
                        nextAction = LevelContinuationAction.DoNothing;
                    }
                    break;
                case LevelAction.ActivateSkill:
                    {
                        _interactionEngine.ToggleActiveSkill(e.ItemId, true);
                        nextAction = LevelContinuationAction.ProcessTurn;
                    }
                    break;
                case LevelAction.DeactivateSkill:
                    {
                        _interactionEngine.ToggleActiveSkill(e.ItemId, false);
                        nextAction = LevelContinuationAction.DoNothing;
                    }
                    break;
                case LevelAction.ToggleMarkForTrade:
                    {
                        _interactionEngine.ToggleMarkForTrade(e.ItemId);
                        nextAction = LevelContinuationAction.DoNothing;
                    }
                    break;
                case LevelAction.ClearTradeMarkedItems:
                    {
                        _interactionEngine.ClearMarkedForTrade();
                        nextAction = LevelContinuationAction.DoNothing;
                    }
                    break;
                case LevelAction.Trade:
                    {
                        _interactionEngine.Trade();
                        nextAction = LevelContinuationAction.DoNothing;
                    }
                    break;
                case LevelAction.CycleActiveSkill:
                    {
                        _interactionEngine.CycleActiveSkill();
                        nextAction = LevelContinuationAction.DoNothing;
                    }
                    break;
                
                //TODO: Remove this
                case LevelAction.DebugNext:
                    {
                        //Give all items and experience to the player and 
                        //put player at exit
                        foreach (Consumable c in this.Level.GetConsumables())
                            this.Player.ConsumableInventory.Add(c);

                        foreach (Equipment eq in this.Level.GetEquipment())
                            this.Player.EquipmentInventory.Add(eq);

                        foreach (Enemy en in this.Level.GetEnemies())
                        {
                            this.Player.Experience += en.ExperienceGiven;
                            foreach (Equipment equip in en.EquipmentInventory)
                                this.Player.EquipmentInventory.Add(equip);

                            foreach (Consumable c in en.ConsumableInventory)
                                this.Player.ConsumableInventory.Add(c);
                        }

                        for (int i = this.Level.GetConsumables().Count() - 1; i >= 0; i--)
                            this.Level.RemoveConsumable(this.Level.GetConsumables()[i]);

                        for (int i = this.Level.GetEquipment().Count() - 1; i >= 0; i--)
                            this.Level.RemoveEquipment(this.Level.GetEquipment()[i]);

                        this.Player.Location = this.Level.GetStairsDown().Location;
                    }
                    break;
                //TODO: Remove this
                case LevelAction.DebugIdentifyAll:
                    {
                        foreach (Consumable c in this.Player.ConsumableInventory)
                            _interactionEngine.Identify(c.Id);

                        foreach (Equipment eq in this.Player.EquipmentInventory)
                            _interactionEngine.Identify(eq.Id);
                    }
                    break;
                case LevelAction.DebugExperience:
                    this.Player.Experience += 10000;
                    break;
                case LevelAction.DebugSkillUp:
                    {
                        _interactionEngine.DebugSkillUp();
                    }
                    break;
            }

            bool regenerate = nextAction == LevelContinuationAction.ProcessTurn;
            if (nextAction == LevelContinuationAction.ProcessTurn || nextAction == LevelContinuationAction.ProcessTurnNoRegeneration)
                ProcessDungeonTurn(regenerate, true);
        }

        private void ProcessDungeonTurn(bool regenerate, bool enemyReactions)
        {
            Cell[] affected = _movementEngine.ProcessExploredCells();
            _interactionEngine.ProcessTurn(_movementEngine, affected, regenerate, enemyReactions);

            this.Level.UpdatePlayerAura(this.Player);
        }

        protected override void OnAnimationCompleted(AnimationCompletedEvent e)
        {
            _interactionEngine.OnAnimationCompleted(e);
            ProcessDungeonTurn(true, true);
        }
    }
}
