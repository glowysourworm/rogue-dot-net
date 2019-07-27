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
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using Rogue.NET.Core.Logic.Static;
using System.Collections.Generic;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.ScenarioMessage;
using Rogue.NET.Core.Model.Scenario.Content.Skill.Extension;
using Rogue.NET.Core.Logic.Content.Enum;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Logic.Processing.Factory.Interface;
using Rogue.NET.Core.Model.Scenario.Content.Extension;
using System.Windows.Media;

namespace Rogue.NET.Core.Logic
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioEngine))]
    public class ScenarioEngine : IScenarioEngine
    {
        readonly ILayoutEngine _layoutEngine;
        readonly IContentEngine _contentEngine;
        readonly ISpellEngine _spellEngine;
        readonly IReligionEngine _religionEngine;
        readonly IModelService _modelService;
        readonly IScenarioMessageService _scenarioMessageService;
        readonly IInteractionProcessor _interactionProcessor;
        readonly IPlayerProcessor _playerProcessor;
        readonly IAlterationProcessor _alterationProcessor;
        readonly IRandomSequenceGenerator _randomSequenceGenerator;
        readonly IRogueUpdateFactory _rogueUpdateFactory;

        public event EventHandler<RogueUpdateEventArgs> RogueUpdateEvent;
        public event EventHandler<ILevelProcessingAction> LevelProcessingActionEvent;

        [ImportingConstructor]
        public ScenarioEngine(
            ILayoutEngine layoutEngine,
            IContentEngine contentEngine,
            ISpellEngine spellEngine,
            IReligionEngine religionEngine,
            IModelService modelService,
            IScenarioMessageService scenarioMessageService,
            IInteractionProcessor interactionProcessor,
            IPlayerProcessor playerProcessor,
            IAlterationProcessor alterationProcessor,
            IRandomSequenceGenerator randomSequenceGenerator,
            IRogueUpdateFactory rogueUpdateFactory)
        {
            _layoutEngine = layoutEngine;
            _contentEngine = contentEngine;
            _spellEngine = spellEngine;
            _religionEngine = religionEngine;
            _modelService = modelService;
            _scenarioMessageService = scenarioMessageService;
            _interactionProcessor = interactionProcessor;
            _playerProcessor = playerProcessor;
            _alterationProcessor = alterationProcessor;
            _randomSequenceGenerator = randomSequenceGenerator;
            _rogueUpdateFactory = rogueUpdateFactory;
        }

        // Pre-check player state before applying desired command
        public LevelContinuationAction ProcessAlteredPlayerState()
        {
            var player = _modelService.Player;

            foreach (var alteredState in player.Alteration.GetStates())
            {
                // Can't Move
                if (alteredState.BaseType == CharacterStateType.CantMove)
                {
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, player.RogueName + " is " + alteredState.RogueName);
                    return LevelContinuationAction.ProcessTurn;
                }

                // Moves Randomly
                else if (alteredState.BaseType == CharacterStateType.MovesRandomly)
                {
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, player.RogueName + " is " + alteredState.RogueName);
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
            var desiredLocation = _modelService.Level.Grid.GetPointInDirection(_modelService.Player.Location, direction);

            // Invalid location
            if (desiredLocation == CellPoint.Empty)
                return null;

            //Look for road blocks - move player
            if (!_layoutEngine.IsPathToAdjacentCellBlocked(_modelService.Level, _modelService.Player.Location, desiredLocation, true))
            {
                // Update player location
                _modelService.Player.Location = desiredLocation;

                // Notify Listener queue
                RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerLocation, _modelService.Player.Id, RogueUpdatePriority.High));
            }

            //See what the player stepped on... Prefer Items first
            return _modelService.Level.GetAtPoint<ItemBase>(_modelService.Player.Location) ??
                   _modelService.Level.GetAtPoint<ScenarioObject>(_modelService.Player.Location);
        }
        public ScenarioObject MoveRandom()
        {
            // Get random adjacent location
            var desiredLocation = _layoutEngine.GetRandomAdjacentLocation(_modelService.Level, _modelService.Player, _modelService.Player.Location, true);

            // Get direction for random move -> Move()
            var direction = LevelGridExtension.GetDirectionBetweenAdjacentPoints(_modelService.Player.Location, desiredLocation);

            return Move(direction);
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
            var playerAdvancement = false;

            // Player: End-Of-Turn
            _playerProcessor.ApplyEndOfTurn(player, regenerate, out playerAdvancement);

            // Player Advancement Event
            if (playerAdvancement)
                RogueUpdateEvent(this, _rogueUpdateFactory.DialogPlayerAdvancement(player, 1));

            // Update player stats
            RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerStats, _modelService.Player.Id));

            //I'm Not DEEEAD!
            if (player.Hunger >= 100 || player.Hp <= 0.1)
            {
                var killedBy = _modelService.GetKilledBy();
                if (killedBy != null)
                    RogueUpdateEvent(this, _rogueUpdateFactory.PlayerDeath("Killed by " + killedBy));
                else
                    RogueUpdateEvent(this, _rogueUpdateFactory.PlayerDeath("Had a rough day..."));
            }

            // Apply End-Of-Turn for the Level content
            _contentEngine.ApplyEndOfTurn();

            // Update Model Content: 0) End Targeting
            //                       1) Update visible contents
            //                       2) Calculate model delta to prepare for UI

            foreach (var target in _modelService.GetTargetedEnemies())
                RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.TargetingEnd, target.Id));

            _modelService.UpdateVisibleLocations();
            _modelService.ClearTargetedEnemies();
            _modelService.UpdateContents();

            // Queue Updates for level
            // QueueLevelUpdate(LevelUpdateType.LayoutVisible, string.Empty);
            RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.ContentVisible, ""));

            // Fire a tick event to update level ticks
            RogueUpdateEvent(this, _rogueUpdateFactory.Tick());
        }

        public void Attack(Compass direction)
        {
            var player = _modelService.Player;

            // Get points involved with the attack
            var location = _modelService.Player.Location;
            var attackLocation = _modelService.Level.Grid.GetPointInDirection(location, direction);

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

                // Set flag to allow enemy to fight back if they're attacked (even if player is invisible)
                enemy.WasAttackedByPlayer = true;

                // Enemy gets hit OR dodges
                _interactionProcessor.CalculateInteraction(_modelService.Player, enemy, PhysicalAttackType.Melee);
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

            // Check that thrown item has level requirement met (ALSO DONE ON FRONT END)
            if (thrownItem.LevelRequired > player.Level)
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Required Level {0} Not Met!", thrownItem.LevelRequired.ToString());
                return LevelContinuationAction.DoNothing;
            }

            // Check Religious Affiliation Requirement
            if (thrownItem.HasReligionRequirement &&
               (!player.ReligiousAlteration.IsAffiliated() ||
                 player.ReligiousAlteration.Religion.RogueName != thrownItem.Religion.RogueName))
            {
                _scenarioMessageService.Publish(
                    ScenarioMessagePriority.Normal,
                    "Required Religious Affiliation {0} Not Met!",
                    thrownItem.Religion.RogueName);

                return LevelContinuationAction.DoNothing;
            }

            // TBD: Create general consumable for projectiles that has melee parameters
            if (thrownItem.HasProjectileSpell)
            {
                // Queue the spell and remove the item
                _spellEngine.QueuePlayerMagicSpell(thrownItem.ProjectileSpell);

                // Remove item from inventory
                player.Consumables.Remove(itemId);

                // Queue Level Update - Player Consumables Remove
                RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerConsumableRemove, itemId));

                return LevelContinuationAction.ProcessTurnNoRegeneration;
            }

            return LevelContinuationAction.DoNothing;
        }
        public LevelContinuationAction Consume(string itemId)
        {
            var player = _modelService.Player;
            var consumable = player.Consumables[itemId];
            var meetsAlterationCost = _alterationProcessor.CalculatePlayerMeetsAlterationCost(player, consumable.Spell.Cost);
            var displayName = _modelService.GetDisplayName(consumable);

            // Check that item has level requirement met (ALSO DONE ON FRONT END)
            if (consumable.LevelRequired > player.Level)
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Required Level {0} Not Met!", consumable.LevelRequired.ToString());
                return LevelContinuationAction.DoNothing;
            }

            // Check Religious Affiliation Requirement
            if (consumable.HasReligionRequirement &&
               (!player.ReligiousAlteration.IsAffiliated() ||
                 player.ReligiousAlteration.Religion.RogueName != consumable.Religion.RogueName))
            {
                _scenarioMessageService.Publish(
                    ScenarioMessagePriority.Normal,
                    "Required Religious Affiliation {0} Not Met!",
                    consumable.Religion.RogueName);

                return LevelContinuationAction.DoNothing;
            }

            // Check for targeting - including Ammo type
            if (consumable.HasSpell || consumable.SubType == ConsumableSubType.Ammo)
            {
                var targetedEnemy = _modelService.GetTargetedEnemies()
                                                 .FirstOrDefault();

                if (_alterationProcessor.CalculateSpellRequiresTarget(consumable.Spell) && targetedEnemy == null)
                {
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Must first target an enemy");
                    return LevelContinuationAction.DoNothing;
                }
            }

            // Check for Religious Affiliation
            //
            // 0) Player does NOT have an affiliation (OK)
            // 1) Player has the SAME affiliation (OK)
            // 2) Player has a DIFFERENT affiliation (NOT OK)
            //
            if (consumable.HasSpell &&
                consumable.Spell.OtherEffectType == AlterationMagicEffectType.AffiliateReligion &&
                player.ReligiousAlteration.IsAffiliated() &&
                player.ReligiousAlteration.Religion.RogueName != consumable.Spell.Religion.RogueName)
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "You must first Renounce your Religion (Press \"R\" to Renounce)");
                return LevelContinuationAction.DoNothing;
            }

            // Proceeding with use - so check for identify on use
            if (consumable.IdentifyOnUse)
            {
                // Set consumable identified
                consumable.IsIdentified = true;

                _modelService.ScenarioEncyclopedia[consumable.RogueName].IsIdentified = true;
                _modelService.ScenarioEncyclopedia[consumable.RogueName].IsCurseIdentified = true;

                // Update UI
                RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.EncyclopediaIdentify, consumable.Id));
            }

            // Check for removal of item
            switch (consumable.Type)
            {
                case ConsumableType.OneUse:
                    if (meetsAlterationCost)
                    {
                        // Remove the item
                        player.Consumables.Remove(itemId);

                        // Queue an update
                        RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerConsumableRemove, itemId));
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
                            RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerConsumableRemove, itemId));
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

                    // Set Skill Identified
                    _modelService.ScenarioEncyclopedia[consumable.LearnedSkill.RogueName].IsIdentified = true;

                    // Queue an update for the skill sets
                    RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerSkillSetAdd, ""));
                    RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.EncyclopediaIdentify, consumable.LearnedSkill.Id));

                    _scenarioMessageService.Publish(ScenarioMessagePriority.Good, player.RogueName + " has been granted a new skill!  \"" + consumable.LearnedSkill.RogueName + "\"");
                }
                return LevelContinuationAction.ProcessTurn;
            }

            // Notes
            if (consumable.SubType == ConsumableSubType.Note)
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Reading " + displayName);

                RogueUpdateEvent(this, _rogueUpdateFactory.DialogNote(consumable.NoteMessage, displayName));

                return LevelContinuationAction.ProcessTurn;
            }

            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Using " + displayName);

            // Queue processing of spell
            return _spellEngine.QueuePlayerMagicSpell((consumable.SubType == ConsumableSubType.Ammo) ?
                                                       consumable.AmmoSpell :
                                                       consumable.Spell);
        }
        public void Identify(string itemId)
        {
            var item = _modelService.Player.Inventory[itemId];
            var metaData = _modelService.ScenarioEncyclopedia[item.RogueName];

            metaData.IsIdentified = true;
            metaData.IsCurseIdentified = true;
            item.IsIdentified = true;

            _scenarioMessageService.Publish(ScenarioMessagePriority.Good, item.RogueName + " Identified");

            // Check for Religious Affiliation and Identify
            if (item.HasReligionRequirement)
                _religionEngine.IdentifyReligion(item.Religion);

            // Queue an update
            if (item is Consumable)
                RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerConsumableAddOrUpdate, itemId));
            else if (item is Equipment)
                RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerEquipmentAddOrUpdate, itemId));

            // Queue meta-data update
            RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.EncyclopediaIdentify, itemId));
        }
        public void Enchant(string equipmentId)
        {
            var equipment = _modelService.Player.Equipment[equipmentId];
            equipment.Class++;

            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Your " + _modelService.GetDisplayName(equipment) + " starts to glow!");

            // Queue update
            RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerEquipmentAddOrUpdate, equipmentId));
        }
        public void ImbueArmor(string equipmentId, IEnumerable<AttackAttribute> attackAttributes)
        {
            var armor = _modelService.Player.Equipment[equipmentId];

            // Update Resistance Attribute
            foreach (var attackAttribute in attackAttributes)
            {
                var armorAttribute = armor.AttackAttributes.First(x => x.RogueName == attackAttribute.RogueName);

                // Increment Resistance
                armorAttribute.Resistance += attackAttribute.Resistance;

                // Publish message
                if (armorAttribute.Resistance > 0)
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Good,
                        "{0} has increased {1} Resistance by {2}",
                        _modelService.GetDisplayName(armor),
                        attackAttribute.RogueName,
                        attackAttribute.Resistance.ToString("F2"));
            }

            // Queue update
            RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerEquipmentAddOrUpdate, equipmentId));
        }
        public void ImbueWeapon(string equipmentId, IEnumerable<AttackAttribute> attackAttributes)
        {
            var weapon = _modelService.Player.Equipment[equipmentId];

            // Update Resistance Attribute
            foreach (var attackAttribute in attackAttributes)
            {
                var weaponAttribute = weapon.AttackAttributes.First(x => x.RogueName == attackAttribute.RogueName);

                // Increment Attack
                weaponAttribute.Attack += attackAttribute.Attack;

                // Publish message
                if (weaponAttribute.Resistance > 0)
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Good,
                        "{0} has increased {1} Attack by {2}",
                        _modelService.GetDisplayName(weapon),
                        attackAttribute.RogueName,
                        attackAttribute.Resistance.ToString("F2"));
            }

            // Queue update
            RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerEquipmentAddOrUpdate, equipmentId));
        }
        public void Uncurse(string itemId)
        {
            var equipment = _modelService.Player.Equipment[itemId];
            equipment.IsCursed = false;

            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, _modelService.GetDisplayName(equipment) + " Uncursed");

            if (equipment.HasCurseSpell)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Your " + equipment.RogueName + " is now safe to use (with caution...)");

            if (equipment.IsEquipped)
                _modelService.Player.Alteration.DeactivatePassiveAlteration(equipment.CurseSpell.Id);

            // Queue an update
            RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerEquipmentAddOrUpdate, itemId));
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
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "No Range Weapons are equipped");

            else if (targetedEnemy == null)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Must first target an enemy");

            else if (Calculator.RoguianDistance(_modelService.Player.Location, targetedEnemy.Location) <= ModelConstants.MinFiringDistance)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Too close to fire your weapon");

            else
            {
                //Should find a better way to identify ammo
                var ammo = _modelService.Player.Consumables.Values.FirstOrDefault(z => z.RogueName == rangeWeapon.AmmoName);
                if (ammo == null)
                {
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "No Ammunition for your weapon");
                    return LevelContinuationAction.ProcessTurn;
                }

                // Check Religious Affiliation Requirement
                if (ammo.HasReligionRequirement &&
                   (!_modelService.Player.ReligiousAlteration.IsAffiliated() ||
                     _modelService.Player.ReligiousAlteration.Religion.RogueName != ammo.Religion.RogueName))
                {
                    _scenarioMessageService.Publish(
                        ScenarioMessagePriority.Normal,
                        "Required Religious Affiliation {0} Not Met!",
                        ammo.Religion.RogueName);

                    return LevelContinuationAction.ProcessTurn;
                }

                // Remove ammo from inventory
                _modelService.Player.Consumables.Remove(ammo.Id);

                // Queue update
                RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerConsumableRemove, ammo.Id));

                // Calculate hit - if enemy hit then queue Ammunition spell
                var enemyHit = _interactionProcessor.CalculateInteraction(_modelService.Player, targetedEnemy, PhysicalAttackType.Range);

                // If enemy hit then process the spell associated with the ammo
                if (enemyHit)
                {
                    return _spellEngine.QueuePlayerMagicSpell(ammo.AmmoSpell);
                }

                // Otherwise, process the animation only
                else if (ammo.AmmoSpell.Animations.Any())
                {
                    RogueUpdateEvent(this, 
                        _rogueUpdateFactory.Animation(
                            ammo.AmmoSpell.Animations, 
                            _modelService.Player.Location, 
                            new CellPoint[] { targetedEnemy.Location }));
                }
            }
            return LevelContinuationAction.ProcessTurn;
        }
        public void Target(Compass direction)
        {
            var targetedEnemy = _modelService.GetTargetedEnemies().FirstOrDefault();
            var enemiesInRange = _modelService.GetVisibleEnemies()
                                              .ToList();

            // Filter out invisible enemies
            if (!_modelService.Player.Alteration.CanSeeInvisibleCharacters())
            {
                enemiesInRange = enemiesInRange.Where(x => !x.IsInvisible && !x.Is(CharacterStateType.Invisible))
                                               .ToList();
            }

            Enemy target = null;

            if (targetedEnemy != null)
            {
                // End targeting of current target
                RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.TargetingEnd, targetedEnemy.Id));

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

            // Clear targeted Enemies regardless of next target
            _modelService.ClearTargetedEnemies();

            // Start targeting of Enemy
            if (target != null)
            {
                // Set the targeted enemy
                _modelService.SetTargetedEnemy(target);

                // Queue update to level to show animation
                RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.TargetingStart, target.Id));
            }
        }
        public void SelectSkill(string skillId)
        {
            var player = _modelService.Player;
            var skillSet = player.SkillSets.FirstOrDefault(x => x.Skills.Any(z => z.Id == skillId));

            if (skillSet != null)
            {
                var skill = skillSet.Skills.First(x => x.Id == skillId);

                if (!skill.IsLearned)
                    throw new Exception("Trying to select non-learned skill");

                // Select the skill
                skillSet.SelectSkill(skillId);

                // Update Player Symbol
                RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerSkillSetRefresh, player.Id));
                RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerLocation, player.Id));
            }
        }
        public void CycleActiveSkillSet()
        {
            var activeSkill = _modelService.Player.SkillSets.FirstOrDefault(x => x.IsActive);
            var learnedSkills = _modelService.Player.SkillSets.Where(x => x.Skills.Any(z => z.IsLearned));

            if (!learnedSkills.Any())
                return;

            // No Active Skill
            if (activeSkill == null)
            {
                var firstLearnedSkill = learnedSkills.FirstOrDefault();
                if (firstLearnedSkill != null)
                    ToggleActiveSkillSet(firstLearnedSkill.Id, true);
                else
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "No Learned Skills");
            }

            // Cycle to next skill
            else
            {
                var skillList = learnedSkills.ToList();
                var activeSkillIndex = skillList.IndexOf(activeSkill);
                var nextIndex = activeSkillIndex;

                // Calculate index of next skill
                if (activeSkillIndex == skillList.Count - 1)
                    nextIndex = 0;
                else
                    nextIndex = activeSkillIndex + 1;

                // Set active skill
                var nextSkill = skillList[nextIndex];
                if (nextSkill != null)
                    ToggleActiveSkillSet(nextSkill.Id, true);
                else
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "No Other Learned Skills");
            }
        }
        public void ToggleActiveSkillSet(string skillSetId, bool activate)
        {
            var skillSet = _modelService.Player.SkillSets.FirstOrDefault(z => z.Id == skillSetId);
            bool isActive = skillSet.IsActive;

            if (isActive && activate)
                return;

            if (!isActive && !activate)
                return;

            // Maintain Passive Effects
            _playerProcessor.DeActivateSkills(_modelService.Player);

            // Activate
            if (skillSet != null)
            {
                skillSet.IsActive = !isActive || activate;

                if (skillSet.IsActive)
                {
                    // If no skill selected then select first skill
                    if (skillSet.SelectedSkill == null)
                        skillSet.SelectSkillDown(_modelService.Player);

                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Activating " + skillSet.RogueName);
                }
            }

            // Queue update for all skill sets
            RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerSkillSetRefresh, ""));
            RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerLocation, _modelService.Player.Id));
        }
        public void UnlockSkill(string skillId)
        {
            var player = _modelService.Player;
            var skillSet = player.SkillSets.FirstOrDefault(x => x.Skills.Any(z => z.Id == skillId));

            if (skillSet != null)
            {
                var skill = skillSet.Skills.First(x => x.Id == skillId);

                // Check skill requirements
                if (skill.AreRequirementsMet(player))
                {
                    // Decrement skill points
                    player.SkillPoints -= skill.SkillPointRequirement;

                    // Set IsLearned true
                    skill.IsLearned = true;

                    _scenarioMessageService.Publish(ScenarioMessagePriority.Good, player.RogueName + " Has Learned " + skill.Alteration.DisplayName);

                    // Select skill if none selected
                    if (skillSet.SelectedSkill == null)
                        skillSet.SelectSkill(skillId);

                    RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerSkillSetRefresh, ""));
                }
            }
        }
        public LevelContinuationAction InvokePlayerSkill()
        {
            var activeSkillSet = _modelService.Player.SkillSets.FirstOrDefault(z => z.IsActive == true);

            // No Active Skill Set
            if (activeSkillSet == null)
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "No Active Skill - (See Skills Panel to Set)");
                return LevelContinuationAction.DoNothing;
            }

            // No Current Skill (Could throw Exception)
            var currentSkill = activeSkillSet.GetCurrentSkillAlteration();
            if (currentSkill == null)
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "No Active Skill - (See Skills Panel to Set)");
                return LevelContinuationAction.DoNothing;
            }

            var enemyTargeted = _modelService.GetTargetedEnemies().FirstOrDefault();

            // Requires Target
            if (_alterationProcessor.CalculateSpellRequiresTarget(currentSkill) && enemyTargeted == null)
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, activeSkillSet.RogueName + " requires a targeted enemy");
                return LevelContinuationAction.DoNothing;
            }

            // Meets Alteration Cost?
            if (!_alterationProcessor.CalculatePlayerMeetsAlterationCost(_modelService.Player, currentSkill.Cost))
                return LevelContinuationAction.DoNothing;

            // For passives - work with IsTurnedOn flag
            if (currentSkill.IsPassive())
            {
                // Turn off passive if it's turned on
                if (activeSkillSet.IsTurnedOn)
                {
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Deactivating - " + currentSkill.DisplayName);

                    // Pass - through method is safe
                    _modelService.Player.Alteration.DeactivatePassiveAlteration(currentSkill.Id);

                    activeSkillSet.IsTurnedOn = false;
                }
                // Turn on the passive and queue processing
                else
                {
                    activeSkillSet.IsTurnedOn = true;

                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Invoking - " + currentSkill.DisplayName);

                    // Queue processing -> Animation -> Process parameters (backend)
                    return _spellEngine.QueuePlayerMagicSpell(currentSkill);
                }
            }
            // All other skill types
            else
            {
                // Publish message
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Invoking - " + currentSkill.DisplayName);

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
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Nothing here to use! (Requires Scenario Object)");

                return LevelContinuationAction.DoNothing;
            }

            //Sets identified
            _modelService.ScenarioEncyclopedia[doodad.RogueName].IsIdentified = true;

            // Update meta-data UI
            RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.EncyclopediaIdentify, doodad.Id));

            // Update statistics
            RogueUpdateEvent(this, _rogueUpdateFactory.StatisticsUpdate(ScenarioUpdateType.StatisticsDoodadUsed, doodad.RogueName));

            switch (doodad.Type)
            {
                case DoodadType.Magic:
                    {
                        var doodadMagic = (DoodadMagic)doodad;

                        // Identify Religion
                        if (doodadMagic.HasReligionRequirement)
                            _religionEngine.IdentifyReligion(doodadMagic.Religion);

                        // Has been exhausted
                        if (doodadMagic.IsOneUse && doodadMagic.HasBeenUsed || !doodadMagic.IsInvoked)
                            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Nothing Happens");

                        // Check for Religious Affiliation
                        //
                        // 0) Player does NOT have an affiliation (OK)
                        // 1) Player has the SAME affiliation (OK)
                        // 2) Player has a DIFFERENT affiliation (NOT OK)
                        //
                        else if (doodadMagic.IsInvoked &&
                                 doodadMagic.InvokedSpell.OtherEffectType == AlterationMagicEffectType.AffiliateReligion &&
                                 player.ReligiousAlteration.IsAffiliated() &&
                                 player.ReligiousAlteration.Religion.RogueName != doodadMagic.InvokedSpell.Religion.RogueName)
                        {
                            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "You must first Renounce your Religion (Press \"R\" to Renounce)");
                        }

                        else
                        {
                            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Using " + doodad.RogueName);

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
                                RogueUpdateEvent(this, _rogueUpdateFactory.Save());
                                break;
                            case DoodadNormalType.StairsDown:
                                RogueUpdateEvent(this, _rogueUpdateFactory.LevelChange(_modelService.Level.Number + 1, PlayerStartLocation.StairsUp));
                                break;
                            case DoodadNormalType.StairsUp:
                                RogueUpdateEvent(this, _rogueUpdateFactory.LevelChange(_modelService.Level.Number - 1, PlayerStartLocation.StairsDown));
                                break;
                        }
                    }
                    break;
            }

            return LevelContinuationAction.ProcessTurn;
        }
        public void PlayerAdvancement(double strength, double agility, double intelligence, int skillPoints)
        {
            var player = _modelService.Player;

            var attributeList = new List<Tuple<string, double, Color>>();

            if (player.StrengthBase != strength)
            {
                attributeList.Add(new Tuple<string, double, Color>("Strength", strength, Colors.Red));

                player.StrengthBase = strength;
            }

            if (player.AgilityBase != agility)
            {
                attributeList.Add(new Tuple<string, double, Color>("Agility", agility, Colors.YellowGreen));

                player.AgilityBase = agility;
            }

            if (player.IntelligenceBase != intelligence)
            {
                attributeList.Add(new Tuple<string, double, Color>("Intelligence", intelligence, Colors.Blue));

                player.IntelligenceBase = intelligence;
            }

            if (player.SkillPoints != skillPoints)
            {
                _scenarioMessageService.Publish(
                    ScenarioMessagePriority.Unique, 
                    string.Format("{0} has earned {1} Skill Points",
                                  player.RogueName,
                                  (skillPoints - player.SkillPoints).ToString()));

                player.SkillPoints = skillPoints;
            }

            // Publish advancement messages
            if (attributeList.Count > 0)
                _scenarioMessageService.PublishPlayerAdvancement(ScenarioMessagePriority.Good, player.RogueName, player.Level, attributeList);
        }

        public void ApplyEndOfTurn()
        {
            throw new NotImplementedException();
        }
    }
}
