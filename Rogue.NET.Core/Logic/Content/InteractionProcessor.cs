using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Logic.Content.Interface;
using Rogue.NET.Core.Logic.Static;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using Rogue.NET.Core.Model.ScenarioMessage;
using Rogue.NET.Core.Service.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Logic.Content
{
    [Export(typeof(IInteractionProcessor))]
    public class InteractionProcessor : IInteractionProcessor
    {
        readonly IScenarioMessageService _scenarioMessageService;
        readonly IRandomSequenceGenerator _randomSequenceGenerator;
        readonly IPlayerProcessor _playerProcessor;
        readonly IModelService _modelService;

        [ImportingConstructor]
        public InteractionProcessor(
            IModelService modelService,
            IScenarioMessageService scenarioMessageService,
            IRandomSequenceGenerator randomSequenceGenerator,
            IPlayerProcessor playerProcessor)
        {
            _modelService = modelService;
            _scenarioMessageService = scenarioMessageService;
            _randomSequenceGenerator = randomSequenceGenerator;
            _playerProcessor = playerProcessor;
        }

        public void CalculateAttackAttributeMelee(string alterationDisplayName, Enemy enemy, IEnumerable<AttackAttribute> offenseAttributes)
        {
            // Create detached attributes to send along the message publisher
            var attackAttributes = offenseAttributes.Select(x => x.DeepClone());

            // Apply the calculation, Filter the results, Create an Attack Attribute Dictionary
            var combatResults = attackAttributes.Select(x => new { AttackAttribute = x, CombatValue = CalculateAttackAttributeMelee(enemy, x) })
                                                .Where(x => x.CombatValue > 0)
                                                .ToDictionary(x => x.AttackAttribute, x => x.CombatValue);

            // Sum the total combat value
            var combatValue = combatResults.Sum(x => x.Value);

            // Apply the effect
            enemy.Hp -= combatValue;

            _scenarioMessageService.PublishAlterationMessage(ScenarioMessagePriority.Normal, "", "HP", combatValue, true, combatResults);
        }
        public void CalculateAttackAttributeMelee(string alterationDisplayName, Player player, IEnumerable<AttackAttribute> offenseAttributes)
        {
            // Create detached attributes to send along the message publisher
            var attackAttributes = offenseAttributes.Select(x => x.DeepClone());

            // Apply the calculation, Filter the results, Create an Attack Attribute Dictionary
            var combatResults = attackAttributes.Select(x => new { AttackAttribute = x, CombatValue = CalculateAttackAttributeMelee(player, x) })
                                                .Where(x => x.CombatValue > 0)
                                                .ToDictionary(x => x.AttackAttribute, x => x.CombatValue);

            // Sum the total combat value
            var combatValue = combatResults.Sum(x => x.Value);

            // Apply the effect
            player.Hp -= combatValue;

            _scenarioMessageService.PublishAlterationMessage(ScenarioMessagePriority.Bad, "", "HP", combatValue, true, combatResults);
        }

        public double CalculateAttackAttributeMelee(Enemy enemy, AttackAttribute offenseAttribute)
        {
            // Offense
            var attack = offenseAttribute.Attack;

            // Resistance (Enemy type only has inate Attack Attributes)
            var resistance = enemy.AttackAttributes[offenseAttribute.RogueName].Resistance;

            // Friendly attack attribute contributions
            foreach (AlterationEffect friendlyEffect in enemy.Alteration.GetTemporaryAttackAttributeAlterations(true))
                resistance += friendlyEffect.AttackAttributes.First(y => y.RogueName == offenseAttribute.RogueName).Resistance;

            // Passive attack attribute contributions
            foreach (AlterationEffect passiveEffect in enemy.Alteration.GetTemporaryAttackAttributeAlterations(true))
            {
                resistance += passiveEffect.AttackAttributes.First(y => y.RogueName == offenseAttribute.RogueName).Resistance;
            }

            // Equipment contributions
            foreach (var equipment in enemy.Equipment.Values.Where(z => z.IsEquipped))
            {
                resistance += equipment.AttackAttributes.First(y => y.RogueName == offenseAttribute.RogueName).Resistance;
            }

            return Calculator.CalculateAttackAttributeMelee(attack, resistance);
        }
        public double CalculateAttackAttributeMelee(Player player, AttackAttribute offenseAttribute)
        {
            // Offense
            var attack = offenseAttribute.Attack;
            var resistance = 0D;

            // Friendly attack attribute contributions
            foreach (AlterationEffect friendlyEffect in player.Alteration.GetTemporaryAttackAttributeAlterations(true))
                resistance += friendlyEffect.AttackAttributes.First(y => y.RogueName == offenseAttribute.RogueName).Resistance;

            // Passive attack attribute contributions
            foreach (AlterationEffect passiveEffect in player.Alteration.GetTemporaryAttackAttributeAlterations(true))
            {
                resistance += passiveEffect.AttackAttributes.First(y => y.RogueName == offenseAttribute.RogueName).Resistance;
            }

            // Equipment contributions
            foreach (var equipment in player.Equipment.Values.Where(z => z.IsEquipped))
            {
                resistance += equipment.AttackAttributes.First(y => y.RogueName == offenseAttribute.RogueName).Resistance;
            }

            return Calculator.CalculateAttackAttributeMelee(attack, resistance);
        }

        public double CalculateEnemyTurn(Player player, Enemy enemy)
        {
            // Check for divide by zero and apply min speed to return a guaranteed turn for the enemy.
            return player.GetSpeed() <= ModelConstants.MinSpeed ? 1.0D : enemy.GetSpeed() / player.GetSpeed();
        }

        public void CalculatePlayerMeleeHit(Player player, Enemy enemy)
        {
            // Start with standard melee - randomized
            var attack = Math.Max(_randomSequenceGenerator.Get() * (player.GetAttack() - enemy.GetDefense()), 0);
            var attackBase = attack;
            var dodge = _randomSequenceGenerator.Get() < enemy.GetDodge();
            var criticalHit = _randomSequenceGenerator.Get() <= player.GetCriticalHitProbability();
            var attackAttributeHitDict = new Dictionary<AttackAttribute, double>();

            //Attack attributes (need empty attributes for player calculation)
            var baseAttributes = enemy.AttackAttributes.Values.Select(x => CopyEmpty(x));

            var playerAttributes = player.GetMeleeAttributes(baseAttributes);
            var enemyAttributes = enemy.GetMeleeAttributes();

            // Calculate Melee
            foreach (var playerAttribute in playerAttributes)
            {
                // Enemy (Defense) Attribute
                var enemyAttribute = enemyAttributes.First(x => x.RogueName == playerAttribute.RogueName);

                var attackAttributeHit = Calculator.CalculateAttackAttributeMelee(
                                            playerAttribute.Attack, 
                                            enemyAttribute.Resistance);

                attack += attackAttributeHit;

                // Store attack attribute hits for broadcasting
                if (attackAttributeHit > 0)
                    attackAttributeHitDict.Add(playerAttribute, attackAttributeHit);
            }

            if (attack <= 0 || dodge)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, player.RogueName + " Misses");

            // Enemy hit
            else
            {
                //Critical hit
                if (criticalHit)
                    attack *= 2;

                enemy.Hp -= attack;

                _scenarioMessageService.PublishMeleeMessage(
                    ScenarioMessagePriority.Normal, 
                    player.RogueName, 
                    enemy.RogueName, 
                    attackBase, 
                    criticalHit, 
                    attackAttributeHitDict.Count > 0, 
                    attackAttributeHitDict);
            }

            // Enemy counter-attacks
            if (_randomSequenceGenerator.Get() < enemy.BehaviorDetails.CounterAttackProbability)
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, enemy.RogueName + " counter attacks!");

                // Calculate and publish counter attack
                CalculateEnemyHit(player, enemy);
            }
        }
        public bool CalculatePlayerRangeHit(Player player, Enemy targetedEnemy)
        {
            // Start with standard melee - randomized
            var attack = Math.Max(_randomSequenceGenerator.Get() * (player.GetAttack() - targetedEnemy.GetDefense()), 0);
            var attackBase = attack;
            var dodge = _randomSequenceGenerator.Get() < targetedEnemy.GetDodge();
            var criticalHit = _randomSequenceGenerator.Get() <= player.GetCriticalHitProbability();
            var attackAttributeHitDict = new Dictionary<AttackAttribute, double>();

            //Attack attributes (need empty attributes for player calculation)
            var baseAttributes = targetedEnemy.AttackAttributes.Values.Select(x => CopyEmpty(x));

            var playerAttributes = player.GetMeleeAttributes(baseAttributes);
            var enemyAttributes = targetedEnemy.GetMeleeAttributes();

            // Calculate Melee
            foreach (var playerAttribute in playerAttributes)
            {
                // Enemy (Defense) Attribute
                var enemyAttribute = enemyAttributes.First(x => x.RogueName == playerAttribute.RogueName);

                var attackAttributeHit = Calculator.CalculateAttackAttributeMelee(
                                            playerAttribute.Attack,
                                            enemyAttribute.Resistance);

                attack += attackAttributeHit;

                // Store attack attribute details for publishing
                if (attackAttributeHit > 0)
                    attackAttributeHitDict.Add(playerAttribute, attackAttributeHit);
            }

            if (attack <= 0 || dodge)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, player.RogueName + " Misses");

            // Player Hits Targeted Enemy
            else
            {
                if (criticalHit)
                    attack *= 2;

                targetedEnemy.Hp -= attack;

                _scenarioMessageService.PublishMeleeMessage(
                    ScenarioMessagePriority.Normal, 
                    player.RogueName, 
                    targetedEnemy.RogueName, 
                    attackBase, 
                    criticalHit, 
                    attackAttributeHitDict.Count > 0, 
                    attackAttributeHitDict);

                return true;
            }

            return false;
        }
        public void CalculateEnemyHit(Player player, Enemy enemy)
        {
            // Start with standard melee - randomized / calculate dodge / calculate critical hit
            var attackBase = Math.Max(_randomSequenceGenerator.Get() * (enemy.GetAttack() - player.GetDefense()), 0);
            var attack = attackBase;
            var dodge = _randomSequenceGenerator.Get() <= player.GetDodge();
            var criticalHit = _randomSequenceGenerator.Get() <= enemy.BehaviorDetails.CriticalRatio;

            // Store attack attribute interaction
            var attackAttributeHitDict = new Dictionary<AttackAttribute, double>();

            //Attack attributes (need empty attributes for player calculation)
            var baseAttributes = enemy.AttackAttributes.Values.Select(x => CopyEmpty(x));

            var playerAttributes = player.GetMeleeAttributes(baseAttributes);
            var enemyAttributes = enemy.GetMeleeAttributes();

            // Calculate Melee
            foreach (var playerAttribute in playerAttributes)
            {
                // Enemy (Defense) Attribute
                var enemyAttribute = enemyAttributes.First(x => x.RogueName == playerAttribute.RogueName);

                var attackAttributeHit = Calculator.CalculateAttackAttributeMelee(
                                            enemyAttribute.Attack,
                                            playerAttribute.Resistance);

                // If there is an effect, then store the attack attribute for use with the output message
                if (attackAttributeHit > 0)
                    attackAttributeHitDict.Add(playerAttribute, attackAttributeHit);

                attack += attackAttributeHit;
            }

            // Finally, calculate damage to player and broadcast message
            if (attack > 0 && !dodge)
            {
                if (criticalHit)
                    attack *= 2;

                player.Hp -= attack;

                if (player.Hp <= 0)
                    _modelService.SetKilledBy(enemy.RogueName);

                // Publish detailed melee message
                _scenarioMessageService.PublishMeleeMessage(
                    ScenarioMessagePriority.Normal, 
                    enemy.RogueName, 
                    player.RogueName, 
                    attackBase, 
                    criticalHit, 
                    attackAttributeHitDict.Count > 0, 
                    attackAttributeHitDict);
            }
            else
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, enemy.RogueName + " Misses");
        }
        public bool CalculateEnemyRangeHit(Player player, Enemy enemy)
        {
            // Start with standard melee - randomized
            var attack = Math.Max(_randomSequenceGenerator.Get() * (enemy.GetAttack() - player.GetDefense()), 0);
            var attackBase = attack;
            var dodge = _randomSequenceGenerator.Get() < enemy.GetDodge();
            var criticalHit = _randomSequenceGenerator.Get() <= enemy.GetCriticalHitProbability();

            // Store attack attribute interaction
            var attackAttributeHitDict = new Dictionary<AttackAttribute, double>();

            //Attack attributes (need empty attributes for player calculation)
            var baseAttributes = enemy.AttackAttributes.Values.Select(x => CopyEmpty(x));

            var playerAttributes = player.GetMeleeAttributes(baseAttributes);
            var enemyAttributes = enemy.GetMeleeAttributes();

            // Calculate Melee
            foreach (var playerAttribute in playerAttributes)
            {
                // Enemy (Defense) Attribute
                var enemyAttribute = enemyAttributes.First(x => x.RogueName == playerAttribute.RogueName);

                var attackAttributeHit = Calculator.CalculateAttackAttributeMelee(
                                            enemyAttribute.Attack,
                                            playerAttribute.Resistance);

                attack += attackAttributeHit;

                // Store attack attribute details for publishing
                if (attackAttributeHit > 0)
                    attackAttributeHitDict.Add(playerAttribute, attackAttributeHit);
            }

            if (attack <= 0 || dodge)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, _modelService.GetDisplayName(enemy.RogueName) + " Misses");

            // Player Hits Targeted Enemy
            else
            {
                if (criticalHit)
                    attack *= 2;

                player.Hp -= attack;

                _scenarioMessageService.PublishMeleeMessage(
                    ScenarioMessagePriority.Bad,
                    _modelService.GetDisplayName(enemy.RogueName),
                    player.RogueName,
                    attackBase,
                    criticalHit,
                    attackAttributeHitDict.Count > 0,
                    attackAttributeHitDict);

                return true;
            }

            return false;
        }

        public bool CalculateSpellBlock(Enemy enemy)
        {
            return _randomSequenceGenerator.Get() < enemy.GetMagicBlock();
        }
        public bool CalculateSpellBlock(Player player)
        {
            return _randomSequenceGenerator.Get() < player.GetMagicBlock();
        }

        private AttackAttribute CopyEmpty(AttackAttribute attackAttribute)
        {
            // Create a deep clone of the attack attribute to capture symbol details
            var result = attackAttribute.DeepClone();

            result.Attack = 0;
            result.Resistance = 0;

            return result;
        }
    }
}
