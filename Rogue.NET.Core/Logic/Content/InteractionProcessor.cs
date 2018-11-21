using Rogue.NET.Core.Logic.Content.Interface;
using Rogue.NET.Core.Logic.Static;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Logic.Content
{
    [Export(typeof(IInteractionProcessor))]
    public class InteractionProcessor : IInteractionProcessor
    {
        readonly IRandomSequenceGenerator _randomSequenceGenerator;
        readonly IPlayerProcessor _playerProcessor;

        [ImportingConstructor]
        public InteractionProcessor(
            IRandomSequenceGenerator randomSequenceGenerator,
            IPlayerProcessor playerProcessor)
        {
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
            // TODO: Calculate this based on total speed
            return enemy.SpeedBase / player.SpeedBase;
        }
        public double CalculatePlayerHit(Player player, Enemy enemy)
        {
            // Start with standard melee - randomized
            double attack = _randomSequenceGenerator.Get() * (player.GetAttack() - enemy.GetDefense());

            //Attack attributes (need empty attributes for player calculation)
            var baseAttributes = enemy.AttackAttributes.Values.Select(x => new AttackAttribute()
            {
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

                attack += Calculator.CalculateAttackAttributeMelee(
                            playerAttribute.Attack, 
                            enemyAttribute.Resistance, 
                            enemyAttribute.Weakness);
            }
            return attack < 0 ? 0 : attack;
        }
        public double CalculateEnemyHit(Player player, Enemy enemy)
        {
            // Start with standard melee - randomized
            double attack = _randomSequenceGenerator.Get() * (enemy.GetAttack() - player.GetDefense());

            //Attack attributes (need empty attributes for player calculation)
            var baseAttributes = enemy.AttackAttributes.Values.Select(x => new AttackAttribute()
            {
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

                attack += Calculator.CalculateAttackAttributeMelee(
                            enemyAttribute.Attack,
                            playerAttribute.Resistance,
                            playerAttribute.Weakness);
            }
            return attack < 0 ? 0 : attack;
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
