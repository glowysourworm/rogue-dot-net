using Rogue.NET.Common.Extension;
using Rogue.NET.Core.GameRouter.GameEvent.Backend.Enum;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Common.Extension;
using Rogue.NET.Core.Model.Scenario.Alteration.Consumable;
using Rogue.NET.Core.Model.Scenario.Alteration.Doodad;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Alteration.Enemy;
using Rogue.NET.Core.Model.Scenario.Alteration.Equipment;
using Rogue.NET.Core.Model.Scenario.Alteration.Skill;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Content.Item.Extension;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Action;
using Rogue.NET.Core.Processing.Action.Enum;
using Rogue.NET.Core.Processing.Event.Backend.EventData.Factory.Interface;
using Rogue.NET.Core.Processing.Event.Backend.EventData.ScenarioMessage.Enum;
using Rogue.NET.Core.Processing.Event.Dialog.Enum;
using Rogue.NET.Core.Processing.Model.Content.Calculator.Interface;
using Rogue.NET.Core.Processing.Model.Content.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Service.Interface;

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Content
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IAlterationProcessor))]
    public class AlterationProcessor : BackendProcessor, IAlterationProcessor
    {
        readonly IModelService _modelService;
        readonly ITargetingService _targetingService;
        readonly ICharacterGenerator _characterGenerator;
        readonly IAlterationCalculator _alterationCalculator;
        readonly IInteractionCalculator _interactionCalculator;
        readonly IScenarioMessageService _scenarioMessageService;
        readonly IRandomSequenceGenerator _randomSequenceGenerator;
        readonly IBackendEventDataFactory _backendEventDataFactory;

        [ImportingConstructor]
        public AlterationProcessor(IModelService modelService,
                                ITargetingService targetingService,
                                ICharacterGenerator characterGenerator,
                                IAlterationCalculator alterationCalculator,
                                IInteractionCalculator interactionCalculator,
                                IScenarioMessageService scenarioMessageService,
                                IRandomSequenceGenerator randomSequenceGenerator,
                                IBackendEventDataFactory backendEventDataFactory)
        {
            _modelService = modelService;
            _targetingService = targetingService;
            _characterGenerator = characterGenerator;
            _alterationCalculator = alterationCalculator;
            _interactionCalculator = interactionCalculator;
            _scenarioMessageService = scenarioMessageService;
            _randomSequenceGenerator = randomSequenceGenerator;
            _backendEventDataFactory = backendEventDataFactory;
        }

        public bool Validate(Character actor, AlterationContainer alteration)
        {
            // First, validate the Alteration Cost
            if (!_alterationCalculator.CalculateMeetsAlterationCost(actor, alteration.Cost))
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
                alteration.Animation.Animations.Count > 0)
            {
                // NOTE*** For animations refactored the animation type
                //         to always assume affected characters. This will greatly simplify 
                //         the parameter space and logic around Alteration -> Animation.

                // TODO: REMOVE THIS.  This is an aid in validating animations. This needs
                //                     to be moved to the editor.
                var animationIssueDetected = false;

                if (!animationIssueDetected)
                {
                    OnAnimationEvent(_backendEventDataFactory.Animation(
                                                alteration.Animation,
                                                _modelService.GetContentLocation(actor),
                                                affectedCharacters.Select(x => _modelService.GetContentLocation(x)).Actualize()));
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
                _alterationCalculator.ApplyOneTimeAlterationCost(actor, alteration.Cost);

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
                              _interactionCalculator.CalculateAlterationBlock(actor, affectedCharacter, alteration.BlockType);

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

                // Alteration engages characters
                if (affectedCharacter is NonPlayerCharacter)
                {
                    // Flags for engagement and provocation
                    (affectedCharacter as NonPlayerCharacter).IsAlerted = true;
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
                _interactionCalculator.CalculateAttackAttributeHit(alteration.RogueName,
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

            else if (alteration.Effect is BlockAlterationAlterationEffect)
                actor.Alteration.Apply(alteration);

            else if (alteration.Effect is ChangeLevelAlterationEffect)
                ProcessChangeLevel(alteration.Effect as ChangeLevelAlterationEffect);

            else if (alteration.Effect is CreateEnemyAlterationEffect)
                ProcessCreateEnemy(alteration.Effect as CreateEnemyAlterationEffect, actor);

            else if (alteration.Effect is CreateFriendlyAlterationEffect)
                ProcessCreateFriendly(alteration.Effect as CreateFriendlyAlterationEffect, actor);

            else if (alteration.Effect is CreateTemporaryCharacterAlterationEffect)
                ProcessCreateTemporaryCharacter(alteration.Effect as CreateTemporaryCharacterAlterationEffect, actor);

            else if (alteration.Effect is DetectAlterationAlignmentAlterationEffect)
                ProcessDetectAlterationAlignment(alteration.Effect as DetectAlterationAlignmentAlterationEffect);

            else if (alteration.Effect is DetectAlterationAlterationEffect)
                ProcessDetectAlteration(alteration.Effect as DetectAlterationAlterationEffect);

            else if (alteration.Effect is DrainMeleeAlterationEffect)
                _alterationCalculator.ApplyDrainMeleeEffect(actor, affectedCharacter, alteration.Effect as DrainMeleeAlterationEffect);

            else if (alteration.Effect is EquipmentDamageAlterationEffect)
                ProcessEquipmentDamage(alteration.Effect as EquipmentDamageAlterationEffect, actor, affectedCharacter);

            else if (alteration.Effect is EquipmentEnhanceAlterationEffect)
                ProcessEquipmentEnhance(alteration.Effect as EquipmentEnhanceAlterationEffect, affectedCharacter);

            else if (alteration.Effect is IdentifyAlterationEffect)
                ProcessIdentify(alteration.Effect as IdentifyAlterationEffect);

            else if (alteration.Effect is PassiveAlterationEffect)
                affectedCharacter.Alteration.Apply(alteration);

            else if (alteration.Effect is PermanentAlterationEffect)
                _alterationCalculator.ApplyPermanentEffect(affectedCharacter, alteration.Effect as PermanentAlterationEffect);

            else if (alteration.Effect is RemedyAlterationEffect)
                affectedCharacter.Alteration.Apply(alteration);

            else if (alteration.Effect is RevealAlterationEffect)
                ProcessReveal(alteration.Effect as RevealAlterationEffect);

            else if (alteration.Effect is RunAwayAlterationEffect)
                ProcessRunAway(affectedCharacter);

            else if (alteration.Effect is StealAlterationEffect)
                ProcessSteal(actor, affectedCharacter);

            else if (alteration.Effect is TeleportRandomAlterationEffect)
                ProcessTeleport(alteration.Effect as TeleportRandomAlterationEffect, actor);

            else if (alteration.Effect is TemporaryAlterationEffect)
                affectedCharacter.Alteration.Apply(alteration);

            else if (alteration.Effect is TransmuteAlterationEffect)
                OnDialogEvent(_backendEventDataFactory.DialogAlterationEffect(alteration.Effect));

            else if (alteration.Effect is UncurseAlterationEffect)
                ProcessUncurse(alteration.Effect as UncurseAlterationEffect);

            else
                throw new Exception("Unhandled Alteration Effect Type IAlterationProcessor.ApplyAlteration");
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
            return CalculateAffectedCharacters(alteration.Animation.TargetType, actor);
        }
        private IEnumerable<Character> CalculateAffectedCharacters(DoodadAlteration alteration, Character actor)
        {
            return CalculateAffectedCharacters(alteration.Animation.TargetType, actor);
        }
        private IEnumerable<Character> CalculateAffectedCharacters(EnemyAlteration alteration, Character actor)
        {
            return CalculateAffectedCharacters(alteration.Animation.TargetType, actor);
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
            return CalculateAffectedCharacters(alteration.Animation.TargetType, actor);
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
                    {
                        var visibleLocations = _modelService.CharacterLayoutInformation.GetVisibleLocations(actor);

                        return _modelService.Level.Content.GetManyAt<Character>(visibleLocations);
                    }
                case AlterationTargetType.AllInRangeExceptSource:
                    {
                        var visibleLocations = _modelService.CharacterLayoutInformation.GetVisibleLocations(actor);

                        return _modelService.Level.Content.GetManyAt<Character>(visibleLocations).Except(new Character[] { actor });
                    }
                default:
                    throw new Exception("Unknown Attack Attribute Target Type");
            }
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
                        actee.Alteration.Remove(equipment.EquipAlteration.Name);

                    if (equipment.HasCurseAlteration)
                        actee.Alteration.Remove(equipment.CurseAlteration.Name);

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
                _modelService.Level.RemoveContent(actor.Id);

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
            if (effect.Type.Has(AlterationRevealType.Food))
                RevealFood();

            if (effect.Type.Has(AlterationRevealType.Items))
                RevealItems();

            if (effect.Type.Has(AlterationRevealType.Layout))
                RevealLayout();

            if (effect.Type.Has(AlterationRevealType.Monsters))
                RevealMonsters();

            if (effect.Type.Has(AlterationRevealType.SavePoint))
                RevealSavePoint();

            if (effect.Type.Has(AlterationRevealType.ScenarioObjects))
                RevealScenarioObjects();

            if (effect.Type.Has(AlterationRevealType.Stairs))
                RevealStairs();

            // Update the UI (TODO:ALTERATION - figure out more systematic updating)
            OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.ContentReveal, ""));
        }
        private void ProcessTeleport(TeleportRandomAlterationEffect effect, Character character)
        {
            var characterLocation = _modelService.GetContentLocation(character);

            // Calculate Teleport Location
            var openLocation = GetRandomLocation(effect.TeleportType, characterLocation, effect.Range);

            // TODO:  Centralize handling of "Find a random cell" and deal with "no open locations"
            if (openLocation == null)
                return;

            // Process content move
            _modelService.Level.MoveContent(character, openLocation);

            // Publish Message
            if (character is Player)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "You were teleported!");

            else
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, _modelService.GetDisplayName(character) + " was teleported!");
            }
        }
        private void ProcessChangeLevel(ChangeLevelAlterationEffect effect)
        {
            // Total Number of Levels
            var numberOfLevels = _modelService.GetNumberOfLevels();

            // Level Desired by Alteration
            var desiredLevel = (_modelService.Level.Parameters.Number + effect.LevelChange);

            // Actual Level clipped by the min / max
            var actualLevel = desiredLevel.Clip(1, numberOfLevels);

            OnScenarioEvent(_backendEventDataFactory.LevelChange(actualLevel, PlayerStartLocation.Random));
        }
        private void ProcessCreateEnemy(CreateEnemyAlterationEffect effect, Character actor)
        {
            // Create Enemy
            var enemy = _characterGenerator.GenerateEnemy(effect.Enemy, _modelService.ScenarioEncyclopedia);

            ProcessCreateNonPlayerCharacter(enemy, effect.RandomPlacementType, effect.Range, actor);
        }
        private void ProcessCreateFriendly(CreateFriendlyAlterationEffect effect, Character actor)
        {
            // Create Friendly
            var friendly = _characterGenerator.GenerateFriendly(effect.Friendly, _modelService.ScenarioEncyclopedia);

            ProcessCreateNonPlayerCharacter(friendly, effect.RandomPlacementType, effect.Range, actor);
        }
        private void ProcessCreateTemporaryCharacter(CreateTemporaryCharacterAlterationEffect effect, Character actor)
        {
            // Create Temporary Character
            var temporaryCharacter = _characterGenerator.GenerateTemporaryCharacter(effect.TemporaryCharacter, _modelService.ScenarioEncyclopedia);

            ProcessCreateNonPlayerCharacter(temporaryCharacter, effect.RandomPlacementType, effect.Range, actor);
        }
        private void ProcessCreateNonPlayerCharacter(NonPlayerCharacter character, AlterationRandomPlacementType randomPlacementType, int range, Character actor)
        {
            // Method that shares code paths for creating { Enemy, Friendly, TemporaryCharacter }
            //

            var actorLocation = _modelService.GetContentLocation(actor);
            var location = GetRandomLocation(randomPlacementType, actorLocation, range);

            // TODO:ALTERATION (Handle Exception ?)
            if (location == null)
                return;

            // Add Content to Level -> Update Visibility
            _modelService.Level.AddContent(character, location);
            _modelService.UpdateVisibility();

            // Publish Message
            switch (randomPlacementType)
            {
                case AlterationRandomPlacementType.InLevel:
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "You hear growling in the distance");
                    break;
                case AlterationRandomPlacementType.InRangeOfSourceCharacter:
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal,
                                                    (actor is Player) ? "{0} has created a(n) {1}" :
                                                                        "The {0} has created a(n) {1}",
                                                     _modelService.GetDisplayName(actor),
                                                     _modelService.GetDisplayName(character));
                    break;
                default:
                    throw new Exception("Unhandled AlterationRandomPlacementType");
            }

            // Notify UI
            OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.ContentAdd, character.Id));
        }
        private void ProcessDetectAlteration(DetectAlterationAlterationEffect effect)
        {
            // Consumables on the map
            var consumables = _modelService.Level.Content.Consumables
                                           .Where(x => (x.HasAlteration &&
                                                        x.Alteration.AlterationCategory.Name == effect.AlterationCategory.RogueName) ||
                                                       (x.HasProjectileAlteration &&
                                                        x.ProjectileAlteration.AlterationCategory.Name == effect.AlterationCategory.RogueName))
                                           .Actualize();

            // Equipment on the map
            var equipment = _modelService.Level.Content.Equipment
                                         .Where(x => (x.HasAttackAlteration &&
                                                      x.AttackAlteration.AlterationCategory.Name == effect.AlterationCategory.RogueName) ||
                                                     (x.HasEquipAlteration &&
                                                      x.EquipAlteration.AlterationCategory.Name == effect.AlterationCategory.RogueName) ||
                                                     (x.HasCurseAlteration &&
                                                      x.CurseAlteration.AlterationCategory.Name == effect.AlterationCategory.RogueName))
                                         .Actualize();

            // Doodads on the map
            var doodads = _modelService.Level.Content.Doodads
                                             .Where(x => (x.IsInvoked &&
                                                          x.InvokedAlteration.AlterationCategory.Name == effect.AlterationCategory.RogueName) ||
                                                         (x.IsAutomatic &&
                                                          x.AutomaticAlteration.AlterationCategory.Name == effect.AlterationCategory.RogueName))
                                             .Actualize();

            // Non-Player Characters (Enemies, Friendlies, and Temp. Characters)
            var nonPlayerCharacters = _modelService.Level.Content.NonPlayerCharacters
                                       .Where(x => x.BehaviorDetails
                                                    .Behaviors
                                                    .Where(behavior => behavior.AttackType == CharacterAttackType.Alteration)
                                                    .Any(behavior => behavior.Alteration.AlterationCategory.Name == effect.AlterationCategory.RogueName) ||

                                                   x.Consumables.Any(consumable => consumable.Value.HasAlteration &&
                                                                                   consumable.Value.Alteration.AlterationCategory.Name == effect.AlterationCategory.RogueName) ||

                                                   x.Consumables.Any(consumable => consumable.Value.HasProjectileAlteration &&
                                                                                   consumable.Value.ProjectileAlteration.AlterationCategory.Name == effect.AlterationCategory.RogueName) ||

                                                   x.Equipment.Any(item => item.Value.HasAttackAlteration &&
                                                                           item.Value.AttackAlteration.AlterationCategory.Name == effect.AlterationCategory.RogueName) ||

                                                   x.Equipment.Any(item => item.Value.HasEquipAlteration &&
                                                                           item.Value.EquipAlteration.AlterationCategory.Name == effect.AlterationCategory.RogueName) ||

                                                   x.Equipment.Any(item => item.Value.HasCurseAlteration &&
                                                                           item.Value.CurseAlteration.AlterationCategory.Name == effect.AlterationCategory.RogueName))
                                       .Actualize();

            var playerConsumables = _modelService.Player
                                           .Consumables
                                           .Values
                                           .Where(consumable =>
                                           {
                                               return (consumable.HasAlteration &&
                                                       consumable.Alteration.AlterationCategory.Name == effect.AlterationCategory.RogueName) ||

                                                      (consumable.HasProjectileAlteration &&
                                                       consumable.ProjectileAlteration.AlterationCategory.Name == effect.AlterationCategory.RogueName);
                                           })
                                           .Actualize();

            var playerEquipment = _modelService.Player
                                           .Equipment
                                           .Values
                                           .Where(item =>
                                           {
                                               return (item.HasAttackAlteration &&
                                                       item.AttackAlteration.AlterationCategory.Name == effect.AlterationCategory.RogueName) ||

                                                       (item.HasEquipAlteration &&
                                                        item.EquipAlteration.AlterationCategory.Name == effect.AlterationCategory.RogueName) ||

                                                       (item.HasCurseAlteration &&
                                                        item.CurseAlteration.AlterationCategory.Name == effect.AlterationCategory.RogueName);
                                           })
                                           .Actualize();

            // Set flags for the front end
            consumables.Cast<ScenarioObject>()
                       .Union(equipment)
                       .Union(doodads)
                       .Union(nonPlayerCharacters)
                       .Union(playerConsumables)
                       .Union(playerEquipment)
                       .ForEach(scenarioObject =>
                       {
                           scenarioObject.IsDetectedCategory = true;
                           scenarioObject.DetectedAlignmentCategory = effect.AlterationCategory.DeepClone();
                       });

            OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerAll, ""));
            OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.ContentReveal, ""));
        }
        private void ProcessDetectAlterationAlignment(DetectAlterationAlignmentAlterationEffect effect)
        {
            // Consumables on the map
            _modelService.Level
                         .Content
                         .Consumables
                         .ForEach(x =>
                         {
                             if (x.HasAlteration &&
                                 x.Alteration.AlterationCategory.AlignmentType.Has(effect.AlignmentType))
                             {
                                 x.IsDetectedAlignment = true;
                                 x.DetectedAlignmentType = x.Alteration.AlterationCategory.AlignmentType;
                             }

                             if (x.HasProjectileAlteration &&
                                 x.ProjectileAlteration.AlterationCategory.AlignmentType.Has(effect.AlignmentType))
                             {
                                 x.IsDetectedAlignment = true;
                                 x.DetectedAlignmentType |= x.ProjectileAlteration.AlterationCategory.AlignmentType;
                             }
                         });

            // Equipment on the map
            _modelService.Level
                         .Content
                         .Equipment
                         .ForEach(x =>
                         {
                             if (x.HasAttackAlteration &&
                                 x.AttackAlteration.AlterationCategory.AlignmentType.Has(effect.AlignmentType))
                             {
                                 x.IsDetectedAlignment = true;
                                 x.DetectedAlignmentType = x.AttackAlteration.AlterationCategory.AlignmentType;
                             }
                             if (x.HasEquipAlteration &&
                                 x.EquipAlteration.AlterationCategory.AlignmentType.Has(effect.AlignmentType))
                             {
                                 x.IsDetectedAlignment = true;
                                 x.DetectedAlignmentType |= x.EquipAlteration.AlterationCategory.AlignmentType;
                             }
                             if (x.HasCurseAlteration &&
                                 x.CurseAlteration.AlterationCategory.AlignmentType.Has(effect.AlignmentType))
                             {
                                 x.IsDetectedAlignment = true;
                                 x.DetectedAlignmentType |= x.CurseAlteration.AlterationCategory.AlignmentType;
                             }
                             if (x.IsCursed && effect.IncludeCursedEquipment)
                             {
                                 x.IsDetectedAlignment = true;
                                 x.DetectedAlignmentType |= AlterationAlignmentType.Bad;
                             }
                         });

            // Doodads on the map
            _modelService.Level
                         .Content
                         .Doodads
                         .ForEach(x =>
                         {
                             if (x.IsInvoked &&
                                 x.InvokedAlteration.AlterationCategory.AlignmentType.Has(effect.AlignmentType))
                             {
                                 x.IsDetectedAlignment = true;
                                 x.DetectedAlignmentType = x.InvokedAlteration.AlterationCategory.AlignmentType;
                             }
                             if (x.IsAutomatic &&
                                 x.AutomaticAlteration.AlterationCategory.AlignmentType.Has(effect.AlignmentType))
                             {
                                 x.IsDetectedAlignment = true;
                                 x.DetectedAlignmentType |= x.AutomaticAlteration.AlterationCategory.AlignmentType;
                             }
                         });

            // Non-Player Characters (Enemies, Friendlies, and Temp. Characters)
            _modelService.Level
                         .Content
                         .NonPlayerCharacters
                         .ForEach(x =>
                         {
                             var behavior = x.BehaviorDetails
                                  .Behaviors
                                  .Where(z => z.AttackType == CharacterAttackType.Alteration)
                                  .FirstOrDefault(z => z.Alteration.AlterationCategory.AlignmentType.Has(effect.AlignmentType));

                             if (behavior != null)
                             {
                                 x.IsDetectedAlignment = true;
                                 x.DetectedAlignmentType = behavior.Alteration.AlterationCategory.AlignmentType;
                             }

                             var consumable = x.Consumables.FirstOrDefault(z => z.Value.HasAlteration &&
                                                                                z.Value.Alteration.AlterationCategory.AlignmentType.Has(effect.AlignmentType));
                             if (consumable.Value != null)
                             {
                                 x.IsDetectedAlignment = true;
                                 x.DetectedAlignmentType |= consumable.Value.Alteration.AlterationCategory.AlignmentType;
                             }

                             consumable = x.Consumables.FirstOrDefault(z => z.Value.HasProjectileAlteration &&
                                                                            z.Value.ProjectileAlteration.AlterationCategory.AlignmentType.Has(effect.AlignmentType));

                             if (consumable.Value != null)
                             {
                                 x.IsDetectedAlignment = true;
                                 x.DetectedAlignmentType |= consumable.Value.ProjectileAlteration.AlterationCategory.AlignmentType;
                             }

                             var equipment = x.Equipment.FirstOrDefault(item => item.Value.HasAttackAlteration &&
                                                                        item.Value.AttackAlteration.AlterationCategory.AlignmentType.Has(effect.AlignmentType));

                             if (equipment.Value != null)
                             {
                                 x.IsDetectedAlignment = true;
                                 x.DetectedAlignmentType |= equipment.Value.AttackAlteration.AlterationCategory.AlignmentType;
                             }

                             equipment = x.Equipment.FirstOrDefault(item => item.Value.HasEquipAlteration &&
                                                                    item.Value.EquipAlteration.AlterationCategory.AlignmentType.Has(effect.AlignmentType));

                             if (equipment.Value != null)
                             {
                                 x.IsDetectedAlignment = true;
                                 x.DetectedAlignmentType |= equipment.Value.EquipAlteration.AlterationCategory.AlignmentType;
                             }

                             equipment = x.Equipment.FirstOrDefault(item => item.Value.HasCurseAlteration &&
                                                                    item.Value.CurseAlteration.AlterationCategory.AlignmentType.Has(effect.AlignmentType));

                             if (equipment.Value != null)
                             {
                                 x.IsDetectedAlignment = true;
                                 x.DetectedAlignmentType |= equipment.Value.CurseAlteration.AlterationCategory.AlignmentType;
                             }

                             equipment = x.Equipment.FirstOrDefault(item => item.Value.IsCursed);

                             if (equipment.Value != null &&
                                 effect.IncludeCursedEquipment)
                             {
                                 x.IsDetectedAlignment = true;
                                 x.DetectedAlignmentType |= AlterationAlignmentType.Bad;
                             }

                         });

            _modelService.Player
                         .Consumables
                         .Values
                         .ForEach(x =>
                         {
                             if (x.HasAlteration &&
                                 x.Alteration.AlterationCategory.AlignmentType.Has(effect.AlignmentType))
                             {
                                 x.IsDetectedAlignment = true;
                                 x.DetectedAlignmentType = x.Alteration.AlterationCategory.AlignmentType;
                             }

                             if (x.HasProjectileAlteration &&
                                 x.ProjectileAlteration.AlterationCategory.AlignmentType.Has(effect.AlignmentType))
                             {
                                 x.IsDetectedAlignment = true;
                                 x.DetectedAlignmentType |= x.ProjectileAlteration.AlterationCategory.AlignmentType;
                             }
                         });

            _modelService.Player
                         .Equipment
                         .Values
                         .ForEach(x =>
                         {
                             if (x.HasAttackAlteration &&
                                 x.AttackAlteration.AlterationCategory.AlignmentType.Has(effect.AlignmentType))
                             {
                                 x.IsDetectedAlignment = true;
                                 x.DetectedAlignmentType = x.AttackAlteration.AlterationCategory.AlignmentType;
                             }
                             if (x.HasEquipAlteration &&
                                 x.EquipAlteration.AlterationCategory.AlignmentType.Has(effect.AlignmentType))
                             {
                                 x.IsDetectedAlignment = true;
                                 x.DetectedAlignmentType |= x.EquipAlteration.AlterationCategory.AlignmentType;
                             }
                             if (x.HasCurseAlteration &&
                                x.CurseAlteration.AlterationCategory.AlignmentType.Has(effect.AlignmentType))
                             {
                                 x.IsDetectedAlignment = true;
                                 x.DetectedAlignmentType |= x.CurseAlteration.AlterationCategory.AlignmentType;
                             }
                             if (x.IsCursed && effect.IncludeCursedEquipment)
                             {
                                 x.IsDetectedAlignment = true;
                                 x.DetectedAlignmentType |= AlterationAlignmentType.Bad;
                             }
                         });

            OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerAll, ""));
            OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.ContentReveal, ""));
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
                    _alterationCalculator.ApplyEquipmentEnhanceEffect(affectedCharacter as Player, effect, randomEquippedItem);

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
                _alterationCalculator.ApplyEquipmentDamageEffect(affectedCharacter, effect, randomEquippedItem);

                // Queue update if Player is affected
                if (affectedCharacter is Player)
                    OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerEquipmentAddOrUpdate, randomEquippedItem.Id));
            }
            else
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "No Equipped Item to Damage");
        }
        private void ProcessIdentify(IdentifyAlterationEffect effect)
        {
            if (!effect.IdentifyAll)
                OnDialogEvent(_backendEventDataFactory.Dialog(DialogEventType.Identify));

            else
            {
                foreach (var equipment in _modelService.Player.Equipment.Values)
                {
                    equipment.IsIdentified = true;
                    _modelService.ScenarioEncyclopedia[equipment.RogueName].IsIdentified = true;
                    _modelService.ScenarioEncyclopedia[equipment.RogueName].IsCurseIdentified = true;

                    OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerEquipmentAddOrUpdate, equipment.Id));
                    OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.EncyclopediaIdentify, equipment.Id));
                }

                foreach (var consumable in _modelService.Player.Consumables.Values)
                {
                    consumable.IsIdentified = true;
                    _modelService.ScenarioEncyclopedia[consumable.RogueName].IsIdentified = true;
                    _modelService.ScenarioEncyclopedia[consumable.RogueName].IsCurseIdentified = true;  // Not necessary

                    OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerConsumableAddOrUpdate, consumable.Id));
                    OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.EncyclopediaIdentify, consumable.Id));
                }
            }
        }
        private void ProcessUncurse(UncurseAlterationEffect effect)
        {
            if (!effect.UncurseAll)
                OnDialogEvent(_backendEventDataFactory.Dialog(DialogEventType.Uncurse));

            else
            {
                foreach (var equipment in _modelService.Player.Equipment.Values)
                {
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, _modelService.GetDisplayName(equipment) + " Uncursed");

                    if (equipment.HasCurseAlteration)
                        _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Your " + equipment.RogueName + " is now safe to use (with caution...)");

                    if (equipment.IsEquipped)
                        _modelService.Player.Alteration.Remove(equipment.CurseAlteration.Name);

                    equipment.IsCursed = false;
                    equipment.HasCurseAlteration = false;

                    OnLevelEvent(_backendEventDataFactory.Event(LevelEventType.PlayerEquipmentAddOrUpdate, equipment.Id));
                }
            }
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
            foreach (var item in chosenItems)
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

            GridLocation openLocation = null;

            switch (placementType)
            {
                case AlterationRandomPlacementType.InLevel:
                    openLocation = _modelService.LayoutService.GetRandomLocation(true);
                    break;
                case AlterationRandomPlacementType.InRangeOfSourceCharacter:
                    {
                        var locationsInRange = _modelService.LayoutService.GetLocationsInRange(sourceLocation, sourceRange, false);
                        var unOccupiedLocations = locationsInRange.Where(x => !level.IsCellOccupied(x));

                        openLocation = unOccupiedLocations.Any() ? unOccupiedLocations.PickRandom() : null;
                    }
                    break;
                default:
                    throw new Exception("Unhandled AlterationRandomPlacementType");
            }

            return openLocation;
        }
        #endregion

        #region (private) Reveal Methods - DOES NOT UPDATE UI. BACKEND MODEL PROCESSING ONLY
        private void RevealSavePoint()
        {
            if (_modelService.Level.HasSavePoint())
                _modelService.Level.GetSavePoint().IsRevealed = true;

            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "You sense odd shrines near by...");
        }
        private void RevealMonsters()
        {
            foreach (var character in _modelService.Level.Content.NonPlayerCharacters)
                character.IsRevealed = true;

            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "You hear growling in the distance...");
        }
        private void RevealLayout()
        {
            foreach (var location in _modelService.Level.Grid.FullMap.GetLocations())
                _modelService.Level.Grid[location.Column, location.Row].IsRevealed = true;

            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Your senses are vastly awakened");
        }
        private void RevealStairs()
        {
            if (_modelService.Level.HasStairsDown())
                _modelService.Level.GetStairsDown().IsRevealed = true;

            if (_modelService.Level.HasStairsUp())
                _modelService.Level.GetStairsUp().IsRevealed = true;

            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "You sense exits nearby");
        }
        private void RevealItems()
        {
            foreach (var consumable in _modelService.Level.Content.Consumables)
                consumable.IsRevealed = true;

            foreach (var equipment in _modelService.Level.Content.Equipment)
                equipment.IsRevealed = true;

            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "You sense objects nearby");
        }
        private void RevealFood()
        {
            foreach (var consumable in _modelService.Level.Content.Consumables.Where(x => x.SubType == ConsumableSubType.Food))
                consumable.IsRevealed = true;

            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Hunger makes a good sauce.....  :)");
        }
        private void RevealScenarioObjects()
        {
            foreach (var scenarioObject in _modelService.Level.Content.Doodads)
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
