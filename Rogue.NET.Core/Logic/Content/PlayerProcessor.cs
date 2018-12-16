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

            // Skill Learning
            // Process skill learning
            foreach (var skillSet in player.SkillSets)
            {
                if (player.Level >= skillSet.LevelLearned && !skillSet.IsLearned)
                {
                    skillSet.IsLearned = true;

                    _scenarioMessageService.Publish(ScenarioMessagePriority.Good, player.RogueName + " Has Learned A New Skill - " + skillSet.RogueName);
                }
            }

            _scenarioMessageService.PublishPlayerAdvancement(ScenarioMessagePriority.Good, player.RogueName, player.Level, attributesChanged);
        }
        public void CalculateEnemyDeathGains(Player player, Enemy slainEnemy)
        {
            // Add to player experience
            player.Experience += slainEnemy.ExperienceGiven;

            // Skill Progress - Player gets boost on enemy death

            // Foreach SkillSet that can still require learning (From slain enemy reward)
            foreach (var skill in player.SkillSets.Where(x => (x.Level < x.Skills.Count) && x.IsActive))
            {
                switch (skill.Emphasis)
                {
                    case 1:
                        skill.SkillProgress += ModelConstants.SkillLowProgressIncrement;
                        player.Hunger += ModelConstants.SkillLowHungerIncrement;
                        break;
                    case 2:
                        skill.SkillProgress += ModelConstants.SkillMediumProgressIncrement;
                        player.Hunger += ModelConstants.SkillMediumHungerIncrement;
                        break;
                    case 3:
                        skill.SkillProgress += ModelConstants.SkillHighProgressIncrement;
                        player.Hunger += ModelConstants.SkillHighHungerIncrement;
                        break;
                }

                // Show message to user
                if (skill.Emphasis > 0 && skill.Level < skill.Skills.Count)
                {
                    _scenarioMessageService.Publish(
                        ScenarioMessagePriority.Normal,
                        "{0} has progressed to {1}",
                        skill.RogueName,
                        skill.SkillProgress.ToString("P1"));
                }

                if (skill.SkillProgress >= 1)
                {
                    if (skill.Level < skill.Skills.Count)
                    {
                        //Deactivate if currently turned on
                        if (skill.IsTurnedOn && (skill.GetCurrentSkill().Type == AlterationType.PassiveAura ||
                                             skill.GetCurrentSkill().Type == AlterationType.PassiveSource))
                        {
                            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Deactivating - " + skill.RogueName);

                            // Deactive the passive alteration - referenced by Spell Id
                            player.Alteration.DeactivatePassiveAlteration(skill.GetCurrentSkill().Id);

                            skill.IsTurnedOn = false;
                        }

                        skill.Level++;
                        skill.SkillProgress = 0;

                        _scenarioMessageService.PublishSkillAdvancement(ScenarioMessagePriority.Good, skill.RogueName, skill.Level + 1);
                    }
                    else
                        skill.SkillProgress = 1;
                }
            }
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
            player.Hp += (regenerate ? player.GetHpRegen() : 0D) - player.GetMalignAttackAttributeHit();
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

            if (player.Experience >= CalculateExperienceNext(player))
            {
                CalculateLevelGains(player);

                //Bonus health and magic refill
                player.Hp = player.HpMax;
                player.Mp = player.MpMax;
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
                    !_alterationProcessor.CalculatePlayerMeetsAlterationCost(player, skillSet.GetCurrentSkill().Cost))
                {
                    skillSet.IsTurnedOn = false;

                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Deactivating " + skillSet.RogueName);

                    var currentSkill = skillSet.GetCurrentSkill();

                    // Deactive the passive alteration - referenced by Spell Id
                    player.Alteration.DeactivatePassiveAlteration(currentSkill.Id);
                }
            }

            player.ApplyLimits();
        }
    }
}
