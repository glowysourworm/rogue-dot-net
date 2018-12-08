using Rogue.NET.Core.Logic.Content.Interface;
using Rogue.NET.Core.Logic.Interface;
using Rogue.NET.Core.Logic.Processing;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Extension;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
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

            //Run animations before applying effects
            if (spell.Animations.Count > 0)
            {
                AnimationUpdateEvent(this, new AnimationUpdate()
                {
                    Animations = spell.Animations,
                    SourceLocation = _modelService.Player.Location,
                    TargetLocations = _modelService.GetTargetedEnemies().Select(x => x.Location)
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

            // Queue animations
            if (spell.Animations.Count > 0)
            {
                AnimationUpdateEvent(this, new AnimationUpdate()
                {
                    Animations = spell.Animations,
                    SourceLocation = enemy.Location,
                    TargetLocations = new CellPoint[] { _modelService.Player.Location }
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
        public void ProcessPlayerMagicSpell(Spell spell)
        {
            var player = _modelService.Player;

            //Calculate alteration from spell's random parameters
            var alteration = _alterationGenerator.GenerateAlteration(spell);

            //Apply alteration cost (ONLY ONE-TIME APPLIED HERE. PER-STEP APPLIED IN CHARACTER ALTERATION)
            if (alteration.Cost.Type == AlterationCostType.OneTime)
                _alterationProcessor.ApplyOneTimeAlterationCost(_modelService.Player, alteration.Cost);

            // TBD - Show (Non)-Stackable Alteration not applied
            switch (alteration.Type)
            {
                case AlterationType.PassiveAura:
                case AlterationType.TemporarySource:
                case AlterationType.PassiveSource:
                    player.Alteration.ActivateAlteration(alteration, true);
                    break;
                case AlterationType.PermanentSource:
                    _alterationProcessor.ApplyPermanentEffect(_modelService.Player, alteration.Effect);
                    break;
                case AlterationType.Remedy:
                    _alterationProcessor.ApplyRemedy(_modelService.Player, alteration.Effect);
                    break;
                case AlterationType.PermanentTarget:
                case AlterationType.PermanentAllTargets:
                case AlterationType.TemporaryTarget:
                case AlterationType.TeleportAllTargets:
                case AlterationType.TemporaryAllTargets:
                    ProcessPlayerTargetAlteration(alteration);
                    break;
                case AlterationType.RunAway:
                    //Not supported for player
                    break;
                case AlterationType.Steal:
                    ProcessPlayerStealAlteration(alteration);
                    break;
                case AlterationType.TeleportSelf:
                    TeleportRandom(_modelService.Player);
                    break;
                case AlterationType.TeleportTarget:
                    TeleportRandom(_modelService.GetTargetedEnemies().First());
                    break;
                case AlterationType.OtherMagicEffect:
                    ProcessPlayerOtherMagicEffect(alteration);
                    break;
                case AlterationType.AttackAttribute:
                    ProcessPlayerAttackAttributeEffect(alteration);
                    break;
            }

            // Apply blanket update for player to ensure symbol alterations are processed
            QueueLevelUpdate(LevelUpdateType.PlayerLocation, _modelService.Player.Id);
            QueueLevelUpdate(LevelUpdateType.ContentUpdate, _modelService.GetTargetedEnemies().Select(x => x.Id).ToArray());
        }
        public void ProcessEnemyMagicSpell(Enemy enemy, Spell spell)
        {
            // Calculate alteration from spell's random parameters
            var alteration = _alterationGenerator.GenerateAlteration(spell);

            // Apply Alteration Cost
            if (alteration.Cost.Type == AlterationCostType.OneTime)
                _alterationProcessor.ApplyOneTimeAlterationCost(enemy, alteration.Cost);

            // Spell blocked by Player
            if (_interactionProcessor.CalculateSpellBlock(_modelService.Player))
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, _modelService.Player + " has blocked the spell!");
                return;
            }

            //TBD - Show Stackable Alterations (in Dialog) when NOT applied
            switch (alteration.Type)
            {
                case AlterationType.PassiveAura:
                    // Not supported for Enemy
                    break;
                case AlterationType.TemporarySource:
                case AlterationType.PassiveSource:
                    enemy.Alteration.ActivateAlteration(alteration, true);
                    break;
                case AlterationType.PermanentSource:
                    _alterationProcessor.ApplyPermanentEffect(enemy, alteration.Effect);
                    break;
                case AlterationType.Remedy:
                    _alterationProcessor.ApplyRemedy(enemy, alteration.Effect);
                    break;
                case AlterationType.TemporaryTarget:
                case AlterationType.TemporaryAllTargets:
                    _modelService.Player.Alteration.ActivateAlteration(alteration, false);
                    break;
                case AlterationType.PermanentTarget:
                case AlterationType.PermanentAllTargets:
                    _alterationProcessor.ApplyPermanentEffect(_modelService.Player, alteration.Effect);
                    break;
                case AlterationType.RunAway:
                    _modelService.Level.RemoveContent(enemy);
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, _modelService.GetDisplayName(enemy.RogueName) + " has run away!");

                    QueueLevelUpdate(LevelUpdateType.ContentRemove, enemy.Id);
                    break;
                case AlterationType.Steal:
                    ProcessEnemyStealAlteration(enemy, alteration);
                    break;
                case AlterationType.TeleportSelf:
                    TeleportRandom(enemy);
                    break;
                case AlterationType.TeleportAllTargets:
                case AlterationType.TeleportTarget:
                    TeleportRandom(_modelService.Player);
                    break;
                case AlterationType.OtherMagicEffect:
                    ProcessEnemyOtherMagicEffect(enemy, alteration);
                    break;
                case AlterationType.AttackAttribute:
                    ProcessEnemyAttackAttributeEffect(enemy, alteration);
                    break;
            }

            // Apply blanket update for player to ensure symbol alterations are processed
            QueueLevelUpdate(LevelUpdateType.PlayerLocation, _modelService.Player.Id);
            QueueLevelUpdate(LevelUpdateType.ContentUpdate, enemy.Id);

            if (_modelService.Player.Hp <= 0)
                _modelService.SetFinalEnemy(enemy);
        }
        private void ProcessPlayerTargetAlteration(AlterationContainer alteration)
        {
            // Targeting is handled separately - so all targets should be processed
            var targets = alteration.Type == AlterationType.PermanentAllTargets ||
                          alteration.Type == AlterationType.TemporaryAllTargets ?
                            _modelService.GetVisibleEnemies() :
                            _modelService.GetTargetedEnemies();

            foreach (var enemy in _modelService.GetTargetedEnemies())
            {
                bool blocked = _interactionProcessor.CalculateSpellBlock(enemy);
                if (blocked)
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, enemy.RogueName + " blocked the spell!");

                else
                {
                    if (alteration.Type == AlterationType.PermanentAllTargets ||
                        alteration.Type == AlterationType.PermanentTarget)
                        _alterationProcessor.ApplyPermanentEffect(enemy, alteration.Effect);

                    else if (alteration.Type == AlterationType.TeleportAllTargets)
                        TeleportRandom(enemy);
                    else
                        enemy.Alteration.ActivateAlteration(alteration, false);
                }

                // Engage enemy after applying alteration effects
                enemy.IsEngaged = true;
            }
        }
        private void ProcessPlayerStealAlteration(AlterationContainer alteration)
        {
            // Must require a target
            var enemy = _modelService.GetTargetedEnemies().First();
            var enemyInventory = enemy.Inventory;
            if (!enemyInventory.Any())
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, _modelService.GetDisplayName(enemy.RogueName) + " has nothing to steal");

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
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "You stole a(n) " + _modelService.GetDisplayName(itemStolen.Value.RogueName));
            }
        }
        private void ProcessPlayerOtherMagicEffect(AlterationContainer alteration)
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
                    CreateMonster(alteration.CreateMonsterEnemy);
                    break;
            }
        }
        private void ProcessPlayerAttackAttributeEffect(AlterationContainer alteration)
        {
            switch (alteration.AttackAttributeType)
            {
                case AlterationAttackAttributeType.ImbueArmor:
                    DialogUpdateEvent(this, new DialogUpdate() { Type = DialogEventType.ImbueArmor, ImbueAttackAttributes = alteration.Effect.AttackAttributes });
                    break;
                case AlterationAttackAttributeType.ImbueWeapon:
                    DialogUpdateEvent(this, new DialogUpdate() { Type = DialogEventType.ImbueWeapon, ImbueAttackAttributes = alteration.Effect.AttackAttributes });
                    break;
                case AlterationAttackAttributeType.Passive:
                case AlterationAttackAttributeType.TemporaryFriendlySource:
                case AlterationAttackAttributeType.TemporaryMalignSource:
                    _modelService.Player.Alteration.ActivateAlteration(alteration, true);
                    break;
                case AlterationAttackAttributeType.TemporaryMalignTarget:
                case AlterationAttackAttributeType.TemporaryFriendlyTarget:
                    foreach (var enemy in _modelService.GetTargetedEnemies())
                    {
                        enemy.Alteration.ActivateAlteration(alteration, false);

                        // Set Enemy Engaged
                        enemy.IsEngaged = true;
                    }
                    break;
                case AlterationAttackAttributeType.MeleeTarget:
                    foreach (var enemy in _modelService.GetTargetedEnemies())
                    {
                        foreach (var attribute in alteration.Effect.AttackAttributes)
                        {
                            var hit = _interactionProcessor.CalculateAttackAttributeMelee(enemy, attribute);
                            if (hit > 0)
                                enemy.Hp -= hit;
                        }

                        // Set Enemy Engaged
                        enemy.IsEngaged = true;
                    }
                    break;
            }
        }

        private void ProcessEnemyStealAlteration(Enemy enemy, AlterationContainer alteration)
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
            }
            else
            {
                _modelService.Player.Consumables.Remove(itemStolen.Key);
                enemy.Consumables.Add(itemStolen.Key, itemStolen.Value as Consumable);
            }

            var enemyDisplayName = _modelService.GetDisplayName(enemy.RogueName);
            var itemDisplayName = _modelService.GetDisplayName(itemStolen.Value.RogueName);

            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "The {0} stole your {1}!", enemyDisplayName, itemDisplayName);
        }
        private void ProcessEnemyOtherMagicEffect(Enemy enemy, AlterationContainer alteration)
        {
            switch (alteration.OtherEffectType)
            {
                case AlterationMagicEffectType.CreateMonster:
                    CreateMonsterMinion(enemy, alteration.CreateMonsterEnemy);
                    break;
            }
        }
        private void ProcessEnemyAttackAttributeEffect(Enemy enemy, AlterationContainer alteration)
        {
            switch (alteration.AttackAttributeType)
            {
                case AlterationAttackAttributeType.ImbueArmor:
                case AlterationAttackAttributeType.ImbueWeapon:
                    // Not supported for Enemy
                    break;
                case AlterationAttackAttributeType.Passive:
                case AlterationAttackAttributeType.TemporaryFriendlySource:
                case AlterationAttackAttributeType.TemporaryMalignSource:
                    enemy.Alteration.ActivateAlteration(alteration, true);
                    break;
                case AlterationAttackAttributeType.TemporaryMalignTarget:
                case AlterationAttackAttributeType.TemporaryFriendlyTarget:
                    _modelService.Player.Alteration.ActivateAlteration(alteration, false);
                    break;
                case AlterationAttackAttributeType.MeleeTarget:
                    foreach (var attackAttribute in alteration.Effect.AttackAttributes)
                    {
                        double hit = _interactionProcessor.CalculateAttackAttributeMelee(_modelService.Player, attackAttribute);
                        if (hit > 0)
                            _modelService.Player.Hp -= hit;
                    }
                    break;
            }
        }

        private void TeleportRandom(Character character)
        {
            character.Location = _modelService.Level.GetRandomLocation(true, _randomSequenceGenerator);

            if (character is Player)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "You were teleported!");

            else
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, _modelService.GetDisplayName(character.RogueName) + " was teleported!");

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

            _modelService.UpdateContents();
            _modelService.UpdateVisibleLocations();

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
                var minion = _characterGenerator.GenerateEnemy(enemyTemplate);

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

                var enemy = _characterGenerator.GenerateEnemy(enemyTemplate);
                enemy.Location = _modelService.Level.GetRandomLocation(true, _randomSequenceGenerator);

                _modelService.Level.AddContent(enemy);

                // Notify UI
                QueueLevelUpdate(LevelUpdateType.ContentAdd, enemy.Id);
            }
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
