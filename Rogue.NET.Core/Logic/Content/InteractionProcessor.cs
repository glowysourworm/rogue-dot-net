using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Logic.Content.Enum;
using Rogue.NET.Core.Logic.Content.Interface;
using Rogue.NET.Core.Logic.Static;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
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

        public void CalculateAttackAttributeHit(string alterationDisplayName, Character attacker, Character defender, IEnumerable<AttackAttribute> offenseAttributes)
        {
            // Create detached attributes to send along the message publisher
            var baseAttributes = _modelService.GetAttackAttributes();

            // Get the defender's attributes
            var defenseAttributes = defender.GetMeleeAttributes(baseAttributes);

            // Apply the calculation, Filter the results, Create an Attack Attribute Dictionary
            var combatResults = CreateAttackAttributeResults(offenseAttributes, defenseAttributes);

            // Sum the total combat value
            var combatValue = combatResults.Sum(x => x.Value);

            // Get message priority
            var messagePriotity = defender is Enemy ? ScenarioMessagePriority.Normal : ScenarioMessagePriority.Bad;

            // Apply the combat value
            if (combatValue > 0)
            {
                // Apply the effect
                defender.Hp -= combatValue;

                _scenarioMessageService.PublishAlterationMessage(messagePriotity, alterationDisplayName, "HP", combatValue, true, combatResults);
            }
            else
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, alterationDisplayName + " misses");
        }

        public double CalculateEnemyTurnIncrement(Player player, Enemy enemy)
        {
            // Check for divide by zero and apply min speed to return a guaranteed turn for the enemy.
            return player.GetSpeed() <= ModelConstants.MinSpeed ? 1.0D : enemy.GetSpeed() / player.GetSpeed();
        }

        public bool CalculateInteraction(Character attacker, Character defender, PhysicalAttackType attackType)
        {
            // Result implies that an attack was made
            var result = false;

            // Start with standard melee - randomized
            var attack = Math.Max(_randomSequenceGenerator.Get() * (attacker.GetAttack() - defender.GetDefense()), 0);
            var attackBase = attack;

            // Calculate dodge
            var dodgeProbability = Calculator.CalculateDodgeProbability(defender.GetDodge(), defender.GetAgility(), attacker.GetAgility());
            var dodge = _randomSequenceGenerator.Get() < dodgeProbability;

            // Calculate critical hit
            var criticalHit = _randomSequenceGenerator.Get() <= attacker.GetCriticalHitProbability();

            //Attack attributes
            var baseAttributes = _modelService.GetAttackAttributes();
            var attackerAttributes = attacker.GetMeleeAttributes(baseAttributes);
            var defenderAttributes = defender.GetMeleeAttributes(baseAttributes);

            // Calculate Attack Attribute Melee
            var attackAttributeResults = CreateAttackAttributeResults(attackerAttributes, defenderAttributes);

            // Add Results to attack
            attack += attackAttributeResults.Sum(x => x.Value);

            if (attack <= 0 || dodge)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, attacker.RogueName + " Misses");

            // Attacker Hits
            else
            {
                //Critical hit
                if (criticalHit)
                    attack *= 2;

                defender.Hp -= attack;

                _scenarioMessageService.PublishMeleeMessage(
                    (attacker is Enemy) ? ScenarioMessagePriority.Bad : ScenarioMessagePriority.Normal, 
                    _modelService.GetDisplayName(attacker), 
                    _modelService.GetDisplayName(defender), 
                    attackBase, 
                    criticalHit,
                    attackAttributeResults.Count > 0,
                    attackAttributeResults);

                result = true;
            }

            // Check for Enemy Counter-attack
            var enemy = defender as Enemy;

            if (enemy != null && attackType == PhysicalAttackType.Melee)
            {
                if (_randomSequenceGenerator.Get() < enemy.BehaviorDetails.CounterAttackProbability)
                {
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, enemy.RogueName + " counter attacks!");

                    // Calculate and publish counter attack
                    CalculateInteraction(defender, attacker, attackType);
                }
            }

            return result;
        }

        public bool CalculateAlterationBlock(Character attacker, Character defender, AlterationBlockType blockType)
        {
            switch (blockType)
            {
                case AlterationBlockType.Mental:
                    return _randomSequenceGenerator.Get() < defender.GetMagicBlock();
                case AlterationBlockType.Physical:
                    return _randomSequenceGenerator.Get() < defender.GetDodge();
                default:
                    throw new Exception("Unknwon Alteration Block Type");
            }
        }

        private IDictionary<AttackAttribute, double> CreateAttackAttributeResults(
                IEnumerable<AttackAttribute> offensiveAttributes,
                IEnumerable<AttackAttribute> defensiveAttributes)
        {
            return offensiveAttributes.Select(offensiveAttribute =>
            {
                var defensiveAttribute = defensiveAttributes.First(x => x.RogueName == offensiveAttribute.RogueName);

                return new
                {
                    Value = Calculator.CalculateAttackAttributeMelee(offensiveAttribute.Attack, defensiveAttribute.Resistance),
                    AttackAttribute = offensiveAttribute
                };
            })
            .Where(x => x.Value > 0)
            .ToDictionary(x => x.AttackAttribute, x => x.Value);
        }
    }
}
