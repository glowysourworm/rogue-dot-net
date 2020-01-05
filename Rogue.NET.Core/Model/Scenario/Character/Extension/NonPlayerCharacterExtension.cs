using Rogue.NET.Core.Model.Enums;
using System.Linq;

namespace Rogue.NET.Core.Model.Scenario.Character.Extension
{
    public static class NonPlayerCharacterExtension
    {
        /// <summary>
        /// Returns true if the enemy is equipped to fire it's range weapon.
        /// </summary>
        public static bool IsEquippedRangeCombat(this NonPlayerCharacter character)
        {
            // NOTE*** Non-Player Characters currently don't support equipping / picking up
            //         items. So, this will pass through.
            //
            var rangeWeapon = character.Equipment       
                                       .Values
                                       .FirstOrDefault(x => x.IsEquipped && 
                                                            x.Type == EquipmentType.RangeWeapon);

            // Check for ammunition
            if (rangeWeapon != null)
            {
                var ammo = character.Consumables
                                    .Values
                                    .FirstOrDefault(x => x.RogueName == rangeWeapon.AmmoName);

                return ammo != null;
            }

            return false;
        }
        public static void ApplyLimits(this NonPlayerCharacter character)
        {
            if (character.Stamina < 0)
                character.Stamina = 0;

            if (character.Health > character.HealthMax)
                character.Health = character.HealthMax;

            if (character.Stamina > character.StaminaMax)
                character.Stamina = character.StaminaMax;

            if (character.SpeedBase < ModelConstants.MinSpeed)
                character.SpeedBase = ModelConstants.MinSpeed;

            if (character.SpeedBase > ModelConstants.MaxSpeed)
                character.SpeedBase = ModelConstants.MaxSpeed;

            if (character.StrengthBase < 0)
                character.StrengthBase = 0;

            if (character.AgilityBase < 0)
                character.AgilityBase = 0;

            if (character.IntelligenceBase < 0)
                character.IntelligenceBase = 0;
        }
    }
}
