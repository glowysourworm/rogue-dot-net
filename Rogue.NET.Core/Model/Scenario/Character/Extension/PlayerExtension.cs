using Rogue.NET.Core.Model.Enums;
using System;

namespace Rogue.NET.Core.Model.Scenario.Character.Extension
{
    public static class PlayerExtension
    {
        public static double GetFoodUsagePerTurn(this Player player)
        {
            var result = player.FoodUsagePerTurnBase + (player.GetHaul() / ModelConstants.HaulFoodUsageDivisor);

            // Alteration
            result += player.Alteration.GetAttribute(CharacterAttribute.FoodUsagePerTurn);

            return System.Math.Max(0, result);
        }

        public static void ApplyLimits(this Player player)
        {
            if (player.Health < 0)
                player.Health = 0;

            if (player.Stamina < 0)
                player.Stamina = 0;

            if (player.Health > player.HealthMax)
                player.Health = player.HealthMax;

            if (player.Stamina > player.StaminaMax)
                player.Stamina = player.StaminaMax;

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

            if (player.VisionBase < 0)
                player.VisionBase = 0;

            if (player.FoodUsagePerTurnBase < 0)
                player.FoodUsagePerTurnBase = 0;

            if (player.Experience < 0)
                player.Experience = 0;
        }

        /// <summary>
        /// Gets character attribute using provided enum
        /// </summary>
        public static double GetAttribute(this Player player, CharacterAttribute attribute)
        {
            switch (attribute)
            {
                case CharacterAttribute.Health:
                    return player.Health;
                case CharacterAttribute.Stamina:
                    return player.Stamina;
                case CharacterAttribute.Strength:
                    return player.GetStrength();
                case CharacterAttribute.Agility:
                    return player.GetAgility();
                case CharacterAttribute.Intelligence:
                    return player.GetIntelligence();
                case CharacterAttribute.Speed:
                    return player.GetSpeed();
                case CharacterAttribute.HealthRegen:
                    return player.GetTotalHealthRegen();
                case CharacterAttribute.StaminaRegen:
                    return player.GetTotalStaminaRegen();
                case CharacterAttribute.Vision:
                    return player.GetVision();
                case CharacterAttribute.Attack:
                    return player.GetAttack();
                case CharacterAttribute.Defense:
                    return player.GetDefense();
                case CharacterAttribute.FoodUsagePerTurn:
                    return player.GetFoodUsagePerTurn();
                default:
                    throw new Exception("Unknown CharacterAttribute PlayerExtension.Get");
            }
        }
    }
}
