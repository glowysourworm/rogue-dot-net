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

            // Alteration
            result += player.Alteration.GetAlterations().Sum(x => x.FoodUsagePerTurn);

            // Religious Alteration
            if (player.ReligiousAlteration.HasAttributeEffect)
                result += player.ReligiousAlteration.AttributeEffect.FoodUsagePerTurn;

            return Math.Max(0, result);
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
