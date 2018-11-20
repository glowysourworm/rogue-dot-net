using Rogue.NET.Core.Logic.Content.Interface;
using Rogue.NET.Core.Logic.Interface;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Doodad;
using Rogue.NET.Core.Service.Interface;

using System.Linq;
using System.ComponentModel.Composition;
using System;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Logic.Processing;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model;

namespace Rogue.NET.Core.Logic
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioEngine))]
    public class ScenarioEngine : IScenarioEngine
    {
        readonly ILayoutEngine _layoutEngine;
        readonly IContentEngine _contentEngine;
        readonly ISpellEngine _spellEngine;
        readonly IModelService _modelService;
        readonly IScenarioMessageService _scenarioMessageService;
        readonly IInteractionProcessor _interactionProcessor;
        readonly ICharacterProcessor _characterProcessor;
        readonly IPlayerProcessor _playerProcessor;
        readonly IAlterationProcessor _alterationProcessor;
        readonly IRandomSequenceGenerator _randomSequenceGenerator;

        public event EventHandler<IScenarioUpdate> ScenarioUpdateEvent;
        public event EventHandler<ISplashUpdate> SplashUpdateEvent;
        public event EventHandler<ILevelUpdate> LevelUpdateEvent;
        public event EventHandler<IAnimationUpdate> AnimationUpdateEvent;
        public event EventHandler<ILevelProcessingAction> LevelProcessingActionEvent;

        [ImportingConstructor]
        public ScenarioEngine(
            ILayoutEngine layoutEngine,
            IContentEngine contentEngine,
            ISpellEngine spellEngine,
            IModelService modelService,
            IScenarioMessageService scenarioMessageService,
            IInteractionProcessor interactionProcessor,
            ICharacterProcessor characterProcessor,
            IPlayerProcessor playerProcessor,
            IAlterationProcessor alterationProcessor,
            IRandomSequenceGenerator randomSequenceGenerator)
        {
            _layoutEngine = layoutEngine;
            _contentEngine = contentEngine;
            _spellEngine = spellEngine;
            _modelService = modelService;
            _scenarioMessageService = scenarioMessageService;
            _interactionProcessor = interactionProcessor;
            _characterProcessor = characterProcessor;
            _playerProcessor = playerProcessor;
            _alterationProcessor = alterationProcessor;
            _randomSequenceGenerator = randomSequenceGenerator;
        }

        // Pre-check player state before applying desired command
        public LevelContinuationAction ProcessAlteredPlayerState()
        {
            var player = _modelService.Player;
            var states = player.Alteration.GetStates();

            if (states.Any(z => z != CharacterStateType.Normal))
            {
                //Sleeping
                if (states.Any(z => z == CharacterStateType.Sleeping))
                {
                    _scenarioMessageService.Publish(player.RogueName + " is asleep!");
                    return LevelContinuationAction.ProcessTurn;
                }

                //Paralyzed
                else if (states.Any(z => z == CharacterStateType.Paralyzed))
                {
                    _scenarioMessageService.Publish(player.RogueName + " is paralyzed!");
                    return LevelContinuationAction.ProcessTurn;
                }

                //Confused
                else if (states.Any(z => z == CharacterStateType.Confused))
                {
                    _scenarioMessageService.Publish(player.RogueName + " is confused!");
                    var obj = MoveRandom();

                    if (obj is Consumable || obj is Equipment)
                        _contentEngine.StepOnItem(player, (ItemBase)obj);
                    else if (obj is DoodadBase)
                        _contentEngine.StepOnDoodad(player, (DoodadBase)obj);

                    return LevelContinuationAction.ProcessTurn;
                }
            }

            return LevelContinuationAction.DoNothing;
        }

        public ScenarioObject Move(Compass direction)
        {
            // Desired Location
            var desiredLocation = _layoutEngine.GetPointInDirection(_modelService.Level.Grid, _modelService.Player.Location, direction);

            // Invalid location
            if (desiredLocation == CellPoint.Empty)
                return null;

            //Look for road blocks - move player
            if (!_layoutEngine.IsPathToAdjacentCellBlocked(_modelService.Level, _modelService.Player.Location, desiredLocation, true))
            {
                // Update player location
                _modelService.Player.Location = desiredLocation;

                // Notify Listener queue
                QueueLevelUpdate(LevelUpdateType.PlayerLocation, _modelService.Player.Id);
            }

            //See what the player stepped on...
            return _modelService.Level.GetAtPoint<ScenarioObject>(_modelService.Player.Location);
        }
        public ScenarioObject MoveRandom()
        {
            // Get random adjacent location
            var desiredLocation = _layoutEngine.GetRandomAdjacentLocation(_modelService.Level,_modelService.Player, _modelService.Player.Location, true);

            // Get direction for random move -> Move()
            return Move(_layoutEngine.GetDirectionBetweenAdjacentPoints(_modelService.Player.Location, desiredLocation));
        }

        /// <summary>
        /// Runs Ray-Tracing for end-of-turn; Applies Player end-of-turn, advancement, and death; and
        /// applies Level end-of-turn (create new monsters). Updates IModelService { visible locations, visible contents, end targeting }
        /// </summary>
        /// <param name="regenerate">Set to false to prevent Player regeneration</param>
        public void ProcessEndOfTurn(bool regenerate)
        {
            var level = _modelService.Level;
            var player = _modelService.Player;

            // Block Scenario Message Processing to prevent sending UI messages
            _scenarioMessageService.Block();

            // Want to now update model service from the model. This will process new visibility of
            // the Level Grid.
            _modelService.UpdateVisibleLocations();

            // Player: End-Of-Turn
            _playerProcessor.ApplyEndOfTurn(player, regenerate);

            // Update player stats
            QueueLevelUpdate(LevelUpdateType.PlayerStats, player.Id);

            //I'm Not DEEEAD! (TODO: Make event message specific to what happened)
            if (player.Hunger >= 100 || player.Hp <= 0.1)
                QueueScenarioPlayerDeath("Had a rough day");

            // Apply End-Of-Turn for the Level content
            _contentEngine.ApplyEndOfTurn();

            // Update Model Content: 0) End Targeting
            //                       1) Update visible contents
            //                       2) Calculate model delta to prepare for UI

            foreach (var target in _modelService.GetTargetedEnemies())
                QueueLevelUpdate(LevelUpdateType.TargetingEnd, target.Id);

            _modelService.ClearTargetedEnemies();
            _modelService.UpdateContents();

            // Queue Updates for level
            QueueLevelUpdate(LevelUpdateType.LayoutVisible, string.Empty);
            QueueLevelUpdate(LevelUpdateType.ContentVisible, string.Empty);

            // Allow passing of messages back to the UI
            _scenarioMessageService.UnBlock(false);

            // Fire a tick event
            QueueScenarioTick();
        }

        public void Attack(Compass direction)
        {
            var player = _modelService.Player;

            // Get points involved with the attack
            var location = _modelService.Player.Location;
            var attackLocation = _layoutEngine.GetPointInDirection(_modelService.Level.Grid, location, direction);

            // Invalid attack location
            if (attackLocation == CellPoint.Empty)
                return;

            // Check to see whether path is clear to attack
            var blocked = _layoutEngine.IsPathToAdjacentCellBlocked(_modelService.Level, location, attackLocation, false);

            // Get target for attack
            var enemy = _modelService.Level.GetAtPoint<Enemy>(attackLocation);

            if (enemy != null && !blocked)
            {
                //Engage enemy if they're attacked
                enemy.IsEngaged = true;

                // Enemy gets hit OR dodges
                var hit = _interactionProcessor.CalculatePlayerHit(_modelService.Player, enemy);

                if (hit <= 0 || _randomSequenceGenerator.Get() < _characterProcessor.GetDodge(enemy))
                    _scenarioMessageService.Publish(player.RogueName + " Misses");

                // Enemy hit
                else
                {
                    //Critical hit
                    if (_randomSequenceGenerator.Get() < _playerProcessor.GetCriticalHitProbability(player))
                    {
                        hit *= 2;
                        _scenarioMessageService.Publish(player.RogueName + " scores a critical hit!");
                    }
                    else
                        _scenarioMessageService.Publish(player.RogueName + " attacks");

                    enemy.Hp -= hit;

                    // TODO - process alterations attached to the player's equipment
                    //IEnumerable<Equipment> magicWeapons = this.Player.EquipmentInventory.Where(eq => eq.HasAttackSpell && eq.IsEquiped);
                    //foreach (Equipment eq in magicWeapons)
                    //{
                    //    //Set target so the enemy is processed with the spell
                    //    //e.IsTargeted = true;

                    //    ProcessPlayerMagicSpell(eq.AttackSpell);
                    //}
                }

                // Enemy counter-attacks
                if (_randomSequenceGenerator.Get() < enemy.BehaviorDetails.CurrentBehavior.CounterAttackProbability)
                {
                    player.Hp -= _interactionProcessor.CalculateEnemyHit(player, enemy);
                    _scenarioMessageService.Publish(enemy.RogueName + " counter attacks");
                }
            }
        }
        public LevelContinuationAction Throw(string itemId)
        {
            if (!_modelService.GetTargetedEnemies().Any())
                return LevelContinuationAction.DoNothing;

            var player = _modelService.Player;
            var enemy = _modelService.GetTargetedEnemies()
                                     .First();

            var thrownItem = player.Consumables[itemId];

            // TBD: Create general consumable for projectiles that has melee parameters
            if (thrownItem.HasProjectileSpell)
            {
                // Queue the spell and remove the item
                _spellEngine.QueuePlayerMagicSpell(thrownItem.ProjectileSpell);

                // Remove item from inventory
                player.Consumables.Remove(itemId);

                // Queue Level Update - Player Consumables Remove
                QueueLevelUpdate(LevelUpdateType.PlayerConsumableRemove, itemId);

                return LevelContinuationAction.ProcessTurnNoRegeneration;
            }

            return LevelContinuationAction.DoNothing;
        }
        public LevelContinuationAction Consume(string itemId)
        {
            var player = _modelService.Player;
            var consumable = player.Consumables[itemId];
            var meetsAlterationCost = _alterationProcessor.CalculatePlayerMeetsAlterationCost(player, consumable.Spell.Cost);
            var displayName = _modelService.GetDisplayName(consumable.RogueName);

            // Check for removal of item
            switch (consumable.Type)
            {
                case ConsumableType.OneUse:
                    if (meetsAlterationCost)
                    {
                        // Remove the item
                        player.Consumables.Remove(itemId);

                        // Queue an update
                        QueueLevelUpdate(LevelUpdateType.PlayerConsumableRemove, itemId);
                    }
                    else
                        return LevelContinuationAction.ProcessTurn;
                    break;
                case ConsumableType.MultipleUses:
                    {
                        if (meetsAlterationCost)
                            consumable.Uses--;
                        else
                            return LevelContinuationAction.ProcessTurn;

                        if (consumable.Uses <= 0)
                        {
                            // Remove the item
                            player.Consumables.Remove(itemId);

                            // Queue an update
                            QueueLevelUpdate(LevelUpdateType.PlayerConsumableRemove, itemId);
                        }
                    }
                    break;
                case ConsumableType.UnlimitedUses:
                default:
                    break;
            }

            // Learned SkillSet Item
            if (consumable.HasLearnedSkillSet)
            {
                if (!player.SkillSets.Any(z => z.RogueName == consumable.LearnedSkill.RogueName))
                {
                    // Message plays on dungeon turn
                    player.SkillSets.Add(consumable.LearnedSkill);

                    // Queue an update for the skill sets
                    QueueLevelUpdate(LevelUpdateType.PlayerSkillSetAdd, string.Empty);

                    _scenarioMessageService.Publish(player.RogueName + " has been granted a new skill!  \"" + consumable.LearnedSkill.RogueName + "\"");
                }
            }

            // All other types - including Ammo type
            if (consumable.HasSpell || consumable.SubType == ConsumableSubType.Ammo)
            {
                var targetedEnemy = _modelService.GetTargetedEnemies()
                                                 .FirstOrDefault();

                if (_alterationProcessor.CalculateSpellRequiresTarget(consumable.Spell) && targetedEnemy == null)
                    _scenarioMessageService.Publish("Must first target an enemy");

                else
                {
                    _scenarioMessageService.Publish("Using " + displayName);
                    
                    // Queue processing of spell
                    return _spellEngine.QueuePlayerMagicSpell((consumable.SubType == ConsumableSubType.Ammo) ? 
                                                               consumable.AmmoSpell : 
                                                               consumable.Spell);
                }
            }

            return LevelContinuationAction.ProcessTurn;
        }
        public void Identify(string itemId)
        {
            var item = _modelService.Player.Inventory[itemId];
            var metaData = _modelService.ScenarioEncyclopedia[item.RogueName];

            metaData.IsIdentified = true;
            metaData.IsCurseIdentified = true;
            item.IsIdentified = true;

            _scenarioMessageService.Publish(item.RogueName + " Identified");

            // Queue an update
            if (item is Consumable)
                QueuePlayerConsumableAddOrUpdate(itemId);
            else if (item is Equipment)
                QueuePlayerEquipmentAddOrUpdate(itemId);
        }
        public void Enchant(string equipmentId)
        {
            var equipment = _modelService.Player.Equipment[equipmentId];
            equipment.Class++;

            _scenarioMessageService.Publish("Your " + _modelService.GetDisplayName(equipment.RogueName) + " starts to glow!");

            // Queue update
            QueuePlayerEquipmentAddOrUpdate(equipmentId);
        }
        public void Uncurse(string itemId)
        {
            var equipment = _modelService.Player.Equipment[itemId];
            equipment.IsCursed = false;

            _scenarioMessageService.Publish(_modelService.GetDisplayName(equipment.RogueName) + " Uncursed");

            if (equipment.HasCurseSpell)
            {
                // TODO
                //if (equipment.EquipSpell.Type == AlterationType.PassiveSource)
                //    this.Player.DeactivatePassiveEffect(equipment.CurseSpell.Id);

                //else
                //    this.Player.DeactivatePassiveAura(equipment.CurseSpell.Id);

                _scenarioMessageService.Publish("Your " + equipment.RogueName + " is now safe to use (with caution...)");
            }

            // Queue an update
            QueuePlayerEquipmentAddOrUpdate(itemId);
        }
        public void Drop(string itemId)
        {
            _contentEngine.DropPlayerItem(itemId);
        }
        public LevelContinuationAction Fire()
        {
            var rangeWeapon = _modelService.Player.Equipment.Values.FirstOrDefault(x => x.IsEquipped && x.Type == EquipmentType.RangeWeapon);
            var targetedEnemy = _modelService.GetTargetedEnemies().FirstOrDefault();

            if (rangeWeapon == null)
                _scenarioMessageService.Publish("No Range Weapons are equipped");

            else if (targetedEnemy == null)
                _scenarioMessageService.Publish("Must first target an enemy");

            else if (_layoutEngine.RoguianDistance(_modelService.Player.Location, targetedEnemy.Location) <= ModelConstants.MIN_FIRING_DISTANCE)
                _scenarioMessageService.Publish("Too close to fire your weapon");

            else
            {
                //Should find a better way to identify ammo
                var ammo = _modelService.Player.Consumables.Values.FirstOrDefault(z => z.RogueName == rangeWeapon.AmmoName);
                if (ammo == null)
                {
                    _scenarioMessageService.Publish("No Ammunition for your weapon");
                    return LevelContinuationAction.ProcessTurn;
                }

                // Remove ammo from inventory
                _modelService.Player.Consumables.Remove(ammo.Id);

                // Queue update
                QueuePlayerConsumableRemove(ammo.Id);

                // Player Misses
                var hit = _interactionProcessor.CalculatePlayerHit(_modelService.Player, targetedEnemy);
                if (hit <= 0 || _randomSequenceGenerator.Get() < _characterProcessor.GetDodge(targetedEnemy))
                    _scenarioMessageService.Publish(_modelService.Player.RogueName + " Misses");

                // Player Hits Targeted Enemy
                else
                {
                    if (_randomSequenceGenerator.Get() < _playerProcessor.GetCriticalHitProbability(_modelService.Player))
                    {
                        _scenarioMessageService.Publish("You fire your " + rangeWeapon.RogueName + " for a critical hit!");
                        targetedEnemy.Hp -= hit * 2;
                    }
                    else
                    {
                        _scenarioMessageService.Publish("You hit the " + targetedEnemy.RogueName + " using your " + rangeWeapon.RogueName);
                        targetedEnemy.Hp -= hit;
                    }

                    return _spellEngine.QueuePlayerMagicSpell(ammo.AmmoSpell);
                }
            }
            return LevelContinuationAction.ProcessTurn;
        }
        public void Target(Compass direction)
        {
            var targetedEnemy = _modelService.GetTargetedEnemies().FirstOrDefault();
            var enemiesInRange = _modelService.GetVisibleEnemies().ToList();

            Enemy target = null;

            if (targetedEnemy != null)
            {
                // End targeting of current target
                QueueLevelUpdate(LevelUpdateType.TargetingEnd, targetedEnemy.Id);

                int targetedEnemyIndex = enemiesInRange.IndexOf(targetedEnemy);
                switch (direction)
                {
                    case Compass.E:
                        {
                            if (targetedEnemyIndex + 1 == enemiesInRange.Count)
                                target = enemiesInRange[0];
                            else
                                target = enemiesInRange[targetedEnemyIndex + 1];
                        }
                        break;
                    case Compass.W:
                        {
                            if (targetedEnemyIndex - 1 == -1)
                                target = enemiesInRange[enemiesInRange.Count - 1];
                            else
                                target = enemiesInRange[targetedEnemyIndex - 1];
                        }
                        break;
                    default:
                        target = enemiesInRange[0];
                        break;
                }
            }
            else
            {
                if (enemiesInRange.Count > 0)
                    target = enemiesInRange[0];
            }

            // Start targeting of Enemy
            if (target != null)
            {
                // Set the targeted enemy
                _modelService.SetTargetedEnemy(target);

                // Queue update to level to show animation
                QueueLevelUpdate(LevelUpdateType.TargetingStart, target.Id);
            }
        }
        public void ToggleActiveSkill(string skillSetId, bool activate)
        {
            var skillSet = _modelService.Player.SkillSets.FirstOrDefault(z => z.Id == skillSetId);
            bool isActive = skillSet.IsActive;

            if (isActive && activate)
                return;

            if (!isActive && !activate)
                return;

            //Good measure - set non active for all skill sets
            foreach (var playerSkillSet in _modelService.Player.SkillSets)
                playerSkillSet.IsActive = false;

            //Shut off aura effects
            foreach (var s in _modelService.Player.SkillSets)
            {
                //Maintain aura effects
                if (s.IsTurnedOn)
                {

                    s.IsTurnedOn = false;
                    _scenarioMessageService.Publish("Deactivating " + s.RogueName);

                    // TODO
                    //if (s.CurrentSkill.Type == AlterationType.PassiveAura)
                    //    this.Player.DeactivatePassiveAura(s.CurrentSkill.Id);

                    //else
                    //    this.Player.DeactivatePassiveEffect(s.CurrentSkill.Id);
                }
            }

            if (skillSet != null)
                skillSet.IsActive = !isActive;

            // Queue update for all skill sets
            QueueLevelUpdate(LevelUpdateType.PlayerSkillSetRefresh, _modelService.Player.SkillSets.Select(x => x.Id).ToArray());
        }
        public void EmphasizeSkillUp(string skillSetId)
        {
            var skillSet = _modelService.Player.SkillSets.FirstOrDefault(z => z.Id == skillSetId);
            if (skillSet != null)
            {
                if (skillSet.Emphasis < 3)
                    skillSet.Emphasis++;
            }

            QueueLevelUpdate(LevelUpdateType.PlayerSkillSetRefresh, skillSet.Id);
        }
        public void EmphasizeSkillDown(string skillSetId)
        {
            var skillSet = _modelService.Player.SkillSets.FirstOrDefault(z => z.Id == skillSetId);
            if (skillSet != null)
            {
                if (skillSet.Emphasis > 0)
                    skillSet.Emphasis--;
            }

            QueueLevelUpdate(LevelUpdateType.PlayerSkillSetRefresh, skillSet.Id);
        }
        public LevelContinuationAction InvokePlayerSkill()
        {
            var activeSkillSet = _modelService.Player.SkillSets.FirstOrDefault(z => z.IsActive == true);

            // No Active Skill Set
            if (activeSkillSet == null)
            {
                _scenarioMessageService.Publish("No Active Skill - (See Skills Panel to Set)");
                return LevelContinuationAction.ProcessTurn;
            }

            // No Current Skill (Could throw Exception)
            var currentSkill = activeSkillSet.GetCurrentSkill();
            if (currentSkill == null)
            {
                _scenarioMessageService.Publish("No Active Skill - (See Skills Panel to Set)");
                return LevelContinuationAction.ProcessTurn;
            }

            var enemyTargeted = _modelService.GetTargetedEnemies().FirstOrDefault();

            // Requires Target
            if (_alterationProcessor.CalculateSpellRequiresTarget(activeSkillSet.GetCurrentSkill()) && enemyTargeted == null)
                _scenarioMessageService.Publish(activeSkillSet.RogueName + " requires a targeted enemy");

            // Meets Alteration Cost?
            if (!_alterationProcessor.CalculatePlayerMeetsAlterationCost(_modelService.Player, currentSkill.Cost))
                return LevelContinuationAction.ProcessTurn;

            // For passives - work with IsTurnedOn flag
            if (activeSkillSet.GetCurrentSkill().Type == AlterationType.PassiveAura ||
                activeSkillSet.GetCurrentSkill().Type == AlterationType.PassiveSource)
            {
                // Turn off passive if it's turned on
                if (activeSkillSet.IsTurnedOn)
                {
                    _scenarioMessageService.Publish("Deactivating - " + activeSkillSet.RogueName);
                    // TODO
                    //this.Player.DeactivatePassiveEffect(activeSkillSet.CurrentSkill.Id);
                    activeSkillSet.IsTurnedOn = false;
                }
                // Turn on the passive and queue processing
                else
                {
                    activeSkillSet.IsTurnedOn = true;

                    // Queue processing -> Animation -> Process parameters (backend)
                    return  _spellEngine.QueuePlayerMagicSpell(currentSkill);
                }
            }
            // All other skill types
            else
            {
                // Publish message
                _scenarioMessageService.Publish("Invoking - " + activeSkillSet.RogueName);

                // Queue processing -> Animation -> Process parameters (backend)
                return _spellEngine.QueuePlayerMagicSpell(currentSkill);
            }

            return LevelContinuationAction.ProcessTurn;
        }
        public LevelContinuationAction InvokeDoodad()
        {
            var player = _modelService.Player;
            var doodad = _modelService.Level.GetAtPoint<DoodadBase>(player.Location);
            if (doodad == null)
            {
                _scenarioMessageService.Publish("Nothing here to use! (Requires Scenario Object)");

                return LevelContinuationAction.DoNothing;
            }

            //Sets identified
            _modelService.ScenarioEncyclopedia[doodad.RogueName].IsIdentified = true;

            switch (doodad.Type)
            {
                case DoodadType.Magic:
                    {
                        if (doodad.IsOneUse && doodad.HasBeenUsed || !((DoodadMagic)doodad).IsInvoked)
                            _scenarioMessageService.Publish("Nothing Happens");
                        else
                        {
                            _scenarioMessageService.Publish("Using " + doodad.RogueName);

                            var doodadMagic = (DoodadMagic)doodad;
                            doodadMagic.HasBeenUsed = true;

                            return _spellEngine.QueuePlayerMagicSpell(doodadMagic.InvokedSpell);
                        }
                    }
                    break;
                case DoodadType.Normal:
                    {
                        switch (((DoodadNormal)doodad).NormalType)
                        {
                            case DoodadNormalType.SavePoint:
                                QueueScenarioSave();
                                break;
                            case DoodadNormalType.StairsDown:
                                QueueScenarioLevelChange(_modelService.Level.Number + 1, PlayerStartLocation.StairsUp);
                                break;
                            case DoodadNormalType.StairsUp:
                                QueueScenarioLevelChange(_modelService.Level.Number - 1, PlayerStartLocation.StairsDown);
                                break;
                        }
                    }
                    break;
            }

            return LevelContinuationAction.ProcessTurn;
        }

        public void ApplyEndOfTurn()
        {
            throw new NotImplementedException();
        }

        #region (private) Event Methods
        private void QueueLevelUpdate(LevelUpdateType type, string contentId)
        {
            LevelUpdateEvent(this, new LevelUpdate()
            {
                LevelUpdateType = type,
                ContentIds = new string[] {contentId}
            });
        }
        private void QueueLevelUpdate(LevelUpdateType type, string[] contentIds)
        {
            LevelUpdateEvent(this, new LevelUpdate()
            {
                LevelUpdateType = type,
                ContentIds = contentIds
            });
        }
        private void QueuePlayerConsumableAddOrUpdate(string consumableId)
        {
            LevelUpdateEvent(this, new LevelUpdate()
            {
                LevelUpdateType = LevelUpdateType.PlayerConsumableAddOrUpdate,
                ContentIds = new string[] {consumableId}
            });
        }
        private void QueuePlayerConsumableRemove(string consumableId)
        {
            LevelUpdateEvent(this, new LevelUpdate()
            {
                LevelUpdateType = LevelUpdateType.PlayerConsumableRemove,
                ContentIds = new string[] { consumableId }
            });
        }
        private void QueuePlayerEquipmentAddOrUpdate(string equipmentId)
        {
            LevelUpdateEvent(this, new LevelUpdate()
            {
                LevelUpdateType = LevelUpdateType.PlayerEquipmentAddOrUpdate,
                ContentIds = new string[] { equipmentId }
            });
        }
        private void QueuePlayerEquipmentRemove(string equipmentId)
        {
            LevelUpdateEvent(this, new LevelUpdate()
            {
                LevelUpdateType = LevelUpdateType.PlayerEquipmentRemove,
                ContentIds = new string[] { equipmentId }
            });
        }
        private void QueueScenarioLevelChange(int levelNumber, PlayerStartLocation playerStartLocation)
        {
            ScenarioUpdateEvent(this, new ScenarioUpdate()
            {
                ScenarioUpdateType = ScenarioUpdateType.LevelChange,
                LevelNumber = levelNumber,
                StartLocation = playerStartLocation
            });
        }
        private void QueueScenarioPlayerDeath(string deathMessage)
        {
            ScenarioUpdateEvent(this, new ScenarioUpdate()
            {
                ScenarioUpdateType = ScenarioUpdateType.PlayerDeath,
                PlayerDeathMessage = deathMessage
            });
        }
        private void QueueScenarioSave()
        {
            ScenarioUpdateEvent(this, new ScenarioUpdate()
            {
                ScenarioUpdateType = ScenarioUpdateType.Save
            });
        }
        private void QueueScenarioTick()
        {
            ScenarioUpdateEvent(this, new ScenarioUpdate()
            {
                ScenarioUpdateType = ScenarioUpdateType.StatisticsTick
            });
        }
        #endregion
    }
}
