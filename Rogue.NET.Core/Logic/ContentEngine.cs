using Rogue.NET.Core.Logic.Algorithm.Interface;
using Rogue.NET.Core.Logic.Content.Interface;
using Rogue.NET.Core.Logic.Interface;
using Rogue.NET.Core.Logic.Processing;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Doodad;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Core.Utility;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using Rogue.NET.Core.Logic.Static;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Scenario.Content.Extension;
using Rogue.NET.Core.Model.ScenarioMessage;
using System.Collections.Generic;
using Rogue.NET.Core.Logic.Content.Enum;
using Rogue.NET.Core.Logic.Processing.Factory.Interface;

namespace Rogue.NET.Core.Logic
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IContentEngine))]
    public class ContentEngine : IContentEngine
    {
        readonly IModelService _modelService;
        readonly IPathFinder _pathFinder;
        readonly ILayoutEngine _layoutEngine;
        readonly ISpellEngine _spellEngine;
        readonly IReligionEngine _religionEngine;
        readonly IEnemyProcessor _enemyProcessor;
        readonly IPlayerProcessor _playerProcessor;        
        readonly IInteractionProcessor _interactionProcessor;
        readonly IScenarioMessageService _scenarioMessageService;
        readonly IRandomSequenceGenerator _randomSequenceGenerator;
        readonly ICharacterGenerator _characterGenerator;
        readonly IRogueUpdateFactory _rogueUpdateFactory;

        public event EventHandler<RogueUpdateEventArgs> RogueUpdateEvent;
        public event EventHandler<ILevelProcessingAction> LevelProcessingActionEvent;

        [ImportingConstructor]
        public ContentEngine(
            IModelService modelService, 
            IPathFinder pathFinder,
            ILayoutEngine layoutEngine, 
            ISpellEngine spellEngine,
            IReligionEngine religionEngine,
            IEnemyProcessor enemyProcessor,
            IPlayerProcessor playerProcessor,
            IInteractionProcessor interactionProcessor,
            IScenarioMessageService scenarioMessageService,
            IRandomSequenceGenerator randomSequenceGenerator,
            ICharacterGenerator characterGenerator,
            IRogueUpdateFactory rogueUpdateFactory)
        {
            _modelService = modelService;
            _pathFinder = pathFinder;
            _layoutEngine = layoutEngine;
            _spellEngine = spellEngine;
            _religionEngine = religionEngine;
            _enemyProcessor = enemyProcessor;
            _playerProcessor = playerProcessor;
            _interactionProcessor = interactionProcessor;
            _scenarioMessageService = scenarioMessageService;
            _randomSequenceGenerator = randomSequenceGenerator;
            _characterGenerator = characterGenerator;
            _rogueUpdateFactory = rogueUpdateFactory;
        }

        #region (public) Methods
        public void StepOnItem(Character character, ItemBase item)
        {
            var level = _modelService.Level;
            var haulMax = character.GetHaulMax();
            var projectedHaul = item.Weight + character.GetHaul();

            if (haulMax >= projectedHaul)
            {
                // Add to Character's inventory
                if (item is Consumable)
                    character.Consumables.Add(item.Id, item as Consumable);

                else if (item is Equipment)
                    character.Equipment.Add(item.Id, item as Equipment);

                // Remove from the level
                level.RemoveContent(item);

                // Queue level update event for removed item
                RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.ContentRemove, item.Id));

                if (character is Player)
                {
                    // Publish message
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Found " + _modelService.GetDisplayName(item));

                    if (item is Consumable)
                        RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerConsumableAddOrUpdate, item.Id));
                    else if (item is Equipment)
                        RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerEquipmentAddOrUpdate, item.Id));

                    //Update level statistics
                    RogueUpdateEvent(this, _rogueUpdateFactory.StatisticsUpdate(ScenarioUpdateType.StatisticsItemFound, item.RogueName));
                }
            }
            else if (character is Player)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Too much weight in your inventory");
        }
        public void StepOnDoodad(Character character, DoodadBase doodad)
        {
            // Show the Doodad (could be hidden)
            doodad.IsHidden = false;

            switch (doodad.Type)
            {
                case DoodadType.Normal:
                    StepOnDoodadNormal(character, doodad as DoodadNormal);
                    break;
                case DoodadType.Magic:
                    StepOnDoodadMagic(character, doodad as DoodadMagic);
                    break;
            }
        }
        public bool Equip(string equipId)
        {
            // Fetch reference to equipment
            var equipment = _modelService.Player.Equipment[equipId];
            var player = _modelService.Player;
            var result = false;

            // Check that item has level requirement met (ALSO DONE ON FRONT END)
            if (equipment.LevelRequired > player.Level)
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Required Level {0} Not Met!", equipment.LevelRequired.ToString());
                return false;
            }

            // Identify Religion
            if (equipment.HasReligionRequirement)
                _religionEngine.IdentifyReligion(equipment.Religion);

            // Check Religious Affiliation Requirement
            if (equipment.HasReligionRequirement &&
               (!player.ReligiousAlteration.IsAffiliated() ||
                 player.ReligiousAlteration.Religion.RogueName != equipment.Religion.RogueName))
            {
                _scenarioMessageService.Publish(
                    ScenarioMessagePriority.Normal,
                    "Required Religious Affiliation {0} Not Met!",
                    equipment.Religion.RogueName);

                return false;
            }
           
            // Process the update
            if (equipment.IsEquipped)
                result = UnEquip(equipment);

            else
                result = Equip(equipment);

            // Queue player update for this item
            RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerEquipmentAddOrUpdate, equipId));

            return result;
        }
        public void DropPlayerItem(string itemId)
        {
            var item = _modelService.Player.Inventory[itemId];
            var adjacentFreeLocations = _layoutEngine.GetFreeAdjacentLocations(_modelService.Level, _modelService.Player, _modelService.Player.Location);
            var dropLocation = adjacentFreeLocations.FirstOrDefault();

            if (dropLocation == null)
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Cannot drop item here");
                return;
            }

            var displayName = _modelService.GetDisplayName(item);

            if (item is Equipment)
            {
                var equipment = item as Equipment;
                if (equipment.IsEquipped)
                {
                    if (!Equip(equipment.Id))
                        return;
                }

                // Set item location
                equipment.Location = dropLocation;

                // Remove from inventory
                _modelService.Player.Equipment.Remove(equipment.Id);

                // Add level content
                _modelService.Level.AddContent(equipment);

                // Queue updates
                RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerEquipmentRemove, equipment.Id));
                RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.ContentAll, ""));
            }
            if (item is Consumable)
            {
                var consumable = item as Consumable;

                // Set item location
                consumable.Location = dropLocation;

                // Remove from inventory
                _modelService.Player.Consumables.Remove(consumable.Id);

                // Add level content
                _modelService.Level.AddContent(consumable);

                // Queue updates
                RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerConsumableRemove, consumable.Id));
                RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.ContentAll, ""));
            }

            // Publish message
            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, displayName + " Dropped");
        }
        public void EnemyDeath(Enemy enemy)
        {
            for (int i = enemy.Equipment.Count - 1; i >= 0; i--)
            {
                DropEnemyItem(enemy, enemy.Equipment.ElementAt(i).Value);
            }
            for (int i = enemy.Consumables.Count - 1; i >= 0; i--)
            {
                DropEnemyItem(enemy, enemy.Consumables.ElementAt(i).Value);
            }

            // Update level object
            var level = _modelService.Level;

            level.RemoveContent(enemy);

            // Queue Animation for enemy death
            if (enemy.DeathAnimations.Count > 0)
                RogueUpdateEvent(this, _rogueUpdateFactory.Animation(enemy.DeathAnimations, enemy.Location, new CellPoint[] { _modelService.Player.Location }));

            // Calculate player gains
            _playerProcessor.CalculateEnemyDeathGains(_modelService.Player, enemy);

            // Update statistics / Player skills
            RogueUpdateEvent(this, _rogueUpdateFactory.StatisticsUpdate(ScenarioUpdateType.StatisticsEnemyDeath, enemy.RogueName));
            RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerSkillSetRefresh, ""));

            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, enemy.RogueName + " Slayed");

            //Set enemy identified
            _modelService.ScenarioEncyclopedia[enemy.RogueName].IsIdentified = true;

            // Check for Enemy Religion to identify
            if (enemy.ReligiousAlteration.IsAffiliated())
                _religionEngine.IdentifyReligion(enemy.ReligiousAlteration.Religion);

            // Publish Level update
            RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.ContentRemove, enemy.Id));
            RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.EncyclopediaIdentify, enemy.Id));
        }
        public void CalculateEnemyReactions()
        {
            var level = _modelService.Level;

            // Enemy Reactions: 0) Check whether enemy is still alive 
            //                  1) Process Enemy Reaction (Applies End-Of-Turn)
            //                  2) Check for Enemy Death (After Enemy Reaction)
            for (int i = level.Enemies.Count() - 1; i >= 0; i--)
            {
                var enemy = level.Enemies.ElementAt(i);

                if (enemy.Hp <= 0)
                    EnemyDeath(enemy);
                else
                    LevelProcessingActionEvent(this, new LevelProcessingAction()
                    {
                        CharacterId = enemy.Id,
                        Type = LevelProcessingActionType.EnemyReaction
                    });
            }
        }
        public void ProcessEnemyReaction(Enemy enemy)
        {
            // Don't let Enemy get the last word. Check this here to prevent Enemy Death checks every where else.
            if (enemy.Hp <= 0)
            {
                EnemyDeath(enemy);
                return;
            }

            var level = _modelService.Level;
            var player = _modelService.Player;

            // Check for invisibility
            if (player.Is(CharacterStateType.Invisible) &&
               !enemy.Alteration.CanSeeInvisibleCharacters() &&
               !enemy.WasAttackedByPlayer)
            {
                enemy.IsEngaged = false;
                return;
            }

            var distance = Calculator.RoguianDistance(enemy.Location, player.Location);

            // Check for engaged
            if (distance < enemy.BehaviorDetails.EngageRadius)
                enemy.IsEngaged = true;

            if (distance > enemy.BehaviorDetails.DisengageRadius)
            {
                enemy.IsEngaged = false;

                // Reset this flag here to allow them to dis-engage at long distances
                enemy.WasAttackedByPlayer = false;
            }

            if (!enemy.IsEngaged)
                return;

            enemy.TurnCounter += _interactionProcessor.CalculateEnemyTurnIncrement(player, enemy);

            if (enemy.TurnCounter >= 1)
                OnEnemyReaction(enemy);

            if (enemy.Hp <= 0)
                EnemyDeath(enemy);
        }
        public void ApplyEndOfTurn()
        {
            ProcessMonsterGeneration();
        }
        #endregion

        #region (private) Sub-Methods
        private void StepOnDoodadNormal(Character character, DoodadNormal doodad)
        {
            var level = _modelService.Level;
            var metaData = _modelService.ScenarioEncyclopedia[doodad.RogueName];

            if (character is Player)
            {
                metaData.IsIdentified = true;
                doodad.IsHidden = false;

                // Update metadata
                RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.EncyclopediaIdentify, doodad.Id));
            }

            switch (doodad.NormalType)
            {
                case DoodadNormalType.SavePoint:
                case DoodadNormalType.StairsDown:
                case DoodadNormalType.StairsUp:
                    if (character is Player)
                    {
                        var doodadTitle = TextUtility.CamelCaseToTitleCase(doodad.NormalType.ToString());
                        _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, doodadTitle + " (Press \"D\" to Use)");
                    }
                    break;
                case DoodadNormalType.TeleportRandom:
                    {
                        character.Location = _modelService.Level.GetRandomLocation(true, _randomSequenceGenerator);
                        if (character is Player)
                            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Teleport!");

                        // Update Content Visibility
                        _modelService.UpdateContents();

                        // Queue update to level 
                        if (character is Enemy)
                            RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.ContentMove, character.Id));

                        else
                            RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerLocation, _modelService.Player.Id));
                    }
                    break;
                case DoodadNormalType.Teleport1:
                case DoodadNormalType.Teleport2:
                    {
                        var otherTeleporter = level.DoodadsNormal.First(x => x.PairId == doodad.Id);

                        // Show the other teleporter also if it's hidden
                        otherTeleporter.IsHidden = false;

                        // Have to boot enemy if it's sitting on other teleporter
                        if (character is Player && level.IsCellOccupiedByEnemy(otherTeleporter.Location))
                        {
                            var enemy = level.GetAtPoint<Enemy>(otherTeleporter.Location);

                            // Remove from the level
                            level.RemoveContent(enemy);

                            // Queue update to the level
                            RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.ContentRemove, enemy.Id));
                        }
                        // Enemy is trying to teleport in where Player is
                        else if (character is Enemy && character.Location == _modelService.Player.Location)
                        {
                            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Enemy is trying to use your teleporter!");
                            break;
                        }

                        // Set character location to other teleporter
                        character.Location = otherTeleporter.Location;

                        // Queue update to level 
                        if (character is Enemy)
                            RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.ContentMove, character.Id));

                        else
                            RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerLocation, _modelService.Player.Id));

                        if (character is Player)
                            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Teleport!");

                        break;
                    }
            }
        }
        private void StepOnDoodadMagic(Character character, DoodadMagic doodad)
        {
            var level = _modelService.Level;
            var metaData = _modelService.ScenarioEncyclopedia[doodad.RogueName];

            var displayName = _modelService.GetDisplayName(doodad);

            if (!(doodad.IsOneUse && doodad.HasBeenUsed))
            {
                if (doodad.IsAutomatic)
                {
                    // Mark that the doodad has been used
                    doodad.HasBeenUsed = true;

                    // Queue magic spell with animation
                    if (character is Player)
                        _spellEngine.QueuePlayerMagicSpell(doodad.AutomaticSpell);

                    else
                        _spellEngine.QueueEnemyMagicSpell(character as Enemy, doodad.AutomaticSpell);
                }
                else
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, displayName + " Press \"D\" to Use");
            }
            else
            {
                if (character is Player)
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, displayName + " seems to be inactive");
            }
        }
        private bool UnEquip(Equipment equipment)
        {
            var metaData = _modelService.ScenarioEncyclopedia[equipment.RogueName];

            if (equipment.IsCursed)
            {
                // Publish message
                _scenarioMessageService.Publish(ScenarioMessagePriority.Bad, _modelService.GetDisplayName(equipment) + " is Cursed!!!");

                // Set Curse Identified
                metaData.IsCurseIdentified = true;

                RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.EncyclopediaCurseIdentify, equipment.Id));

                return false;
            }
            else
            {
                // Unequip
                equipment.IsEquipped = false;

                // Publish Message
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Un-Equipped " + (metaData.IsIdentified ? equipment.RogueName : "???"));

                // If equip spell is present - make sure it's deactivated and removed
                if (equipment.HasEquipSpell)
                    _modelService.Player.Alteration.DeactivatePassiveAlteration(equipment.EquipSpell.Id);

                return true;
            }

        }
        private bool Equip(Equipment equipment)
        {
            switch (equipment.Type)
            {
                case EquipmentType.Armor:
                case EquipmentType.Boots:
                case EquipmentType.Gauntlets:
                case EquipmentType.Helmet:
                case EquipmentType.Amulet:
                case EquipmentType.Orb:
                case EquipmentType.Belt:
                case EquipmentType.Shoulder:
                    {
                        var equippedItem = _playerProcessor.GetEquippedType(_modelService.Player, equipment.Type);
                        if (equippedItem != null)
                        {
                            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Must first un-equip " + _modelService.GetDisplayName(equippedItem));

                            return false;
                        }
                    }
                    break;
                case EquipmentType.TwoHandedMeleeWeapon:
                case EquipmentType.OneHandedMeleeWeapon:
                case EquipmentType.Shield:
                case EquipmentType.RangeWeapon:
                    {
                        var handsFree = _playerProcessor.GetNumberOfFreeHands(_modelService.Player);

                        if (((equipment.Type == EquipmentType.TwoHandedMeleeWeapon || equipment.Type == EquipmentType.RangeWeapon) && handsFree < 2) || handsFree < 1)
                        {
                            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Must first free up a hand");
                            return false;
                        }
                    }
                    break;
                case EquipmentType.Ring:
                    {
                        var ring = _playerProcessor.GetEquippedType(_modelService.Player, EquipmentType.Ring);
                        var numberEquipped = _playerProcessor.GetNumberEquipped(_modelService.Player, EquipmentType.Ring);

                        if (ring != null && numberEquipped >= 2)
                        {
                            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Must first un-equip " + _modelService.GetDisplayName(equipment));

                            return false;
                        }
                    }
                    break;
                default:
                    break;
            }

            equipment.IsEquipped = true;

            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Equipped " + _modelService.GetDisplayName(equipment));

            // Fire equip spell -> This will activate any passive effects after animating
            if (equipment.HasEquipSpell)
                _spellEngine.QueuePlayerMagicSpell(equipment.EquipSpell);

            if (equipment.HasCurseSpell && equipment.IsCursed)
                _spellEngine.QueuePlayerMagicSpell(equipment.CurseSpell);

            return true;
        }
        private void DropEnemyItem(Enemy enemy, ItemBase item)
        {
            var adjacentFreeLocations = _layoutEngine.GetFreeAdjacentLocations(_modelService.Level, _modelService.Player, enemy.Location);
            var location = adjacentFreeLocations.FirstOrDefault();

            if (location == null)
                return;

            // Provide new location for item
            item.Location = location;

            // Remove from enemy inventory
            if (item is Equipment)
            {
                var equipment = item as Equipment;

                enemy.Equipment.Remove(item.Id);

                // This has a chance of happening if enemy steals something that is marked equiped. Though, it
                // SHOULD NEVER HAPPEN
                equipment.IsEquipped = false;

                // These cases should never happen; but want to cover them here to be sure.
                if (equipment.HasEquipSpell && equipment.IsEquipped)
                    enemy.Alteration.DeactivatePassiveAlteration(equipment.EquipSpell.Id);

                if (equipment.HasCurseSpell && equipment.IsEquipped)
                    enemy.Alteration.DeactivatePassiveAlteration(equipment.CurseSpell.Id);
            }

            if (item is Consumable)
                enemy.Consumables.Remove(item.Id);

            // Add to level
            _modelService.Level.AddContent(item);
        }
        #endregion

        #region (private) Enemy Reactions
        private void OnEnemyReaction(Enemy enemy)
        {
            // Sets turn counter 
            int turns = (int)enemy.TurnCounter;
            enemy.TurnCounter = enemy.TurnCounter % 1;

            for (int j = 0; j < turns && enemy.Hp > 0; j++)
            {
                // Apply Beginning of Turn
                _enemyProcessor.ApplyBeginningOfTurn(enemy);

                if (enemy.Hp < 0)
                    break;

                //Check altered states

                // Can't Move (Is sleeping, paralyzed, etc..)
                if (enemy.Is(CharacterStateType.CantMove))
                {
                    // Apply end-of-turn behavior for enemy
                    _enemyProcessor.ApplyEndOfTurn(enemy, _modelService.Player, false);
                    continue;
                }

                //Confused - check during calculate character move
                var willRandomStrikeMelee = _randomSequenceGenerator.Get() <= ModelConstants.RandomStrikeProbability;

                var actionTaken = false;

                switch (enemy.BehaviorDetails.CurrentBehavior.AttackType)
                {
                    case CharacterAttackType.Melee:
                        {
                            var adjacentCells = _modelService.Level.Grid.GetAdjacentLocations(enemy.Location);
                            var attackLocation = adjacentCells.FirstOrDefault(z => z == _modelService.Player.Location);

                            // Check to see what kind of melee attack will happen - based on enemy equipment
                            if (enemy.IsRangeMelee())
                            {
                                // Check for line of sight and firing range
                                var isLineOfSight = _modelService.GetLineOfSightLocations().Any(x => x == enemy.Location);
                                var range = Calculator.RoguianDistance(enemy.Location, _modelService.Player.Location);

                                // These are guaranteed by the enemy check IsRangeMelee()
                                var rangeWeapon = enemy.Equipment.Values.First(x => x.IsEquipped && x.Type == EquipmentType.RangeWeapon);
                                var ammo = enemy.Consumables.Values.First(x => x.RogueName == rangeWeapon.AmmoName);

                                if (range > ModelConstants.MinFiringDistance && isLineOfSight)
                                {
                                    // Remove ammo from enemy inventory
                                    enemy.Consumables.Remove(ammo.Id);

                                    // Calculate hit - if enemy hit then queue Ammunition spell
                                    var enemyHit = _interactionProcessor.CalculateInteraction(enemy, _modelService.Player, PhysicalAttackType.Range);

                                    // If enemy hit then process the spell associated with the ammo
                                    if (enemyHit)
                                        _spellEngine.QueueEnemyMagicSpell(enemy, ammo.AmmoSpell);

                                    // Otherwise, process the animation only
                                    else if (ammo.AmmoSpell.Animations.Any())
                                        RogueUpdateEvent(this, _rogueUpdateFactory.Animation(ammo.AmmoSpell.Animations, enemy.Location, new CellPoint[] { _modelService.Player.Location }));

                                    actionTaken = true;
                                }
                            }

                            // Attack Conditions: !actionTaken (no range attack), location is non-null; AND enemy is not confused; OR enemy is confused and can strike
                            if (!actionTaken && 
                                 attackLocation != null && (!enemy.Is(CharacterStateType.MovesRandomly | CharacterStateType.Blind) || 
                                                            (enemy.Is(CharacterStateType.MovesRandomly | CharacterStateType.Blind) && willRandomStrikeMelee)))
                            {
                                if (!_layoutEngine.IsPathToAdjacentCellBlocked(_modelService.Level, enemy.Location, attackLocation, true))
                                {
                                    _interactionProcessor.CalculateInteraction(enemy, _modelService.Player, PhysicalAttackType.Melee);
                                    actionTaken = true;
                                }
                            }
                        }
                        break;
                    case CharacterAttackType.Skill:
                    case CharacterAttackType.SkillCloseRange:
                        if (!enemy.Is(CharacterStateType.MovesRandomly | CharacterStateType.Blind))
                        {
                            // Must have line of sight to player
                            var isLineOfSight = _modelService.GetLineOfSightLocations().Any(x => x == enemy.Location);
                            var isInRange = true;

                            // Add a check for close range skills
                            if (enemy.BehaviorDetails.CurrentBehavior.AttackType == CharacterAttackType.SkillCloseRange)
                            {
                                isInRange = _modelService.Level
                                                         .Grid
                                                         .GetAdjacentLocations(enemy.Location)
                                                         .Any(x => x == _modelService.Player.Location) && isLineOfSight;
                            }

                            // Queue Enemy Magic Spell -> Animation -> Post Animation Processing
                            if (isLineOfSight && isInRange)
                            {
                                _scenarioMessageService.PublishEnemyAlterationMessage(
                                    ScenarioMessagePriority.Normal,
                                    _modelService.Player.RogueName,
                                    _modelService.GetDisplayName(enemy),
                                    enemy.BehaviorDetails.CurrentBehavior.EnemySkill.DisplayName);

                                _spellEngine.QueueEnemyMagicSpell(enemy, enemy.BehaviorDetails.CurrentBehavior.EnemySkill);

                                actionTaken = true;
                            }
                        }
                        break;
                    case CharacterAttackType.None:
                    default:
                        break;
                }

                // Action Taken => enemy did some kind of attack
                if (!actionTaken)
                {
                    var moveLocation = CalculateEnemyMoveLocation(enemy, _modelService.Player.Location);
                    if (moveLocation != null)
                        ProcessEnemyMove(enemy, moveLocation);
                }

                // Apply end-of-turn behavior for enemy
                _enemyProcessor.ApplyEndOfTurn(enemy, _modelService.Player, actionTaken);
            }
        }
        private CellPoint CalculateEnemyMoveLocation(Enemy enemy, CellPoint desiredLocation)
        {
            //Return random if confused
            if (enemy.Is(CharacterStateType.MovesRandomly))
                return _layoutEngine.GetRandomAdjacentLocation(_modelService.Level, _modelService.Player, enemy.Location, true);

            switch (enemy.BehaviorDetails.CurrentBehavior.MovementType)
            {
                case CharacterMovementType.Random:
                    return _layoutEngine.GetRandomAdjacentLocation(_modelService.Level, _modelService.Player, enemy.Location, true);
                case CharacterMovementType.HeatSeeker:
                    return _layoutEngine.GetFreeAdjacentLocationsForMovement(_modelService.Level, _modelService.Player, enemy.Location)
                                        .OrderBy(x => Calculator.RoguianDistance(x, desiredLocation))
                                        .FirstOrDefault();
                case CharacterMovementType.StandOffIsh:
                    return _layoutEngine.GetFreeAdjacentLocationsForMovement(_modelService.Level, _modelService.Player, enemy.Location)
                                        .OrderBy(x => Calculator.RoguianDistance(x, desiredLocation))
                                        .LastOrDefault();
                case CharacterMovementType.PathFinder:
                    var nextLocation = _pathFinder.FindPath(enemy.Location, _modelService.Player.Location, enemy.BehaviorDetails.DisengageRadius, enemy.BehaviorDetails.CanOpenDoors);
                    return nextLocation ?? _layoutEngine.GetFreeAdjacentLocationsForMovement(_modelService.Level,  _modelService.Player, enemy.Location)
                                                        .OrderBy(x => Calculator.RoguianDistance(x, desiredLocation))
                                                        .FirstOrDefault();
                default:
                    throw new Exception("Unknown Enemy Movement Type");
            }
        }

        // Processes logic for found path point. This includes anything required for Enemy to
        // relocate to point "moveLocation". This point has been calculated as the next point
        // towards Player
        private void ProcessEnemyMove(Enemy enemy, CellPoint moveLocation)
        {
            // Case where path finding algorithm returns null; or heat seeking algorithm
            // returns null.
            if (moveLocation == null)
                return;

            // Check for opening of doors
            var openingPosition1 = CellPoint.Empty;
            var openingPosition2 = CellPoint.Empty;
            var openingDirection2 = Compass.Null;
            var shouldMoveToOpeningPosition1 = false;

            var moveDirection = LevelGridExtension.GetDirectionBetweenAdjacentPoints(enemy.Location, moveLocation);

            var throughDoor = moveDirection == Compass.Null ? false : _layoutEngine.IsPathToCellThroughDoor(
                                _modelService.Level.Grid, 
                                enemy.Location, 
                                moveDirection, 
                                out openingPosition1, 
                                out openingPosition2, 
                                out openingDirection2,
                                out shouldMoveToOpeningPosition1);

            // Behavior allows opening of doors
            if (enemy.BehaviorDetails.CanOpenDoors && throughDoor)
            {
                // If have to move into position first then move
                if (shouldMoveToOpeningPosition1)
                {
                    // Have to move to opening position 1 first - which means the door is on one of the off-diagonal locations
                    if (_layoutEngine.IsPathToAdjacentCellBlocked(_modelService.Level, enemy.Location, openingPosition1, true))
                    {
                        // Update enemy location
                        enemy.Location = openingPosition1;

                        // Update Content Visibility
                        _modelService.UpdateContents();

                        // Notify listener queue
                        RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.ContentMove, enemy.Id));
                    }
                }

                // Can open the door where the enemy is at
                else
                {
                    // Open the door -> Notifies UI listeners
                    _layoutEngine.ToggleDoor(_modelService.Level.Grid, moveDirection, enemy.Location);
                }
            }
            else if (!throughDoor &&
                     !_layoutEngine.IsPathToAdjacentCellBlocked(_modelService.Level, enemy.Location, moveLocation, true))
            {
                // Update enemy location
                enemy.Location = moveLocation;

                // Update Content Visibility
                _modelService.UpdateContents();

                // Notify listener queue
                RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.ContentMove, enemy.Id));
            }

            // Check for items
            var item = _modelService.Level.GetAtPoint<ItemBase>(enemy.Location);
            if (item != null)
                StepOnItem(enemy, item);

            // Check for doodad
            var doodad = _modelService.Level.GetAtPoint<DoodadBase>(enemy.Location);
            if (doodad != null)
                StepOnDoodad(enemy, doodad);
        }
        #endregion

        #region (private) End-Of-Turn Methods
        private void ProcessMonsterGeneration()
        {
            // Create monster on generation rate roll
            var createMonster = _modelService.ScenarioConfiguration.DungeonTemplate.MonsterGenerationBase > _randomSequenceGenerator.Get();

            if (!createMonster)
                return;

            // Select enemy templates with: 0) this level range, 2) non-objective, 3) non-unique enemies, 4) Generate on step
            var enemyTemplates = _modelService.ScenarioConfiguration
                                              .EnemyTemplates
                                              .Where(x =>
                                              {
                                                  return !x.IsUnique &&
                                                         !x.IsObjectiveItem &&
                                                          x.GenerateOnStep &&
                                                          x.Level.Contains(_modelService.Level.Number);
                                              })
                                              .ToList();

            if (enemyTemplates.Count <= 0)
                return;

            // Check to see that there is an empty cell available
            var availableLocation = _modelService.Level
                                                 .GetRandomLocation(_modelService
                                                                        .GetVisibleLocations()
                                                                        .Union(new List<CellPoint>() { _modelService.Player.Location }),
                                                                    true,
                                                                    _randomSequenceGenerator);

            if (availableLocation == CellPoint.Empty)
                return;

            // Create enemy from template
            var template = enemyTemplates[_randomSequenceGenerator.Get(0, enemyTemplates.Count)];
            var enemy = _characterGenerator.GenerateEnemy(template, _modelService.Religions, _modelService.GetAttackAttributes());
            
            // Map enemy location to level
            enemy.Location = availableLocation;

            // Add content to level
            _modelService.Level.AddContent(enemy);

            // Queue level update for added content
            RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.ContentAdd, enemy.Id));
        }
        #endregion
    }
}
