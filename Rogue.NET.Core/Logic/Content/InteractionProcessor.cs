﻿using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Logic.Content.Enum;
using Rogue.NET.Core.Logic.Content.Interface;
using Rogue.NET.Core.Logic.Static;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
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

        public void CalculateAttackAttributeHit(string alterationDisplayName, Character defender, IEnumerable<AttackAttribute> offenseAttributes)
        {
            // Create detached attributes to send along the message publisher
            var baseAttributes = _modelService.AttackAttributes;

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

            // Attack attributes
            var baseAttributes = _modelService.AttackAttributes;
            var attackerAttributes = attacker.GetMeleeAttributes(baseAttributes);
            var defenderAttributes = defender.GetMeleeAttributes(baseAttributes);

            // Calculate Attack Attribute Melee
            var specializedHits = CreateAttackAttributeResults(attackerAttributes, defenderAttributes);

            // Add Results to attack
            attack += specializedHits.Sum(x => x.Value);

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
                    specializedHits.Count > 0,
                    specializedHits);

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
                    return _randomSequenceGenerator.Get() < defender.GetMentalBlock().Clip();
                case AlterationBlockType.Physical:
                    return _randomSequenceGenerator.Get() < defender.GetDodge().Clip();
                case AlterationBlockType.NonBlockable:
                    return false;
                default:
                    throw new Exception("Unknwon Alteration Block Type");
            }
        }

        public bool GetAnimationRequiresTarget(IEnumerable<AnimationTemplate> animations)
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

        private IDictionary<ScenarioImage, double> CreateAttackAttributeResults(
                IEnumerable<AttackAttribute> offensiveAttributes,
                IEnumerable<AttackAttribute> defensiveAttributes)
        {
            return offensiveAttributes.Select(offensiveAttribute =>
            {
                var defensiveAttribute = defensiveAttributes.First(x => x.RogueName == offensiveAttribute.RogueName);

                return new
                {
                    Value = Calculator.CalculateAttackAttributeMelee(offensiveAttribute.Attack, defensiveAttribute.Resistance, defensiveAttribute.Weakness),
                    AttackAttribute = offensiveAttribute as ScenarioImage
                };
            })
            .Where(x => x.Value > 0)
            .ToDictionary(x => x.AttackAttribute, x => x.Value);
        }

    }
}
