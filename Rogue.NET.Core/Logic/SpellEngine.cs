using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Logic.Content.Interface;
using Rogue.NET.Core.Logic.Interface;
using Rogue.NET.Core.Logic.Processing;
using Rogue.NET.Core.Logic.Processing.Enum;
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

        public event EventHandler<IScenarioUpdate> ScenarioUpdateEvent;
        public event EventHandler<ISplashUpdate> SplashUpdateEvent;
        public event EventHandler<IDialogUpdate> DialogUpdateEvent;
        public event EventHandler<ILevelUpdate> LevelUpdateEvent;
        public event EventHandler<IAnimationUpdate> AnimationUpdateEvent;
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
            IRandomSequenceGenerator randomSequenceGenerator)
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
        }

        public LevelContinuationAction QueuePlayerMagicSpell(Spell spell)
        {
            // Cost will be applied on turn - after animations are processed
            if (!_alterationProcessor.CalculatePlayerMeetsAlterationCost(_modelService.Player, spell.Cost))
                return LevelContinuationAction.DoNothing;

            // Affected characters
            var affectedCharacters = _interactionProcessor.CalculateAffectedAlterationCharacters(spell.Type, spell.AttackAttributeType, spell.EffectRange, _modelService.Player);

            // Check for target requirements for animations
            var animationRequirementsNotMet = !affectedCharacters.Any() && GetAnimationRequiresTarget(spell.Animations);

            //Run animations before applying effects
            if (spell.Animations.Count > 0 && !animationRequirementsNotMet)
            {
                AnimationUpdateEvent(this, new AnimationUpdate()
                {
                    Animations = spell.Animations,
                    SourceLocation = _modelService.Player.Location,
                    TargetLocations = affectedCharacters.Select(x => x.Location).Actualize()
                });
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
            var affectedCharacters = _interactionProcessor.CalculateAffectedAlterationCharacters(spell.Type, spell.AttackAttributeType, spell.EffectRange, enemy);

            // Check for target requirements for animations
            var animationRequirementsNotMet = !affectedCharacters.Any() && GetAnimationRequiresTarget(spell.Animations);

            // Queue animations
            if (spell.Animations.Count > 0 && !animationRequirementsNotMet)
            {
                AnimationUpdateEvent(this, new AnimationUpdate()
                {
                    Animations = spell.Animations,
                    SourceLocation = enemy.Location,
                    TargetLocations = affectedCharacters.Select(x => x.Location).Actualize()
                });
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

            // Get all affected characters
            var affectedCharacters = _interactionProcessor.CalculateAffectedAlterationCharacters(alteration.Type, alteration.AttackAttributeType, alteration.EffectRange, caster);

            // Enter loop:  Characters -> Attempt Block -> Apply Alteration -> Show Messages -> Enemy.IsEngaged = true;
            //
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
                ApplyAlteration(alteration, character, caster);

                // Casting a spell engages enemies
                if (character is Enemy)
                {
                    // Flags for engagement and provocation
                    (character as Enemy).IsEngaged = true;
                    (character as Enemy).WasAttackedByPlayer = true;
                }

                // Apply blanket update for player to ensure symbol alterations are processed
                QueueLevelUpdate(LevelUpdateType.ContentUpdate, character.Id);
            }
        }

        private void ApplyAlteration(AlterationContainer alteration, Character affectedCharacter, Character sourceCharacter)
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
                        ApplyEnemyStealAlteration(affectedCharacter as Enemy, alteration);
                    else
                        ApplyPlayerStealAlteration(alteration);
                    break;
                case AlterationType.RunAway:
                    // TODO: Create separate method
                    if (affectedCharacter is Enemy)
                    {
                        _modelService.Level.RemoveContent(affectedCharacter);
                        _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, _modelService.GetDisplayName(affectedCharacter) + " has run away!");

                        QueueLevelUpdate(LevelUpdateType.ContentRemove, affectedCharacter.Id);
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
                    break;
            }
        }
        private void ApplyAttackAttributeEffect(AlterationContainer alteration, Character affectedCharacter, Character sourceCharacter)
        {
            switch (alteration.AttackAttributeType)
            {
                case AlterationAttackAttributeType.ImbueArmor:
                    if (affectedCharacter is Player)
                        DialogUpdateEvent(this, new DialogUpdate() { Type = DialogEventType.ImbueArmor, ImbueAttackAttributes = alteration.Effect.AttackAttributes });
                    break;
                case AlterationAttackAttributeType.ImbueWeapon:
                    if (affectedCharacter is Player)
                        DialogUpdateEvent(this, new DialogUpdate() { Type = DialogEventType.ImbueWeapon, ImbueAttackAttributes = alteration.Effect.AttackAttributes });
                    break;
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
                default:
                    break;
            }
        }
        private void ApplyOtherMagicEffect(AlterationContainer alteration, Character character)
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

                        ScenarioUpdateEvent(this, new ScenarioUpdate()
                        {
                            ScenarioUpdateType = ScenarioUpdateType.LevelChange,

                            LevelNumber = level,
                            StartLocation = PlayerStartLocation.Random
                        });
                    }
                    break;
                case AlterationMagicEffectType.ChangeLevelRandomUp:
                    {
                        var minLevel = _modelService.Level.Number - 10;
                        var maxLevel = _modelService.Level.Number - 1;
                        var numberOfLevels = _modelService.ScenarioConfiguration.DungeonTemplate.NumberOfLevels;
                        var randomLevel = _randomSequenceGenerator.Get(minLevel, maxLevel);
                        var level = Math.Max(randomLevel, 1);

                        ScenarioUpdateEvent(this, new ScenarioUpdate()
                        {
                            ScenarioUpdateType = ScenarioUpdateType.LevelChange,

                            LevelNumber = level,
                            StartLocation = PlayerStartLocation.Random
                        });
                    }
                    break;
                case AlterationMagicEffectType.EnchantArmor:
                    DialogUpdateEvent(this, new DialogUpdate() { Type = DialogEventType.EnchantArmor });
                    break;
                case AlterationMagicEffectType.EnchantWeapon:
                    DialogUpdateEvent(this, new DialogUpdate() { Type = DialogEventType.EnchantWeapon });
                    break;
                case AlterationMagicEffectType.Identify:
                    DialogUpdateEvent(this, new DialogUpdate() { Type = DialogEventType.Identify });
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
                    DialogUpdateEvent(this, new DialogUpdate() { Type = DialogEventType.Uncurse });
                    break;
                case AlterationMagicEffectType.CreateMonster:
                    if (character is Player)
                        CreateMonster(alteration.CreateMonsterEnemy);
                    else
                        CreateMonsterMinion(character as Enemy, alteration.CreateMonsterEnemy);
                    break;
                case AlterationMagicEffectType.RenounceReligion:
                    // Forced renunciation by Alteration
                    _religionEngine.RenounceReligion(true);
                    break;
                case AlterationMagicEffectType.IncreaseReligiousAffiliation:
                    _religionEngine.Affiliate(alteration.ReligiousAffiliationReligionName,
                                              alteration.ReligiousAffiliationIncrease);
                    break;
            }
        }
        private void ApplyPlayerStealAlteration(AlterationContainer alteration)
        {
            // Must require a target
            var enemy = _modelService.GetTargetedEnemies().First();
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
                QueueLevelUpdate(LevelUpdateType.PlayerEquipmentRemove, itemStolen.Key);
            }
            else
            {
                _modelService.Player.Consumables.Remove(itemStolen.Key);
                enemy.Consumables.Add(itemStolen.Key, itemStolen.Value as Consumable);

                // Update UI
                QueueLevelUpdate(LevelUpdateType.PlayerConsumableRemove, itemStolen.Key);
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

            QueueLevelUpdate(LevelUpdateType.ContentReveal, "");
        }
        private void RevealMonsters()
        {
            foreach (var enemy in _modelService.Level.Enemies)
                enemy.IsRevealed = true;

            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "You hear growling in the distance...");

            QueueLevelUpdate(LevelUpdateType.ContentReveal, "");
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

            QueueLevelUpdate(LevelUpdateType.LayoutReveal, "");
        }
        private void RevealStairs()
        {
            if (_modelService.Level.HasStairsDown)
                _modelService.Level.StairsDown.IsRevealed = true;

            if (_modelService.Level.HasStairsUp)
                _modelService.Level.StairsUp.IsRevealed = true;

            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "You sense exits nearby");

            QueueLevelUpdate(LevelUpdateType.ContentReveal, "");
        }
        private void RevealItems()
        {
            foreach (var consumable in _modelService.Level.Consumables)
                consumable.IsRevealed = true;

            foreach (var equipment in _modelService.Level.Equipment)
                equipment.IsRevealed = true;

            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "You sense objects nearby");

            QueueLevelUpdate(LevelUpdateType.ContentReveal, "");
        }
        private void RevealContent()
        {
            foreach (var scenarioObject in _modelService.Level.GetContents())
            {
                scenarioObject.IsHidden = false;
                scenarioObject.IsRevealed = true;
            }

            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "You sense objects nearby");

            QueueLevelUpdate(LevelUpdateType.ContentReveal, "");
        }
        private void RevealFood()
        {
            foreach (var consumable in _modelService.Level.Consumables.Where(x => x.SubType == ConsumableSubType.Food))
                consumable.IsRevealed = true;

            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Hunger makes a good sauce.....  :)");

            QueueLevelUpdate(LevelUpdateType.ContentReveal, "");
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
                QueueLevelUpdate(LevelUpdateType.ContentAdd, minion.Id);
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
                QueueLevelUpdate(LevelUpdateType.ContentAdd, enemy.Id);
            }
        }

        // TODO: Move this and refactor for checking target requirements
        private bool GetAnimationRequiresTarget(IEnumerable<AnimationTemplate> animations)
        {
            foreach (var animation in animations)
            {
                switch (animation.Type)
                {
                    case AnimationType.ProjectileSelfToTarget:
                    case AnimationType.ProjectileTargetToSelf:
                    case AnimationType.ProjectileSelfToTargetsInRange:
                    case AnimationType.ProjectileTargetsInRangeToSelf:
                    case AnimationType.AuraTarget:
                    case AnimationType.BubblesTarget:
                    case AnimationType.BarrageTarget:
                    case AnimationType.SpiralTarget:
                    case AnimationType.ChainSelfToTargetsInRange:
                        return true;
                    case AnimationType.AuraSelf:
                    case AnimationType.BubblesSelf:
                    case AnimationType.BubblesScreen:
                    case AnimationType.BarrageSelf:
                    case AnimationType.SpiralSelf:
                    case AnimationType.ScreenBlink:
                        break;
                    default:
                        throw new Exception("Animation Type not recognized for target calculation");
                }
            }
            return false;
        }

        public void ApplyEndOfTurn()
        {
            throw new NotImplementedException();
        }

        #region (private) Event Update Methods
        private void QueueLevelUpdate(LevelUpdateType type, string contentId)
        {
            LevelUpdateEvent(this, new LevelUpdate()
            {
                LevelUpdateType = type,
                ContentIds = new string[] { contentId }
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
        #endregion
    }
}
