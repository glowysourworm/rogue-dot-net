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

namespace Rogue.NET.Core.Logic.Content
{
    [Export(typeof(IPlayerProcessor))]
    public class PlayerProcessor : IPlayerProcessor
    {
        readonly ICharacterProcessor _characterProcessor;
        readonly IScenarioMessageService _scenarioMessageService;
        readonly IRandomSequenceGenerator _randomSequenceGenerator;

        [ImportingConstructor]
        public PlayerProcessor(
            ICharacterProcessor characterProcessor, 
            IScenarioMessageService scenarioMessageService,
            IRandomSequenceGenerator randomSequenceGenerator)
        {
            _characterProcessor = characterProcessor;
            _scenarioMessageService = scenarioMessageService;
            _randomSequenceGenerator = randomSequenceGenerator;
        }

        public double GetAttackBase(Player player)
        {
            return player.StrengthBase;
        }
        public double GetDefenseBase(Player player)
        {
            return player.StrengthBase / 5.0D;
        }
        public double GetAttack(Player player)
        {
            double a = _characterProcessor.GetStrength(player);

            foreach (var equipment in player.Equipment.Values.Where(x => x.IsEquipped))
            {
                switch (equipment.Type)
                {
                    case EquipmentType.OneHandedMeleeWeapon:
                        a += ((equipment.Class + 1) * equipment.Quality);
                        break;
                    case EquipmentType.TwoHandedMeleeWeapon:
                        a += ((equipment.Class + 1) * equipment.Quality) * 2;
                        break;
                    case EquipmentType.RangeWeapon:
                        a += ((equipment.Class + 1) * equipment.Quality) / 2;
                        break;
                }
            }

            // TODO
            //foreach (AlterationEffect alt in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)))
            //    a += alt.Attack;

            return Math.Max(0, a);
        }
        public double GetDefense(Player player)
        {
            double defense = _characterProcessor.GetStrength(player) / 5.0D;

            foreach (var equipment in player.Equipment.Values.Where(x => x.IsEquipped))
            {
                switch (equipment.Type)
                {
                    case EquipmentType.Armor:
                        defense += ((equipment.Class + 1) * equipment.Quality);
                        break;
                    case EquipmentType.Shoulder:
                        defense += (((equipment.Class + 1) * equipment.Quality) / 5);
                        break;
                    case EquipmentType.Boots:
                        defense += (((equipment.Class + 1) * equipment.Quality) / 10);
                        break;
                    case EquipmentType.Gauntlets:
                        defense += (((equipment.Class + 1) * equipment.Quality) / 10);
                        break;
                    case EquipmentType.Belt:
                        defense += (((equipment.Class + 1) * equipment.Quality) / 8);
                        break;
                    case EquipmentType.Shield:
                        defense += (((equipment.Class + 1) * equipment.Quality) / 5);
                        break;
                    case EquipmentType.Helmet:
                        defense += (((equipment.Class + 1) * equipment.Quality) / 5);
                        break;
                }
            }

            // TODO
            //foreach (AlterationEffect alt in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)))
            //    defense += alt.Defense;

            return Math.Max(0, defense);
        }
        public double GetFoodUsagePerTurn(Player player)
        {
            double d = player.FoodUsagePerTurnBase + (_characterProcessor.GetHaul(player) / ModelConstants.HAUL_FOOD_USAGE_DIVISOR);

            // TODO
            //foreach (AlterationEffect alt in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)))
            //    d += alt.FoodUsagePerTurn;

            return Math.Max(0, d);
        }
        public double GetCriticalHitProbability(Player player)
        {
            double d = ModelConstants.CRITICAL_HIT_BASE;

            // TODO
            //foreach (AlterationEffect alt in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)))
            //    d += alt.CriticalHit;

            return Math.Max(0, d);
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
            // TODO
            //this.MalignAuraEffects.Clear();
            //this.MalignAuraEffects.AddRange(passiveAuraEffects);

            //Normal turn stuff
            player.Hp += _characterProcessor.GetHpRegen(player, regenerate);
            player.Mp += _characterProcessor.GetMpRegen(player);

            player.Hunger += GetFoodUsagePerTurn(player);

            if (player.Experience >= CalculateExperienceNext(player))
            {
                CalculateLevelGains(player);

                //Bonus health and magic refill
                player.Hp = player.HpMax;
                player.Mp = player.MpMax;
            }

            // TODO
            ////Normal temporary effects
            //for (int i = this.ActiveTemporaryEffects.Count - 1; i >= 0; i--)
            //{
            //    //Check temporary event time
            //    this.ActiveTemporaryEffects[i].EventTime--;
            //    if (this.ActiveTemporaryEffects[i].EventTime < 0)
            //    {
            //        msgs.Add(new LevelMessageEventArgs(this.ActiveTemporaryEffects[i].PostEffectString));
            //        this.ActiveTemporaryEffects.RemoveAt(i);
            //    }
            //}

            ////Attack attribute temporary effects
            //for (int i = this.AttackAttributeTemporaryFriendlyEffects.Count - 1; i >= 0; i--)
            //{
            //    this.AttackAttributeTemporaryFriendlyEffects[i].EventTime--;
            //    if (this.AttackAttributeTemporaryFriendlyEffects[i].EventTime < 0)
            //    {
            //        msgs.Add(new LevelMessageEventArgs(this.AttackAttributeTemporaryFriendlyEffects[i].PostEffectString));
            //        this.AttackAttributeTemporaryFriendlyEffects.RemoveAt(i);
            //    }
            //}
            //for (int i = this.AttackAttributeTemporaryMalignEffects.Count - 1; i >= 0; i--)
            //{
            //    this.AttackAttributeTemporaryMalignEffects[i].EventTime--;
            //    if (this.AttackAttributeTemporaryMalignEffects[i].EventTime < 0)
            //    {
            //        msgs.Add(new LevelMessageEventArgs(this.AttackAttributeTemporaryMalignEffects[i].PostEffectString));
            //        this.AttackAttributeTemporaryMalignEffects.RemoveAt(i);
            //    }
            //}

            ////Apply per step alteration costs
            //foreach (AlterationCost alt in this.PerStepAlterationCosts)
            //{
            //    this.AgilityBase -= alt.Agility;
            //    this.AuraRadiusBase -= alt.AuraRadius;
            //    this.Experience -= alt.Experience;
            //    this.FoodUsagePerTurnBase += alt.FoodUsagePerTurn;
            //    this.Hp -= alt.Hp;
            //    this.Hunger += alt.Hunger;
            //    this.IntelligenceBase -= alt.Intelligence;
            //    this.Mp -= alt.Mp;
            //    this.StrengthBase -= alt.Strength;
            //}

            //Maintain aura effects
            //foreach (SkillSet s in this.Player.Skills)
            //{
            //    //Maintain aura effects
            //    LevelMessageEventArgs args = null;
            //    if (s.IsTurnedOn && !Calculator.CalculatePlayerMeetsAlterationCost(s.CurrentSkill.Cost, this.Player, out args))
            //    {
            //        s.IsTurnedOn = false;
            //        PublishScenarioMessage(args.Message);
            //        PublishScenarioMessage("Deactivating " + s.RogueName);

            //        if (s.CurrentSkill.Type == AlterationType.PassiveAura)
            //            this.Player.DeactivatePassiveAura(s.CurrentSkill.Id);

            //        else
            //            this.Player.DeactivatePassiveEffect(s.CurrentSkill.Id);
            //    }
            //}

            //Identify new skills
            //foreach (SkillSet s in this.Player.Skills)
            //{
            //    //Check for new learned skills
            //    if (this.Player.Level >= s.LevelLearned && !s.IsLearned)
            //    {
            //        this.Encyclopedia[s.RogueName].IsIdentified = true;
            //        s.IsLearned = true;
            //        //OnPlayerAdvancementEvent(this, new PlayerAdvancementEventArgs(this.Player.Rogue2Name + " Has Learned A New Skill!", new string[]{s.Rogue2Name}));
            //    }
            //}

            ////Message for effects that player can't support
            //foreach (AlterationEffect effect in this.Alterations.Where(alt => !alt.ProjectCharacterCanSupport(this)))
            //    msgs.Add(new LevelMessageEventArgs(effect.PostEffectString));

            ApplyLimits(player);
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
        public void ApplyLimits(Player player)
        {
            if (player.Hp < 0)
                player.Hp = 0;

            if (player.Mp < 0)
                player.Mp = 0;

            if (player.Hp > player.HpMax)
                player.Hp = player.HpMax;

            if (player.Mp > player.MpMax)
                player.Mp = player.MpMax;

            if (player.Hunger < 0)
                player.Hunger = 0;

            if (player.StrengthBase < 0)
                player.StrengthBase = 0;

            if (player.AgilityBase < 0)
                player.AgilityBase = 0;

            if (player.IntelligenceBase < 0)
                player.IntelligenceBase = 0;

            if (player.AuraRadiusBase < 0)
                player.AuraRadiusBase = 0;

            if (player.FoodUsagePerTurnBase < 0)
                player.FoodUsagePerTurnBase = 0;

            if (player.Experience < 0)
                player.Experience = 0;
        }
    }
}
