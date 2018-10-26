using Rogue.NET.Core.Logic.Content.Interface;
using Rogue.NET.Core.Logic.Event;
using Rogue.NET.Core.Logic.Interface;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Core.Service.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Game
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

        public event EventHandler<LevelChangeEventArgs> LevelChangeEvent;
        public event EventHandler<SplashEventType> SplashEvent;
        public event EventHandler<AnimationEventArgs> AnimationEvent;

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

        public LevelContinuationAction InvokePlayerMagicSpell(Spell spell)
        {
            // Cost will be applied on turn - after animations are processed
            if (!_alterationProcessor.CalculatePlayerMeetsAlterationCost(_modelService.Player, spell.Cost))
                return LevelContinuationAction.DoNothing;

            //Run animations before applying effects
            if (spell.Animations.Count > 0)
            {
                AnimationEvent(this, new AnimationEventArgs()
                {
                    Animations = spell.Animations,
                    SourceLocation = _modelService.Player.Location,
                    TargetLocations = _modelService.GetTargetedEnemies().Select(x => x.Location)
                });

                return LevelContinuationAction.DoNothing;
            }
            //No animations - just apply effects
            else
            {
                OnPlayerMagicSpell(spell);

                return LevelContinuationAction.ProcessTurn;
            }
        }
        //Used for item throw effects
        public LevelContinuationAction InvokeEnemyMagicSpell(Enemy enemy, Spell spell)
        {
            // TODO: MOVE THIS!  Cost will be applied on turn - after animations are processed
            if (!_alterationProcessor.CalculateEnemyMeetsAlterationCost(enemy, spell.Cost))
                return LevelContinuationAction.ProcessTurn;

            //Run animations before applying effects
            if (spell.Animations.Count > 0)
            {
                AnimationEvent(this, new AnimationEventArgs()
                {
                    Animations = spell.Animations,
                    SourceLocation = enemy.Location,
                    TargetLocations = new CellPoint[] { _modelService.Player.Location }
                });

                return LevelContinuationAction.DoNothing;
            }
            //No animations - just apply effects
            else
            {
                OnEnemyMagicSpell(enemy, spell);

                return LevelContinuationAction.ProcessTurn;
            }
        }
        private void OnPlayerMagicSpell(Spell spell)
        {
            var player = _modelService.Player;

            //Calculate alteration from spell's random parameters
            var alteration = _alterationGenerator.GenerateAlteration(spell);

            //Apply alteration cost
            _alterationProcessor.ApplyAlterationCost(_modelService.Player, alteration.Cost);

            // TODO: Apply stackable
            switch (alteration.Type)
            {
                case AlterationType.PassiveAura:
                    player.Alteration.ActiveAuras.Add(alteration.AuraEffect);
                    break;
                case AlterationType.TemporarySource:
                    player.Alteration.ActiveTemporaryEffects.Add(alteration.Effect);
                    break;
                case AlterationType.PassiveSource:
                    player.Alteration.ActivePassiveEffects.Add(alteration.Effect);
                    break;
                case AlterationType.PermanentSource:
                    _alterationProcessor.ApplyPermanentEffect(_modelService.Player, alteration.Effect);
                    break;
                case AlterationType.PermanentTarget:
                case AlterationType.TemporaryTarget:
                case AlterationType.TeleportAllTargets:
                case AlterationType.PermanentAllTargets:
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
        }
        private void OnEnemyMagicSpell(Enemy enemy, Spell spell)
        {
            // Calculate alteration from spell's random parameters
            var alteration = _alterationGenerator.GenerateAlteration(spell);

            // Spell blocked by Player
            if (_interactionProcessor.CalculateSpellBlock(_modelService.Player, alteration.BlockType == AlterationBlockType.Physical))
            {
                _scenarioMessageService.Publish(_modelService.Player + " has blocked the spell!");
                return;
            }

            //TODO - apply stackable
            switch (alteration.Type)
            {
                case AlterationType.PassiveAura:
                    // Not supported for Enemy
                    break;
                case AlterationType.TemporarySource:
                    enemy.Alteration.ActiveTemporaryEffects.Add(alteration.Effect);
                    break;
                case AlterationType.PassiveSource:
                    enemy.Alteration.ActivePassiveEffects.Add(alteration.Effect);
                    break;
                case AlterationType.PermanentSource:
                    _alterationProcessor.ApplyPermanentEffect(enemy, alteration.Effect);
                    break;
                case AlterationType.TemporaryTarget:
                case AlterationType.TemporaryAllTargets:
                    _modelService.Player.Alteration.ActiveTemporaryEffects.Add(alteration.Effect);
                    break;
                case AlterationType.PermanentTarget:
                case AlterationType.PermanentAllTargets:
                    _alterationProcessor.ApplyPermanentEffect(_modelService.Player, alteration.Effect);
                    break;
                case AlterationType.RunAway:
                    _modelService.CurrentLevel.RemoveContent(enemy);
                    _scenarioMessageService.Publish(_modelService.GetDisplayName(enemy.RogueName) + " has run away!");
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
                bool blocked = _interactionProcessor.CalculateSpellBlock(enemy, alteration.BlockType == AlterationBlockType.Physical);
                if (blocked)
                    _scenarioMessageService.Publish(enemy.RogueName + " blocked the spell!");

                else
                {
                    if (alteration.Type == AlterationType.PermanentAllTargets ||
                        alteration.Type == AlterationType.PermanentTarget)
                        _alterationProcessor.ApplyPermanentEffect(enemy, alteration.Effect);

                    else if (alteration.Type == AlterationType.TeleportAllTargets)
                        TeleportRandom(enemy);
                    else
                        enemy.Alteration.ActiveTemporaryEffects.Add(alteration.Effect);
                }
            }
        }
        private void ProcessPlayerStealAlteration(AlterationContainer alteration)
        {
            // Must require a target
            var enemy = _modelService.GetTargetedEnemies().First();
            var enemyInventory = enemy.Inventory;
            if (!enemyInventory.Any())
                _scenarioMessageService.Publish(_modelService.GetDisplayName(enemy.RogueName) + " has nothing to steal");

            else
            {
                var itemStolen = enemyInventory.ElementAt(_randomSequenceGenerator.Get(0, enemyInventory.Count()));
                if (itemStolen.Value is Equipment)
                {
                    _modelService.Player.Equipment.Add(itemStolen.Key, itemStolen.Value as Equipment);
                    enemy.Equipment.Remove(itemStolen.Key);
                }
                else
                {
                    _modelService.Player.Consumables.Add(itemStolen.Key, itemStolen.Value as Consumable);
                    enemy.Consumables.Remove(itemStolen.Key);
                }
                _scenarioMessageService.Publish("You stole a(n) " + _modelService.GetDisplayName(itemStolen.Value.RogueName));
            }
        }
        private void ProcessPlayerOtherMagicEffect(AlterationContainer alteration)
        {
            switch (alteration.OtherEffectType)
            {
                case AlterationMagicEffectType.ChangeLevelRandomDown:
                    {
                        var minLevel = _modelService.CurrentLevel.Number + 1;
                        var maxLevel = _modelService.CurrentLevel.Number + 10;
                        var numberOfLevels = _modelService.ScenarioConfiguration.DungeonTemplate.NumberOfLevels;
                        var randomLevel = _randomSequenceGenerator.Get(minLevel, maxLevel);
                        var level = Math.Min(randomLevel, numberOfLevels);

                        LevelChangeEvent(this, new LevelChangeEventArgs()
                        {
                            LevelNumber = level,
                            StartLocation = PlayerStartLocation.Random
                        });
                    }
                    break;
                case AlterationMagicEffectType.ChangeLevelRandomUp:
                    {
                        var minLevel = _modelService.CurrentLevel.Number - 10;
                        var maxLevel = _modelService.CurrentLevel.Number - 1;
                        var numberOfLevels = _modelService.ScenarioConfiguration.DungeonTemplate.NumberOfLevels;
                        var randomLevel = _randomSequenceGenerator.Get(minLevel, maxLevel);
                        var level = Math.Max(randomLevel, 1);

                        LevelChangeEvent(this, new LevelChangeEventArgs()
                        {
                            LevelNumber = level,
                            StartLocation = PlayerStartLocation.Random
                        });
                    }
                    break;
                case AlterationMagicEffectType.EnchantArmor:
                    SplashEvent(this, SplashEventType.EnchantArmor);
                    break;
                case AlterationMagicEffectType.EnchantWeapon:
                    SplashEvent(this, SplashEventType.EnchantWeapon);
                    break;
                case AlterationMagicEffectType.Identify:
                    SplashEvent(this, SplashEventType.Identify);
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
                    SplashEvent(this, SplashEventType.Uncurse);
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
                case AlterationAttackAttributeType.Imbue:
                    SplashEvent(this, SplashEventType.Imbue);
                    break;
                case AlterationAttackAttributeType.Passive:
                    _modelService.Player.Alteration.AttackAttributePassiveEffects.Add(alteration.Effect);
                    break;
                case AlterationAttackAttributeType.TemporaryFriendlySource:
                    _modelService.Player.Alteration.AttackAttributeTemporaryFriendlyEffects.Add(alteration.Effect);
                    break;
                case AlterationAttackAttributeType.TemporaryFriendlyTarget:
                    foreach (var enemy in _modelService.GetTargetedEnemies())
                        enemy.Alteration.AttackAttributeTemporaryFriendlyEffects.Add(alteration.Effect);
                    break;
                case AlterationAttackAttributeType.TemporaryMalignSource:
                    _modelService.Player.Alteration.AttackAttributeTemporaryMalignEffects.Add(alteration.Effect);
                    break;
                case AlterationAttackAttributeType.TemporaryMalignTarget:
                    foreach (var enemy in _modelService.GetTargetedEnemies())
                        enemy.Alteration.AttackAttributeTemporaryMalignEffects.Add(alteration.Effect);
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
                _modelService.Player.Equipment.Remove(itemStolen.Key);
                enemy.Equipment.Add(itemStolen.Key, itemStolen.Value as Equipment);
            }
            else
            {
                _modelService.Player.Consumables.Remove(itemStolen.Key);
                enemy.Consumables.Add(itemStolen.Key, itemStolen.Value as Consumable);
            }

            var enemyDisplayName = _modelService.GetDisplayName(enemy.RogueName);
            var itemDisplayName = _modelService.GetDisplayName(itemStolen.Value.RogueName);

            _scenarioMessageService.Publish("The {0} stole your {1}!", enemyDisplayName, itemDisplayName);
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
                case AlterationAttackAttributeType.Imbue:
                    // Not supported for Enemy
                    break;
                case AlterationAttackAttributeType.Passive:
                    enemy.Alteration.AttackAttributePassiveEffects.Add(alteration.Effect);
                    break;
                case AlterationAttackAttributeType.TemporaryFriendlySource:
                    enemy.Alteration.AttackAttributeTemporaryFriendlyEffects.Add(alteration.Effect);
                    break;
                case AlterationAttackAttributeType.TemporaryFriendlyTarget:
                    _modelService.Player.Alteration.AttackAttributeTemporaryFriendlyEffects.Add(alteration.Effect);
                    break;
                case AlterationAttackAttributeType.TemporaryMalignSource:
                    enemy.Alteration.AttackAttributeTemporaryMalignEffects.Add(alteration.Effect);
                    break;
                case AlterationAttackAttributeType.TemporaryMalignTarget:
                    _modelService.Player.Alteration.AttackAttributeTemporaryMalignEffects.Add(alteration.Effect);
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
            character.Location = _layoutEngine.GetRandomLocation(true);

            if (character is Player)
                _scenarioMessageService.Publish("You were teleported!");

            else
                _scenarioMessageService.Publish(_modelService.GetDisplayName(character.RogueName) + " was teleported!");
        }
        private void RevealSavePoint()
        {
            if (_modelService.CurrentLevel.HasSavePoint)
            {
                _modelService.CurrentLevel.SavePoint.IsRevealed = true;
            }
            _scenarioMessageService.Publish("You sense odd shrines near by...");
        }
        private void RevealMonsters()
        {
            foreach (var enemy in _modelService.CurrentLevel.Enemies)
                enemy.IsRevealed = true;

            _scenarioMessageService.Publish("You hear growling in the distance...");
        }
        private void RevealLevel()
        {
            foreach (var cell in _modelService.CurrentLevel.Grid.GetCells())
                cell.IsRevealed = true;

            RevealSavePoint();
            RevealStairs();
            RevealMonsters();
            RevealContent();

            _scenarioMessageService.Publish("Your senses are vastly awakened");
        }
        private void RevealStairs()
        {
            if (_modelService.CurrentLevel.HasStairsDown)
                _modelService.CurrentLevel.StairsDown.IsRevealed = true;

            if (_modelService.CurrentLevel.HasStairsUp)
                _modelService.CurrentLevel.StairsUp.IsRevealed = true;

            _scenarioMessageService.Publish("You sense exits nearby");
        }
        private void RevealItems()
        {
            foreach (var consumable in _modelService.CurrentLevel.Consumables)
                consumable.IsRevealed = true;

            foreach (var equipment in _modelService.CurrentLevel.Equipment)
                equipment.IsRevealed = true;

            _scenarioMessageService.Publish("You sense objects nearby");
        }
        private void RevealContent()
        {
            foreach (var scenarioObject in _modelService.CurrentLevel.GetContents())
            {
                scenarioObject.IsHidden = false;
                scenarioObject.IsRevealed = true;
            }

            _scenarioMessageService.Publish("You sense objects nearby");
        }
        private void RevealFood()
        {
            foreach (var consumable in _modelService.CurrentLevel.Consumables.Where(x => x.SubType == ConsumableSubType.Food))
                consumable.IsRevealed = true;

            _scenarioMessageService.Publish("Hunger makes a good sauce.....  :)");
        }
        private void CreateMonsterMinion(Enemy enemy, string monsterName)
        {
            var location = _layoutEngine.GetRandomAdjacentLocation(enemy.Location, true);
            if (location == CellPoint.Empty)
                return;

            var enemyTemplate = _modelService.ScenarioConfiguration.EnemyTemplates.FirstOrDefault(x => x.Name == monsterName);

            if (enemyTemplate != null)
            {
                var minion = _characterGenerator.GenerateEnemy(enemyTemplate);

                minion.Location = location;

                _modelService.CurrentLevel.AddContent(minion);
            }
        }
        private void CreateMonster(string monsterName)
        {
            var enemyTemplate = _modelService.ScenarioConfiguration.EnemyTemplates.FirstOrDefault(x => x.Name == monsterName);

            if (enemyTemplate != null)
            {
                _scenarioMessageService.Publish("You hear growling in the distance");

                var enemy = _characterGenerator.GenerateEnemy(enemyTemplate);
                enemy.Location = _layoutEngine.GetRandomLocation(true);

                _modelService.CurrentLevel.AddContent(enemy);
            }
        }
    }
}
