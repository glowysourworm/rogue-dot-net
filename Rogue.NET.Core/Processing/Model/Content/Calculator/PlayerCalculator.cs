using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Content.Skill.Extension;
using Rogue.NET.Core.Processing.Event.Backend.EventData.ScenarioMessage.Enum;
using Rogue.NET.Core.Processing.Model.Content.Calculator.Interface;
using Rogue.NET.Core.Processing.Model.Static;
using Rogue.NET.Core.Processing.Service.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Media;

namespace Rogue.NET.Core.Processing.Model.Content.Calculator
{
    [Export(typeof(IPlayerCalculator))]
    public class PlayerSubProcessor : IPlayerCalculator
    {
        readonly IAlterationCalculator _alterationCalculator;
        readonly IScenarioMessageService _scenarioMessageService;
        readonly IModelService _modelService;

        // TODO: Untangle reference to model service.
        [ImportingConstructor]
        public PlayerSubProcessor(
            IAlterationCalculator alterationCalculator,
            IScenarioMessageService scenarioMessageService,
            IModelService modelService)
        {
            _alterationCalculator = alterationCalculator;
            _scenarioMessageService = scenarioMessageService;
            _modelService = modelService;
        }

        public double CalculateExperienceNext(Player player)
        {
            return RogueCalculator.CalculateExperienceNext(player.Level);
        }

        public void CalculateLevelGains(Player player)
        {
            var attributesChanged = new List<Tuple<string, double, Color>>();

            // TODO:STATS
            //// Hp Max
            //var change = PlayerCalculator.CalculateHpGain(player.StrengthBase * _randomSequenceGenerator.Get());
            //player.HpMax += change;
            //attributesChanged.Add(new Tuple<string, double, Color>("HP", change, Colors.Red));

            //// Mp Max
            //change = PlayerCalculator.CalculateMpGain(player.IntelligenceBase * _randomSequenceGenerator.Get());
            //player.StaminaMax += change;
            //attributesChanged.Add(new Tuple<string, double, Color>("MP", change, Colors.Blue));

            // Level :)
            player.Level++;

            _scenarioMessageService.PublishPlayerAdvancement(ScenarioMessagePriority.Good, player.RogueName, player.Level, attributesChanged);
        }
        public void CalculateEnemyDeathGains(Player player, Enemy slainEnemy)
        {
            // Add to player experience
            player.Experience += slainEnemy.ExperienceGiven;
        }

        public Equipment GetEquippedType(Player player, EquipmentType type)
        {
            return player.Equipment.Values.FirstOrDefault(x => x.Type == type && x.IsEquipped);
        }
        public int GetNumberEquipped(Player player, EquipmentType type)
        {
            return player.Equipment.Values.Count(x => x.Type == type && x.IsEquipped);
        }
        public int GetNumberOfFreeHands(Player player)
        {
            int handsFree = 2 - player.Equipment.Values.Count(z => (z.Type == EquipmentType.OneHandedMeleeWeapon || z.Type == EquipmentType.Shield) && z.IsEquipped);
            handsFree -= 2 * player.Equipment.Values.Count(z => (z.Type == EquipmentType.TwoHandedMeleeWeapon || z.Type == EquipmentType.RangeWeapon) && z.IsEquipped);

            return handsFree;
        }

        public void ApplyEndOfTurn(Player player, bool regenerate, out bool playerAdvancement)
        {
            // Initialize Player Advancement
            playerAdvancement = false;

            // Apply Malign Attack Attribute Hits
            var malignAttackAttributeHit = player.GetMalignAttackAttributeHit();
            var appliedToHp = (malignAttackAttributeHit - player.Stamina).LowLimit(0);

            player.Stamina -= malignAttackAttributeHit;
            player.Health -= appliedToHp;

            // Apply Regeneration
            var staminaRegenerationAlteration = player.Alteration.GetAttribute(CharacterAttribute.StaminaRegen);
            var hpRegenerationAlteration = player.Alteration.GetAttribute(CharacterAttribute.HealthRegen);

            // Penalized for action taken - no natural regeneration
            if (!regenerate)
            {
                // First, regenerate stamina
                player.Stamina += staminaRegenerationAlteration;

                // Then, allow Hp regeneration
                if (player.Stamina >= player.StaminaMax)
                    player.Health += hpRegenerationAlteration;
            }
            else
            {
                // First, regenerate stamina
                player.Stamina += player.GetTotalStaminaRegen();

                // Then, allow Hp regeneration
                if (player.Stamina >= player.StaminaMax)
                    player.Health += player.GetTotalHealthRegen();

                // Override if character has any alterations present
                else if (hpRegenerationAlteration > 0)
                    player.Health += hpRegenerationAlteration;
            }

            // Set Killed By if malign attribute hit is great enough
            if (player.Health <= 0)
            {
                var malignAlterationName = player.Alteration
                                                 .GetKilledBy();

                if (!string.IsNullOrEmpty(malignAlterationName))
                    _modelService.SetKilledBy(malignAlterationName);
            }

            // Broadcast hungry, starving, critical messages
            var hunger = player.Hunger;
            var nextHunger = hunger + player.GetFoodUsagePerTurn();

            if (nextHunger >= ModelConstants.Hunger.HungryThreshold &&
                hunger < ModelConstants.Hunger.HungryThreshold)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, player.RogueName + " is getting Hungry");

            if (nextHunger >= ModelConstants.Hunger.VeryHungryThreshold &&
                hunger < ModelConstants.Hunger.VeryHungryThreshold)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, player.RogueName + " is Very Hungry");

            if (nextHunger >= ModelConstants.Hunger.CriticalThreshold &&
                hunger < ModelConstants.Hunger.CriticalThreshold)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Bad, player.RogueName + " is Starving!!!");

            player.Hunger += player.GetFoodUsagePerTurn();

            // Player Gains a Level
            if (player.Experience >= CalculateExperienceNext(player))
            {
                CalculateLevelGains(player);

                // Refill HP and MP as a bonus
                player.Health = player.HealthMax;
                player.Stamina = player.StaminaMax;

                // Mark player advancement
                playerAdvancement = true;
            }

            // Check for Skill Set Requirements
            foreach (var skillSet in player.SkillSets)
            {
                // If Player fell below level requirements then have to de-activate the skills
                if (skillSet.IsTurnedOn && skillSet.SelectedSkill.AreRequirementsMet(player))
                {
                    skillSet.IsTurnedOn = false;

                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Deactivating " + skillSet.RogueName);

                    // Pass-Through method is Safe to call
                    _modelService.Player.Alteration.Remove(skillSet.GetCurrentSkillAlteration().Name);
                }

                // Check for skills that have requirements met / or lost
                foreach (var skill in skillSet.Skills)
                {
                    var areRequirementsMet = skill.AreRequirementsMet(player);
                    var wereRequirementsMet = skill.AreRequirementsMet;

                    if (!wereRequirementsMet && areRequirementsMet && skill.IsLearned)
                        _scenarioMessageService.Publish(ScenarioMessagePriority.Good, player.RogueName + " has regained use of the Skill " + _modelService.GetDisplayName(skillSet));

                    else if (wereRequirementsMet && !areRequirementsMet && skill.IsLearned)
                        _scenarioMessageService.Publish(ScenarioMessagePriority.Bad, player.RogueName + " can no longer use the Skill " + _modelService.GetDisplayName(skillSet));

                    // Store flag to know if requirements change
                    skill.AreRequirementsMet = areRequirementsMet;
                }
            }

            // Normal temporary effects
            var effectsFinishedNames = player.Alteration.DecrementEventTimes();

            // Display PostEffect Messages
            foreach (var effectName in effectsFinishedNames)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, effectName + " has worn off");

            //Apply per step alteration costs
            foreach (var alterationCost in player.Alteration.GetAlterationCosts())
            {
                player.Health -= alterationCost.Value.Health;
                player.Stamina -= alterationCost.Value.Stamina;
                player.Hunger += alterationCost.Value.Hunger;
                player.Experience -= alterationCost.Value.Experience;
            }

            //Maintain Passive Effects
            foreach (var skillSet in player.SkillSets)
            {
                // Skill set is turned on; but can't afford the cost
                if (skillSet.IsTurnedOn &&
                    !_alterationCalculator.CalculatePlayerMeetsAlterationCost(player, skillSet.GetCurrentSkillAlteration().Cost))
                {
                    skillSet.IsTurnedOn = false;

                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Deactivating " + skillSet.RogueName);

                    var currentSkill = skillSet.GetCurrentSkillAlteration();

                    // Deactive the passive alteration - referenced by Spell Id
                    player.Alteration.Remove(currentSkill.Name);
                }
            }

            // TODO: Figure out a nicer way to clear these; but they are only cleared out when
            //       new ones are applied.. so have to clear them this way for now.
            player.Alteration.ApplyTargetAuraEffects(new AttackAttributeAuraAlterationEffect[] { });
            player.Alteration.ApplyTargetAuraEffects(new AuraAlterationEffect[] { });

            // Apply Enemy Auras (where Player is in an affected cell)
            foreach (var character in _modelService.Level.Content.NonPlayerCharacters)
            {
                var characterAuras = character.Alteration
                                              .GetAuras()
                                              .Where(x => _modelService.Level
                                                                       .AuraGrid
                                                                       .GetAuraAffectedLocations(x.Item1.Id)
                                                                       .Contains(_modelService.PlayerLocation))
                                              .Select(x => x.Item1)
                                              .GroupBy(x => x.GetType());

                // Set Effect to Enemies
                foreach (var auraGroup in characterAuras)
                {
                    if (auraGroup.Key == typeof(AttackAttributeAuraAlterationEffect))
                        player.Alteration.ApplyTargetAuraEffects(auraGroup.Cast<AttackAttributeAuraAlterationEffect>());

                    else if (auraGroup.Key == typeof(AuraAlterationEffect))
                        player.Alteration.ApplyTargetAuraEffects(auraGroup.Cast<AuraAlterationEffect>());

                    else
                        throw new Exception("Unknwon Aura Alteration Effect Type");
                }
            }

            player.ApplyLimits();
        }

        public void DeActivateSkills(Player player)
        {
            // Deactivate skill sets
            player.SkillSets.ForEach(x =>
            {
                // Maintain Passive Effects
                if (x.IsTurnedOn)
                {
                    x.IsTurnedOn = false;
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Deactivating " + x.RogueName);

                    // Pass-Through method is Safe to call
                    player.Alteration.Remove(x.GetCurrentSkillAlteration().Name);
                }

                x.IsActive = false;
            });
        }
    }
}
