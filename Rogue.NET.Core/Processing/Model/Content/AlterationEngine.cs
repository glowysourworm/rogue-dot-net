﻿using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Consumable;
using Rogue.NET.Core.Model.Scenario.Character;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Alteration.Doodad;
using Rogue.NET.Core.Model.Scenario.Alteration.Enemy;
using Rogue.NET.Core.Model.Scenario.Alteration.Equipment;
using Rogue.NET.Core.Model.Scenario.Alteration.Skill;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Content.Extension;
using Rogue.NET.Core.Model.Scenario.Alteration.Extension;
using Rogue.NET.Core.Model.Scenario.Content.Item.Extension;
using Rogue.NET.Core.Processing.Event.Backend.EventData.Factory.Interface;
using Rogue.NET.Core.Processing.Action;
using Rogue.NET.Core.Processing.Event.Backend.EventData.ScenarioMessage.Enum;
using Rogue.NET.Core.Processing.Action.Enum;
using Rogue.NET.Core.GameRouter.GameEvent.Backend.Enum;
using Rogue.NET.Core.Processing.Event.Dialog.Enum;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Model.Content.Interface;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.Core.Model.Scenario.Character.Extension;

namespace Rogue.NET.Core.Processing.Model.Content
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IAlterationEngine))]
    public class AlterationEngine : BackendEngine, IAlterationEngine
    {
        readonly IModelService _modelService;
        readonly ILayoutEngine _layoutEngine;
        readonly ITargetingService _targetingService;
        readonly ICharacterGenerator _characterGenerator;
        readonly IAlterationProcessor _alterationProcessor;
        readonly IInteractionProcessor _interactionProcessor;        
        readonly IScenarioMessageService _scenarioMessageService;
        readonly IRandomSequenceGenerator _randomSequenceGenerator;
        readonly IBackendEventDataFactory _backendEventDataFactory;

        [ImportingConstructor]
        public AlterationEngine(IModelService modelService,
                                ILayoutEngine layoutEngine,
                                ITargetingService targetingService,
                                ICharacterGenerator characterGenerator,
                                IAlterationProcessor alterationProcessor,
                                IInteractionProcessor interactionProcessor,
                                IScenarioMessageService scenarioMessageService,
                                IRandomSequenceGenerator randomSequenceGenerator,
                                IBackendEventDataFactory backendEventDataFactory)
        {
            _modelService = modelService;
            _layoutEngine = layoutEngine;
            _targetingService = targetingService;
            _characterGenerator = characterGenerator;
            _alterationProcessor = alterationProcessor;
            _interactionProcessor = interactionProcessor;
            _scenarioMessageService = scenarioMessageService;
            _randomSequenceGenerator = randomSequenceGenerator;
            _backendEventDataFactory = backendEventDataFactory;
        }

        public bool Validate(Character actor, AlterationContainer alteration)
        {
            // First, validate the Alteration Cost
            if (!_alterationProcessor.CalculateMeetsAlterationCost(actor, alteration.Cost))
                return false;

            // Then, anything else involving the Alteration

            return true;
        }

        public void Queue(Character actor, AlterationContainer alteration)
        {
            // Calculate Affected Characters
            var affectedCharacters = CalculateAffectedCharacters(alteration, actor);

            Queue(actor, affectedCharacters, alteration);
        }

        public void Queue(Character actor, IEnumerable<Character> affectedCharacters, AlterationContainer alteration)
        {
            // Run animations before applying alterations
            if (alteration.SupportsAnimations() &&
                alteration.AnimationGroup.Animations.Count > 0)
            {
                // NOTE*** For animations refactored the animation type
                //         to always assume affected characters. This will greatly simplify 
                //         the parameter space and logic around Alteration -> Animation.

                // TODO: REMOVE THIS.  This is an aid in validating animations. This needs
                //                     to be moved to the editor.
                var animationIssueDetected = false;

                alteration.AnimationGroup.Animations.ForEach(x =>
                {
                    if (x.BaseType == AnimationBaseType.Chain ||
                        x.BaseType == AnimationBaseType.ChainReverse ||
                        x.BaseType == AnimationBaseType.Projectile ||
                        x.BaseType == AnimationBaseType.ProjectileReverse)
                    {
                        if (affectedCharacters.Any(z => z == actor))
                        {
                            animationIssueDetected = true;

                            _scenarioMessageService.Publish(ScenarioMessagePriority.Bad,
                                                            "***Alteration has improper animation usage - " + alteration.RogueName);
                        }
                    }
                });

                if (!animationIssueDetected)
                {
                    OnAnimationEvent(_backendEventDataFactory.Animation(
                                                alteration.AnimationGroup.Animations,
                                                actor.Location,
                                                affectedCharacters.Select(x => x.Location).Actualize()));
                }
            }

            // Apply Effect on Queue
            OnLevelProcessingEvent(new LevelProcessingAction()
            {
                Type = LevelProcessingActionType.CharacterAlteration,
                Actor = actor,
                Alteration = alteration,
                AlterationAffectedCharacters = affectedCharacters
            });
        }

        public void Process(Character actor, IEnumerable<Character> affectedCharacters, AlterationContainer alteration)
        {
            // Apply alteration cost (ONLY ONE-TIME APPLIED HERE. PER-STEP APPLIED IN CHARACTER ALTERATION)
            if (alteration.Effect.GetCostType(alteration) == AlterationCostType.OneTime)
                _alterationProcessor.ApplyOneTimeAlterationCost(actor, alteration.Cost);

            // Affected Character Alterations:
            //      
            //    Enter loop:  Attempt Block -> 
            //                 Apply Alteration -> 
            //                 Show Messages -> 
            //                 Enemy.IsEngaged = true; -> 
            //                 Queue UI Update
            foreach (var affectedCharacter in affectedCharacters)
            {
                // Character attempts block
                bool blocked = affectedCharacter != actor &&
                               alteration.Effect.GetSupportsBlocking(alteration) &&
                              _interactionProcessor.CalculateAlterationBlock(actor, affectedCharacter, alteration.BlockType);

                // Blocked -> Message and continue
                if (blocked)
                {
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, _modelService.GetDisplayName(affectedCharacter) + " blocked the attack!");
                    continue;
                }

                // Apply Alteration
                //
                // NOTE*** Thinking that it would be easier to ALWAYS indicate an affected character. This 
                //         would be to simplify thinking about the processing. In the least, there should always
                //         be the actor (player) as affected. (Example: Change Level)
                //
                //         This should simplify thinking about both processing the effect and animation.
                ApplyAlteration(alteration, affectedCharacter, actor);

                // Alteration engages enemies
                if (affectedCharacter is Enemy)
                {
                    // Flags for engagement and provocation
                    (affectedCharacter as Enemy).IsEngaged = true;
                    (affectedCharacter as Enemy).WasAttackedByPlayer = true;
                }

                // DESIGN ISSUE - THIS CALL UPDATES ALL AFFECTED DATA FOR VISIBILITY, AND AURA ALTERATIONS
                //                BY DOING A CELL CALCULATION.
                //
                //                THE DESIGN NEEDS TO BE PUT IN SEQUENCE SO THAT THESE CAN BE DONE BETWEEN
                //                CHARACTER ACTIONS WHEN THEY'RE NEEDED.
                _modelService.UpdateVisibility();


                // Apply blanket update for player AND affected character to ensure symbol alterations are processed.
                //
                // NOTE*** Player is treated SEPARATELY from level content. This should be refactored on the UI side
                //         to make processing easier. (TODO)
                OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.ContentUpdate, affectedCharacter.Id));

                // TODO: REMOVE THIS!!!  And just use the above affectedCharacter.Id
                //                       THE DESIGN NEEDS TO SUPPORT THIS!

                // Update Player Symbol (REMOVE THIS!)
                if (affectedCharacter is Player)
                {
                    OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerSkillSetRefresh, _modelService.Player.Id));
                    OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerLocation, _modelService.Player.Id));
                }
            }

            // Clear the targeting service (TODO: BUILD BACKEND SEQUENCER!!!)
            //
            // Should go:  Player Action -> All Intermediate Events -> End of Turn (Player) { Does lots of things, Clears Targeting Service }
            //
            //             Then Enemy 1 Reaction -> ...
            //

            // This SHOULD be the last place that's required for the targeting service to get the
            // stored character. 
            _targetingService.Clear();
        }

        #region (private) Alteration Apply Methods
        private void ApplyAlteration(AlterationContainer alteration, Character affectedCharacter, Character actor)
        {
            // Aura => Actor is affected because the aura is collected by the source character to be applied
            //         to the target characters in the CharacterAlteration on turn.
            if (alteration.Effect is AttackAttributeAuraAlterationEffect)
                actor.Alteration.Apply(alteration);

            else if (alteration.Effect is AttackAttributeMeleeAlterationEffect)
                _interactionProcessor.CalculateAttackAttributeHit(alteration.RogueName,
                                                                  affectedCharacter,
                                                                  (alteration.Effect as AttackAttributeMeleeAlterationEffect).AttackAttributes);

            else if (alteration.Effect is AttackAttributePassiveAlterationEffect)
                affectedCharacter.Alteration.Apply(alteration);

            else if (alteration.Effect is AttackAttributeTemporaryAlterationEffect)
                affectedCharacter.Alteration.Apply(alteration);

            // Aura => Actor is affected because the aura is collected by the source character to be applied
            //         to the target characters in the CharacterAlteration on turn.
            else if (alteration.Effect is AuraAlterationEffect)
                actor.Alteration.Apply(alteration);

            else if (alteration.Effect is ChangeLevelAlterationEffect)
                ProcessChangeLevel(alteration.Effect as ChangeLevelAlterationEffect);

            else if (alteration.Effect is CreateMonsterAlterationEffect)
                ProcessCreateMonster(alteration.Effect as CreateMonsterAlterationEffect, actor);

            else if (alteration.Effect is DrainMeleeAlterationEffect)
                _alterationProcessor.ApplyDrainMeleeEffect(actor, affectedCharacter, alteration.Effect as DrainMeleeAlterationEffect);

            else if (alteration.Effect is EquipmentDamageAlterationEffect)
                ProcessEquipmentDamage(alteration.Effect as EquipmentDamageAlterationEffect, actor, affectedCharacter);

            else if (alteration.Effect is EquipmentEnhanceAlterationEffect)
                ProcessEquipmentEnhance(alteration.Effect as EquipmentEnhanceAlterationEffect, affectedCharacter);

            else if (alteration.Effect is OtherAlterationEffect)
            {
                switch ((alteration.Effect as OtherAlterationEffect).Type)
                {
                    case AlterationOtherEffectType.Identify:
                        OnDialogEvent(_backendEventDataFactory.Dialog(DialogEventType.Identify));
                        break;
                    case AlterationOtherEffectType.Uncurse:
                        OnDialogEvent(_backendEventDataFactory.Dialog(DialogEventType.Uncurse));
                        break;
                    default:
                        break;
                }
            }

            else if (alteration.Effect is PassiveAlterationEffect)
                affectedCharacter.Alteration.Apply(alteration);

            else if (alteration.Effect is PermanentAlterationEffect)
                _alterationProcessor.ApplyPermanentEffect(affectedCharacter, alteration.Effect as PermanentAlterationEffect);

            else if (alteration.Effect is RemedyAlterationEffect)
                affectedCharacter.Alteration.Apply(alteration);

            else if (alteration.Effect is RevealAlterationEffect)
                ProcessReveal(alteration.Effect as RevealAlterationEffect);

            else if (alteration.Effect is RunAwayAlterationEffect)
                ProcessRunAway(affectedCharacter);

            else if (alteration.Effect is StealAlterationEffect)
                ProcessSteal(actor, affectedCharacter);

            else if (alteration.Effect is TeleportAlterationEffect)
                ProcessTeleport(alteration.Effect as TeleportAlterationEffect, actor);

            else if (alteration.Effect is TemporaryAlterationEffect)
                affectedCharacter.Alteration.Apply(alteration);

            else if (alteration.Effect is TransmuteAlterationEffect)
                OnDialogEvent(_backendEventDataFactory.DialogAlterationEffect(alteration.Effect));

            else
                throw new Exception("Unhandled Alteration Effect Type IAlterationEngine.ApplyAlteration");
        }
        #endregion

        #region (private) Alteration Calculation Methods
        public IEnumerable<Character> CalculateAffectedCharacters(AlterationContainer alteration, Character actor)
        {
            if (alteration is ConsumableAlteration)
                return CalculateAffectedCharacters(alteration as ConsumableAlteration, actor);

            else if (alteration is ConsumableProjectileAlteration)
                return CalculateAffectedCharacters(AlterationTargetType.Target, actor);

            else if (alteration is DoodadAlteration)
                return CalculateAffectedCharacters(alteration as DoodadAlteration, actor);

            else if (alteration is EnemyAlteration)
                return CalculateAffectedCharacters(alteration as EnemyAlteration, actor);

            // EQUIPMENT ATTACK ALTERATION APPLIED DIRECTLY TO ATTACKED ENEMY
            else if (alteration is EquipmentAttackAlteration)
                throw new Exception("Equipment Attack Alteration Effect trying to calculate affected characters");

            else if (alteration is EquipmentCurseAlteration)
                return CalculateAffectedCharacters(alteration as EquipmentCurseAlteration, actor);

            else if (alteration is EquipmentEquipAlteration)
                return CalculateAffectedCharacters(alteration as EquipmentEquipAlteration, actor);

            else if (alteration is SkillAlteration)
                return CalculateAffectedCharacters(alteration as SkillAlteration, actor);

            else
                throw new Exception("Unknown AlterationBase type");
        }
        private IEnumerable<Character> CalculateAffectedCharacters(ConsumableAlteration alteration, Character actor)
        {
            return CalculateAffectedCharacters(alteration.AnimationGroup.TargetType, actor);
        }
        private IEnumerable<Character> CalculateAffectedCharacters(DoodadAlteration alteration, Character actor)
        {
            return CalculateAffectedCharacters(alteration.AnimationGroup.TargetType, actor);
        }
        private IEnumerable<Character> CalculateAffectedCharacters(EnemyAlteration alteration, Character actor)
        {
            return CalculateAffectedCharacters(alteration.AnimationGroup.TargetType, actor);
        }
        private IEnumerable<Character> CalculateAffectedCharacters(EquipmentCurseAlteration alteration, Character actor)
        {
            return CalculateAffectedCharacters(AlterationTargetType.Source, actor);
        }
        private IEnumerable<Character> CalculateAffectedCharacters(EquipmentEquipAlteration alteration, Character actor)
        {
            return CalculateAffectedCharacters(AlterationTargetType.Source, actor);
        }
        private IEnumerable<Character> CalculateAffectedCharacters(SkillAlteration alteration, Character actor)
        {
            return CalculateAffectedCharacters(alteration.AnimationGroup.TargetType, actor);
        }
        private IEnumerable<Character> CalculateAffectedCharacters(AlterationTargetType targetType, Character actor)
        {
            var targetedCharacter = _targetingService.GetTargetedCharacter();

            switch (targetType)
            {
                case AlterationTargetType.Source:
                    return new List<Character>() { actor };
                case AlterationTargetType.Target:
                    return (actor is Player) ? new Character[] { targetedCharacter } : new Character[] { _modelService.Player };
                case AlterationTargetType.AllInRange:
                    return CalculateCharactersInRange(actor.Location, (int)actor.GetLightRadius());
                case AlterationTargetType.AllInRangeExceptSource:
                    return CalculateCharactersInRange(actor.Location, (int)actor.GetLightRadius()).Except(new Character[] { actor });
                default:
                    throw new Exception("Unknown Attack Attribute Target Type");
            }
        }
        private IEnumerable<Character> CalculateCharactersInRange(GridLocation location, int cellRange)
        {
            // TODO:ALTERATION - LINE OF SIGHT! Consider calculating line-of-sight characters for each enemy on end of turn
            //                   and storing them on the character (or creating a new component to store them)

            var result = new List<Character>();
            var locationsInRange = _layoutEngine.GetLocationsInRange(_modelService.Level, location, cellRange);

            // Create a list of characters in range
            foreach (var enemy in _modelService.Level.Enemies)
            {
                if (locationsInRange.Any(x => x.Equals(enemy.Location)))
                    result.Add(enemy);
            }

            // Check the player
            if (locationsInRange.Any(x => x.Equals(_modelService.Player.Location)))
                result.Add(_modelService.Player);

            return result;
        }
        #endregion

        #region (private) Other Methods
        private void ProcessSteal(Character actor, Character actee)
        {
            // Get random item from actee's inventory
            var acteeInventory = actee.Inventory;
            if (!acteeInventory.Any())
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, _modelService.GetDisplayName(actee) + " has nothing to steal");

            else
            {
                var itemStolen = acteeInventory.ElementAt(_randomSequenceGenerator.Get(0, acteeInventory.Count()));
                if (itemStolen.Value is Equipment)
                {
                    var equipment = itemStolen.Value as Equipment;

                    // Add item to actor's inventory
                    actor.Equipment.Add(equipment.Id, equipment);

                    // Mark equipment not-equiped (SHOULD NEVER BE EQUIPPED)
                    equipment.IsEquipped = false;

                    // Remove Equipment from actee's inventory and deactivate passive effects
                    actee.Equipment.Remove(equipment.Id);

                    // Be sure to de-activate alterations
                    if (equipment.HasEquipAlteration)
                        actee.Alteration.Remove(equipment.EquipAlteration.Guid);

                    if (equipment.HasCurseAlteration)
                        actee.Alteration.Remove(equipment.CurseAlteration.Guid);

                    // Update UI
                    if (actor is Player)
                        OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerEquipmentAddOrUpdate, itemStolen.Key));

                    else
                        OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerEquipmentRemove, itemStolen.Key));
                }
                // For consumables - nothing to do except to remove / add
                else
                {
                    // Remove Consumable from actee's inventory
                    actee.Consumables.Remove(itemStolen.Key);

                    // Add Consumable to actor's inventory
                    actor.Consumables.Add(itemStolen.Key, itemStolen.Value as Consumable);

                    // Update UI
                    if (actor is Player)
                        OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerConsumableAddOrUpdate, itemStolen.Key));

                    else
                        OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerConsumableRemove, itemStolen.Key));
                }
                _scenarioMessageService.Publish(
                    ScenarioMessagePriority.Normal, 
                    (actor is Player) ? "{0} stole a(n) {1} from the {2}!" : "The {0} stole a(n) {1} from {2}!",
                    _modelService.GetDisplayName(actor),
                    _modelService.GetDisplayName(itemStolen.Value),
                    _modelService.GetDisplayName(actee));
            }
        }
        private void ProcessRunAway(Character actor)
        {
            if (actor is Enemy)
            {
                // Remove Content
                _modelService.Level.RemoveContent(actor);

                // Publish Message
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, 
                                                "The {0} has run away!",
                                                _modelService.GetDisplayName(actor));

                // Update UI
                OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.ContentRemove, actor.Id));
            }
            else
                throw new Exception("Player trying to invoke RunAway Alteration Effect");
        }
        private void ProcessReveal(RevealAlterationEffect effect)
        {
            if (effect.Type.HasFlag(AlterationRevealType.Food))
                RevealFood();

            if (effect.Type.HasFlag(AlterationRevealType.Items))
                RevealItems();

            if (effect.Type.HasFlag(AlterationRevealType.Layout))
                RevealLayout();

            if (effect.Type.HasFlag(AlterationRevealType.Monsters))
                RevealMonsters();

            if (effect.Type.HasFlag(AlterationRevealType.SavePoint))
                RevealSavePoint();

            if (effect.Type.HasFlag(AlterationRevealType.ScenarioObjects))
                RevealScenarioObjects();

            if (effect.Type.HasFlag(AlterationRevealType.Stairs))
                RevealStairs();

            // Update the UI (TODO:ALTERATION - figure out more systematic updating)
            OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.ContentReveal, ""));
        }
        private void ProcessTeleport(TeleportAlterationEffect effect, Character character)
        {
            GridLocation openLocation = GridLocation.Empty;

            // Select location based on effect parameters
            if (effect.LocationSelectionType == AlterationLocationSelectionType.ManualInVisibleRange)
            {
                openLocation = _targetingService.GetTargetLocation();
            }
            else
            {
                // Calculate Teleport Location
                openLocation = GetRandomLocation(effect.TeleportType, character.Location, effect.Range);
            }

            // TODO:  Centralize handling of "Find a random cell" and deal with "no open locations"
            if (openLocation == null ||
                openLocation == GridLocation.Empty)
                return;

            character.Location = openLocation;

            // Publish Message
            if (character is Player)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "You were teleported!");

            else
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, _modelService.GetDisplayName(character) + " was teleported!");

                // Set Enemy Engaged
                (character as Enemy).IsEngaged = true;
            }
        }
        private void ProcessChangeLevel(ChangeLevelAlterationEffect effect)
        {
            // Total Number of Levels
            var numberOfLevels = _modelService.ScenarioConfiguration.DungeonTemplate.NumberOfLevels;

            // Level Desired by Alteration
            var desiredLevel = (_modelService.Level.Number + effect.LevelChange);

            // Actual Level clipped by the min / max
            var actualLevel = desiredLevel.Clip(1, numberOfLevels);

            OnScenarioEvent(_backendEventDataFactory.LevelChange(actualLevel, PlayerStartLocation.Random));
        }
        private void ProcessCreateMonster(CreateMonsterAlterationEffect effect, Character actor)
        {
            var location = GetRandomLocation(effect.RandomPlacementType, actor.Location, effect.Range);

            // TODO:ALTERATION (Handle Exception ?)
            if (location == GridLocation.Empty)
                return;

            // Get the enemy template
            var enemyTemplate = _modelService.ScenarioConfiguration
                                             .EnemyTemplates
                                             .FirstOrDefault(x => x.Name == effect.CreateMonsterEnemy);

            if (enemyTemplate != null)
            {
                // Create Enemy
                var enemy = _characterGenerator.GenerateEnemy(enemyTemplate, _modelService.AttackAttributes);

                // Set Enemy Location
                enemy.Location = location;

                // Add Content to Level -> Update Visibility
                _modelService.Level.AddContent(enemy);
                _modelService.UpdateVisibility();

                // Publish Message
                switch (effect.RandomPlacementType)
                {
                    case AlterationRandomPlacementType.InLevel:
                        _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "You hear growling in the distance");
                        break;
                    case AlterationRandomPlacementType.InRangeOfCharacter:
                    case AlterationRandomPlacementType.InPlayerVisibleRange:
                        _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, 
                                                        (actor is Player) ? "{0} has created a(n) {1}" :
                                                                            "The {0} has created a(n) {1}",
                                                         _modelService.GetDisplayName(actor),
                                                         _modelService.GetDisplayName(enemy));
                        break;
                    default:
                        throw new Exception("Unhandled AlterationRandomPlacementType");
                }

                // Notify UI
                OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.ContentAdd, enemy.Id));
            }
        }
        private void ProcessEquipmentEnhance(EquipmentEnhanceAlterationEffect effect, Character affectedCharacter)
        {
            // Enhance Equipment:
            //
            //  Player: Bring up the Quality / Class / Imbue Dialog
            //  Enemy:  (NOT SUPPORTED)

            // First, Check for Player Invoke
            if (affectedCharacter is Enemy)
                throw new NotSupportedException("Equipment Enhance Alteration Effect not supported for Enemies");

            // Use the dialog to select an item
            if (effect.UseDialog)
                OnDialogEvent(_backendEventDataFactory.DialogAlterationEffect(effect));

            // Select a random equipped item
            else
            {
                var isWeapon = effect.Type == AlterationModifyEquipmentType.WeaponClass ||
                               effect.Type == AlterationModifyEquipmentType.WeaponImbue ||
                               effect.Type == AlterationModifyEquipmentType.WeaponQuality;
                
                var randomEquippedItem = isWeapon ? affectedCharacter.Equipment
                                                         .Values
                                                         .Where(x => x.IsEquipped &&
                                                                     x.IsWeaponType())
                                                         .PickRandom()
                                                  : affectedCharacter.Equipment
                                                                     .Values
                                                                     .Where(x => x.IsEquipped &&
                                                                                 x.IsArmorType())
                                                                     .PickRandom();

                if (randomEquippedItem != null)
                {
                    // Apply Effect -> Publish Messages
                    _alterationProcessor.ApplyEquipmentEnhanceEffect(affectedCharacter as Player, effect, randomEquippedItem);

                    // Queue update
                    OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerEquipmentAddOrUpdate, randomEquippedItem.Id));
                }
                else
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "No Equipped Item to Enhance");
            }
        }
        private void ProcessEquipmentDamage(EquipmentDamageAlterationEffect effect, Character actor, Character affectedCharacter)
        {
            // Damage Equipment: Choose an equipped item at random and apply damage effect
            //

            // Select a random equipped item
            var isWeapon = effect.Type == AlterationModifyEquipmentType.WeaponClass ||
                           effect.Type == AlterationModifyEquipmentType.WeaponImbue ||
                           effect.Type == AlterationModifyEquipmentType.WeaponQuality;

            var randomEquippedItem = isWeapon ? affectedCharacter.Equipment
                                                     .Values
                                                     .Where(x => x.IsEquipped &&
                                                                 x.IsWeaponType())
                                                     .PickRandom()
                                              : affectedCharacter.Equipment
                                                                 .Values
                                                                 .Where(x => x.IsEquipped &&
                                                                             x.IsArmorType())
                                                                 .PickRandom();

            if (randomEquippedItem != null)
            {
                // Apply Effect -> Publish Messages
                _alterationProcessor.ApplyEquipmentDamageEffect(affectedCharacter, effect, randomEquippedItem);

                // Queue update if Player is affected
                if (affectedCharacter is Player)
                    OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerEquipmentAddOrUpdate, randomEquippedItem.Id));
            }
            else
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "No Equipped Item to Damage");
        }
        public void ProcessTransmute(TransmuteAlterationEffect effect, IEnumerable<string> chosenItemIds)
        {
            // Transmute is for Player only
            var player = _modelService.Player;

            // Chosen items from player inventory
            var chosenItems = player.Inventory.Values.Where(x => chosenItemIds.Contains(x.Id));

            // Requirements met for Equipment
            var requirementsMetEquipment = effect.TransmuteItems
                                                 .Where(x => x.EquipmentRequirements
                                                              .All(e => chosenItems.Any(z => z.RogueName == e.RogueName)) ||
                                                            !x.EquipmentRequirements.Any())
                                                 .Actualize();

            // Requirements met for Consumables
            var requirementsMetConsumables = effect.TransmuteItems
                                                   .Where(x => x.ConsumableRequirements
                                                                .All(e => chosenItems.Any(z => z.RogueName == e.RogueName)) ||
                                                              !x.ConsumableRequirements.Any())
                                                   .Actualize();

            // Possible Product Items
            var possibleProductItems = requirementsMetConsumables.Intersect(requirementsMetEquipment).Actualize();

            // Nothing Happens
            if (possibleProductItems.None())
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Nothing Happens...");
                return;
            }

            // Draw weighted random item from the possible products
            var productItem = _randomSequenceGenerator.GetWeightedRandom(possibleProductItems, x => x.Weighting);

            // Remove required items from the actor's inventory
            foreach(var item in chosenItems)
            {
                if (player.Consumables.ContainsKey(item.Id))
                    player.Consumables.Remove(item.Id);

                else if (player.Equipment.ContainsKey(item.Id))
                    player.Equipment.Remove(item.Id);
            }

            // Add product item to the actor's inventory
            if (productItem.IsConsumableProduct)
                player.Consumables.Add(productItem.ConsumableProduct.Id, productItem.ConsumableProduct);

            else if (productItem.IsEquipmentProduct)
                player.Equipment.Add(productItem.EquipmentProduct.Id, productItem.EquipmentProduct);

            var itemBase = productItem.IsConsumableProduct ? (ItemBase)productItem.ConsumableProduct 
                                                           : (ItemBase)productItem.EquipmentProduct;

            _scenarioMessageService.Publish(
                ScenarioMessagePriority.Good,
                "{0} has produced a(n) {1}",
                _modelService.Player.RogueName,
                _modelService.GetDisplayName(itemBase));

            OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerAll, ""));
        }

        private GridLocation GetRandomLocation(AlterationRandomPlacementType placementType, GridLocation sourceLocation, int sourceRange)
        {
            var level = _modelService.Level;
            var player = _modelService.Player;

            GridLocation openLocation = GridLocation.Empty;

            switch (placementType)
            {
                case AlterationRandomPlacementType.InLevel:
                    openLocation = _modelService.Level.GetRandomLocation(true, _randomSequenceGenerator);
                    break;
                case AlterationRandomPlacementType.InRangeOfCharacter:
                    openLocation = _layoutEngine.GetLocationsInRange(level, sourceLocation, sourceRange)
                                                .Where(x => !level.IsCellOccupied(x, player.Location))
                                                .PickRandom();
                    break;
                case AlterationRandomPlacementType.InPlayerVisibleRange:
                    openLocation = _modelService.CharacterLayoutInformation
                                                .GetVisibleLocations(player)
                                                .Where(x => !level.IsCellOccupied(x, player.Location))
                                                .PickRandom();
                    break;
                default:
                    throw new Exception("Unhandled AlterationRandomPlacementType");
            }

            return openLocation ?? (openLocation == GridLocation.Empty ? sourceLocation : openLocation);
        }
        #endregion

        #region (private) Reveal Methods - DOES NOT UPDATE UI. BACKEND MODEL PROCESSING ONLY
        private void RevealSavePoint()
        {
            if (_modelService.Level.HasSavePoint)
                _modelService.Level.SavePoint.IsRevealed = true;

            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "You sense odd shrines near by...");
        }
        private void RevealMonsters()
        {
            foreach (var enemy in _modelService.Level.Enemies)
                enemy.IsRevealed = true;

            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "You hear growling in the distance...");
        }
        private void RevealLayout()
        {
            foreach (var cell in _modelService.Level.Grid.GetCells())
                cell.IsRevealed = true;

            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Your senses are vastly awakened");
        }
        private void RevealStairs()
        {
            if (_modelService.Level.HasStairsDown)
                _modelService.Level.StairsDown.IsRevealed = true;

            if (_modelService.Level.HasStairsUp)
                _modelService.Level.StairsUp.IsRevealed = true;

            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "You sense exits nearby");
        }
        private void RevealItems()
        {
            foreach (var consumable in _modelService.Level.Consumables)
                consumable.IsRevealed = true;

            foreach (var equipment in _modelService.Level.Equipment)
                equipment.IsRevealed = true;

            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "You sense objects nearby");
        }
        private void RevealFood()
        {
            foreach (var consumable in _modelService.Level.Consumables.Where(x => x.SubType == ConsumableSubType.Food))
                consumable.IsRevealed = true;

            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Hunger makes a good sauce.....  :)");
        }
        private void RevealScenarioObjects()
        {
            foreach (var scenarioObject in _modelService.Level.Doodads)
            {
                // Remove Hidden Status
                scenarioObject.IsHidden = false;

                // Set Revealed
                scenarioObject.IsRevealed = true;
            }

            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "You sense special objects near by...");
        }
        #endregion
    }
}