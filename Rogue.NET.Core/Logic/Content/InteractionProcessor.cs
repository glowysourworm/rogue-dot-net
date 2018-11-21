using Rogue.NET.Core.Logic.Content.Interface;
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

        public double CalculateAttackAttributeMelee(Character character, AttackAttribute offenseAttribute)
        {
            // Offense
            var attack = offenseAttribute.Attack;

            // Resistance (Enemy type only has inate Attack Attributes)
            var resistance = character is Enemy ? (character as Enemy).AttackAttributes[offenseAttribute.RogueName].Resistance
                                                : 0;

            // Weakness (Enemy type only has inate Attack Attributes)
            var weakness = character is Enemy ? (character as Enemy).AttackAttributes[offenseAttribute.RogueName].Weakness
                                                : 0;

            //Friendly attack attribute contributions - TODO
            //foreach (AlterationEffect friendlyEffect in this.AttackAttributeTemporaryFriendlyEffects)
            //    resistance += friendlyEffect.AttackAttributes.First(y => y.RogueName == attrib.RogueName).Resistance;

            //Equipment contributions
            foreach (var equipment in character.Equipment.Values.Where(z => z.IsEquipped))
            {
                resistance += equipment.AttackAttributes.First(y => y.RogueName == offenseAttribute.RogueName).Resistance;
                weakness += equipment.AttackAttributes.First(y => y.RogueName == offenseAttribute.RogueName).Weakness;
            }

            return CalculateAttackAttributeMelee(attack, resistance, weakness);
        }
        public double CalculateEnemyTurn(Player player, Enemy enemy)
        {
            // TODO: Calculate this based on total speed
            return enemy.SpeedBase / player.SpeedBase;
        }
        public double CalculatePlayerHit(Player player, Enemy enemy)
        {
            double baseAttk = player.GetAttack();
            double attk = _randomSequenceGenerator.Get() * (baseAttk - (enemy.StrengthBase / 5.0D));

            //Attack attributes (TODO)
            //foreach (var attackAttribute in p.MeleeAttackAttributes)
            //    attk += e.GetAttackAttributeMelee(attrib);

            return attk < 0 ? 0 : attk;
        }
        public double CalculateEnemyHit(Player player, Enemy enemy)
        {
            double baseDef = player.GetDefenseBase();
            double attk = _randomSequenceGenerator.Get() * (enemy.GetStrength() - baseDef);

            //Attack attributes (TODO)
            //foreach (AttackAttribute attrib in e.MeleeAttackAttributes)
            //    attk += p.GetAttackAttributeMelee(attrib);

            return attk < 0 ? 0 : attk;
        }
        public double CalculateAttackAttributeMelee(double attack, double resistance, double weakness)
        {
            return attack > 0 ? attack * (1 - (resistance / (attack + resistance)) + weakness) : 0;
        }
        public bool CalculateSpellBlock(Character character, bool physicalBlock)
        {
            return physicalBlock ? _randomSequenceGenerator.Get() < character.GetDodge() :
                                   _randomSequenceGenerator.Get() < character.GetMagicBlock();
        }
    }
}
