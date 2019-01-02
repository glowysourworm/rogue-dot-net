using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Scenario.Alteration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Model.Scenario.Character.Extension
{
    public static class PlayerExtension
    {
        public static double GetFoodUsagePerTurn(this Player player)
        {
            var result = player.FoodUsagePerTurnBase + (player.GetHaul() / ModelConstants.HaulFoodUsageDivisor);

            result += player.Alteration.GetAlterations().Sum(x => x.FoodUsagePerTurn);

            return Math.Max(0, result);
        }

        /// <summary>
        /// Returns effective attack attributes for use with direct melee calculation. Scenario Attributes are empty containers for scenario 
        /// defined attack attributes. NOTE - THESE SHOULD NOT BE REFERENCED.
        /// </summary>
        public static IEnumerable<AttackAttribute> GetMeleeAttributes(this Player player, IEnumerable<AttackAttribute> scenarioAttributes)
        {
            var result = new List<AttackAttribute>();

            // Base Attributes
            foreach (var baseAttribute in scenarioAttributes)
                result.Add(baseAttribute.DeepClone());

            // Zero out Attack / Resistance
            result.ForEach(x => { x.Attack = 0; x.Resistance = 0; });

            // Friendly attack attribute contributions
            foreach (var friendlyAttackAttributes in player.Alteration.GetTemporaryAttackAttributeAlterations(true).Select(x => x.AttackAttributes))
            {
                foreach (var attribute in result)
                {
                    attribute.Resistance += friendlyAttackAttributes.First(y => y.RogueName == attribute.RogueName).Resistance;
                }
            }

            // Passive attack attribute contributions
            foreach (var passiveAttackAttributes in player.Alteration.GetPassiveAttackAttributeAlterations().Select(x => x.AttackAttributes))
            {
                foreach (var attribute in result)
                {
                    var passiveAttribute = passiveAttackAttributes.First(y => y.RogueName == attribute.RogueName);

                    attribute.Attack += passiveAttribute.Attack;
                    attribute.Resistance += passiveAttribute.Resistance;
                }
            }

            //Equipment contributions
            foreach (var equipment in player.Equipment.Values.Where(z => z.IsEquipped))
            {
                foreach (var attribute in result)
                {
                    var passiveAttribute = equipment.AttackAttributes.First(y => y.RogueName == attribute.RogueName);

                    attribute.Attack += passiveAttribute.Attack;
                    attribute.Resistance += passiveAttribute.Resistance;
                }
            }

            // Filter result by attributes that apply to strength base combat ONLY
            return result.Where(x => x.AppliesToStrengthBasedCombat);
        }

        public static void ApplyLimits(this Player player)
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

            if (player.SpeedBase < ModelConstants.MinSpeed)
                player.SpeedBase = ModelConstants.MinSpeed;

            if (player.SpeedBase > ModelConstants.MaxSpeed)
                player.SpeedBase = ModelConstants.MaxSpeed;

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
