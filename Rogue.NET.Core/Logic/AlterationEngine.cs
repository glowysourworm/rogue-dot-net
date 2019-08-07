using Rogue.NET.Core.Logic.Content.Interface;
using Rogue.NET.Core.Logic.Interface;
using Rogue.NET.Core.Logic.Processing;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Consumable;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Service.Interface;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using Rogue.NET.Core.Model.Scenario.Alteration.Doodad;
using Rogue.NET.Core.Model.Scenario.Alteration.Enemy;
using Rogue.NET.Core.Model.Scenario.Alteration.Equipment;
using Rogue.NET.Core.Model.Scenario.Alteration.Skill;
using Rogue.NET.Core.Logic.Processing.Factory.Interface;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.ScenarioMessage;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Content.Extension;

namespace Rogue.NET.Core.Logic
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IAlterationEngine))]
    public class AlterationEngine : IAlterationEngine
    {
        readonly IModelService _modelService;
        readonly ILayoutEngine _layoutEngine;
        readonly ICharacterGenerator _characterGenerator;
        readonly IAlterationGenerator _alterationGenerator;
        readonly IAlterationProcessor _alterationProcessor;
        readonly IInteractionProcessor _interactionProcessor;        
        readonly IScenarioMessageService _scenarioMessageService;
        readonly IRandomSequenceGenerator _randomSequenceGenerator;
        readonly IRogueUpdateFactory _rogueUpdateFactory;

        public event EventHandler<RogueUpdateEventArgs> RogueUpdateEvent;
        public event EventHandler<ILevelProcessingAction> LevelProcessingActionEvent;

        public AlterationEngine(IModelService modelService,
                                ILayoutEngine layoutEngine,
                                ICharacterGenerator characterGenerator,
                                IAlterationGenerator alterationGenerator,
                                IAlterationProcessor alterationProcessor,
                                IInteractionProcessor interactionProcessor,
                                IScenarioMessageService scenarioMessageService,
                                IRandomSequenceGenerator randomSequenceGenerator,
                                IRogueUpdateFactory rogueUpdateFactory)
        {
            _modelService = modelService;
            _layoutEngine = layoutEngine;
            _characterGenerator = characterGenerator;
            _alterationGenerator = alterationGenerator;
            _alterationProcessor = alterationProcessor;
            _interactionProcessor = interactionProcessor;
            _scenarioMessageService = scenarioMessageService;
            _randomSequenceGenerator = randomSequenceGenerator;
            _rogueUpdateFactory = rogueUpdateFactory;
        }

        public bool Validate(Character actor, AlterationBase alteration)
        {
            return _alterationProcessor.CalculateMeetsAlterationCost(actor, alteration.Cost);
        }

        public void Queue(Character actor, AlterationBase alteration)
        {
            // Affected characters
            var affectedCharacters = CalculateAffectedCharacters(alteration, actor);

            // NOTE*** For animations that require a target - allow the source / target
            //         to be the same and just adjust the animation generator to not
            //         apply the location animation. Also, refactored the animation type
            //         to always assume affected characters. This will greatly simplify 
            //         the parameter space and logic around Alteration -> Animation.

            // Run animations before applying alterations
            if (alteration.SupportsAnimations && 
                alteration.AnimationGroup.Animations.Count > 0)
            {
                RogueUpdateEvent(this, _rogueUpdateFactory.Animation(
                                            alteration.AnimationGroup.Animations,
                                            _modelService.Player.Location,
                                            affectedCharacters.Select(x => x.Location).Actualize()));
            }

            // Apply Effect on Queue
            LevelProcessingActionEvent(this, new LevelProcessingAction()
            {
                Type = LevelProcessingActionType.CharacterAlteration,
                Actor = actor,
                Alteration = alteration
            });
        }

        public void Process(Character actor, AlterationBase alteration)
        {
            // Apply alteration cost (ONLY ONE-TIME APPLIED HERE. PER-STEP APPLIED IN CHARACTER ALTERATION)
            if (alteration.CostType == AlterationCostType.OneTime)
                _alterationProcessor.ApplyOneTimeAlterationCost(actor, alteration.Cost);

            // Get all affected characters -> ONLY CHARACTERS WHOSE STATS ARE AFFECTED
            var affectedCharacters = CalculateAffectedCharacters(alteration, actor);

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
                               alteration.SupportsBlocking &&
                              _interactionProcessor.CalculateAlterationBlock(_modelService.Player, affectedCharacter, alteration.BlockType);

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
                ApplyAlteration(alteration, actor, affectedCharacter);

                // Alteration engages enemies
                if (affectedCharacter is Enemy)
                {
                    // Flags for engagement and provocation
                    (affectedCharacter as Enemy).IsEngaged = true;
                    (affectedCharacter as Enemy).WasAttackedByPlayer = true;
                }

                // Apply blanket update for player to ensure symbol alterations are processed
                RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.ContentUpdate, affectedCharacter.Id));
            }
        }

        public void ApplyEndOfTurn()
        {
            throw new NotImplementedException();
        }

        #region (private) Alteration Apply Methods
        private void ApplyAlteration(AlterationBase alteration, Character affectedCharacter, Character actor)
        {
            // Aura => Actor is affected.. But always use the affected character - which should be the same.
            //         Aura effects for the target characters are applied in the CharacterAlteration on turn.
            if (alteration.Effect is AttackAttributeAuraAlterationEffect)
                affectedCharacter.Alteration_NEW.Apply(alteration);

            else if (alteration.Effect is AttackAttributeMeleeAlterationEffect)
                _interactionProcessor.CalculateAttackAttributeHit(alteration.RogueName,
                                                                  affectedCharacter,
                                                                  (alteration.Effect as AttackAttributeMeleeAlterationEffect).AttackAttributes);

            else if (alteration.Effect is AttackAttributePassiveAlterationEffect)
                affectedCharacter.Alteration_NEW.Apply(alteration);

            else if (alteration.Effect is AttackAttributeTemporaryAlterationEffect)
                affectedCharacter.Alteration_NEW.Apply(alteration);

            else if (alteration.Effect is AuraAlterationEffect)
                affectedCharacter.Alteration_NEW.Apply(alteration);

            else if (alteration.Effect is ChangeLevelAlterationEffect)
                ProcessChangeLevel(alteration.Effect as ChangeLevelAlterationEffect);

            else if (alteration.Effect is CreateMonsterAlterationEffect)
                ProcessCreateMonster(alteration.Effect as CreateMonsterAlterationEffect, actor);

            else if (alteration.Effect is EquipmentModifyAlterationEffect)
            {
                // TODO:ALTERATION
            }

            else if (alteration.Effect is OtherAlterationEffect)
            {
                switch ((alteration.Effect as OtherAlterationEffect).Type)
                {
                    case AlterationOtherEffectType.Identify:
                        RogueUpdateEvent(this, _rogueUpdateFactory.Dialog(DialogEventType.Identify));
                        break;
                    case AlterationOtherEffectType.Uncurse:
                        RogueUpdateEvent(this, _rogueUpdateFactory.Dialog(DialogEventType.Uncurse));
                        break;
                    default:
                        break;
                }
            }

            else if (alteration.Effect is PassiveAlterationEffect)
                affectedCharacter.Alteration_NEW.Apply(alteration);

            else if (alteration.Effect is PermanentAlterationEffect)
                _alterationProcessor.ApplyPermanentEffect(affectedCharacter, alteration.Effect as PermanentAlterationEffect);

            else if (alteration.Effect is RemedyAlterationEffect)
                affectedCharacter.Alteration_NEW.Apply(alteration);

            else if (alteration.Effect is RevealAlterationEffect)
                ProcessReveal(alteration.Effect as RevealAlterationEffect);

            else if (alteration.Effect is RunAwayAlterationEffect)
                ProcessRunAway(affectedCharacter);

            else if (alteration.Effect is StealAlterationEffect)
                ProcessSteal(actor, affectedCharacter);

            else if (alteration.Effect is TeleportAlterationEffect)
                ProcessTeleport(alteration.Effect as TeleportAlterationEffect, actor);

            else if (alteration.Effect is TemporaryAlterationEffect)
                actor.Alteration_NEW.Apply(alteration);
        }
        #endregion

        #region (private) Alteration Calculation Methods
        private IEnumerable<Character> CalculateAffectedCharacters(AlterationBase alteration, Character actor)
        {
            var type = alteration.GetType();

            if (type == typeof(ConsumableAlteration))
                return CalculateAffectedCharacters(alteration as ConsumableAlteration, actor);

            else if (type == typeof(ConsumableProjectileAlteration))
                return CalculateAffectedCharacters(AlterationTargetType.Target, actor);

            else if (type == typeof(DoodadAlteration))
                return CalculateAffectedCharacters(alteration as DoodadAlteration, actor);

            else if (type == typeof(EnemyAlteration))
                return CalculateAffectedCharacters(alteration as EnemyAlteration, actor);

            else if (type == typeof(EquipmentAttackAlteration))
                return CalculateAffectedCharacters(alteration as EquipmentAttackAlteration, actor);

            else if (type == typeof(EquipmentCurseAlteration))
                return CalculateAffectedCharacters(alteration as EquipmentCurseAlteration, actor);

            else if (type == typeof(EquipmentEquipAlteration))
                return CalculateAffectedCharacters(alteration as EquipmentEquipAlteration, actor);

            else if (type == typeof(SkillAlteration))
                return CalculateAffectedCharacters(alteration as SkillAlteration, actor);

            else
                throw new Exception("Unknown AlterationBase type");
        }
        private IEnumerable<Character> CalculateAffectedCharacters(ConsumableAlteration alteration, Character actor)
        {
            return CalculateAffectedCharacters(alteration.TargetType, actor);
        }
        private IEnumerable<Character> CalculateAffectedCharacters(ConsumableProjectileAlteration alteration, Character actor)
        {
            return CalculateAffectedCharacters(AlterationTargetType.Target, actor);
        }
        private IEnumerable<Character> CalculateAffectedCharacters(DoodadAlteration alteration, Character actor)
        {
            return CalculateAffectedCharacters(alteration.TargetType, actor);
        }
        private IEnumerable<Character> CalculateAffectedCharacters(EnemyAlteration alteration, Character actor)
        {
            return CalculateAffectedCharacters(alteration.TargetType, actor);
        }
        private IEnumerable<Character> CalculateAffectedCharacters(EquipmentAttackAlteration alteration, Character actor)
        {
            throw new NotImplementedException("Have to add affected character based on player action");
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
            return CalculateAffectedCharacters(alteration.TargetType, actor);
        }
        private IEnumerable<Character> CalculateAffectedCharacters(AttackAttributeMeleeAlterationEffect effect, Character actor)
        {
            return CalculateAffectedCharacters(effect.TargetType, actor);
        }
        private IEnumerable<Character> CalculateAffectedCharacters(AttackAttributeTemporaryAlterationEffect effect, Character actor)
        {
            return CalculateAffectedCharacters(effect.TargetType, actor);
        }
        private IEnumerable<Character> CalculateAffectedCharacters(OtherAlterationEffect effect, Character actor)
        {
            switch (effect.Type)
            {
                case AlterationOtherEffectType.Identify:
                case AlterationOtherEffectType.Uncurse:
                    return new List<Character>() { actor };
                default:
                    throw new Exception("Unknown Other Alteration Effect");
            }
        }
        private IEnumerable<Character> CalculateAffectedCharacters(PassiveAlterationEffect effect, Character actor)
        {
            return CalculateAffectedCharacters(AlterationTargetType.Source, actor);
        }
        private IEnumerable<Character> CalculateAffectedCharacters(PermanentAlterationEffect effect, Character actor)
        {
            return CalculateAffectedCharacters(AlterationTargetType.Source, actor);
        }
        private IEnumerable<Character> CalculateAffectedCharacters(RemedyAlterationEffect effect, Character actor)
        {
            return CalculateAffectedCharacters(AlterationTargetType.Source, actor);
        }
        private IEnumerable<Character> CalculateAffectedCharacters(AlterationTargetType targetType, Character actor)
        {
            switch (targetType)
            {
                case AlterationTargetType.Source:
                    return new List<Character>() { actor };
                case AlterationTargetType.Target:
                    return (actor is Player) ? _modelService.GetTargetedEnemies()
                                                            .Cast<Character>() : new List<Character>() { _modelService.Player };
                case AlterationTargetType.AllInRange:
                    return CalculateCharactersInRange(actor.Location, (int)actor.GetLightRadius());
                case AlterationTargetType.AllInRangeExceptSource:
                    return CalculateCharactersInRange(actor.Location, (int)actor.GetLightRadius()).Except(new Character[] { actor });
                default:
                    throw new Exception("Unknown Attack Attribute Target Type");
            }
        }
        private IEnumerable<Character> CalculateCharactersInRange(CellPoint location, int cellRange)
        {
            // TODO:ALTERATION - LINE OF SIGHT! Consider calculating line-of-sight characters for each enemy on end of turn
            //                   and storing them on the character

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

                    // These should never be turned on; but doing for good measure
                    if (equipment.HasEquipAlteration)
                        actee.Alteration_NEW.Remove(equipment.EquipAlteration.Id);

                    if (equipment.HasCurseAlteration)
                        actee.Alteration_NEW.Remove(equipment.CurseAlteration.Id);

                    // Update UI
                    if (actor is Player)
                        RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerEquipmentAddOrUpdate, itemStolen.Key));

                    else
                        RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerEquipmentRemove, itemStolen.Key));
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
                        RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerConsumableAddOrUpdate, itemStolen.Key));

                    else
                        RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerConsumableRemove, itemStolen.Key));
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
                RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.ContentRemove, actor.Id));
            }
            else
                throw new Exception("Player trying to invoke RunAway Alteration Effect");
        }
        private void ProcessReveal(RevealAlterationEffect effect)
        {
            switch (effect.Type)
            {
                case AlterationRevealType.Items:
                    break;
                case AlterationRevealType.Monsters:
                    break;
                case AlterationRevealType.SavePoint:
                    break;
                case AlterationRevealType.Food:
                    break;
                case AlterationRevealType.Layout:
                    _modelService.UpdateVisibleLocations();
                    _modelService.UpdateContents();
                    break;
                default:
                    break;
            }

            // Update the UI
            RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.ContentReveal, ""));
        }
        private void ProcessTeleport(TeleportAlterationEffect effect, Character character)
        {
            CellPoint openLocation = CellPoint.Empty;

            // Calculate Teleport Location
            character.Location = GetRandomLocation(effect.TeleportType, character.Location, effect.Range);

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

            RogueUpdateEvent(this, _rogueUpdateFactory.LevelChange(actualLevel, PlayerStartLocation.Random));
        }
        private void ProcessCreateMonster(CreateMonsterAlterationEffect effect, Character actor)
        {
            var location = GetRandomLocation(effect.RandomPlacementType, actor.Location, effect.Range);

            // TODO:ALTERATION
            if (location == CellPoint.Empty)
                return;

            // Get the enemy template
            var enemyTemplate = _modelService.ScenarioConfiguration
                                             .EnemyTemplates
                                             .FirstOrDefault(x => x.Name == effect.CreateMonsterEnemy);

            if (enemyTemplate != null)
            {
                // Create Enemy
                var enemy = _characterGenerator.GenerateEnemy(enemyTemplate, _modelService.CharacterClasses, _modelService.AttackAttributes);

                // Set Enemy Location
                enemy.Location = location;

                // Add Content to Level
                _modelService.Level.AddContent(enemy);

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
                RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.ContentAdd, enemy.Id));
            }
        }
        private CellPoint GetRandomLocation(AlterationRandomPlacementType placementType, CellPoint sourceLocation, int sourceRange)
        {
            var level = _modelService.Level;
            var player = _modelService.Player;

            CellPoint openLocation = CellPoint.Empty;

            switch (placementType)
            {
                case AlterationRandomPlacementType.InLevel:
                    openLocation = _modelService.Level.GetRandomLocation(true, _randomSequenceGenerator);
                    break;
                case AlterationRandomPlacementType.InRangeOfCharacter:
                    openLocation = _layoutEngine.GetLocationsInRange(level, sourceLocation, sourceRange)
                                                .Where(x => level.IsCellOccupied(sourceLocation, player.Location))
                                                .PickRandom(_randomSequenceGenerator.Get());
                    break;
                case AlterationRandomPlacementType.InPlayerVisibleRange:
                    openLocation = _modelService.GetVisibleLocations()
                                                .Where(x => level.IsCellOccupied(sourceLocation, player.Location))
                                                .PickRandom(_randomSequenceGenerator.Get());
                    break;
                default:
                    throw new Exception("Unhandled AlterationRandomPlacementType");
            }

            return openLocation ?? (openLocation == CellPoint.Empty ? sourceLocation : openLocation);
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
        private void RevealLevel()
        {
            foreach (var cell in _modelService.Level.Grid.GetCells())
                cell.IsRevealed = true;

            RevealSavePoint();
            RevealStairs();
            RevealMonsters();
            RevealContent();

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
        private void RevealContent()
        {
            foreach (var scenarioObject in _modelService.Level.GetContents())
            {
                scenarioObject.IsHidden = false;
                scenarioObject.IsRevealed = true;
            }

            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "You sense objects nearby");
        }
        private void RevealFood()
        {
            foreach (var consumable in _modelService.Level.Consumables.Where(x => x.SubType == ConsumableSubType.Food))
                consumable.IsRevealed = true;

            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Hunger makes a good sauce.....  :)");
        }
        #endregion
    }
}
