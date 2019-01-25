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
using Rogue.NET.Core.Model.Scenario.Content;
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

            // Attack attributes
            var baseAttributes = _modelService.GetAttackAttributes();
            var attackerAttributes = attacker.GetMeleeAttributes(baseAttributes);
            var defenderAttributes = defender.GetMeleeAttributes(baseAttributes);

            // Calculate Attack Attribute Melee
            var specializedHits = CreateAttackAttributeResults(attackerAttributes, defenderAttributes);

            // Add Results to attack
            attack += specializedHits.Sum(x => x.Value);

            // Calculate Religion Interaction
            if (attacker.ReligiousAlteration.IsAffiliated() &&
                defender.ReligiousAlteration.IsAffiliated())
            {
                var attackerReligion = attacker.ReligiousAlteration.ReligionName;
                var defenderReligion = defender.ReligiousAlteration.ReligionName;

                var attackerParameters = attacker.ReligiousAlteration.GetParameters(defenderReligion);
                var defenderParameters = defender.ReligiousAlteration.GetParameters(attackerReligion);

                // Attack = I_A * M_A * A_A - I_D * M_D * D_D
                var religiousAttack = attacker.GetIntelligence() *
                                      attackerParameters.AttackMultiplier *
                                      attacker.ReligiousAlteration.Affiliation;

                var religiousDefense = defender.GetIntelligence() *
                                       defenderParameters.DefenseMultiplier *
                                       defender.ReligiousAlteration.Affiliation;

                var hit = Math.Max(0, religiousAttack - religiousDefense);

                if (hit > 0)
                {
                    attack += hit;
                    specializedHits.Add(attacker.ReligiousAlteration.Symbol, hit);
                }
            }

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
            var religionComponent = 0D;
            
            if (attacker.ReligiousAlteration.IsAffiliated() &&
                defender.ReligiousAlteration.IsAffiliated())
            {
                // Get Defender Parameters to calculate the block probability
                var parameters = defender.ReligiousAlteration.GetParameters(attacker.ReligiousAlteration.ReligionName);

                // Contribution = Intelligence * Affiliation * Block Multiplier
                var blockValue = defender.GetIntelligence() * 
                                 defender.ReligiousAlteration.Affiliation * 
                                 parameters.BlockMultiplier;

                religionComponent = blockValue.Clip(-1, 1);
            }

            switch (blockType)
            {
                case AlterationBlockType.Mental:
                    return _randomSequenceGenerator.Get() < (defender.GetMagicBlock() + religionComponent).Clip();
                case AlterationBlockType.Physical:
                    return _randomSequenceGenerator.Get() < (defender.GetDodge() + religionComponent).Clip();
                case AlterationBlockType.NonBlockable:
                    return false;
                default:
                    throw new Exception("Unknwon Alteration Block Type");
            }
        }

        public IEnumerable<Character> CalculateAffectedAlterationCharacters(AlterationType type, AlterationAttackAttributeType attackAttributeType, double effectRange, Character character)
        {
            var singleTarget = character is Player ?
                                (Character)_modelService.GetTargetedEnemies().FirstOrDefault() :
                                _modelService.Player;

            var allTargets = character is Player ?
                                    _modelService.GetTargetedEnemies().ToArray() :
                                    new Character[] { _modelService.Player };

            var allInRange = _modelService.Level
                                          .Enemies
                                          .Where(x =>
                                          {
                                              return Calculator.EuclideanSquareDistance(x.Location, character.Location) <=
                                                     effectRange * effectRange;
                                          })
                                          .Cast<Character>()
                                          .ToList();

            allInRange.Add(_modelService.Player);

            var allInRangeExceptSource = allInRange.Except(new Character[] { character });



            switch (type)
            {
                case AlterationType.PassiveSource:
                case AlterationType.PassiveAura:
                case AlterationType.TemporarySource:
                case AlterationType.PermanentSource:
                case AlterationType.Steal:
                case AlterationType.RunAway:
                case AlterationType.TeleportSelf:
                case AlterationType.Remedy:
                    return new Character[] { character };
                case AlterationType.TemporaryTarget:
                case AlterationType.PermanentTarget:
                case AlterationType.TeleportTarget:
                    return new Character[] { singleTarget };
                case AlterationType.TemporaryAllTargets:
                case AlterationType.PermanentAllTargets:
                case AlterationType.TeleportAllTargets:
                    return allTargets;
                case AlterationType.OtherMagicEffect:
                    break;
                case AlterationType.AttackAttribute:
                    {
                        switch (attackAttributeType)
                        {
                            case AlterationAttackAttributeType.ImbueArmor:
                            case AlterationAttackAttributeType.ImbueWeapon:
                            case AlterationAttackAttributeType.Passive:
                            case AlterationAttackAttributeType.TemporaryFriendlySource:
                            case AlterationAttackAttributeType.TemporaryMalignSource:
                                return new Character[] { character };
                            case AlterationAttackAttributeType.TemporaryFriendlyTarget:
                            case AlterationAttackAttributeType.TemporaryMalignTarget:
                            case AlterationAttackAttributeType.MeleeTarget:
                                return new Character[] { singleTarget };
                            case AlterationAttackAttributeType.MeleeAllInRange:
                            case AlterationAttackAttributeType.TemporaryMalignAllInRange:
                                return allInRange;
                            case AlterationAttackAttributeType.MeleeAllInRangeExceptSource:
                            case AlterationAttackAttributeType.TemporaryMalignAllInRangeExceptSource:
                                return allInRangeExceptSource;
                            default:
                                break;
                        }
                    }
                    break;
                case AlterationType.TemporaryAllInRange:
                case AlterationType.PermanentAllInRange:
                case AlterationType.TeleportAllInRange:
                    return allInRange;
                case AlterationType.TemporaryAllInRangeExceptSource:
                case AlterationType.PermanentAllInRangeExceptSource:
                case AlterationType.TeleportAllInRangeExceptSource:
                    return allInRangeExceptSource;
                default:
                    break;
            }

            return new Character[] { };
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
