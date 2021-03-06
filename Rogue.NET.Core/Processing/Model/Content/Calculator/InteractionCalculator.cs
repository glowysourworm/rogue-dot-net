﻿using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Processing.Event.Backend.EventData.ScenarioMessage.Enum;
using Rogue.NET.Core.Processing.Model.Content.Calculator.Interface;
using Rogue.NET.Core.Processing.Model.Content.Enum;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Model.Static;
using Rogue.NET.Core.Processing.Service.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Content.Calculator
{
    [Export(typeof(IInteractionCalculator))]
    public class InteractionSubProcessor : IInteractionCalculator
    {
        readonly IScenarioMessageService _scenarioMessageService;
        readonly IRandomSequenceGenerator _randomSequenceGenerator;
        readonly IModelService _modelService;

        [ImportingConstructor]
        public InteractionSubProcessor(
            IModelService modelService,
            IScenarioMessageService scenarioMessageService,
            IRandomSequenceGenerator randomSequenceGenerator)
        {
            _modelService = modelService;
            _scenarioMessageService = scenarioMessageService;
            _randomSequenceGenerator = randomSequenceGenerator;
        }

        public void CalculateAttackAttributeHit(string alterationDisplayName, CharacterBase defender, IEnumerable<AttackAttribute> offenseAttributes)
        {
            // Get the defender's attributes
            var defenseAttributes = defender.GetMeleeAttributes();

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
                // Subtract attack from defender stamina -> then Hp
                var appliedToHp = (combatValue - defender.Stamina).LowLimit(0);

                defender.Stamina -= combatValue;
                defender.Health -= appliedToHp;

                _scenarioMessageService.PublishAlterationMessage(messagePriotity, alterationDisplayName, "HP", combatValue, true, combatResults);
            }
            else
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, alterationDisplayName + " misses");
        }

        public double CalculateCharacterTurnIncrement(Player player, CharacterBase character)
        {
            // Check for divide by zero and apply min speed to return a guaranteed turn for the enemy.
            return player.GetSpeed() <= ModelConstants.MinSpeed ? 1.0D : character.GetSpeed() / player.GetSpeed();
        }

        public bool CalculateInteraction(CharacterBase attacker, CharacterBase defender, PhysicalAttackType attackType)
        {
            // Result implies that an attack was made
            var result = false;

            // Start with standard melee - randomized
            var attack = System.Math.Max(_randomSequenceGenerator.Get() * (attacker.GetAttack() - defender.GetDefense()), 0);
            var attackBase = attack;

            // Calculate dodge
            var dodge = CalculateDodge(attacker, defender);

            // TODO:STATS
            // Calculate critical hit
            // var criticalHit = _randomSequenceGenerator.Get() <= attacker.GetCriticalHitProbability();

            // Attack attributes
            var attackerAttributes = attacker.GetMeleeAttributes();
            var defenderAttributes = defender.GetMeleeAttributes();

            // Calculate Attack Attribute Melee
            var specializedHits = CreateAttackAttributeResults(attackerAttributes, defenderAttributes);

            // Add Results to attack
            attack += specializedHits.Sum(x => x.Value);

            if (attack <= 0 || dodge)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, attacker.RogueName + " Misses");

            // Attacker Hits
            else
            {
                // TODO:STATS
                //Critical hit
                // if (criticalHit)
                //    attack *= 2;

                // Subtract attack from defender stamina -> then Hp
                var appliedToHp = (attack - defender.Stamina).LowLimit(0);

                defender.Stamina -= attack;
                defender.Health -= appliedToHp;

                _scenarioMessageService.PublishMeleeMessage(
                    (attacker is Enemy) ? ScenarioMessagePriority.Bad : ScenarioMessagePriority.Normal,
                    _modelService.GetDisplayName(attacker),
                    _modelService.GetDisplayName(defender),
                    attackBase,
                    //criticalHit,
                    false,
                    specializedHits.Count > 0,
                    specializedHits);

                result = true;
            }

            return result;
        }

        public bool CalculateEquipmentThrow(CharacterBase attacker, CharacterBase defender, Equipment thrownItem)
        {
            // Result implies that the item hit the defender
            var result = false;

            // Start with standard melee - randomized
            var attack = System.Math.Max(_randomSequenceGenerator.Get() * (attacker.GetThrowAttack(thrownItem) - defender.GetDefense()), 0);
            var attackBase = attack;

            // Calculate dodge
            var dodge = CalculateDodge(attacker, defender);

            // Attack attributes - take from the Equipment's attack attributes
            var attackerAttributes = thrownItem.AttackAttributes;
            var defenderAttributes = defender.GetMeleeAttributes();

            // Calculate Attack Attribute Melee
            var specializedHits = CreateAttackAttributeResults(attackerAttributes, defenderAttributes);

            // Add Results to attack
            attack += specializedHits.Sum(x => x.Value);

            if (attack <= 0 || dodge)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, attacker.RogueName + " Misses");

            // Attacker Hits
            else
            {
                // Subtract attack from defender stamina -> then Hp
                var appliedToHp = (attack - defender.Stamina).LowLimit(0);

                defender.Stamina -= attack;
                defender.Health -= appliedToHp;

                _scenarioMessageService.PublishMeleeMessage(
                    (attacker is Enemy) ? ScenarioMessagePriority.Bad : ScenarioMessagePriority.Normal,
                    _modelService.GetDisplayName(attacker),
                    _modelService.GetDisplayName(defender),
                    attackBase,
                    false,
                    specializedHits.Count > 0,
                    specializedHits);

                result = true;
            }

            return result;
        }

        public bool CalculateDodge(CharacterBase attacker, CharacterBase defender)
        {
            // Calculate dodge
            var dodgeProbability = RogueCalculator.CalculateAttributeProbability(defender.GetAgility(), attacker.GetAgility());

            // Return random draw
            return _randomSequenceGenerator.Get() < dodgeProbability;
        }

        public bool CalculateAlterationBlock(CharacterBase attacker, CharacterBase defender, AlterationBlockType blockType)
        {
            switch (blockType)
            {
                case AlterationBlockType.Mental:
                    return _randomSequenceGenerator.Get() < RogueCalculator.CalculateAttributeProbability(defender.GetIntelligence(), attacker.GetIntelligence());
                case AlterationBlockType.Physical:
                    return _randomSequenceGenerator.Get() < RogueCalculator.CalculateAttributeProbability(defender.GetAgility(), attacker.GetAgility());
                case AlterationBlockType.NonBlockable:
                    return false;
                default:
                    throw new Exception("Unknwon Alteration Block Type");
            }
        }

        private SimpleDictionary<ScenarioImage, double> CreateAttackAttributeResults(
                                    IEnumerable<AttackAttribute> offensiveAttributes,
                                    IEnumerable<AttackAttribute> defensiveAttributes)
        {
            return offensiveAttributes.Select(offensiveAttribute =>
            {
                // Get Defensive Attribute
                var defensiveAttribute = defensiveAttributes.FirstOrDefault(x => x.RogueName == offensiveAttribute.RogueName);

                // Matching attribute found
                if (defensiveAttribute != null)
                {
                    return new
                    {
                        Value = RogueCalculator.CalculateAttackAttributeMelee(offensiveAttribute.Attack,
                                                                         defensiveAttribute.Resistance,
                                                                         defensiveAttribute.Weakness,
                                                                         defensiveAttribute.Immune),
                        AttackAttribute = offensiveAttribute as ScenarioImage
                    };
                }

                // No matching attribute - fill in zeros
                else
                {
                    return new
                    {
                        Value = RogueCalculator.CalculateAttackAttributeMelee(offensiveAttribute.Attack,
                                                                         0.0D,
                                                                         0,
                                                                         false),
                        AttackAttribute = offensiveAttribute as ScenarioImage
                    };
                }
            })
            .Where(x => x.Value > 0)
            .ToSimpleDictionary(x => x.AttackAttribute, x => x.Value);
        }

    }
}
