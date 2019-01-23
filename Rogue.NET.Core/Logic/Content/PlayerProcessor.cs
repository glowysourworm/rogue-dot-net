using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Logic.Content.Interface;

using System;
using System.Linq;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using Rogue.NET.Core.Model.ScenarioMessage;
using System.Windows.Media;
using Rogue.NET.Core.Logic.Static;
using Rogue.NET.Core.Model.Scenario.Content.Skill.Extension;
using Rogue.NET.Common.Extension;

namespace Rogue.NET.Core.Logic.Content
{
    [Export(typeof(IPlayerProcessor))]
    public class PlayerProcessor : IPlayerProcessor
    {
        readonly IAlterationProcessor _alterationProcessor;
        readonly IScenarioMessageService _scenarioMessageService;
        readonly IRandomSequenceGenerator _randomSequenceGenerator;
        readonly IModelService _modelService;

        // TODO: Untangle reference to model service.
        [ImportingConstructor]
        public PlayerProcessor(
            IAlterationProcessor alterationProcessor,
            IScenarioMessageService scenarioMessageService,
            IRandomSequenceGenerator randomSequenceGenerator,
            IModelService modelService)
        {
            _alterationProcessor = alterationProcessor;
            _scenarioMessageService = scenarioMessageService;
            _randomSequenceGenerator = randomSequenceGenerator;
            _modelService = modelService;
        }

        public double CalculateExperienceNext(Player player)
        {
            return PlayerCalculator.CalculateExperienceNext(player.Level);
        }

        public double CalculateSkillPointExperienceNext(Player player)
        {
            return PlayerCalculator.CalculateExperienceNextSkillPoint(player.SkillPointsEarned, _modelService.ScenarioConfiguration.DungeonTemplate.SkillPointMultiplier);
        }

        public void CalculateLevelGains(Player player)
        {
            var attributesChanged = new List<Tuple<string, double, Color>>();

            // Hp Max
            var change = PlayerCalculator.CalculateHpGain(player.StrengthBase * _randomSequenceGenerator.Get());
            player.HpMax += change;
            attributesChanged.Add(new Tuple<string, double, Color>("HP", change, Colors.Red));

            // Mp Max
            change = PlayerCalculator.CalculateMpGain(player.IntelligenceBase * _randomSequenceGenerator.Get());
            player.MpMax += change;
            attributesChanged.Add(new Tuple<string, double, Color>("MP", change, Colors.Blue));

            // Strength
            change = PlayerCalculator.CalculateStrengthGain(_randomSequenceGenerator.Get(), player.AttributeEmphasis == AttributeEmphasis.Strength);
            player.StrengthBase += change;

            attributesChanged.Add(new Tuple<string, double, Color>("Strength", change, Colors.Salmon));

            // Intelligence
            change = PlayerCalculator.CalculateIntelligenceGain(_randomSequenceGenerator.Get(), player.AttributeEmphasis == AttributeEmphasis.Intelligence);
            player.IntelligenceBase += change;

            attributesChanged.Add(new Tuple<string, double, Color>("Intelligence", change, Colors.LightBlue));

            // Agility
            change = PlayerCalculator.CalculateAgilityGain(_randomSequenceGenerator.Get(), player.AttributeEmphasis == AttributeEmphasis.Agility);
            player.AgilityBase += change;

            attributesChanged.Add(new Tuple<string, double, Color>("Agility", change, Colors.Tan));

            // Level :)
            player.Level++;

            _scenarioMessageService.PublishPlayerAdvancement(ScenarioMessagePriority.Good, player.RogueName, player.Level, attributesChanged);
        }
        public void CalculateEnemyDeathGains(Player player, Enemy slainEnemy)
        {
            // Add to player experience
            player.Experience += slainEnemy.ExperienceGiven;

            // Check for point advancement

            // TODO
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

        public void ApplyEndOfTurn(Player player, bool regenerate)
        {
            //Normal turn stuff
            player.Hp += (regenerate ? player.GetHpRegen() : 0D) - player.GetMalignAttackAttributeHit(_modelService.GetAttackAttributes());
            player.Mp += player.GetMpRegen();

            // Set Killed By if malign attribute hit is great enough
            if (player.Hp <= 0)
            {
                var malignAlteration = player.Alteration
                                            .GetTemporaryAttackAttributeAlterations(false)
                                            .FirstOrDefault();

                if (malignAlteration != null)
                    _modelService.SetKilledBy(malignAlteration.DisplayName);
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

                //Bonus health and magic refill
                player.Hp = player.HpMax;
                player.Mp = player.MpMax;
            }

            // Player Gains a Skill Point
            if (player.Experience >= CalculateSkillPointExperienceNext(player))
            {
                player.SkillPoints++;
                player.SkillPointsEarned++;

                _scenarioMessageService.Publish(ScenarioMessagePriority.Unique, player.RogueName + " has earned a Skill Point");
            }

            // Check for Skill Set Requirements
            foreach (var skillSet in player.SkillSets)
            {
                var skillSetLearned = skillSet.IsLearned;

                var skillSetRequirementsMet = player.Level >= skillSet.LevelLearned &&
                                             (!skillSet.HasReligiousAffiliationRequirement ||
                                              (skillSet.HasReligiousAffiliationRequirement &&
                                               player.ReligiousAlteration.IsAffiliated() &&
                                               player.ReligiousAlteration.Affiliation >= skillSet.ReligiousAffiliationRequirement.RequiredAffiliationLevel &&
                                               player.ReligiousAlteration.ReligionName == skillSet.ReligiousAffiliationRequirement.ReligionName));

                // If Player fell below level requirements then have to de-activate the skills and mark them not learned
                if (!skillSetRequirementsMet && skillSet.IsTurnedOn)
                {
                    skillSet.IsTurnedOn = false;

                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Deactivating " + skillSet.RogueName);

                    // Pass-Through method is Safe to call
                    _modelService.Player.Alteration.DeactivatePassiveAlteration(skillSet.GetCurrentSkillAlteration().Id);

                    // SET ALL SKILLS TO NOT-LEARNED
                    skillSet.Skills.ForEach(x => x.IsLearned = false);
                }

                // Level Requirement Only
                skillSet.IsLearned = skillSetRequirementsMet;

                if (!skillSetLearned && skillSetRequirementsMet)
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Good, player.RogueName + " Has Learned the Skill " + _modelService.GetDisplayName(skillSet));

                else if (skillSetLearned && !skillSetRequirementsMet)
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Bad, player.RogueName + " can no longer use the Skill " + _modelService.GetDisplayName(skillSet));
            }

            // Normal temporary effects
            var effectsFinished = player.Alteration.DecrementEventTimes();

            // Display PostEffect Messages
            foreach (var effect in effectsFinished)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, effect.DisplayName + " has worn off");

            //Apply per step alteration costs
            foreach (AlterationCost alterationCost in player.Alteration.GetAlterationCosts())
            {
                player.AgilityBase -= alterationCost.Agility;
                player.AuraRadiusBase -= alterationCost.AuraRadius;
                player.Experience -= alterationCost.Experience;
                player.FoodUsagePerTurnBase += alterationCost.FoodUsagePerTurn;
                player.Hp -= alterationCost.Hp;
                player.Hunger += alterationCost.Hunger;
                player.IntelligenceBase -= alterationCost.Intelligence;
                player.Mp -= alterationCost.Mp;
                player.StrengthBase -= alterationCost.Strength;
            }

            //Maintain Passive Effects
            foreach (var skillSet in player.SkillSets)
            {
                // Skill set is turned on; but can't afford the cost
                if (skillSet.IsTurnedOn && 
                    !_alterationProcessor.CalculatePlayerMeetsAlterationCost(player, skillSet.GetCurrentSkillAlteration().Cost))
                {
                    skillSet.IsTurnedOn = false;

                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Deactivating " + skillSet.RogueName);

                    var currentSkill = skillSet.GetCurrentSkillAlteration();

                    // Deactive the passive alteration - referenced by Spell Id
                    player.Alteration.DeactivatePassiveAlteration(currentSkill.Id);
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
                    player.Alteration.DeactivatePassiveAlteration(x.GetCurrentSkillAlteration().Id);
                }

                x.IsActive = false;
            });
        }
    }
}
