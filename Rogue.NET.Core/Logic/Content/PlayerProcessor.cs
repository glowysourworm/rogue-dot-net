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

namespace Rogue.NET.Core.Logic.Content
{
    [Export(typeof(IPlayerProcessor))]
    public class PlayerProcessor : IPlayerProcessor
    {
        readonly IAlterationProcessor _alterationProcessor;
        readonly IScenarioMessageService _scenarioMessageService;
        readonly IRandomSequenceGenerator _randomSequenceGenerator;

        [ImportingConstructor]
        public PlayerProcessor(
            IAlterationProcessor alterationProcessor,
            IScenarioMessageService scenarioMessageService,
            IRandomSequenceGenerator randomSequenceGenerator)
        {
            _alterationProcessor = alterationProcessor;
            _scenarioMessageService = scenarioMessageService;
            _randomSequenceGenerator = randomSequenceGenerator;
        }

        public double CalculateExperienceNext(Player player)
        {
            //	y = 100e^(0.25)x  - based on player level 8 with 8500 experience points
            //  available by level 15; and 100 to reach first level + linear component to
            //  avoid easy leveling during low levels
            //return (100 * Math.Exp(0.25*p.Level)) + (300 * p.Level);

            return (player.Level == 0) ? 100 : ((10 * Math.Pow(player.Level + 1, 3)) + (300 + player.Level));
        }
        public void CalculateLevelGains(Player player)
        {
            var messages = new List<string>();

            string header = player.RogueName + " Has Reached Level " + (player.Level + 1).ToString() + "!";

            //Hp Max
            var d = (player.StrengthBase) * ModelConstants.LVL_GAIN_BASE * 2 * _randomSequenceGenerator.Get();
            player.HpMax += d;
            messages.Add("Hp Increased By:  " + d.ToString("F3"));

            //Mp Max
            d = (player.IntelligenceBase) * ModelConstants.LVL_GAIN_BASE * 2 * _randomSequenceGenerator.Get();
            player.MpMax += d;
            messages.Add("Mp Increased By:  " + d.ToString("F3"));

            //Strength
            d = ModelConstants.LVL_GAIN_BASE * _randomSequenceGenerator.Get();
            player.StrengthBase += (player.AttributeEmphasis == AttributeEmphasis.Strength) ? 3 * d : d;
            messages.Add("Strength Increased By:  " + d.ToString("F3"));

            //Intelligence
            d = ModelConstants.LVL_GAIN_BASE * _randomSequenceGenerator.Get();
            player.IntelligenceBase += (player.AttributeEmphasis == AttributeEmphasis.Intelligence) ? 3 * d : d;
            messages.Add("Intelligence Increased By:  " + d.ToString("F3"));

            //Agility
            d = ModelConstants.LVL_GAIN_BASE * _randomSequenceGenerator.Get();
            player.AgilityBase += (player.AttributeEmphasis == AttributeEmphasis.Agility) ? 3 * d : d;
            messages.Add("Agility Increased By:  " + d.ToString("F3"));

            //Level :)
            player.Level++;

            _scenarioMessageService.PublishPlayerAdvancement(header, messages);
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
            player.Hp += player.GetHpRegen(regenerate);
            player.Mp += player.GetMpRegen();

            player.Hunger += player.GetFoodUsagePerTurn();

            if (player.Experience >= CalculateExperienceNext(player))
            {
                CalculateLevelGains(player);

                //Bonus health and magic refill
                player.Hp = player.HpMax;
                player.Mp = player.MpMax;
            }

            //Normal temporary effects
            UpdateAlterationEffectCollection(player.Alteration.ActiveTemporaryEffects);
            UpdateAlterationEffectCollection(player.Alteration.AttackAttributeTemporaryFriendlyEffects);
            UpdateAlterationEffectCollection(player.Alteration.AttackAttributeTemporaryMalignEffects);

            //Apply per step alteration costs
            foreach (AlterationCost alt in player.Alteration.PerStepAlterationCosts.Values)
            {
                player.AgilityBase -= alt.Agility;
                player.AuraRadiusBase -= alt.AuraRadius;
                player.Experience -= alt.Experience;
                player.FoodUsagePerTurnBase += alt.FoodUsagePerTurn;
                player.Hp -= alt.Hp;
                player.Hunger += alt.Hunger;
                player.IntelligenceBase -= alt.Intelligence;
                player.Mp -= alt.Mp;
                player.StrengthBase -= alt.Strength;
            }

            //Maintain Passive Effects
            foreach (var skillSet in player.SkillSets)
            {
                // Skill set is turned on; but can't afford the cost
                if (skillSet.IsTurnedOn && 
                    !_alterationProcessor.CalculatePlayerMeetsAlterationCost(player, skillSet.GetCurrentSkill().Cost))
                {
                    skillSet.IsTurnedOn = false;

                    _scenarioMessageService.Publish("Deactivating " + skillSet.RogueName);

                    var currentSkill = skillSet.GetCurrentSkill();

                    // Remove Per Step Alteration Cost
                    if (currentSkill.Cost.Type == AlterationCostType.PerStep)
                        player.Alteration.PerStepAlterationCosts.Remove(currentSkill.Id);

                    // Current turned on skill is a Passive Aura (PLAYER ONLY)
                    if (currentSkill.Type == AlterationType.PassiveAura)
                        player.Alteration.ActiveAuras.Remove(currentSkill.Id);

                    // Current turned on skill is a Passive Source (Active Passive Effect)
                    else if (currentSkill.Type == AlterationType.PassiveSource)
                        player.Alteration.ActivePassiveEffects.Remove(currentSkill.Id);

                    // Current turned on skill is a Passive Attack Attribute Effect
                    else if (currentSkill.Type == AlterationType.AttackAttribute &&
                             currentSkill.AttackAttributeType == AlterationAttackAttributeType.Passive)
                        player.Alteration.AttackAttributePassiveEffects.Remove(currentSkill.Id);

                    else
                        throw new Exception("Trying to Deactivate Unknown Passive Effect Type");
                }
            }

            player.ApplyLimits();
        }
        public void ProcessSkillLearning(Player player)
        {
            // Foreach SkillSet that can still require learning
            foreach (var skill in player.SkillSets.Where(x => x.Level < x.Skills.Count))
            {
                switch (skill.Emphasis)
                {
                    case 1:
                        skill.SkillProgress += (0.001);
                        player.Hunger += 0.1;
                        break;
                    case 2:
                        skill.SkillProgress += (0.005);
                        player.Hunger += 0.75;
                        break;
                    case 3:
                        skill.SkillProgress += (0.01);
                        player.Hunger += 1.5;
                        break;
                }
                if (skill.SkillProgress >= 1)
                {
                    if (skill.Level < skill.Skills.Count)
                    {
                        //Deactivate if currently turned on
                        if (skill.IsTurnedOn && (skill.GetCurrentSkill().Type == AlterationType.PassiveAura ||
                                             skill.GetCurrentSkill().Type == AlterationType.PassiveSource))
                        {
                            _scenarioMessageService.Publish("Deactivating - " + skill.RogueName);
                            // TODO
                            //this.Player.DeactivatePassiveEffect(s.CurrentSkill.Id);
                            skill.IsTurnedOn = false;
                        }

                        skill.Level++;
                        skill.SkillProgress = 0;

                        _scenarioMessageService.PublishPlayerAdvancement("Player Skill Has Reached a New Level!", new string[]
                        {
                            skill.RogueName + " has reached Level " + (skill.Level + 1).ToString()
                        });
                    }
                    else
                        skill.SkillProgress = 1;
                }
            }
        }

        private void UpdateAlterationEffectCollection(IList<AlterationEffect> collection)
        {
            for (int i = collection.Count - 1; i >= 0; i--)
            {
                //Check temporary event time
                collection[i].EventTime--;
                if (collection[i].EventTime <= 0)
                {
                    // Publish message after effect wears off
                    _scenarioMessageService.Publish(collection[i].PostEffectString);

                    // Remove the effect
                    collection.RemoveAt(i);
                }
            }
        }
    }
}
