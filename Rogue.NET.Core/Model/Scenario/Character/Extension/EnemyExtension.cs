using Rogue.NET.Core.Logic.Static;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Model.Scenario.Character.Extension
{
    public static class EnemyExtension
    {
        public static double GetMpRegen(this Enemy enemy)
        {
            return enemy.MpRegenBase + enemy.Alteration.GetAlterations().Sum(x => x.MpPerStep);
        }
        public static double GetHpRegen(this Enemy enemy, bool regenerate)
        {
            var result = regenerate ? enemy.HpRegenBase : 0;

            // Normal alteration effects
            if (regenerate)
                result += enemy.Alteration.GetAlterations().Sum(x => x.HpPerStep);

            // Malign attack attribute contributions
            foreach (var malignEffect in enemy.Alteration.GetTemporaryAttackAttributeAlterations(false))
            {
                foreach (var malignAttribute in malignEffect.AttackAttributes)
                {
                    double resistance = 0;
                    double weakness = 0;
                    double attack = malignAttribute.Attack;

                    // Friendly attack attribute contributions
                    foreach (var friendlyEffect in enemy.Alteration.GetTemporaryAttackAttributeAlterations(true))
                    {
                        resistance += friendlyEffect.AttackAttributes.First(z => z.RogueName == malignAttribute.RogueName).Resistance;
                        weakness += friendlyEffect.AttackAttributes.First(z => z.RogueName == malignAttribute.RogueName).Weakness;
                    }

                    // Passive attack attribute contributions
                    foreach (var passiveEffect in enemy.Alteration.GetPassiveAttackAttributeAlterations())
                    {
                        resistance += passiveEffect.AttackAttributes.First(z => z.RogueName == malignAttribute.RogueName).Resistance;
                        weakness += passiveEffect.AttackAttributes.First(z => z.RogueName == malignAttribute.RogueName).Weakness;
                    }

                    // Equipment contributions
                    foreach (var equipment in enemy.Equipment.Where(z => z.Value.IsEquipped).Select(x => x.Value))
                    {
                        resistance += equipment.AttackAttributes.First(z => z.RogueName == malignAttribute.RogueName).Resistance;
                        weakness += equipment.AttackAttributes.First(z => z.RogueName == malignAttribute.RogueName).Weakness;
                    }

                    // Enemy's Natural Attack Attributes
                    var enemyAttackAttribute = enemy.AttackAttributes[malignAttribute.RogueName];

                    resistance += enemyAttackAttribute.Resistance;
                    weakness += enemyAttackAttribute.Weakness;

                    result -= Calculator.CalculateAttackAttributeMelee(attack, resistance, weakness);
                }
            }

            return result;
        }
        public static double GetStrength(this Enemy enemy)
        {
            var result = enemy.StrengthBase;

            result += enemy.Alteration.GetAlterations().Sum(x => x.Strength);

            return Math.Max(0.1, result);
        }
        public static double GetAgility(this Enemy enemy)
        {
            var result = enemy.AgilityBase;

            result += enemy.Alteration.GetAlterations().Sum(x => x.Agility);

            return Math.Max(0.1, result);
        }
        public static double GetIntelligence(this Enemy enemy)
        {
            var result = enemy.IntelligenceBase;

            result += enemy.Alteration.GetAlterations().Sum(x => x.Intelligence);

            return Math.Max(0.1, result);
        }
        public static double GetAuraRadius(this Enemy enemy)
        {
            var result = enemy.AuraRadiusBase;

            result += enemy.Alteration.GetAlterations().Sum(x => x.AuraRadius);

            return Math.Max(0.1, result);
        }
        public static double GetMagicBlock(this Enemy enemy)
        {
            var result = enemy.GetIntelligence() / 100;

            result += enemy.Alteration.GetAlterations().Sum(x => x.MagicBlockProbability);

            return Math.Max(Math.Min(1, result), 0);
        }
        public static double GetDodge(this Enemy enemy)
        {
            var result = enemy.GetAgility() / 100;

            result += enemy.Alteration.GetAlterations().Sum(x => x.DodgeProbability);

            return Math.Max(Math.Min(1, result), 0);
        }
        public static double GetSpeed(this Enemy enemy)
        {
            var speed = enemy.SpeedBase + enemy.Alteration.GetAlterations().Sum(x => x.Speed);

            // 0.1 < speed < 1
            return Math.Max(Math.Min(1, speed), 0.1);
        }

        public static double GetAttack(this Enemy enemy)
        {
            double result = enemy.GetStrength();

            foreach (var equipment in enemy.Equipment.Values.Where(x => x.IsEquipped))
            {
                switch (equipment.Type)
                {
                    case EquipmentType.OneHandedMeleeWeapon:
                        result += ((equipment.Class + 1) * equipment.Quality);
                        break;
                    case EquipmentType.TwoHandedMeleeWeapon:
                        result += ((equipment.Class + 1) * equipment.Quality) * 2;
                        break;
                    case EquipmentType.RangeWeapon:
                        result += ((equipment.Class + 1) * equipment.Quality) / 2;
                        break;
                }
            }

            result += enemy.Alteration.GetAlterations().Sum(x => x.Attack);

            return Math.Max(0, result);
        }
        public static double GetDefense(this Enemy enemy)
        {
            var defense = enemy.GetStrength() / 5.0D;

            foreach (var equipment in enemy.Equipment.Values.Where(x => x.IsEquipped))
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

            defense += enemy.Alteration.GetAlterations().Sum(x => x.Defense);

            return Math.Max(0, defense);
        }

        public static double GetCriticalHitProbability(this Enemy enemy)
        {
            var result = ModelConstants.CriticalHitBase;

            result += enemy.Alteration.GetAlterations().Sum(x => x.CriticalHit);

            return Math.Max(0, result);
        }

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

        public static bool IsConfused(this Enemy enemy)
        {
            return enemy.Alteration.GetAlterations().Any(x => x.State == CharacterStateType.Confused);
        }
        public static bool IsMuted(this Enemy enemy)
        {
            return enemy.Alteration.GetAlterations().Any(x => x.State == CharacterStateType.Silenced);
        }
        public static bool IsParalyzed(this Enemy enemy)
        {
            return enemy.Alteration.GetAlterations().Any(x => x.State == CharacterStateType.Paralyzed);
        }
        public static bool IsBlind(this Enemy enemy)
        {
            return enemy.Alteration.GetAlterations().Any(x => x.State == CharacterStateType.Blind);
        }
        public static bool IsSleeping(this Enemy enemy)
        {
            return enemy.Alteration.GetAlterations().Any(x => x.State == CharacterStateType.Sleeping);
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
