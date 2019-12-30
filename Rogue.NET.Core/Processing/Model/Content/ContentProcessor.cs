using Rogue.NET.Common.Extension;
using Rogue.NET.Core.GameRouter.GameEvent.Backend.Enum;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using Rogue.NET.Core.Model.Scenario.Content.Doodad;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Action;
using Rogue.NET.Core.Processing.Action.Enum;
using Rogue.NET.Core.Processing.Event.Backend.EventData.Factory.Interface;
using Rogue.NET.Core.Processing.Event.Backend.EventData.ScenarioMessage.Enum;
using Rogue.NET.Core.Processing.Model.Algorithm.Interface;
using Rogue.NET.Core.Processing.Model.Content.Calculator;
using Rogue.NET.Core.Processing.Model.Content.Calculator.Interface;
using Rogue.NET.Core.Processing.Model.Content.Enum;
using Rogue.NET.Core.Processing.Model.Content.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.Core.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Content
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IContentProcessor))]
    public class ContentProcessor : BackendProcessor, IContentProcessor
    {
        readonly IModelService _modelService;
        readonly IPathFinder _pathFinder;
        readonly IAlterationProcessor _alterationProcessor;
        readonly INonPlayerCharacterCalculator _nonPlayerCharacterCalculator;
        readonly IPlayerCalculator _playerCalculator;
        readonly IInteractionCalculator _interactionCalculator;
        readonly IScenarioMessageService _scenarioMessageService;
        readonly IRandomSequenceGenerator _randomSequenceGenerator;
        readonly IAlterationGenerator _alterationGenerator;
        readonly ICharacterGenerator _characterGenerator;
        readonly IBackendEventDataFactory _backendEventDataFactory;

        [ImportingConstructor]
        public ContentProcessor(
            IModelService modelService,
            IPathFinder pathFinder,
            IAlterationProcessor alterationProcessor,
            INonPlayerCharacterCalculator nonPlayerCharacterCalculator,
            IPlayerCalculator playerCalculator,
            IInteractionCalculator interactionCalculator,
            IScenarioMessageService scenarioMessageService,
            IRandomSequenceGenerator randomSequenceGenerator,
            IAlterationGenerator alterationGenerator,
            ICharacterGenerator characterGenerator,
            IBackendEventDataFactory backendEventDataFactory)
        {
            _modelService = modelService;
            _pathFinder = pathFinder;
            _alterationProcessor = alterationProcessor;
            _nonPlayerCharacterCalculator = nonPlayerCharacterCalculator;
            _playerCalculator = playerCalculator;
            _interactionCalculator = interactionCalculator;
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
                level.RemoveContent(item.Id);

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
            var adjacentFreeLocations = _modelService.LayoutService.GetFreeAdjacentLocations(_modelService.PlayerLocation);
            var dropLocation = _randomSequenceGenerator.GetRandomElement(adjacentFreeLocations);

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

                // Remove from inventory
                _modelService.Player.Equipment.Remove(equipment.Id);

                // Add level content
                _modelService.Level.AddContent(equipment, dropLocation);

                // Queue updates
                OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerEquipmentRemove, equipment.Id));
                OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.ContentAll, ""));
            }
            if (item is Consumable)
            {
                var consumable = item as Consumable;

                // Remove from inventory
                _modelService.Player.Consumables.Remove(consumable.Id);

                // Add level content
                _modelService.Level.AddContent(consumable, dropLocation);

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
            var characterLocation = _modelService.GetLocation(character);

            level.RemoveContent(character.Id);

            // Queue Animation for enemy death
            if (character.DeathAnimation.Animations.Count > 0)
                OnAnimationEvent(_backendEventDataFactory.Animation(character.DeathAnimation, characterLocation, new GridLocation[] { characterLocation }));

            // (ENEMY ONLY) Calculate player gains
            if (character is Enemy)
            {
                _playerCalculator.CalculateEnemyDeathGains(_modelService.Player, character as Enemy);

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
            character.TurnCounter += _interactionCalculator.CalculateCharacterTurnIncrement(_modelService.Player, character);

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
#if DEBUG_MINIMUM_CONTENT

            // Don't process monster generation

#else
            ProcessMonsterGeneration();
#endif
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
                case DoodadNormalType.Transporter:
                    {
                        // Cycle through the ID list and take the next transporter
                        //
                        // TODO: IF ENEMY IS ON THE TRANSPORTER - FIND A NEW LOCATION FOR IT. DON'T REMOVE IT - 
                        //       IT MAY BE AN OBJECTIVE
                        //

                        //var nextTransporterId = doodad.TransportGroupIds.Next(doodad.Id);
                        //var nextTransporter = level.GetContent(nextTransporterId);

                        //// Have to swap locations if character is sitting on other teleporter
                        //if (level.IsCellOccupiedByCharacter(nextTransporter.Location, _modelService.Player.Location))
                        //{
                        //    // Fetch other character
                        //    var otherCharacter = level.GetAt<Character>(nextTransporter.Location);

                        //    // Set location to character location
                        //    otherCharacter.Location = character.Location;

                        //    OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.ContentMove, otherCharacter.Id));
                        //}

                        //// Set character location to other teleporter
                        //character.Location = nextTransporter.Location;

                        //if (character is Player)
                        //{
                        //    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "You were transported!");

                        //    OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerLocation, _modelService.Player.Id));
                        //}
                        //else
                        //    OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.ContentMove, character.Id));

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
                    if (_alterationProcessor.Validate(character, alteration))
                        _alterationProcessor.Queue(character, alteration);
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
                        var equippedItem = _playerCalculator.GetEquippedType(_modelService.Player, equipment.Type);
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
                        var handsFree = _playerCalculator.GetNumberOfFreeHands(_modelService.Player);

                        if (((equipment.Type == EquipmentType.TwoHandedMeleeWeapon || equipment.Type == EquipmentType.RangeWeapon) && handsFree < 2) || handsFree < 1)
                        {
                            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Must first free up a hand");
                            return false;
                        }
                    }
                    break;
                case EquipmentType.Ring:
                    {
                        var ring = _playerCalculator.GetEquippedType(_modelService.Player, EquipmentType.Ring);
                        var numberEquipped = _playerCalculator.GetNumberEquipped(_modelService.Player, EquipmentType.Ring);

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
                _alterationProcessor.Queue(_modelService.Player, _alterationGenerator.GenerateAlteration(equipment.EquipAlteration));

            if (equipment.HasCurseAlteration && equipment.IsCursed)
                _alterationProcessor.Queue(_modelService.Player, _alterationGenerator.GenerateAlteration(equipment.CurseAlteration));

            return true;
        }
        private void DropCharacterItem(NonPlayerCharacter character, ItemBase item)
        {
            var characterLocation = _modelService.GetLocation(character);
            var adjacentFreeLocations = _modelService.LayoutService.GetFreeAdjacentLocations(characterLocation);
            var location = adjacentFreeLocations.FirstOrDefault();

            if (location == null)
                return;

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
            _modelService.Level.AddContent(item, location);
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
                _nonPlayerCharacterCalculator.ApplyBeginningOfTurn(character);

                if (character.Hp < 0)
                    break;

                //Check altered states

                // Can't Move (Is sleeping, paralyzed, etc..)
                if (character.Is(CharacterStateType.CantMove))
                {
                    // Apply end-of-turn behavior for enemy
                    _nonPlayerCharacterCalculator.ApplyEndOfTurn(character, _modelService.Player, false);
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

                    if (desiredLocation != null)
                    {
                        // Move location is the next location for the character
                        var moveLocation = CalculateCharacterMoveLocation(character, desiredLocation);
                        if (moveLocation != null &&
                            moveLocation != null &&
                           !moveLocation.Equals(_modelService.PlayerLocation)) // TODO: MAKE THIS PART OF THE LAYOUT ENGINE METHODS
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
                _nonPlayerCharacterCalculator.ApplyEndOfTurn(character, _modelService.Player, actionTaken);
            }
        }
        private GridLocation CalculateCharacterMoveLocation(NonPlayerCharacter character, GridLocation desiredLocation)
        {
            // NOTE*** Reserving character-character swapping for the Player ONLY
            //

            var characterLocation = _modelService.GetLocation(character);

            //Return random if confused
            if (character.Is(CharacterStateType.MovesRandomly))
                return _modelService.LayoutService.GetRandomAdjacentLocationForMovement(characterLocation, character.AlignmentType);

            // TODO:TERRAIN
            switch (character.BehaviorDetails.CurrentBehavior.MovementType)
            {
                case CharacterMovementType.Random:
                    {
                        return _modelService.LayoutService.GetRandomAdjacentLocationForMovement(characterLocation, character.AlignmentType);
                    }
                case CharacterMovementType.HeatSeeker:
                    {
                        return _modelService.LayoutService
                                            .GetFreeAdjacentLocationsForMovement(characterLocation, character.AlignmentType)
                                            .MinBy(x => Metric.RoguianDistance(x, desiredLocation));
                    }
                case CharacterMovementType.StandOffIsh:
                    {
                        return _modelService.LayoutService
                                            .GetFreeAdjacentLocationsForMovement(characterLocation, character.AlignmentType)
                                            .OrderBy(x => Metric.RoguianDistance(x, desiredLocation))
                                            .LastOrDefault();
                    }
                case CharacterMovementType.PathFinder:
                    {
                        var nextLocation = _pathFinder.FindCharacterNextPathLocation(characterLocation, desiredLocation, character.AlignmentType);

                        if (nextLocation == null)
                            return _modelService.LayoutService
                                                .GetFreeAdjacentLocationsForMovement(characterLocation, character.AlignmentType)
                                                .OrderBy(x => Metric.RoguianDistance(x, desiredLocation))
                                                .FirstOrDefault();
                        else
                            return nextLocation;
                    }
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

            var characterLocation = _modelService.GetLocation(character);
            var opposingCharacters = CalculateCharactersInVisibleRange(character, true);
            var alignedCharacters = CalculateCharactersInVisibleRange(character, false);

            // Move into attack position
            if (opposingCharacters.Any())
            {
                var opposingCharacter = opposingCharacters.MinBy(x => Metric.RoguianDistance(characterLocation, _modelService.GetLocation(x)));

                return _modelService.GetLocation(opposingCharacter);
            }

            // If Player-Aligned - Move with Player (if they're in range)
            else if (character.AlignmentType == CharacterAlignmentType.PlayerAligned &&
                     alignedCharacters.Contains(_modelService.Player))
            {
                // For friendlies - have to set flag to notify that they're now in the player "party"
                // which will move them from level to level
                if (character is Friendly)
                    (character as Friendly).InPlayerParty = true;

                // TODO: Need some other parameters like: "Keep a certain distance from Player"
                return _modelService.PlayerLocation;
            }
            else
                return null;
        }
        private void ProcessCharacterMove(NonPlayerCharacter character, GridLocation moveLocation)
        {
            // Case where path finding algorithm returns null; or heat seeking algorithm
            // returns null.
            if (moveLocation == null)
                return;

            var characterLocation = _modelService.GetLocation(character);
            var moveDirection = GridCalculator.GetDirectionOfAdjacentLocation(characterLocation, moveLocation);

            // GOING TO EXCLUDE CHARACTER / CHARACTER SWAP FOR THE SAME ALIGNMENT (reserved for Player only)
            //
            if (!_modelService.LayoutService.IsPathToAdjacentCellBlocked(characterLocation, moveLocation, true, CharacterAlignmentType.None))
            {
                // Update enemy location
                _modelService.Level.MoveContent(character, moveLocation);

                // Notify listener queue
                OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.ContentMove, character.Id));
            }

            // Check for items - DON'T ALLOW TEMPORARY CHARACTERS / FRIENDLIES TO PICK UP ITEMS
            var item = _modelService.Level.GetAt<ItemBase>(moveLocation);
            if (item != null &&
                character is Enemy)
                StepOnItem(character, item);

            // Check for doodad
            var doodad = _modelService.Level.GetAt<DoodadBase>(moveLocation);
            if (doodad != null)
                StepOnDoodad(character, doodad);
        }
        private IEnumerable<Character> CalculateCharactersInVisibleRange(NonPlayerCharacter character, bool opposingAlignment)
        {
            IEnumerable<Character> charactersInRange = null;

            var visibleLocations = _modelService.CharacterLayoutInformation.GetVisibleLocations(character);

            var playerAligned = (character.AlignmentType == CharacterAlignmentType.PlayerAligned && !opposingAlignment) ||
                                (character.AlignmentType == CharacterAlignmentType.EnemyAligned && opposingAlignment);

            // Player Aligned
            if (playerAligned)
            {
                charactersInRange = _modelService.Level
                                                 .GetManyAt<Character>(visibleLocations)
                                                 .Where(character => character is Player || 
                                                                    (character is NonPlayerCharacter && 
                                                                    (character as NonPlayerCharacter).AlignmentType == CharacterAlignmentType.PlayerAligned))
                                                 .Actualize();
            }

            // Enemy Aligned
            else
            {
                charactersInRange = _modelService.Level
                                                 .GetManyAt<NonPlayerCharacter>(visibleLocations)
                                                 .Where(character => character.AlignmentType == CharacterAlignmentType.EnemyAligned)
                                                 .Actualize();
            }

            return charactersInRange;
        }
        private bool CalculateCharacterWillAttack(NonPlayerCharacter character, out Character targetCharacter, out bool anyCharactersInVisibleRange)
        {
            targetCharacter = null;
            anyCharactersInVisibleRange = false;

            // Check that character doens't have abnormal state that prevents attack
            if (character.Is(CharacterStateType.MovesRandomly | CharacterStateType.Blind))
                return false;

            // Get all characters in sight range
            var opposingCharactersInVisibleRange = CalculateCharactersInVisibleRange(character, true);

            // Filter out cases where they're not "noticed" (use IsAlerted flag en-mas)
            var opposingCharacterTargets = opposingCharactersInVisibleRange
                                                .Where(x => character.IsAlerted ||
                                                           !x.Is(CharacterStateType.Invisible) ||
                                                          (!character.IsAlerted && x.Is(CharacterStateType.Invisible))).Actualize();

            var characterLocation = _modelService.GetLocation(character);
            var adjacentLocations = _modelService.Level.Grid.GetAdjacentLocations(characterLocation);
            var nearestTargetCharacter = opposingCharacterTargets.MinBy(x => Metric.RoguianDistance(_modelService.GetLocation(x), characterLocation));

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
                                return opposingCharacterTargets.Select(z => _modelService.GetLocation(z)).Contains(x);
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
            var characterLocation = _modelService.GetLocation(character);
            var targetLocation = _modelService.GetLocation(targetCharacter);
            var adjacentLocations = _modelService.Level.Grid.GetAdjacentLocations(characterLocation);
            var isTargetAdjacent = adjacentLocations.Contains(targetLocation);

            // If Adjacent Opposing Character
            if (isTargetAdjacent &&
                _interactionCalculator.CalculateInteraction(character, targetCharacter, PhysicalAttackType.Melee))
            {
                // If Successful, process Equipment Attack Alterations
                foreach (var alteration in character.Equipment
                                                    .Values
                                                    .Where(x => x.IsEquipped)
                                                    .Where(x => x.HasAttackAlteration)
                                                    .Select(x => _alterationGenerator.GenerateAlteration(x.AttackAlteration)))
                {
                    // Validate -> Queue Equipment Attack Alteration
                    if (_alterationProcessor.Validate(character, alteration))
                        _alterationProcessor.Queue(character, new Character[] { targetCharacter }, alteration);
                }
            }

            // Otherwise, check for range combat
            else if (character.IsEquippedRangeCombat())
            {
                // Check for line of sight and firing range
                var range = Metric.RoguianDistance(characterLocation, targetLocation);

                // These are guaranteed by the enemy check IsRangeMelee()
                var rangeWeapon = character.Equipment.Values.First(x => x.IsEquipped && x.Type == EquipmentType.RangeWeapon);
                var ammo = character.Consumables.Values.First(x => x.RogueName == rangeWeapon.AmmoName);

                if (range > ModelConstants.MinFiringDistance)
                {
                    // Remove ammo from enemy inventory
                    character.Consumables.Remove(ammo.Id);

                    // Calculate hit - if enemy hit then queue Ammunition spell
                    _interactionCalculator.CalculateInteraction(character, targetCharacter, PhysicalAttackType.Range);

                    // Queue animation
                    OnProjectileAnimationEvent(_backendEventDataFactory.AmmoAnimation(ammo,
                                                                                      characterLocation,
                                                                                      targetLocation));
                }
            }
        }
        private void ProcessCharacterAlterationAttack(NonPlayerCharacter character, Character targetCharacter)
        {
            // Cast the appropriate behavior
            var template = character.BehaviorDetails.CurrentBehavior.Alteration;

            // Create the alteration
            var alteration = _alterationGenerator.GenerateAlteration(template);

            // Calculate Alteration Block
            var blocked = _interactionCalculator.CalculateAlterationBlock(character, targetCharacter, alteration.BlockType);

            // Check for specific Alteation Category blocking
            var categoryBlocked = targetCharacter.Alteration.IsAlterationBlocked(alteration.Category);

            // Check Alteration Validation
            var validated = _alterationProcessor.Validate(character, alteration);

            if (!validated)
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal,
                                                "{0} can't use their skill {1}",
                                                _modelService.GetDisplayName(character));

                return;
            }

            // TODO: ADD SPECIAL BLOCK MESSAGE FOR CATEGORY BLOCK

            if (blocked || categoryBlocked)
            {
                // Post block message
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal,
                                                "{0} has blocked the attack!",
                                                _modelService.GetDisplayName(targetCharacter));
            }
            else
            {
                // Post alteration message 
                _scenarioMessageService.PublishAlterationCombatMessage(
                    character.AlignmentType,
                    _modelService.GetDisplayName(character),
                    _modelService.GetDisplayName(targetCharacter),
                    alteration.RogueName);

                // Queue alteration
                _alterationProcessor.Queue(character, alteration);
            }
        }
        #endregion

        #region (private) End-Of-Turn Methods
        private void ProcessMonsterGeneration()
        {
            // Create monster on generation rate roll
            if (_randomSequenceGenerator.Get() >= _modelService.Level.Parameters.EnemyGenerationPerStep)
                return;

            // Select enemy templates with: 0) this level range, 2) non-objective, 3) non-unique enemies, 4) Generate on step
            var enemyTemplates = _modelService.GetEnemyTemplates()
                                              .Where(x =>
                                              {
                                                  return !x.Asset.IsUnique &&
                                                         !x.Asset.IsObjectiveItem &&
                                                          x.Asset.GenerateOnStep;
                                              })
                                              .ToList();

            if (enemyTemplates.Count <= 0)
                return;

            // Fetch locations visible to the player
            var visibleLocations = _modelService.CharacterLayoutInformation.GetVisibleLocations(_modelService.Player);

            // Create enemy from template
            var template = _randomSequenceGenerator.GetWeightedRandom(enemyTemplates, x => x.GenerationWeight);
            var enemy = _characterGenerator.GenerateEnemy(template.Asset, _modelService.ScenarioEncyclopedia);

            // Add content to level -> Update Visibility
            _modelService.Level.AddContentRandom(_randomSequenceGenerator, enemy, ContentRandomPlacementType.Random, visibleLocations);
            _modelService.UpdateVisibility();

            // Queue level update for added content
            OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.ContentAdd, enemy.Id));
        }
        #endregion
    }
}
