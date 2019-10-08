using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Doodad;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using Rogue.NET.Core.Utility;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Scenario.Content.Extension;
using Rogue.NET.Core.Processing.Event.Backend.EventData.Factory.Interface;
using Rogue.NET.Core.Processing.Event.Backend.EventData.ScenarioMessage.Enum;
using Rogue.NET.Core.GameRouter.GameEvent.Backend.Enum;
using Rogue.NET.Core.Processing.Action;
using Rogue.NET.Core.Processing.Action.Enum;
using Rogue.NET.Core.Processing.Model.Content.Interface;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.Core.Processing.Model.Algorithm.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Model.Static;
using Rogue.NET.Core.Processing.Model.Content.Enum;
using Rogue.NET.Common.Extension;
using System.Collections.Generic;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Character.Behavior;

namespace Rogue.NET.Core.Processing.Model.Content
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IContentEngine))]
    public class ContentEngine : BackendEngine, IContentEngine
    {
        readonly IModelService _modelService;
        readonly IPathFinder _pathFinder;
        readonly ILayoutEngine _layoutEngine;
        readonly IAlterationEngine _alterationEngine;
        readonly INonPlayerCharacterProcessor _nonPlayerCharacterProcessor;
        readonly IPlayerProcessor _playerProcessor;        
        readonly IInteractionProcessor _interactionProcessor;
        readonly IScenarioMessageService _scenarioMessageService;
        readonly IRandomSequenceGenerator _randomSequenceGenerator;
        readonly IAlterationGenerator _alterationGenerator;
        readonly ICharacterGenerator _characterGenerator;
        readonly IBackendEventDataFactory _backendEventDataFactory;

        [ImportingConstructor]
        public ContentEngine(
            IModelService modelService, 
            IPathFinder pathFinder,
            ILayoutEngine layoutEngine, 
            IAlterationEngine alterationEngine,
            INonPlayerCharacterProcessor enemyProcessor,
            IPlayerProcessor playerProcessor,
            IInteractionProcessor interactionProcessor,
            IScenarioMessageService scenarioMessageService,
            IRandomSequenceGenerator randomSequenceGenerator,
            IAlterationGenerator alterationGenerator,
            ICharacterGenerator characterGenerator,
            IBackendEventDataFactory backendEventDataFactory)
        {
            _modelService = modelService;
            _pathFinder = pathFinder;
            _layoutEngine = layoutEngine;
            _alterationEngine = alterationEngine;
            _nonPlayerCharacterProcessor = enemyProcessor;
            _playerProcessor = playerProcessor;
            _interactionProcessor = interactionProcessor;
            _scenarioMessageService = scenarioMessageService;
            _randomSequenceGenerator = randomSequenceGenerator;
            _alterationGenerator = alterationGenerator;
            _characterGenerator = characterGenerator;
            _backendEventDataFactory = backendEventDataFactory;
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
                OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.ContentRemove, item.Id));

                if (character is Player)
                {
                    // Publish message
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Found " + _modelService.GetDisplayName(item));

                    if (item is Consumable)
                        OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerConsumableAddOrUpdate, item.Id));
                    else if (item is Equipment)
                        OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerEquipmentAddOrUpdate, item.Id));

                    //Update level statistics
                    OnScenarioEvent(_backendEventDataFactory.StatisticsUpdate(ScenarioUpdateType.StatisticsItemFound, item.RogueName));
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

            // Check Character Class Requirement
            if (equipment.HasCharacterClassRequirement &&
                player.Class != equipment.CharacterClass)
            {
                _scenarioMessageService.Publish(
                    ScenarioMessagePriority.Normal,
                    "Required Character Class Not Met!",
                    equipment.CharacterClass);

                return false;
            }
           
            // Process the update
            if (equipment.IsEquipped)
                result = UnEquip(equipment);

            else
                result = Equip(equipment);

            // Queue player update for this item
            OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerEquipmentAddOrUpdate, equipId));

            return result;
        }
        public void DropPlayerItem(string itemId)
        {
            var item = _modelService.Player.Inventory[itemId];
            var adjacentFreeLocations = _layoutEngine.GetFreeAdjacentLocations(_modelService.Player.Location);
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
                OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerEquipmentRemove, equipment.Id));
                OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.ContentAll, ""));
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
                OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerConsumableRemove, consumable.Id));
                OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.ContentAll, ""));
            }

            // Publish message
            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, displayName + " Dropped");
        }
        public void CharacterDeath(NonPlayerCharacter character)
        {
            for (int i = character.Equipment.Count - 1; i >= 0; i--)
            {
                DropCharacterItem(character, character.Equipment.ElementAt(i).Value);
            }
            for (int i = character.Consumables.Count - 1; i >= 0; i--)
            {
                DropCharacterItem(character, character.Consumables.ElementAt(i).Value);
            }

            // Update level object
            var level = _modelService.Level;

            level.RemoveContent(character);

            // Queue Animation for enemy death
            if (character.DeathAnimation.Animations.Count > 0)
                OnAnimationEvent(_backendEventDataFactory.Animation(character.DeathAnimation, character.Location, new GridLocation[] { character.Location }));

            // (ENEMY ONLY) Calculate player gains
            if (character is Enemy)
            {
                _playerProcessor.CalculateEnemyDeathGains(_modelService.Player, character as Enemy);

                // Update statistics / Player skills
                OnScenarioEvent(_backendEventDataFactory.StatisticsUpdate(ScenarioUpdateType.StatisticsEnemyDeath, character.RogueName));
                OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerSkillSetRefresh, ""));                
            }

            // Publish message depending on character alignment
            if (character.AlignmentType == CharacterAlignmentType.EnemyAligned)
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, character.RogueName + " Slayed");
            }
            else
            {
                if (character is TemporaryCharacter)
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, character.RogueName + " Vanished");

                else
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, character.RogueName + " Died");
            }

            //Set enemy identified
            _modelService.ScenarioEncyclopedia[character.RogueName].IsIdentified = true;

            // Publish Level update
            OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.ContentRemove, character.Id));
            OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.EncyclopediaIdentify, character.Id));
        }
        public void CalculateCharacterReactions()
        {
            // TODO: BUILD A BACKEND SEQUENCER!!!
            //
            // Character Reactions: 0) Check whether character is still alive 
            //                      1) Process Enemy Reaction (Applies End-Of-Turn)
            //                      2) Check for Enemy Death (After Enemy Reaction)
            //
            for (int i = _modelService.Level.NonPlayerCharacters.Count() - 1; i >= 0; i--)
            {
                var character = _modelService.Level.NonPlayerCharacters.ElementAt(i);

                if (character.Hp <= 0)
                    CharacterDeath(character);
                else
                    OnLevelProcessingEvent(new LevelProcessingAction()
                    {
                        Actor = character,
                        Type = LevelProcessingActionType.Reaction
                    });
            }
        }
        public void ProcessCharacterReaction(NonPlayerCharacter character)
        {
            //if (character.Hp <= 0)
            //{
            //    CharacterDeath(character);
            //    return;
            //}

            // All speed is calculated relative to the Player
            character.TurnCounter += _interactionProcessor.CalculateCharacterTurnIncrement(_modelService.Player, character);

            if (character.TurnCounter >= 1)
                OnNonPlayerCharacterReaction(character);

            if (character.Hp <= 0)
            {
                CharacterDeath(character);
                return;
            }

            // Check for temporary character expiration - TODO: MOVE TO A "END OF TURN" METHOD OF SOME KIND
            if (character is TemporaryCharacter)
            {
                var temporaryCharacter = character as TemporaryCharacter;

                // Decrement Lifetime Counter
                temporaryCharacter.LifetimeCounter--;

                // Check for timer expiration
                if (temporaryCharacter.LifetimeCounter <= 0)
                    CharacterDeath(character);
            }
        }
        public override void ApplyEndOfTurn(bool regenerate)
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
                OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.EncyclopediaIdentify, doodad.Id));
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

                        // Queue update to level 
                        if (character is Enemy)
                            OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.ContentMove, character.Id));

                        else
                            OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerLocation, _modelService.Player.Id));
                    }
                    break;
                case DoodadNormalType.Teleport1:
                case DoodadNormalType.Teleport2:
                    {
                        var otherTeleporter = level.DoodadsNormal.First(x => x.PairId == doodad.Id);

                        // Show the other teleporter also if it's hidden
                        otherTeleporter.IsHidden = false;

                        // Have to boot enemy if it's sitting on other teleporter
                        if (character is Player && level.IsCellOccupiedByCharacter(otherTeleporter.Location, _modelService.Player.Location))
                        {
                            var enemy = level.GetAt<Enemy>(otherTeleporter.Location);

                            // Remove from the level
                            level.RemoveContent(enemy);

                            // Queue update to the level
                            OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.ContentRemove, enemy.Id));
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
                            OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.ContentMove, character.Id));

                        else
                            OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerLocation, _modelService.Player.Id));

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

                    // Create Alteration
                    var alteration = _alterationGenerator.GenerateAlteration(doodad.AutomaticAlteration);

                    // Validate Alteration Cost -> Queue with animation
                    if (_alterationEngine.Validate(character, alteration))
                        _alterationEngine.Queue(character, alteration);
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

                OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.EncyclopediaCurseIdentify, equipment.Id));

                return false;
            }
            else
            {
                // Unequip
                equipment.IsEquipped = false;

                // Publish Message
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Un-Equipped " + (metaData.IsIdentified ? equipment.RogueName : "???"));

                // If equip alteration is present - make sure it's deactivated and removed
                if (equipment.HasEquipAlteration)
                {
                    _modelService.Player.Alteration.Remove(equipment.EquipAlteration.Name);

                    // THIS SEEMS TO BE HANDLED BY A BLANKET UPDATE EACH TURN. SO, SOME REFACTORING
                    // NEEDED TO FIX THE UPDATING ISSUE
                    //
                    // Fire update for content to update the aura
                    // RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.ContentUpdate, _modelService.Player.Id));
                }

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

            // Fire equip alteration -> This will activate any passive effects after animating
            if (equipment.HasEquipAlteration)
                _alterationEngine.Queue(_modelService.Player, _alterationGenerator.GenerateAlteration(equipment.EquipAlteration));

            if (equipment.HasCurseAlteration && equipment.IsCursed)
                _alterationEngine.Queue(_modelService.Player, _alterationGenerator.GenerateAlteration(equipment.CurseAlteration));

            return true;
        }
        private void DropCharacterItem(NonPlayerCharacter character, ItemBase item)
        {
            var adjacentFreeLocations = _layoutEngine.GetFreeAdjacentLocations(character.Location);
            var location = adjacentFreeLocations.FirstOrDefault();

            if (location == null)
                return;

            // Provide new location for item
            item.Location = location;

            // Remove from enemy inventory
            if (item is Equipment)
            {
                var equipment = item as Equipment;

                character.Equipment.Remove(item.Id);

                // Mark as non-equipped
                equipment.IsEquipped = false;

                if (equipment.HasEquipAlteration && equipment.IsEquipped)
                    character.Alteration.Remove(equipment.EquipAlteration.Name);

                if (equipment.HasCurseAlteration && equipment.IsEquipped)
                    character.Alteration.Remove(equipment.CurseAlteration.Name);
            }

            if (item is Consumable)
                character.Consumables.Remove(item.Id);

            // Add to level
            _modelService.Level.AddContent(item);
        }
        #endregion

        #region (private) Non-Player Character Reactions
        private void OnNonPlayerCharacterReaction(NonPlayerCharacter character)
        {
            // Sets turn counter 
            int turns = (int)character.TurnCounter;
            character.TurnCounter = character.TurnCounter % 1;

            for (int j = 0; j < turns && character.Hp > 0; j++)
            {
                // Procedure
                //
                // 1) Beginning of Turn:        Applies Start of Turn Process
                // 2) Check Character Death:    Alterations may have affected character
                // 3) Altered States:           Movement / turn taking may be impaired
                // 4) Character Attack:         Attack (If Conditions Met)
                // 5) Character Move:           Move (If No Attack) to desired location
                // 6) End of Turn:              Applies end of turn Process

                // Apply Beginning of Turn
                _nonPlayerCharacterProcessor.ApplyBeginningOfTurn(character);

                if (character.Hp < 0)
                    break;

                //Check altered states

                // Can't Move (Is sleeping, paralyzed, etc..)
                if (character.Is(CharacterStateType.CantMove))
                {
                    // Apply end-of-turn behavior for enemy
                    _nonPlayerCharacterProcessor.ApplyEndOfTurn(character, _modelService.Player, false);
                    continue;
                }

                var actionTaken = false;

                // Calculate Attack if Requirements Met
                Character targetCharacter;
                bool anyCharactersInVisibleRange;
                if (CalculateCharacterWillAttack(character, out targetCharacter, out anyCharactersInVisibleRange))
                {
                    switch (character.BehaviorDetails.CurrentBehavior.AttackType)
                    {
                        case CharacterAttackType.PhysicalCombat:
                            ProcessCharacterAttack(character, targetCharacter);
                            break;
                        case CharacterAttackType.Alteration:
                            ProcessCharacterAlterationAttack(character, targetCharacter);
                            break;
                        case CharacterAttackType.None:
                        default:
                            break;
                    }
                    actionTaken = true;

                    // Set target character alerted
                    if (targetCharacter is NonPlayerCharacter)
                        (targetCharacter as NonPlayerCharacter).IsAlerted = true;
                }
                // If no attack taken calculate a move instead
                else
                {
                    // Desired location is the ultimate destination for the character
                    var desiredLocation = CalculateDesiredLocationInRange(character);

                    if (desiredLocation != GridLocation.Empty)
                    {
                        // Move location is the next location for the character
                        var moveLocation = CalculateCharacterMoveLocation(character, desiredLocation);
                        if (moveLocation != null &&
                            moveLocation != GridLocation.Empty &&
                            moveLocation != _modelService.Player.Location) // TODO: MAKE THIS PART OF THE LAYOUT ENGINE METHODS
                        {
                            ProcessCharacterMove(character, moveLocation);
                            actionTaken = true;
                        }
                    }
                }

                // Reset IsAlerted status if no opposing characters present
                if (!anyCharactersInVisibleRange)
                    character.IsAlerted = false;

                // Apply end-of-turn behavior for enemy
                _nonPlayerCharacterProcessor.ApplyEndOfTurn(character, _modelService.Player, actionTaken);
            }
        }
        private GridLocation CalculateCharacterMoveLocation(NonPlayerCharacter character, GridLocation desiredLocation)
        {
            // NOTE*** Reserving character-character swapping for the Player ONLY
            //

            //Return random if confused
            if (character.Is(CharacterStateType.MovesRandomly))
                return _layoutEngine.GetRandomAdjacentLocationForMovement(character.Location, CharacterAlignmentType.None) ?? GridLocation.Empty;

            switch (character.BehaviorDetails.CurrentBehavior.MovementType)
            {
                case CharacterMovementType.Random:
                    return _layoutEngine.GetRandomAdjacentLocationForMovement(character.Location, CharacterAlignmentType.None);
                case CharacterMovementType.HeatSeeker:
                    return _layoutEngine.GetFreeAdjacentLocationsForMovement(character.Location, CharacterAlignmentType.None)
                                        .MinBy(x => Calculator.RoguianDistance(x, desiredLocation)) ?? GridLocation.Empty;
                case CharacterMovementType.StandOffIsh:
                    return _layoutEngine.GetFreeAdjacentLocationsForMovement(character.Location, CharacterAlignmentType.None)
                                        .OrderBy(x => Calculator.RoguianDistance(x, desiredLocation))
                                        .LastOrDefault() ?? GridLocation.Empty;
                case CharacterMovementType.PathFinder:
                    var nextLocation = _pathFinder.FindPath(character.Location, desiredLocation, character.GetLightRadius(), character.BehaviorDetails.CanOpenDoors, CharacterAlignmentType.None);
                    return nextLocation ?? _layoutEngine.GetFreeAdjacentLocationsForMovement(character.Location, CharacterAlignmentType.None)
                                                        .OrderBy(x => Calculator.RoguianDistance(x, desiredLocation))
                                                        .FirstOrDefault() ?? GridLocation.Empty;
                default:
                    throw new Exception("Unknown Enemy Movement Type");
            }
        }
        private GridLocation CalculateDesiredLocationInRange(NonPlayerCharacter character)
        {
            // Desired Location
            //
            // 1) Player-Aligned:  Either nearest target character in range (or) towards Player
            // 2) Enemy-Aligned:   Nearest target character in range
            //

            var opposingCharacters = CalculateOpposingCharactersInVisibleRange(character);

            // Move into attack position
            if (opposingCharacters.Any())
            {
                return opposingCharacters.MinBy(x => Calculator.RoguianDistance(character.Location, x.Location))
                                         .Location;
            }

            // If Player-Aligned - Move with Player (if they're in range)
            else if (character.AlignmentType == CharacterAlignmentType.PlayerAligned &&
                     _modelService.CharacterContentInformation
                                  .GetVisibleCharacters(character)
                                  .Contains(_modelService.Player))
            {
                // For friendlies - have to set flag to notify that they're now in the player "party"
                // which will move them from level to level
                if (character is Friendly)
                    (character as Friendly).InPlayerParty = true;

                // TODO: Need some other parameters like: "Keep a certain distance from Player"
                return _modelService.Player.Location;
            }
            else
                return GridLocation.Empty;
        }
        private void ProcessCharacterMove(NonPlayerCharacter character, GridLocation moveLocation)
        {
            // Case where path finding algorithm returns null; or heat seeking algorithm
            // returns null.
            if (moveLocation == null)
                return;

            // Check for opening of doors
            var openingPosition1 = GridLocation.Empty;
            var openingPosition2 = GridLocation.Empty;
            var openingDirection2 = Compass.Null;
            var shouldMoveToOpeningPosition1 = false;

            var moveDirection = LevelGridExtension.GetDirectionBetweenAdjacentPoints(character.Location, moveLocation);

            var throughDoor = moveDirection == Compass.Null ? false : _layoutEngine.IsPathToCellThroughDoor(
                                character.Location,
                                moveDirection,
                                out openingPosition1,
                                out openingPosition2,
                                out openingDirection2,
                                out shouldMoveToOpeningPosition1);

            // Behavior allows opening of doors
            if (character.BehaviorDetails.CanOpenDoors && throughDoor)
            {
                // If have to move into position first then move
                if (shouldMoveToOpeningPosition1)
                {
                    // Have to move to opening position 1 first - which means the door is on one of the off-diagonal locations
                    if (_layoutEngine.IsPathToAdjacentCellBlocked(character.Location, openingPosition1, true, character.AlignmentType))
                    {
                        // Update enemy location
                        character.Location = openingPosition1;

                        // Notify listener queue
                        OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.ContentMove, character.Id));
                    }
                }

                // Can open the door where the enemy is at
                else
                {
                    // Open the door -> Notifies UI listeners
                    _layoutEngine.ToggleDoor(moveDirection, character.Location);
                }
            }

            // GOING TO EXCLUDE CHARACTER / CHARACTER SWAP FOR THE SAME ALIGNMENT (reserved for Player only)
            //
            else if (!throughDoor &&
                     !_layoutEngine.IsPathToAdjacentCellBlocked(character.Location, moveLocation, true, CharacterAlignmentType.None))
            {
                // Update enemy location
                character.Location = moveLocation;

                // Notify listener queue
                OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.ContentMove, character.Id));
            }

            // Check for items - DON'T ALLOW TEMPORARY CHARACTERS / FRIENDLIES TO PICK UP ITEMS
            var item = _modelService.Level.GetAt<ItemBase>(character.Location);
            if (item != null &&
                character is Enemy)
                StepOnItem(character, item);

            // Check for doodad
            var doodad = _modelService.Level.GetAt<DoodadBase>(character.Location);
            if (doodad != null)
                StepOnDoodad(character, doodad);
        }
        private IEnumerable<Character> CalculateOpposingCharactersInVisibleRange(NonPlayerCharacter character)
        {
            IEnumerable<Character> opposingCharactersInRange = null;

            // Player Aligned
            if (character.AlignmentType == CharacterAlignmentType.PlayerAligned)
            {
                opposingCharactersInRange = _modelService
                                                .CharacterContentInformation
                                                .GetVisibleCharacters(character)
                                                .Where(x => x is NonPlayerCharacter)
                                                .Cast<NonPlayerCharacter>()
                                                .Where(x => x.AlignmentType == CharacterAlignmentType.EnemyAligned)
                                                .Actualize();
            }

            // Enemy Aligned
            else
            {
                // Adding a cast to provide a way to see IComparable (see MoreLinq MinBy)
                opposingCharactersInRange = _modelService
                                                .CharacterContentInformation
                                                .GetVisibleCharacters(character)
                                                .Where(x => x is Friendly || x is Player)
                                                .Cast<Character>()
                                                .Actualize();
            }

            return opposingCharactersInRange.Actualize();
        }
        private bool CalculateCharacterWillAttack(NonPlayerCharacter character, out Character targetCharacter, out bool anyCharactersInVisibleRange)
        {
            targetCharacter = null;
            anyCharactersInVisibleRange = false;

            // Check that character doens't have abnormal state that prevents attack
            if (character.Is(CharacterStateType.MovesRandomly | CharacterStateType.Blind))
                return false;

            // Get all characters in sight range
            var opposingCharactersInVisibleRange = CalculateOpposingCharactersInVisibleRange(character);

            // Filter out cases where they're not "noticed" (use IsAlerted flag en-mas)
            var opposingCharacterTargets = opposingCharactersInVisibleRange
                                                .Where(x => character.IsAlerted || 
                                                           !x.Is(CharacterStateType.Invisible) || 
                                                          (!character.IsAlerted && x.Is(CharacterStateType.Invisible))).Actualize();

            var adjacentLocations = _modelService.Level.Grid.GetAdjacentLocations(character.Location);
            var nearestTargetCharacter = opposingCharacterTargets.MinBy(x => Calculator.RoguianDistance(x.Location, character.Location));

            // Set flag to notify any characters in sight range
            anyCharactersInVisibleRange = opposingCharactersInVisibleRange.Any();

            // Target character is nearest visible character
            targetCharacter = nearestTargetCharacter;

            if (targetCharacter == null)
                return false;

            // Check that target character isn't invisible
            if (targetCharacter.Is(CharacterStateType.Invisible) &&
               !character.IsAlerted)
                return false;

            // Figure out whether or not character will attack or move
            switch (character.BehaviorDetails.CurrentBehavior.AttackType)
            {
                case CharacterAttackType.PhysicalCombat:
                    {
                        if (character.IsEquippedRangeCombat())
                            return opposingCharacterTargets.Any();
                        else
                        {
                            return adjacentLocations.Any(x =>
                            {
                                return opposingCharacterTargets.Select(z => z.Location).Contains(x);
                            });
                        }
                    }
                // This should depend on the alteration details
                case CharacterAttackType.Alteration:
                    return opposingCharacterTargets.Any();
                case CharacterAttackType.None:
                    return false;
                default:
                    throw new Exception("Unhandled Character Attack Type");
            }
        }
        private void ProcessCharacterAttack(NonPlayerCharacter character, Character targetCharacter)
        {
            var adjacentLocations = _modelService.Level.Grid.GetAdjacentLocations(character.Location);
            var isTargetAdjacent = adjacentLocations.Contains(targetCharacter.Location);

            // If Adjacent Opposing Character
            if (isTargetAdjacent)
            {
                var success = _interactionProcessor.CalculateInteraction(character, targetCharacter, PhysicalAttackType.Melee);

                // If Successful, process Equipment Attack Alterations
                foreach (var alteration in character.Equipment
                                                    .Values
                                                    .Where(x => x.IsEquipped)
                                                    .Where(x => x.HasAttackAlteration)
                                                    .Select(x => _alterationGenerator.GenerateAlteration(x.AttackAlteration)))
                {
                    // Validate -> Queue Equipment Attack Alteration
                    if (_alterationEngine.Validate(character, alteration))
                        _alterationEngine.Queue(character, new Character[] { targetCharacter }, alteration);
                }
            }

            // Otherwise, check for range combat
            else if (character.IsEquippedRangeCombat())
            {
                // Check for line of sight and firing range
                var range = Calculator.RoguianDistance(character.Location, targetCharacter.Location);

                // These are guaranteed by the enemy check IsRangeMelee()
                var rangeWeapon = character.Equipment.Values.First(x => x.IsEquipped && x.Type == EquipmentType.RangeWeapon);
                var ammo = character.Consumables.Values.First(x => x.RogueName == rangeWeapon.AmmoName);

                if (range > ModelConstants.MinFiringDistance)
                {
                    // Remove ammo from enemy inventory
                    character.Consumables.Remove(ammo.Id);

                    // Calculate hit - if enemy hit then queue Ammunition spell
                    _interactionProcessor.CalculateInteraction(character, targetCharacter, PhysicalAttackType.Range);
                }
            }
        }
        private void ProcessCharacterAlterationAttack(NonPlayerCharacter character, Character targetCharacter)
        {
            AlterationContainer alteration;

            // Cast the appropriate behavior
            var template = character.BehaviorDetails.CurrentBehavior.Alteration;

            // Create the alteration
            alteration = _alterationGenerator.GenerateAlteration(template);

            // Post alteration message 
            _scenarioMessageService.PublishAlterationCombatMessage(
                character.AlignmentType,
                _modelService.GetDisplayName(character),
                _modelService.GetDisplayName(targetCharacter),
                alteration.RogueName);

            // Validate the cost and process
            if (_alterationEngine.Validate(character, alteration))
                _alterationEngine.Queue(character, alteration);
        }
        #endregion

        #region (private) End-Of-Turn Methods
        private void ProcessMonsterGeneration()
        {
            // Create monster on generation rate roll
            var createMonster = _modelService.GetLevelBranch().MonsterGenerationPerStep > _randomSequenceGenerator.Get();

            if (!createMonster)
                return;

            // Select enemy templates with: 0) this level range, 2) non-objective, 3) non-unique enemies, 4) Generate on step
            var enemyTemplates = _modelService.GetLevelBranch()
                                              .Enemies
                                              .Where(x =>
                                              {
                                                  return !x.Asset.IsUnique &&
                                                         !x.Asset.IsObjectiveItem &&
                                                          x.Asset.GenerateOnStep;
                                              })
                                              .ToList();

            if (enemyTemplates.Count <= 0)
                return;

            // Check to see that there is an empty cell available
            var availableLocation = 
                    _modelService.Level
                                 .GetRandomLocation(_modelService.CharacterLayoutInformation
                                                                 .GetVisibleLocations(_modelService.Player),
                                                    true,
                                                    _randomSequenceGenerator);

            if (availableLocation == GridLocation.Empty)
                return;

            // Create enemy from template
            var template = _randomSequenceGenerator.GetWeightedRandom(enemyTemplates, x => x.GenerationWeight);
            var enemy = _characterGenerator.GenerateEnemy(template.Asset);
            
            // Map enemy location to level
            enemy.Location = availableLocation;

            // Add content to level -> Update Visibility
            _modelService.Level.AddContent(enemy);
            _modelService.UpdateVisibility();

            // Queue level update for added content
            OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.ContentAdd, enemy.Id));
        }
        #endregion
    }
}
