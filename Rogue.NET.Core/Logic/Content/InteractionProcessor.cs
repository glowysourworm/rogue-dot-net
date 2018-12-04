using Rogue.NET.Core.Logic.Content.Interface;
using Rogue.NET.Core.Logic.Static;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.ScenarioMessage;
using Rogue.NET.Core.Service.Interface;
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

        public double CalculateAttackAttributeMelee(Enemy enemy, AttackAttribute offenseAttribute)
        {
            // Offense
            var attack = offenseAttribute.Attack;

            // Resistance (Enemy type only has inate Attack Attributes)
            var resistance = enemy.AttackAttributes[offenseAttribute.RogueName].Resistance;

            // Weakness (Enemy type only has inate Attack Attributes)
            var weakness = enemy.AttackAttributes[offenseAttribute.RogueName].Weakness;

            // Friendly attack attribute contributions
            foreach (AlterationEffect friendlyEffect in enemy.Alteration.GetTemporaryAttackAttributeAlterations(true))
                resistance += friendlyEffect.AttackAttributes.First(y => y.RogueName == offenseAttribute.RogueName).Resistance;

            // Passive attack attribute contributions
            foreach (AlterationEffect passiveEffect in enemy.Alteration.GetTemporaryAttackAttributeAlterations(true))
            {
                resistance += passiveEffect.AttackAttributes.First(y => y.RogueName == offenseAttribute.RogueName).Resistance;
                weakness += passiveEffect.AttackAttributes.First(y => y.RogueName == offenseAttribute.RogueName).Weakness;
            }

            // Equipment contributions
            foreach (var equipment in enemy.Equipment.Values.Where(z => z.IsEquipped))
            {
                resistance += equipment.AttackAttributes.First(y => y.RogueName == offenseAttribute.RogueName).Resistance;
                weakness += equipment.AttackAttributes.First(y => y.RogueName == offenseAttribute.RogueName).Weakness;
            }

            return Calculator.CalculateAttackAttributeMelee(attack, resistance, weakness);
        }
        public double CalculateAttackAttributeMelee(Player player, AttackAttribute offenseAttribute)
        {
            // Offense
            var attack = offenseAttribute.Attack;
            var resistance = 0D;
            var weakness = 0;

            // Friendly attack attribute contributions
            foreach (AlterationEffect friendlyEffect in player.Alteration.GetTemporaryAttackAttributeAlterations(true))
                resistance += friendlyEffect.AttackAttributes.First(y => y.RogueName == offenseAttribute.RogueName).Resistance;

            // Passive attack attribute contributions
            foreach (AlterationEffect passiveEffect in player.Alteration.GetTemporaryAttackAttributeAlterations(true))
            {
                resistance += passiveEffect.AttackAttributes.First(y => y.RogueName == offenseAttribute.RogueName).Resistance;
                weakness += passiveEffect.AttackAttributes.First(y => y.RogueName == offenseAttribute.RogueName).Weakness;
            }

            // Equipment contributions
            foreach (var equipment in player.Equipment.Values.Where(z => z.IsEquipped))
            {
                resistance += equipment.AttackAttributes.First(y => y.RogueName == offenseAttribute.RogueName).Resistance;
                weakness += equipment.AttackAttributes.First(y => y.RogueName == offenseAttribute.RogueName).Weakness;
            }

            return Calculator.CalculateAttackAttributeMelee(attack, resistance, weakness);
        }

        public double CalculateEnemyTurn(Player player, Enemy enemy)
        {
            return enemy.GetSpeed() / player.GetSpeed();
        }
        public void CalculatePlayerMeleeHit(Player player, Enemy enemy)
        {
            // Start with standard melee - randomized
            var attack = _randomSequenceGenerator.Get() * (player.GetAttack() - enemy.GetDefense());
            var attackBase = attack;
            var dodge = _randomSequenceGenerator.Get() < enemy.GetDodge();
            var criticalHit = _randomSequenceGenerator.Get() <= player.GetCriticalHitProbability();
            var attackAttributeHitDict = new Dictionary<AttackAttribute, double>();

            //Attack attributes (need empty attributes for player calculation)
            var baseAttributes = enemy.AttackAttributes.Values.Select(x => new AttackAttribute()
            {
                CharacterColor = x.CharacterColor,
                CharacterSymbol = x.CharacterSymbol,
                Icon = x.Icon,
                SmileyAuraColor = x.SmileyAuraColor,
                SmileyBodyColor = x.SmileyBodyColor,
                SmileyLineColor = x.SmileyLineColor,
                SmileyMood = x.SmileyMood,
                SymbolType = x.SymbolType,
                RogueName = x.RogueName,
                Attack = 0,
                Resistance = 0, 
                Weakness = 0
            });

            var playerAttributes = player.GetMeleeAttributes(baseAttributes);
            var enemyAttributes = enemy.GetMeleeAttributes();

            // Calculate Melee
            foreach (var playerAttribute in playerAttributes)
            {
                // Enemy (Defense) Attribute
                var enemyAttribute = enemyAttributes.First(x => x.RogueName == playerAttribute.RogueName);

                var attackAttributeHit = Calculator.CalculateAttackAttributeMelee(
                                            playerAttribute.Attack, 
                                            enemyAttribute.Resistance, 
                                            enemyAttribute.Weakness);

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
                _scenarioMessageService.Publish(ScenarioMessagePriority.Bad, enemy.RogueName + " counter attacks");

                // Calculate and publish counter attack
                CalculateEnemyHit(player, enemy);
            }
        }
        public bool CalculatePlayerRangeHit(Player player, Enemy targetedEnemy)
        {
            // Start with standard melee - randomized
            var attack = _randomSequenceGenerator.Get() * (player.GetAttack() - targetedEnemy.GetDefense());
            var attackBase = attack;
            var dodge = _randomSequenceGenerator.Get() < targetedEnemy.GetDodge();
            var criticalHit = _randomSequenceGenerator.Get() <= player.GetCriticalHitProbability();
            var attackAttributeHitDict = new Dictionary<AttackAttribute, double>();

            //Attack attributes (need empty attributes for player calculation)
            var baseAttributes = targetedEnemy.AttackAttributes.Values.Select(x => new AttackAttribute()
            {
                CharacterColor = x.CharacterColor,
                CharacterSymbol = x.CharacterSymbol,
                Icon = x.Icon,
                SmileyAuraColor = x.SmileyAuraColor,
                SmileyBodyColor = x.SmileyBodyColor,
                SmileyLineColor = x.SmileyLineColor,
                SmileyMood = x.SmileyMood,
                SymbolType = x.SymbolType,
                RogueName = x.RogueName,
                Attack = 0,
                Resistance = 0,
                Weakness = 0
            });

            var playerAttributes = player.GetMeleeAttributes(baseAttributes);
            var enemyAttributes = targetedEnemy.GetMeleeAttributes();

            // Calculate Melee
            foreach (var playerAttribute in playerAttributes)
            {
                // Enemy (Defense) Attribute
                var enemyAttribute = enemyAttributes.First(x => x.RogueName == playerAttribute.RogueName);

                var attackAttributeHit = Calculator.CalculateAttackAttributeMelee(
                                            playerAttribute.Attack,
                                            enemyAttribute.Resistance,
                                            enemyAttribute.Weakness);

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
            var attackBase = _randomSequenceGenerator.Get() * (enemy.GetAttack() - player.GetDefense());
            var attack = attackBase;
            var dodge = _randomSequenceGenerator.Get() <= player.GetDodge();
            var criticalHit = _randomSequenceGenerator.Get() <= enemy.BehaviorDetails.CriticalRatio;

            // Store attack attribute interaction
            var attackAttributeHitDict = new Dictionary<AttackAttribute, double>();

            //Attack attributes (need empty attributes for player calculation)
            var baseAttributes = enemy.AttackAttributes.Values.Select(x => new AttackAttribute()
            {
                CharacterColor = x.CharacterColor,
                CharacterSymbol = x.CharacterSymbol,
                Icon = x.Icon,
                SmileyAuraColor = x.SmileyAuraColor,
                SmileyBodyColor = x.SmileyBodyColor,
                SmileyLineColor = x.SmileyLineColor,
                SmileyMood = x.SmileyMood,
                SymbolType = x.SymbolType,
                RogueName = x.RogueName,
                Attack = 0,
                Resistance = 0,
                Weakness = 0
            });

            var playerAttributes = player.GetMeleeAttributes(baseAttributes);
            var enemyAttributes = enemy.GetMeleeAttributes();

            // Calculate Melee
            foreach (var playerAttribute in playerAttributes)
            {
                // Enemy (Defense) Attribute
                var enemyAttribute = enemyAttributes.First(x => x.RogueName == playerAttribute.RogueName);

                var attackAttributeHit = Calculator.CalculateAttackAttributeMelee(
                                            enemyAttribute.Attack,
                                            playerAttribute.Resistance,
                                            playerAttribute.Weakness);

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
                    _modelService.SetFinalEnemy(enemy);

                // Publish detailed melee message
                _scenarioMessageService.PublishMeleeMessage(
                    ScenarioMessagePriority.Bad, 
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
        public bool CalculateSpellBlock(Enemy enemy, bool physicalBlock)
        {
            return physicalBlock ? _randomSequenceGenerator.Get() < enemy.GetDodge() :
                                   _randomSequenceGenerator.Get() < enemy.GetMagicBlock();
        }
        public bool CalculateSpellBlock(Player player, bool physicalBlock)
        {
            return physicalBlock ? _randomSequenceGenerator.Get() < player.GetDodge() :
                                   _randomSequenceGenerator.Get() < player.GetMagicBlock();
        }
    }
}
