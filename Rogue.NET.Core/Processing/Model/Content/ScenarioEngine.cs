using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Doodad;

using System.Linq;
using System.ComponentModel.Composition;
using System;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System.Collections.Generic;
using Rogue.NET.Core.Model.Scenario.Content.Skill.Extension;
using Rogue.NET.Core.Model.Scenario.Content.Extension;
using System.Windows.Media;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Processing.Event.Backend.EventData.Factory.Interface;
using Rogue.NET.Core.Processing.Event.Backend.EventData.ScenarioMessage.Enum;
using Rogue.NET.Core.GameRouter.GameEvent.Backend.Enum;
using Rogue.NET.Core.Processing.Model.Content.Interface;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Model.Content.Enum;
using Rogue.NET.Core.Processing.Model.Static;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Processing.Event.Backend.Enum;
using Rogue.NET.Core.Model.Scenario.Alteration.Common.Extension;

namespace Rogue.NET.Core.Processing.Model.Content
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioEngine))]
    public class ScenarioEngine : BackendEngine, IScenarioEngine
    {
        readonly ILayoutEngine _layoutEngine;
        readonly IContentEngine _contentEngine;
        readonly IAlterationEngine _alterationEngine;
        readonly IModelService _modelService;
        readonly ITargetingService _targetingService;
        readonly IScenarioMessageService _scenarioMessageService;
        readonly IInteractionProcessor _interactionProcessor;
        readonly IPlayerProcessor _playerProcessor;        
        readonly IAlterationProcessor _alterationProcessor;
        readonly IAlterationGenerator _alterationGenerator;
        readonly IBackendEventDataFactory _backendEventDataFactory;

        [ImportingConstructor]
        public ScenarioEngine(
            ILayoutEngine layoutEngine,
            IContentEngine contentEngine,
            IAlterationEngine alterationEngine,
            IModelService modelService,
            ITargetingService targetingService,
            IScenarioMessageService scenarioMessageService,
            IInteractionProcessor interactionProcessor,
            IPlayerProcessor playerProcessor,
            IAlterationProcessor alterationProcessor,
            IAlterationGenerator alterationGenerator,
            IBackendEventDataFactory backendEventDataFactory)
        {
            _layoutEngine = layoutEngine;
            _contentEngine = contentEngine;
            _alterationEngine = alterationEngine;
            _modelService = modelService;
            _targetingService = targetingService;
            _scenarioMessageService = scenarioMessageService;
            _interactionProcessor = interactionProcessor;
            _playerProcessor = playerProcessor;
            _alterationProcessor = alterationProcessor;
            _alterationGenerator = alterationGenerator;
            _backendEventDataFactory = backendEventDataFactory;
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
            if (desiredLocation == GridLocation.Empty)
                return null;

            //Look for road blocks - move player
            if (!_layoutEngine.IsPathToAdjacentCellBlocked(_modelService.Player.Location, desiredLocation, true, CharacterAlignmentType.PlayerAligned))
            {
                // Check for character swaps
                var swapCharacter = _modelService.Level.GetAt<NonPlayerCharacter>(desiredLocation);

                // Swap the charcter's location
                if (swapCharacter != null &&
                    swapCharacter.AlignmentType == CharacterAlignmentType.PlayerAligned)
                {
                    swapCharacter.Location = _modelService.Player.Location;

                    OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.ContentMove, swapCharacter.Id));
                }

                // Update player location
                _modelService.Player.Location = desiredLocation;

                // Notify Listener queue
                OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerLocation, _modelService.Player.Id));
            }

            //See what the player stepped on... Prefer Items first
            return _modelService.Level.GetAt<ItemBase>(_modelService.Player.Location) ??
                   _modelService.Level.GetAt<ScenarioObject>(_modelService.Player.Location);
        }
        public ScenarioObject MoveRandom()
        {
            // Get random adjacent location
            var desiredLocation = _layoutEngine.GetRandomAdjacentLocationForMovement(_modelService.Player.Location, CharacterAlignmentType.PlayerAligned);

            // Get direction for random move -> Move()
            var direction = LevelGridExtension.GetDirectionBetweenAdjacentPoints(_modelService.Player.Location, desiredLocation);

            return Move(direction);
        }

        public override void ApplyEndOfTurn(bool regenerate)
        {
            var level = _modelService.Level;
            var player = _modelService.Player;
            var playerAdvancement = false;

            // Player: End-Of-Turn
            _playerProcessor.ApplyEndOfTurn(player, regenerate, out playerAdvancement);

            // Player Advancement Event
            if (playerAdvancement)
                OnDialogEvent(_backendEventDataFactory.DialogPlayerAdvancement(player, 1));

            // Update player stats
            OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerStats, _modelService.Player.Id));

            //I'm Not DEEEAD!
            if (player.Hunger >= 100 || player.Hp <= 0.1)
            {
                var killedBy = _modelService.GetKilledBy();
                if (killedBy != null)
                    OnScenarioEvent(_backendEventDataFactory.PlayerDeath("Killed by " + killedBy));
                else
                    OnScenarioEvent(_backendEventDataFactory.PlayerDeath("Had a rough day..."));
            }

            // Update Model Content: 0) End Targeting
            //                       1) Update visible contents
            //                       2) Calculate model delta to prepare for UI

            _modelService.UpdateVisibility();
            

            // Queue Updates for level
            // QueueLevelUpdate(LevelUpdateType.LayoutVisible, string.Empty);
            OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.ContentVisible, ""));

            // Fire a tick event to update level ticks
            OnScenarioEvent(_backendEventDataFactory.Tick());
        }

        public void Attack(Compass direction)
        {
            var player = _modelService.Player;

            // Get points involved with the attack
            var location = _modelService.Player.Location;
            var attackLocation = _modelService.Level.Grid.GetPointInDirection(location, direction);

            // Invalid attack location
            if (attackLocation == GridLocation.Empty)
                return;

            // Check to see whether path is clear to attack
            var blocked = _layoutEngine.IsPathToAdjacentCellBlocked(location, attackLocation, false, CharacterAlignmentType.PlayerAligned);

            // Get target for attack
            var character = _modelService.Level.GetAt<NonPlayerCharacter>(attackLocation);

            if (character != null && !blocked)
            {
                //Engage enemy if they're attacked
                character.IsAlerted = true;

                // Enemy gets hit OR dodges
                var success = _interactionProcessor.CalculateInteraction(_modelService.Player, character, PhysicalAttackType.Melee);

                // If hit was successful - then add on any additional equipment attack effects
                if (success)
                {
                    // Validate -> Queue all alterations
                    foreach (var alteration in player.Equipment
                                                     .Values
                                                     .Where(x => x.IsEquipped)
                                                     .Where(x => x.HasAttackAlteration)
                                                     .Select(x => _alterationGenerator.GenerateAlteration(x.AttackAlteration)))
                    {
                        // If player meets alteration cost, queue with affected enemy
                        if (_alterationEngine.Validate(player, alteration))
                            _alterationEngine.Queue(player, new Character[] { character }, alteration);
                    }
                }
            }
        }
        public LevelContinuationAction Throw(string itemId)
        {
            var targetedCharacter = _targetingService.GetTargetedCharacter();
            var player = _modelService.Player;
            var thrownItem = player.Consumables[itemId];

            // Check that thrown item has level requirement met (ALSO DONE ON FRONT END)
            if (thrownItem.LevelRequired > player.Level)
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Required Level {0} Not Met!", thrownItem.LevelRequired.ToString());
                return LevelContinuationAction.DoNothing;
            }

            // Check Character Class Requirement
            if (thrownItem.HasCharacterClassRequirement &&
                player.Class != thrownItem.CharacterClass)
            {
                _scenarioMessageService.Publish(
                    ScenarioMessagePriority.Normal,
                    "Required Character Class Not Met!",
                    thrownItem.CharacterClass);

                return LevelContinuationAction.DoNothing;
            }

            // Must select a target
            if (targetedCharacter == null)
            {
                OnTargetingRequesetEvent(_backendEventDataFactory.TargetRequest(TargetRequestType.Throw, itemId));
                return LevelContinuationAction.DoNothing;
            }

            // TBD: Create general consumable for projectiles that has melee parameters
            if (thrownItem.HasProjectileAlteration)
            {
                // Create Alteration 
                var alteration = _alterationGenerator.GenerateAlteration(thrownItem.ProjectileAlteration);

                // If Alteration Cost is Met (PUBLISHES MESSAGES)
                if (_alterationEngine.Validate(_modelService.Player, alteration))
                {
                    // Queue the alteration and remove the item
                    _alterationEngine.Queue(_modelService.Player, alteration);

                    // Remove item from inventory
                    player.Consumables.Remove(itemId);

                    // Queue Level Update - Player Consumables Remove
                    OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerConsumableRemove, itemId));

                    return LevelContinuationAction.ProcessTurnNoRegeneration;
                }
                else
                    return LevelContinuationAction.ProcessTurnNoRegeneration;
            }

            return LevelContinuationAction.DoNothing;
        }
        public LevelContinuationAction Consume(string itemId)
        {
            var player = _modelService.Player;
            var consumable = player.Consumables[itemId];
            var alteration = consumable.HasAlteration ? _alterationGenerator.GenerateAlteration(consumable.Alteration) : null;
            var displayName = _modelService.GetDisplayName(consumable);

            // TODO:ALTERATION - Have to validate that each consumable has an alteration. Also, REMOVE 
            //                   ALTERATION USE / SETTING FOR AMMO TYPE (JUST MAKE A SEPARATE ONE IF
            //                   WE PLAN TO SUPPORT IT FOR FIRING RANGE WEAPON ONLY)

            // Check that item has level requirement met (ALSO DONE ON FRONT END)
            if (consumable.LevelRequired > player.Level)
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Required Level {0} Not Met!", consumable.LevelRequired.ToString());
                return LevelContinuationAction.DoNothing;
            }

            // Check Character Class Requirement
            if (consumable.HasCharacterClassRequirement &&
                player.Class != consumable.CharacterClass)
            {
                _scenarioMessageService.Publish(
                    ScenarioMessagePriority.Normal,
                    "Required Character Class Not Met!",
                    consumable.CharacterClass);

                return LevelContinuationAction.DoNothing;
            }

            // Check for targeting
            if (consumable.HasAlteration)
            {
                // If Alteration requires a target - then have to send an event to enter targeting mode
                if (alteration.RequiresTarget() && _targetingService.GetTargetedCharacter() == null)
                {
                    // Signal targeting start event
                    OnTargetingRequesetEvent(_backendEventDataFactory.TargetRequest(TargetRequestType.Consume, itemId));
                    return LevelContinuationAction.DoNothing;
                }

                else if (alteration.RequiresTargetLocation() && _targetingService.GetTargetLocation() == null)
                {
                    // Signal targeting start event
                    OnTargetingRequesetEvent(_backendEventDataFactory.TargetRequest(TargetRequestType.Consume, itemId));
                    return LevelContinuationAction.DoNothing;
                }

                else if (alteration.RequiresCharacterInRange() &&
                        !_modelService.CharacterContentInformation.GetVisibleCharacters(player).Any())
                {
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Must have enemies in range");
                    return LevelContinuationAction.DoNothing;
                }
            }

            // Proceeding with use - so check for identify on use
            if (consumable.IdentifyOnUse)
            {
                // Set consumable identified
                consumable.IsIdentified = true;

                _modelService.ScenarioEncyclopedia[consumable.RogueName].IsIdentified = true;
                _modelService.ScenarioEncyclopedia[consumable.RogueName].IsCurseIdentified = true;

                // Update UI
                OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.EncyclopediaIdentify, consumable.Id));
            }

            // Validate for the Alteration
            if (consumable.HasAlteration &&
               !_alterationEngine.Validate(_modelService.Player, alteration))
                return LevelContinuationAction.ProcessTurn;


            // Check for removal of item
            switch (consumable.Type)
            {
                case ConsumableType.OneUse:
                    {
                        // Remove the item
                        player.Consumables.Remove(itemId);

                        // Queue an update
                        OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerConsumableRemove, itemId));
                    }
                    break;
                case ConsumableType.MultipleUses:
                    {
                        // Subtract a Use
                        consumable.Uses--;

                        if (consumable.Uses <= 0)
                        {
                            // Remove the item
                            player.Consumables.Remove(itemId);

                            // Queue an update
                            OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerConsumableRemove, itemId));
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
                    OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerSkillSetAdd, ""));
                    OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.EncyclopediaIdentify, consumable.LearnedSkill.Id));

                    _scenarioMessageService.Publish(ScenarioMessagePriority.Good, player.RogueName + " has been granted a new skill!  \"" + consumable.LearnedSkill.RogueName + "\"");
                }
                return LevelContinuationAction.ProcessTurn;
            }

            // Notes
            if (consumable.SubType == ConsumableSubType.Note)
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Reading " + displayName);

                OnDialogEvent(_backendEventDataFactory.DialogNote(consumable.NoteMessage, displayName));

                return LevelContinuationAction.ProcessTurn;
            }

            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Using " + displayName);

            // Queue processing of alteration
            _alterationEngine.Queue(_modelService.Player, alteration);

            return LevelContinuationAction.ProcessTurnNoRegeneration;
        }
        public void Identify(string itemId)
        {
            var item = _modelService.Player.Inventory[itemId];
            var metaData = _modelService.ScenarioEncyclopedia[item.RogueName];

            metaData.IsIdentified = true;
            metaData.IsCurseIdentified = true;
            item.IsIdentified = true;

            _scenarioMessageService.Publish(ScenarioMessagePriority.Good, item.RogueName + " Identified");

            // Queue an update
            if (item is Consumable)
                OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerConsumableAddOrUpdate, itemId));
            else if (item is Equipment)
                OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerEquipmentAddOrUpdate, itemId));

            // Queue meta-data update
            OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.EncyclopediaIdentify, itemId));
        }
        public void EnhanceEquipment(EquipmentEnhanceAlterationEffect effect, string itemId)
        {
            // Get Equipment from Player Inventory
            var equipment = _modelService.Player.Equipment[itemId];

            // Apply Effect -> Publish Messages
            _alterationProcessor.ApplyEquipmentEnhanceEffect(_modelService.Player, effect, equipment);

            // Queue update
            OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerEquipmentAddOrUpdate, itemId));
        }
        public void Uncurse(string itemId)
        {
            var equipment = _modelService.Player.Equipment[itemId];
            equipment.IsCursed = false;

            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, _modelService.GetDisplayName(equipment) + " Uncursed");

            if (equipment.HasCurseAlteration)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Your " + equipment.RogueName + " is now safe to use (with caution...)");

            if (equipment.IsEquipped)
                _modelService.Player.Alteration.Remove(equipment.CurseAlteration.Name);

            // Queue an update
            OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerEquipmentAddOrUpdate, itemId));
        }
        public void Drop(string itemId)
        {
            _contentEngine.DropPlayerItem(itemId);
        }
        public LevelContinuationAction Fire()
        {
            var rangeWeapon = _modelService.Player.Equipment.Values.FirstOrDefault(x => x.IsEquipped && x.Type == EquipmentType.RangeWeapon);
            var targetedEnemy = _targetingService.GetTargetedCharacter();

            if (rangeWeapon == null)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "No Range Weapons are equipped");

            else if (targetedEnemy == null)
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Must first target an enemy");
                OnTargetingRequesetEvent(_backendEventDataFactory.TargetRequest(TargetRequestType.Fire, ""));
                return LevelContinuationAction.DoNothing;
            }

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

                // Check Character Class Requirement
                if (ammo.HasCharacterClassRequirement &&
                    _modelService.Player.Class != ammo.CharacterClass)
                {
                    _scenarioMessageService.Publish(
                        ScenarioMessagePriority.Normal,
                        "Required Character Class Not Met!",
                        ammo.CharacterClass);

                    return LevelContinuationAction.ProcessTurn;
                }

                // Remove ammo from inventory
                _modelService.Player.Consumables.Remove(ammo.Id);

                // Queue update
                OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerConsumableRemove, ammo.Id));

                // Calculate hit - if enemy hit then queue ammunition alteration
                var enemyHit = _interactionProcessor.CalculateInteraction(_modelService.Player, targetedEnemy, PhysicalAttackType.Range);


                // TODO:ANIMATION - CHANGE THIS TO "SHOOT SVG"
                //// Process the animation
                //if (ammo.AmmoAnimationGroup.Animations.Any())
                //{
                //    // TODO:ANIMATION
                //    //OnAnimationEvent(_backendEventDataFactory.Animation(ammo.AmmoAnimationGroup.Animations,
                //    //                                                    _modelService.Player.Location,
                //    //                                                    new GridLocation[] { targetedEnemy.Location }));
                //}

                // Clear the targeting service
                _targetingService.Clear();
            }
            return LevelContinuationAction.ProcessTurn;
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
                OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerSkillSetRefresh, player.Id));
                OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerLocation, player.Id));
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
            OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerSkillSetRefresh, ""));
            OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerLocation, _modelService.Player.Id));
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

                    _scenarioMessageService.Publish(ScenarioMessagePriority.Good, player.RogueName + " Has Learned " + skill.Alteration.Name);

                    // Select skill if none selected
                    if (skillSet.SelectedSkill == null)
                        skillSet.SelectSkill(skillId);

                    OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerSkillSetRefresh, ""));
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

            // No Current Skill(Could throw Exception)
            var currentSkill = activeSkillSet.GetCurrentSkillAlteration();
            if (currentSkill == null)
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "No Active Skill - (See Skills Panel to Set)");
                return LevelContinuationAction.DoNothing;
            }

            var enemyTargeted = _targetingService.GetTargetedCharacter();
            var skillAlteration = _alterationGenerator.GenerateAlteration(currentSkill);

            // Requires Target Character
            if (skillAlteration.RequiresTarget() && enemyTargeted == null)
            {
                OnTargetingRequesetEvent(_backendEventDataFactory.TargetRequest(TargetRequestType.InvokeSkill, ""));
                return LevelContinuationAction.DoNothing;
            }

            // Requires Target Location
            if (skillAlteration.RequiresTargetLocation() && _targetingService.GetTargetLocation() == null)
            {
                OnTargetingRequesetEvent(_backendEventDataFactory.TargetRequest(TargetRequestType.InvokeSkill, ""));
                return LevelContinuationAction.DoNothing;
            }

            // Meets Alteration Cost ?
            if (!_alterationEngine.Validate(_modelService.Player, skillAlteration))
                return LevelContinuationAction.DoNothing;

            // For passives / auras - work with IsTurnedOn flag
            if (skillAlteration.IsPassiveOrAura())
            {
                // Turn off passive if it's turned on
                if (activeSkillSet.IsTurnedOn)
                {
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Deactivating - " + skillAlteration.RogueName);

                    // Pass - through method is safe
                   _modelService.Player.Alteration.Remove(currentSkill.Name);

                    activeSkillSet.IsTurnedOn = false;
                }
                // Turn on the passive and queue processing
                else
                {
                    activeSkillSet.IsTurnedOn = true;

                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Invoking - " + skillAlteration.RogueName);

                    // Queue processing -> Animation->Process parameters(backend)
                    _alterationEngine.Queue(_modelService.Player, skillAlteration);
                }
            }
            // All other skill types
            else
            {
                // Publish message
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Invoking - " + skillAlteration.RogueName);

                // Queue processing -> Animation->Process parameters(backend)
                _alterationEngine.Queue(_modelService.Player, skillAlteration);
            }

            return LevelContinuationAction.ProcessTurn;
        }
        public LevelContinuationAction InvokeDoodad()
        {
            var player = _modelService.Player;
            var doodad = _modelService.Level.GetAt<DoodadBase>(player.Location);
            if (doodad == null)
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Nothing here to use! (Requires Scenario Object)");

                return LevelContinuationAction.DoNothing;
            }

            //Sets identified
            _modelService.ScenarioEncyclopedia[doodad.RogueName].IsIdentified = true;

            // Update meta-data UI
            OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.EncyclopediaIdentify, doodad.Id));

            // Update statistics
            OnScenarioEvent(_backendEventDataFactory.StatisticsUpdate(ScenarioUpdateType.StatisticsDoodadUsed, doodad.RogueName));

            switch (doodad.Type)
            {
                case DoodadType.Magic:
                    {
                        var doodadMagic = (DoodadMagic)doodad;

                        // Has been exhausted
                        if (doodadMagic.IsOneUse && doodadMagic.HasBeenUsed || !doodadMagic.IsInvoked)
                            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Nothing Happens");

                        else
                        {
                            // Generate Alteration
                            var alteration = _alterationGenerator.GenerateAlteration(doodadMagic.InvokedAlteration);

                            // Publish Message
                            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Using " + doodad.RogueName);

                            // Mark the Doodad as HasBeenUsed
                            doodadMagic.HasBeenUsed = true;

                            // Validate -> Queue Alteration
                            if (_alterationEngine.Validate(_modelService.Player, alteration))
                                _alterationEngine.Queue(_modelService.Player, alteration);

                            // Failed Validation (Player Doesn't Meet Cost)
                            else
                                return LevelContinuationAction.DoNothing;
                        }
                    }
                    break;
                case DoodadType.Normal:
                    {
                        switch (((DoodadNormal)doodad).NormalType)
                        {
                            case DoodadNormalType.SavePoint:
                                OnScenarioEvent(_backendEventDataFactory.Save());
                                break;
                            case DoodadNormalType.StairsDown:
                                OnScenarioEvent(_backendEventDataFactory.LevelChange(_modelService.Level.Number + 1, PlayerStartLocation.StairsUp));
                                break;
                            case DoodadNormalType.StairsUp:
                                OnScenarioEvent(_backendEventDataFactory.LevelChange(_modelService.Level.Number - 1, PlayerStartLocation.StairsDown));
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
