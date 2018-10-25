using Rogue.NET.Core.Logic.Content.Interface;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Character;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Logic.Content
{
    [Export(typeof(IInteractionProcessor))]
    public class InteractionProcessor : IInteractionProcessor
    {
        readonly IRandomSequenceGenerator _randomSequenceGenerator;
        readonly IPlayerProcessor _playerProcessor;
        readonly ICharacterProcessor _characterProcessor;

        [ImportingConstructor]
        public InteractionProcessor(
            IRandomSequenceGenerator randomSequenceGenerator,
            IPlayerProcessor playerProcessor,
            ICharacterProcessor characterProcessor)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
            _playerProcessor = playerProcessor;
            _characterProcessor = characterProcessor;
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
            double baseAttk = _playerProcessor.GetAttack(player);
            double attk = _randomSequenceGenerator.Get() * (baseAttk - (enemy.StrengthBase / 5.0D));

            //Attack attributes (TODO)
            //foreach (var attackAttribute in p.MeleeAttackAttributes)
            //    attk += e.GetAttackAttributeMelee(attrib);

            return attk < 0 ? 0 : attk;
        }
        public double CalculateEnemyHit(Player player, Enemy enemy)
        {
            double baseDef = _playerProcessor.GetDefenseBase(player);
            double attk = _randomSequenceGenerator.Get() * (_characterProcessor.GetStrength(enemy) - baseDef);

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
            return physicalBlock ? _randomSequenceGenerator.Get() < _characterProcessor.GetDodge(character) :
                                   _randomSequenceGenerator.Get() < _characterProcessor.GetMagicBlock(character);
        }
    }
}
