using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Logic.Content.Interface;
using Rogue.NET.Core.Logic.Interface;
using Rogue.NET.Core.Logic.Processing;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing.Factory.Interface;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Logic.Static;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using Rogue.NET.Core.Model.Scenario.Content.Extension;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using Rogue.NET.Core.Model.ScenarioMessage;
using Rogue.NET.Core.Service.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Logic
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(ISpellEngine))]
    public class SpellEngine : ISpellEngine
    {
        readonly IModelService _modelService;
        readonly ILayoutEngine _layoutEngine;
        readonly IReligionEngine _religionEngine;
        readonly IAlterationProcessor _alterationProcessor;
        readonly IPlayerProcessor _playerProcessor;
        readonly IInteractionProcessor _interactionProcessor;
        readonly IAlterationGenerator _alterationGenerator;
        readonly IScenarioMessageService _scenarioMessageService;
        readonly ICharacterGenerator _characterGenerator;
        readonly IRandomSequenceGenerator _randomSequenceGenerator;
        readonly IRogueUpdateFactory _rogueUpdateFactory;

        public event EventHandler<RogueUpdateEventArgs> RogueUpdateEvent;
        public event EventHandler<ILevelProcessingAction> LevelProcessingActionEvent;

        [ImportingConstructor]
        public SpellEngine(
            IModelService modelService,
            ILayoutEngine layoutEngine,
            IReligionEngine religionEngine,
            IAlterationProcessor alterationProcessor, 
            IPlayerProcessor playerProcessor,
            IInteractionProcessor interactionProcessor,
            IAlterationGenerator alterationGenerator,
            IScenarioMessageService scenarioMessageService,
            ICharacterGenerator characterGenerator,
            IRandomSequenceGenerator randomSequenceGenerator,
            IRogueUpdateFactory rogueUpdateFactory)
        {
            _modelService = modelService;
            _layoutEngine = layoutEngine;
            _religionEngine = religionEngine;
            _alterationProcessor = alterationProcessor;
            _playerProcessor = playerProcessor;
            _interactionProcessor = interactionProcessor;
            _alterationGenerator = alterationGenerator;
            _scenarioMessageService = scenarioMessageService;
            _characterGenerator = characterGenerator;
            _randomSequenceGenerator = randomSequenceGenerator;
            _rogueUpdateFactory = rogueUpdateFactory;
        }

        public LevelContinuationAction QueuePlayerMagicSpell(Spell spell)
        {
            // Cost will be applied on turn - after animations are processed
            if (!_alterationProcessor.CalculatePlayerMeetsAlterationCost(_modelService.Player, spell.Cost))
                return LevelContinuationAction.DoNothing;

            // Affected characters
            var affectedCharacterExpected = false;
            var affectedCharacters = _interactionProcessor
                                        .CalculateAffectedAlterationCharacters(
                                            spell.Type, 
                                            spell.AttackAttributeType, 
                                            spell.OtherEffectType,
                                            spell.EffectRange, 
                                            _modelService.Player,
                                            out affectedCharacterExpected);

            // Check for target requirements for animations
            var animationRequirementsNotMet = !affectedCharacters.Any() && 
                                              _interactionProcessor.GetAnimationRequiresTarget(spell.Animations);

            //Run animations before applying effects
            if (spell.Animations.Count > 0 && !animationRequirementsNotMet)
            {
                RogueUpdateEvent(this, _rogueUpdateFactory.Animation(
                                            spell.Animations,
                                            _modelService.Player.Location,
                                            affectedCharacters.Select(x => x.Location).Actualize()));
            }

            // Apply Effect on Queue
            LevelProcessingActionEvent(this, new LevelProcessingAction()
            {
                Type = LevelProcessingActionType.PlayerSpell,
                PlayerSpell = spell
            });

            return LevelContinuationAction.ProcessTurn;
        }
        public LevelContinuationAction QueueEnemyMagicSpell(Enemy enemy, Spell spell)
        {
            // Cost will be applied on turn - after animations are processed
            if (!_alterationProcessor.CalculateEnemyMeetsAlterationCost(enemy, spell.Cost))
                return LevelContinuationAction.ProcessTurn;

            // Affected Characters
            var affectedCharacterExpected = false;
            var affectedCharacters = _interactionProcessor
                                        .CalculateAffectedAlterationCharacters(
                                            spell.Type, 
                                            spell.AttackAttributeType, 
                                            spell.OtherEffectType,
                                            spell.EffectRange, 
                                            enemy,
                                            out affectedCharacterExpected);

            // Check for target requirements for animations
            var animationRequirementsNotMet = !affectedCharacters.Any() && 
                                              _interactionProcessor.GetAnimationRequiresTarget(spell.Animations);

            // Queue animations
            if (spell.Animations.Count > 0 && !animationRequirementsNotMet)
            {
                RogueUpdateEvent(this, _rogueUpdateFactory.Animation(
                                            spell.Animations,
                                            enemy.Location,
                                            affectedCharacters.Select(x => x.Location).Actualize()));
            }
            // Then queue effect processing
            LevelProcessingActionEvent(this, new LevelProcessingAction()
            {
                Type = LevelProcessingActionType.EnemySpell,
                EnemySpell = spell,
                Enemy = enemy
            });

            return LevelContinuationAction.ProcessTurn;
        }

        public void ProcessMagicSpell(Character caster, Spell spell)
        {
            //Calculate alteration from spell's random parameters
            var alteration = _alterationGenerator.GenerateAlteration(spell);

            //Apply alteration cost (ONLY ONE-TIME APPLIED HERE. PER-STEP APPLIED IN CHARACTER ALTERATION)
            if (alteration.Cost.Type == AlterationCostType.OneTime)
                _alterationProcessor.ApplyOneTimeAlterationCost(caster, alteration.Cost);

            // Get all affected characters -> ONLY CHARACTERS WHOSE STATS ARE AFFECTED
            var affectedCharactersExpected = false;
            var affectedCharacters = 
                _interactionProcessor.CalculateAffectedAlterationCharacters(
                    alteration.Type, 
                    alteration.AttackAttributeType, 
                    alteration.OtherEffectType,
                    alteration.EffectRange, 
                    caster,
                    out affectedCharactersExpected);

            // PROCESS ALTERATION
            //
            // 0) Non-Affected Character Alterations -> Return
            // 1) Affected Character Alterations

            // Non-Affected Character Alterations:  Don't allow blocking
            if (!affectedCharacters.Any() && !affectedCharactersExpected)
            {
                // Apply Alteration
                ApplyNonCharacterAlteration(alteration);

                return;
            }

            // Affected Character Alterations:
            //      
            //    Enter loop:  Characters -> Attempt Block -> Apply Alteration -> Show Messages -> Enemy.IsEngaged = true;
            foreach (var character in affectedCharacters)
            {
                // Enemy attempts block
                bool blocked = character != caster && 
                              _interactionProcessor.CalculateAlterationBlock(_modelService.Player, character, alteration.BlockType);

                // Blocked -> Message and continue
                if (blocked)
                {
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, _modelService.GetDisplayName(character) + " blocked the attack!");
                    continue;
                }

                // Apply Alteration
                ApplyCharacterAlteration(alteration, character, caster);

                // Casting a spell engages enemies
                if (character is Enemy)
                {
                    // Flags for engagement and provocation
                    (character as Enemy).IsEngaged = true;
                    (character as Enemy).WasAttackedByPlayer = true;
                }

                // Apply blanket update for player to ensure symbol alterations are processed
                RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.ContentUpdate, character.Id));
            }
        }

        private void ApplyCharacterAlteration(AlterationContainer alteration, Character affectedCharacter, Character sourceCharacter)
        {
            switch (alteration.Type)
            {
                case AlterationType.PassiveSource:
                case AlterationType.PassiveAura:
                case AlterationType.TemporarySource:                
                case AlterationType.TemporaryTarget:
                case AlterationType.TemporaryAllTargets:
                case AlterationType.TemporaryAllInRange:
                case AlterationType.TemporaryAllInRangeExceptSource:
                    affectedCharacter.Alteration.ActivateAlteration(alteration, true);
                    break;
                case AlterationType.PermanentSource:
                case AlterationType.PermanentTarget:
                case AlterationType.PermanentAllTargets:
                case AlterationType.PermanentAllInRange:
                case AlterationType.PermanentAllInRangeExceptSource:
                    _alterationProcessor.ApplyPermanentEffect(affectedCharacter, alteration.Effect);
                    break;
                case AlterationType.Steal:
                    if (affectedCharacter is Enemy)
                        ApplyPlayerStealAlteration(affectedCharacter as Enemy, alteration);
                    else
                        ApplyEnemyStealAlteration(sourceCharacter as Enemy, alteration);
                    break;
                case AlterationType.RunAway:
                    // TODO: Create separate method
                    if (affectedCharacter is Enemy)
                    {
                        _modelService.Level.RemoveContent(affectedCharacter);
                        _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, _modelService.GetDisplayName(affectedCharacter) + " has run away!");

                        RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.ContentRemove, affectedCharacter.Id));
                    }
                    break;
                case AlterationType.TeleportSelf:
                case AlterationType.TeleportTarget:
                case AlterationType.TeleportAllTargets:
                case AlterationType.TeleportAllInRange:
                case AlterationType.TeleportAllInRangeExceptSource:
                    TeleportRandom(affectedCharacter);
                    break;
                case AlterationType.OtherMagicEffect:
                    ApplyOtherMagicEffect(alteration, affectedCharacter);
                    break;
                case AlterationType.AttackAttribute:
                    ApplyAttackAttributeEffect(alteration, affectedCharacter, sourceCharacter);
                    break;
                case AlterationType.Remedy:
                    _alterationProcessor.ApplyRemedy(affectedCharacter, alteration.Effect);
                    break;
                default:
                    throw new Exception("Unhandled Alteration Type ApplyCharacterAlteration");
            }
        }
        private void ApplyAttackAttributeEffect(AlterationContainer alteration, Character affectedCharacter, Character sourceCharacter)
        {
            switch (alteration.AttackAttributeType)
            {
                case AlterationAttackAttributeType.Passive:
                case AlterationAttackAttributeType.TemporaryFriendlySource:
                case AlterationAttackAttributeType.TemporaryFriendlyTarget:
                case AlterationAttackAttributeType.TemporaryMalignSource:
                case AlterationAttackAttributeType.TemporaryMalignTarget:
                case AlterationAttackAttributeType.TemporaryMalignAllInRange:
                case AlterationAttackAttributeType.TemporaryMalignAllInRangeExceptSource:
                    affectedCharacter.Alteration.ActivateAlteration(alteration, affectedCharacter == sourceCharacter);
                    break;
                case AlterationAttackAttributeType.MeleeTarget:
                case AlterationAttackAttributeType.MeleeAllInRange:
                case AlterationAttackAttributeType.MeleeAllInRangeExceptSource:
                    // Apply the Alteration Effect -> Publish the results
                    _interactionProcessor.CalculateAttackAttributeHit(
                        alteration.Effect.DisplayName,
                        sourceCharacter,
                        affectedCharacter,
                        alteration.Effect.AttackAttributes);
                    break;
                case AlterationAttackAttributeType.ImbueArmor:
                case AlterationAttackAttributeType.ImbueWeapon:
                default:
                    throw new Exception("Improper use of Attack Attribute type for character alteration");
            }
        }
        private void ApplyOtherMagicEffect(AlterationContainer alteration, Character character)
        {
            switch (alteration.OtherEffectType)
            {
                case AlterationMagicEffectType.ChangeLevelRandomDown:
                case AlterationMagicEffectType.ChangeLevelRandomUp:
                case AlterationMagicEffectType.EnchantArmor:
                case AlterationMagicEffectType.EnchantWeapon:
                case AlterationMagicEffectType.Identify:
                case AlterationMagicEffectType.RevealFood:
                case AlterationMagicEffectType.RevealItems:
                case AlterationMagicEffectType.RevealLevel:
                case AlterationMagicEffectType.RevealMonsters:
                case AlterationMagicEffectType.RevealSavePoint:
                case AlterationMagicEffectType.Uncurse:
                case AlterationMagicEffectType.RenounceReligion:
                case AlterationMagicEffectType.AffiliateReligion:
                default:
                    throw new Exception("Improper use of Other Magic Effect type for character-affecting alteration");
                case AlterationMagicEffectType.CreateMonster:
                    if (character is Player)
                        CreateMonster(alteration.CreateMonsterEnemy);
                    else
                        CreateMonsterMinion(character as Enemy, alteration.CreateMonsterEnemy);
                    break;
            }
        }

        private void ApplyNonCharacterAlteration(AlterationContainer alteration)
        {
            switch (alteration.Type)
            {
                case AlterationType.PassiveSource:
                case AlterationType.PassiveAura:
                case AlterationType.TemporarySource:
                case AlterationType.TemporaryTarget:
                case AlterationType.TemporaryAllTargets:
                case AlterationType.TemporaryAllInRange:
                case AlterationType.TemporaryAllInRangeExceptSource:
                case AlterationType.PermanentSource:
                case AlterationType.PermanentTarget:
                case AlterationType.PermanentAllTargets:
                case AlterationType.PermanentAllInRange:
                case AlterationType.PermanentAllInRangeExceptSource:
                case AlterationType.RunAway:
                case AlterationType.TeleportSelf:
                case AlterationType.TeleportTarget:
                case AlterationType.TeleportAllTargets:
                case AlterationType.TeleportAllInRange:
                case AlterationType.TeleportAllInRangeExceptSource:
                case AlterationType.Remedy:
                default:
                    throw new Exception("Improper use of Alteration Type for non-character alteration");
                case AlterationType.OtherMagicEffect:
                    ApplyNonCharacterOtherMagicEffect(alteration);
                    break;
                case AlterationType.AttackAttribute:
                    ApplyNonCharacterAttackAttributeEffect(alteration);
                    break;
            }
        }
        private void ApplyNonCharacterOtherMagicEffect(AlterationContainer alteration)
        {
            switch (alteration.OtherEffectType)
            {
                case AlterationMagicEffectType.ChangeLevelRandomDown:
                    {
                        var minLevel = _modelService.Level.Number + 1;
                        var maxLevel = _modelService.Level.Number + 10;
                        var numberOfLevels = _modelService.ScenarioConfiguration.DungeonTemplate.NumberOfLevels;
                        var randomLevel = _randomSequenceGenerator.Get(minLevel, maxLevel);
                        var level = Math.Min(randomLevel, numberOfLevels);

                        RogueUpdateEvent(this, _rogueUpdateFactory.LevelChange(level, PlayerStartLocation.Random));
                    }
                    break;
                case AlterationMagicEffectType.ChangeLevelRandomUp:
                    {
                        var minLevel = _modelService.Level.Number - 10;
                        var maxLevel = _modelService.Level.Number - 1;
                        var numberOfLevels = _modelService.ScenarioConfiguration.DungeonTemplate.NumberOfLevels;
                        var randomLevel = _randomSequenceGenerator.Get(minLevel, maxLevel);
                        var level = Math.Max(randomLevel, 1);

                        RogueUpdateEvent(this, _rogueUpdateFactory.LevelChange(level, PlayerStartLocation.Random));
                    }
                    break;
                case AlterationMagicEffectType.EnchantArmor:
                    RogueUpdateEvent(this, _rogueUpdateFactory.Dialog(DialogEventType.EnchantArmor));
                    break;
                case AlterationMagicEffectType.EnchantWeapon:
                    RogueUpdateEvent(this, _rogueUpdateFactory.Dialog(DialogEventType.EnchantWeapon));
                    break;
                case AlterationMagicEffectType.Identify:
                    RogueUpdateEvent(this, _rogueUpdateFactory.Dialog(DialogEventType.Identify));
                    break;
                case AlterationMagicEffectType.RevealFood:
                    RevealFood();
                    break;
                case AlterationMagicEffectType.RevealItems:
                    RevealItems();
                    break;
                case AlterationMagicEffectType.RevealLevel:
                    RevealLevel();
                    break;
                case AlterationMagicEffectType.RevealMonsters:
                    RevealMonsters();
                    break;
                case AlterationMagicEffectType.RevealSavePoint:
                    RevealSavePoint();
                    break;
                case AlterationMagicEffectType.Uncurse:
                    RogueUpdateEvent(this, _rogueUpdateFactory.Dialog(DialogEventType.Uncurse));
                    break;
                case AlterationMagicEffectType.RenounceReligion:
                    // Forced renunciation by Alteration
                    _religionEngine.RenounceReligion(true);
                    break;
                case AlterationMagicEffectType.AffiliateReligion:
                    _religionEngine.Affiliate(alteration.Religion);
                    break;
                case AlterationMagicEffectType.CreateMonster:
                default:
                    throw new Exception("Improper use of Other Magic Effect type for non-character Alteration");
            }
        }
        private void ApplyNonCharacterAttackAttributeEffect(AlterationContainer alteration)
        {
            switch (alteration.AttackAttributeType)
            {
                case AlterationAttackAttributeType.ImbueArmor:
                    RogueUpdateEvent(this, _rogueUpdateFactory.Dialog(DialogEventType.ImbueArmor, alteration.Effect.AttackAttributes));
                    break;
                case AlterationAttackAttributeType.ImbueWeapon:
                    RogueUpdateEvent(this, _rogueUpdateFactory.Dialog(DialogEventType.ImbueWeapon, alteration.Effect.AttackAttributes));
                    break;
                case AlterationAttackAttributeType.Passive:
                case AlterationAttackAttributeType.TemporaryFriendlySource:
                case AlterationAttackAttributeType.TemporaryFriendlyTarget:
                case AlterationAttackAttributeType.TemporaryMalignSource:
                case AlterationAttackAttributeType.TemporaryMalignTarget:
                case AlterationAttackAttributeType.MeleeTarget:
                case AlterationAttackAttributeType.MeleeAllInRange:
                case AlterationAttackAttributeType.MeleeAllInRangeExceptSource:
                case AlterationAttackAttributeType.TemporaryMalignAllInRange:
                case AlterationAttackAttributeType.TemporaryMalignAllInRangeExceptSource:
                    throw new Exception("Improper use of Attack Attribute type for Non-Character Alteration");
                default:
                    break;
            }
        }

        private void ApplyPlayerStealAlteration(Enemy enemy, AlterationContainer alteration)
        {
            // Must require a target
            var enemyInventory = enemy.Inventory;
            if (!enemyInventory.Any())
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, _modelService.GetDisplayName(enemy) + " has nothing to steal");

            else
            {
                var itemStolen = enemyInventory.ElementAt(_randomSequenceGenerator.Get(0, enemyInventory.Count()));
                if (itemStolen.Value is Equipment)
                {
                    var equipment = itemStolen.Value as Equipment;

                    _modelService.Player.Equipment.Add(equipment.Id, equipment);

                    // Mark equipment not-equiped (SHOULD NEVER BE EQUIPPED)
                    equipment.IsEquipped = false;

                    // Remove Equipment from enemy inventory and deactivate passive effects
                    enemy.Equipment.Remove(equipment.Id);

                    // These should never be turned on; but doing for good measure
                    if (equipment.HasEquipSpell)
                        enemy.Alteration.DeactivatePassiveAlteration(equipment.EquipSpell.Id);

                    if (equipment.HasCurseSpell)
                        enemy.Alteration.DeactivatePassiveAlteration(equipment.CurseSpell.Id);
                }
                else
                {
                    _modelService.Player.Consumables.Add(itemStolen.Key, itemStolen.Value as Consumable);
                    enemy.Consumables.Remove(itemStolen.Key);
                }
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "You stole a(n) " + _modelService.GetDisplayName(itemStolen.Value));
            }
        }
        private void ApplyEnemyStealAlteration(Enemy enemy, AlterationContainer alteration)
        {
            var inventory = _modelService.Player.Inventory;
            if (!inventory.Any())
                return;

            var itemStolen = _modelService.Player.Inventory.ElementAt(_randomSequenceGenerator.Get(0, inventory.Count));
            if (itemStolen.Value is Equipment)
            {
                var equipment = itemStolen.Value as Equipment;

                // Remove equipment from player inventory
                _modelService.Player.Equipment.Remove(equipment.Id);

                // Add equipment to enemy inventory
                enemy.Equipment.Add(equipment.Id, equipment);

                // Mark un-equipped
                equipment.IsEquipped = false;

                // Deactivate passive alterations
                if (equipment.HasEquipSpell)
                    _modelService.Player.Alteration.DeactivatePassiveAlteration(equipment.EquipSpell.Id);

                // (Forgiven curse spell when enemy steals item)
                if (equipment.HasCurseSpell)
                    _modelService.Player.Alteration.DeactivatePassiveAlteration(equipment.CurseSpell.Id);

                // Update UI
                RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerEquipmentRemove, itemStolen.Key));
            }
            else
            {
                _modelService.Player.Consumables.Remove(itemStolen.Key);
                enemy.Consumables.Add(itemStolen.Key, itemStolen.Value as Consumable);

                // Update UI
                RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerConsumableRemove, itemStolen.Key));
            }

            var enemyDisplayName = _modelService.GetDisplayName(enemy);
            var itemDisplayName = _modelService.GetDisplayName(itemStolen.Value);

            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "The {0} stole your {1}!", enemyDisplayName, itemDisplayName);
        }

        private void TeleportRandom(Character character)
        {
            character.Location = _modelService.Level.GetRandomLocation(true, _randomSequenceGenerator);

            if (character is Player)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "You were teleported!");

            else
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, _modelService.GetDisplayName(character) + " was teleported!");

                // Set character engaged
                (character as Enemy).IsEngaged = true;                
            }
        }
        private void RevealSavePoint()
        {
            if (_modelService.Level.HasSavePoint)
            {
                _modelService.Level.SavePoint.IsRevealed = true;
            }
            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "You sense odd shrines near by...");

            RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.ContentReveal, ""));
        }
        private void RevealMonsters()
        {
            foreach (var enemy in _modelService.Level.Enemies)
                enemy.IsRevealed = true;

            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "You hear growling in the distance...");

            RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.ContentReveal, ""));
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

            _modelService.UpdateVisibleLocations();
            _modelService.UpdateContents();

            RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.ContentReveal, ""));
        }
        private void RevealStairs()
        {
            if (_modelService.Level.HasStairsDown)
                _modelService.Level.StairsDown.IsRevealed = true;

            if (_modelService.Level.HasStairsUp)
                _modelService.Level.StairsUp.IsRevealed = true;

            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "You sense exits nearby");

            RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.ContentReveal, ""));
        }
        private void RevealItems()
        {
            foreach (var consumable in _modelService.Level.Consumables)
                consumable.IsRevealed = true;

            foreach (var equipment in _modelService.Level.Equipment)
                equipment.IsRevealed = true;

            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "You sense objects nearby");

            RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.ContentReveal, ""));
        }
        private void RevealContent()
        {
            foreach (var scenarioObject in _modelService.Level.GetContents())
            {
                scenarioObject.IsHidden = false;
                scenarioObject.IsRevealed = true;
            }

            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "You sense objects nearby");

            RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.ContentReveal, ""));
        }
        private void RevealFood()
        {
            foreach (var consumable in _modelService.Level.Consumables.Where(x => x.SubType == ConsumableSubType.Food))
                consumable.IsRevealed = true;

            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Hunger makes a good sauce.....  :)");

            RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.ContentReveal, ""));
        }
        private void CreateMonsterMinion(Enemy enemy, string monsterName)
        {
            var location = _layoutEngine.GetRandomAdjacentLocation(_modelService.Level, _modelService.Player, enemy.Location, true);
            if (location == CellPoint.Empty)
                return;

            var enemyTemplate = _modelService.ScenarioConfiguration.EnemyTemplates.FirstOrDefault(x => x.Name == monsterName);

            if (enemyTemplate != null)
            {
                var minion = _characterGenerator.GenerateEnemy(enemyTemplate, _modelService.Religions, _modelService.GetAttackAttributes());

                minion.Location = location;

                _modelService.Level.AddContent(minion);

                // Notify UI
                RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.ContentAdd, minion.Id));
            }
        }
        private void CreateMonster(string monsterName)
        {
            var enemyTemplate = _modelService.ScenarioConfiguration.EnemyTemplates.FirstOrDefault(x => x.Name == monsterName);

            if (enemyTemplate != null)
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "You hear growling in the distance");

                var enemy = _characterGenerator.GenerateEnemy(enemyTemplate, _modelService.Religions, _modelService.GetAttackAttributes());
                enemy.Location = _modelService.Level.GetRandomLocation(true, _randomSequenceGenerator);

                _modelService.Level.AddContent(enemy);

                // Notify UI
                RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.ContentAdd, enemy.Id));
            }
        }

        public void ApplyEndOfTurn()
        {
            throw new NotImplementedException();
        }
    }
}
