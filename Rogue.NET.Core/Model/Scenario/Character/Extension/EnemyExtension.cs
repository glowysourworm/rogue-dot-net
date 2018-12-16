using Rogue.NET.Core.Logic.Static;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Content.Item.Extension;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Model.Scenario.Character.Extension
{
    public static class EnemyExtension
    {
        /// <summary>
        /// Returns effective attack attributes for use with direct melee calculation
        /// </summary>
        public static IEnumerable<AttackAttribute> GetMeleeAttributes(this Enemy enemy)
        {
            var result = new List<AttackAttribute>();

            // Enemy Base Attributes
            foreach (var baseAttribute in enemy.AttackAttributes)
                result.Add(new AttackAttribute()
                {
                    RogueName = baseAttribute.Value.RogueName,
                    CharacterColor = baseAttribute.Value.CharacterColor,
                    CharacterSymbol = baseAttribute.Value.CharacterSymbol,
                    Icon = baseAttribute.Value.Icon,
                    SmileyAuraColor = baseAttribute.Value.SmileyAuraColor,
                    SmileyBodyColor = baseAttribute.Value.SmileyBodyColor,
                    SmileyLineColor = baseAttribute.Value.SmileyLineColor,
                    SmileyMood = baseAttribute.Value.SmileyMood,
                    SymbolType = baseAttribute.Value.SymbolType,
                    Resistance = baseAttribute.Value.Resistance,
                    Weakness = baseAttribute.Value.Weakness,
                    Attack = baseAttribute.Value.Attack
                });

            // Friendly attack attribute contributions
            foreach (var friendlyAttackAttributes in enemy.Alteration.GetTemporaryAttackAttributeAlterations(true).Select(x => x.AttackAttributes))
            {
                foreach (var attribute in result)
                {
                    attribute.Resistance += friendlyAttackAttributes.First(y => y.RogueName == attribute.RogueName).Resistance;
                }
            }

            // Passive attack attribute contributions
            foreach (var passiveAttackAttributes in enemy.Alteration.GetPassiveAttackAttributeAlterations().Select(x => x.AttackAttributes))
            {
                foreach (var attribute in result)
                {
                    var passiveAttribute = passiveAttackAttributes.First(y => y.RogueName == attribute.RogueName);

                    attribute.Attack += passiveAttribute.Attack;
                    attribute.Resistance += passiveAttribute.Resistance;
                    attribute.Weakness += passiveAttribute.Weakness;
                }
            }

            //Equipment contributions
            foreach (var equipment in enemy.Equipment.Values.Where(z => z.IsEquipped))
            {
                foreach (var attribute in result)
                {
                    var passiveAttribute = equipment.AttackAttributes.First(y => y.RogueName == attribute.RogueName);

                    attribute.Attack += passiveAttribute.Attack;
                    attribute.Resistance += passiveAttribute.Resistance;
                    attribute.Weakness += passiveAttribute.Weakness;
                }
            }

            return result;
        }

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
