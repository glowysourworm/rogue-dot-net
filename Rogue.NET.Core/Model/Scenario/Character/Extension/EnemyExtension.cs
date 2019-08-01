using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Logic.Content.Interface;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Model.Scenario.Character.Extension
{
    public static class EnemyExtension
    {
        /// <summary>
        /// Returns true if the enemy is equipped to fire it's range weapon.
        /// </summary>
        public static bool IsRangeMelee(this Enemy enemy)
        {
            var rangeWeapon = enemy.Equipment.Values.FirstOrDefault(x => x.IsEquipped && x.Type == EquipmentType.RangeWeapon);

            // Check for ammunition
            if (rangeWeapon != null)
            {
                var ammo = enemy.Consumables
                                .Values
                                .FirstOrDefault(x => x.RogueName == rangeWeapon.AmmoName);

                return ammo != null;
            }

            return false;
        }

        public static void ApplyLimits(this Enemy enemy)
        {
            if (enemy.Mp < 0)
                enemy.Mp = 0;

            if (enemy.Hp > enemy.HpMax)
                enemy.Hp = enemy.HpMax;

            if (enemy.Mp > enemy.MpMax)
                enemy.Mp = enemy.MpMax;

            if (enemy.SpeedBase < ModelConstants.MinSpeed)
                enemy.SpeedBase = ModelConstants.MinSpeed;

            if (enemy.SpeedBase > ModelConstants.MaxSpeed)
                enemy.SpeedBase = ModelConstants.MaxSpeed;

            if (enemy.StrengthBase < 0)
                enemy.StrengthBase = 0;

            if (enemy.AgilityBase < 0)
                enemy.AgilityBase = 0;

            if (enemy.IntelligenceBase < 0)
                enemy.IntelligenceBase = 0;
        }
    }
}
