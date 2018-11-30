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

            // Hp Max
            var d = (player.StrengthBase) * ModelConstants.LevelGainBase * 2 * _randomSequenceGenerator.Get();
            player.HpMax += d;
            messages.Add("Hp Increased By:  " + d.ToString("F3"));

            // Mp Max
            d = (player.IntelligenceBase) * ModelConstants.LevelGainBase * 2 * _randomSequenceGenerator.Get();
            player.MpMax += d;
            messages.Add("Mp Increased By:  " + d.ToString("F3"));

            // Strength
            d = ModelConstants.LevelGainBase * _randomSequenceGenerator.Get();
            player.StrengthBase += (player.AttributeEmphasis == AttributeEmphasis.Strength) ? 3 * d : d;
            messages.Add("Strength Increased By:  " + d.ToString("F3"));

            // Intelligence
            d = ModelConstants.LevelGainBase * _randomSequenceGenerator.Get();
            player.IntelligenceBase += (player.AttributeEmphasis == AttributeEmphasis.Intelligence) ? 3 * d : d;
            messages.Add("Intelligence Increased By:  " + d.ToString("F3"));

            // Agility
            d = ModelConstants.LevelGainBase * _randomSequenceGenerator.Get();
            player.AgilityBase += (player.AttributeEmphasis == AttributeEmphasis.Agility) ? 3 * d : d;
            messages.Add("Agility Increased By:  " + d.ToString("F3"));

            // Level :)
            player.Level++;

            // Skill Learning
            // Process skill learning
            foreach (var skillSet in player.SkillSets)
            {
                if (player.Level >= skillSet.LevelLearned && !skillSet.IsLearned)
                {
                    skillSet.IsLearned = true;

                    messages.Add(player.RogueName + " Has Learned A New Skill - " + skillSet.RogueName);
                }
            }

            _scenarioMessageService.PublishPlayerAdvancement(header, messages);
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
                if (skill.SkillProgress >= 1)
                {
                    if (skill.Level < skill.Skills.Count)
                    {
                        //Deactivate if currently turned on
                        if (skill.IsTurnedOn && (skill.GetCurrentSkill().Type == AlterationType.PassiveAura ||
                                             skill.GetCurrentSkill().Type == AlterationType.PassiveSource))
                        {
                            _scenarioMessageService.Publish("Deactivating - " + skill.RogueName);

                            // Deactive the passive alteration - referenced by Spell Id
                            player.Alteration.DeactivatePassiveAlteration(skill.GetCurrentSkill().Id);

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

            // Normal temporary effects
            var effectsFinished = player.Alteration.DecrementEventTimes();

            // Display PostEffect Messages
            foreach (var effect in effectsFinished)
                _scenarioMessageService.Publish(effect.PostEffectString);

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

                    _scenarioMessageService.Publish("Deactivating " + skillSet.RogueName);

                    var currentSkill = skillSet.GetCurrentSkill();

                    // Deactive the passive alteration - referenced by Spell Id
                    player.Alteration.DeactivatePassiveAlteration(currentSkill.Id);
                }
            }

            player.ApplyLimits();
        }
    }
}
