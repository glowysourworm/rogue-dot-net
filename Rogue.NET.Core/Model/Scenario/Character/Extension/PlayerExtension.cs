using Rogue.NET.Core.Logic.Static;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Scenario.Character.Extension
{
    public static class PlayerExtension
    {
        public static double GetMpRegen(this Player player)
        {
            return player.MpRegenBase + player.Alteration.GetAlterations().Sum(x => x.MpPerStep);
        }
        public static double GetHpRegen(this Player player, bool regenerate)
        {
            var result = regenerate ? player.HpRegenBase : 0;

            // Normal alteration effects
            if (regenerate)
                result += player.Alteration.GetAlterations().Sum(x => x.HpPerStep);

            // Malign attack attribute contributions
            foreach (var malignEffect in player.Alteration.GetTemporaryAttackAttributeAlterations(false))
            {
                foreach (var malignAttribute in malignEffect.AttackAttributes)
                {
                    double resistance = 0;
                    double weakness = 0;
                    double attack = malignAttribute.Attack;

                    // Friendly attack attribute contributions
                    foreach (var friendlyEffect in player.Alteration.GetTemporaryAttackAttributeAlterations(true))
                    {
                        resistance += friendlyEffect.AttackAttributes.First(z => z.RogueName == malignAttribute.RogueName).Resistance;
                        weakness += friendlyEffect.AttackAttributes.First(z => z.RogueName == malignAttribute.RogueName).Weakness;
                    }

                    // Passive attack attribute contributions
                    foreach (var passiveEffect in player.Alteration.GetPassiveAttackAttributeAlterations())
                    {
                        resistance += passiveEffect.AttackAttributes.First(z => z.RogueName == malignAttribute.RogueName).Resistance;
                        weakness += passiveEffect.AttackAttributes.First(z => z.RogueName == malignAttribute.RogueName).Weakness;
                    }

                    // Equipment contributions
                    foreach (var equipment in player.Equipment.Where(z => z.Value.IsEquipped).Select(x => x.Value))
                    {
                        resistance += equipment.AttackAttributes.First(z => z.RogueName == malignAttribute.RogueName).Resistance;
                        weakness += equipment.AttackAttributes.First(z => z.RogueName == malignAttribute.RogueName).Weakness;
                    }

                    result -= Calculator.CalculateAttackAttributeMelee(attack, resistance, weakness);
                }
            }

            return result;
        }
        public static double GetStrength(this Player player)
        {
            var result = player.StrengthBase;

            result += player.Alteration.GetAlterations().Sum(x => x.Strength);

            return Math.Max(0.1, result);
        }
        public static double GetAgility(this Player player)
        {
            var result = player.AgilityBase;

            result += player.Alteration.GetAlterations().Sum(x => x.Agility);

            return Math.Max(0.1, result);
        }
        public static double GetIntelligence(this Player player)
        {
            var result = player.IntelligenceBase;

            result += player.Alteration.GetAlterations().Sum(x => x.Intelligence);

            return Math.Max(0.1, result);
        }
        public static double GetAuraRadius(this Player player)
        {
            var result = player.AuraRadiusBase;

            result += player.Alteration.GetAlterations().Sum(x => x.AuraRadius);

            return Math.Max(0.1, result);
        }
        public static double GetMagicBlock(this Player player)
        {
            var result = player.GetIntelligence() / 100;

            result += player.Alteration.GetAlterations().Sum(x => x.MagicBlockProbability);

            return Math.Max(Math.Min(1, result), 0);
        }
        public static double GetDodge(this Player player)
        {
            var result = player.GetAgility() / 100;

            result += player.Alteration.GetAlterations().Sum(x => x.DodgeProbability);

            return Math.Max(Math.Min(1, result), 0);
        }
        public static double GetSpeed(this Player player)
        {
            var speed = player.SpeedBase + player.Alteration.GetAlterations().Sum(x => x.Speed);

            // 0.1 < speed < 1
            return Math.Max(Math.Min(1, speed), 0.1);
        }

        public static double GetAttack(this Player player)
        {
            double result = player.GetStrength();

            foreach (var equipment in player.Equipment.Values.Where(x => x.IsEquipped))
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

            result += player.Alteration.GetAlterations().Sum(x => x.Attack);

            return Math.Max(0, result);
        }
        public static double GetDefense(this Player player)
        {
            var defense = player.GetStrength() / 5.0D;

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

            defense += player.Alteration.GetAlterations().Sum(x => x.Defense);

            return Math.Max(0, defense);
        }
        public static double GetFoodUsagePerTurn(this Player player)
        {
            var result = player.FoodUsagePerTurnBase + (player.GetHaul() / ModelConstants.HaulFoodUsageDivisor);

            result += player.Alteration.GetAlterations().Sum(x => x.FoodUsagePerTurn);

            return Math.Max(0, result);
        }
        public static double GetCriticalHitProbability(this Player player)
        {
            var result = ModelConstants.CriticalHitBase;

            result += player.Alteration.GetAlterations().Sum(x => x.CriticalHit);

            return Math.Max(0, result);
        }

        /// <summary>
        /// Returns effective attack attributes for use with direct melee calculation. Scenario Attributes are empty containers for scenario 
        /// defined attack attributes. NOTE - THESE SHOULD NOT BE REFERENCED.
        /// </summary>
        public static IEnumerable<AttackAttribute> GetMeleeAttributes(this Player player, IEnumerable<AttackAttribute> scenarioAttributes)
        {
            var result = new List<AttackAttribute>();

            // Enemy Base Attributes
            foreach (var baseAttribute in scenarioAttributes)
                result.Add(new AttackAttribute()
                {
                    RogueName = baseAttribute.RogueName,
                    CharacterColor = baseAttribute.CharacterColor,
                    CharacterSymbol = baseAttribute.CharacterSymbol,
                    Icon = baseAttribute.Icon,
                    SmileyAuraColor = baseAttribute.SmileyAuraColor,
                    SmileyBodyColor = baseAttribute.SmileyBodyColor,
                    SmileyLineColor = baseAttribute.SmileyLineColor,
                    SmileyMood = baseAttribute.SmileyMood,
                    SymbolType = baseAttribute.SymbolType,
                    Resistance = 0,
                    Weakness = 0,
                    Attack = 0
                });

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
                    attribute.Weakness += passiveAttribute.Weakness;
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
                    attribute.Weakness += passiveAttribute.Weakness;
                }
            }

            return result;
        }

        public static bool Is(this Player player, CharacterStateType characterStateType)
        {
            return player.Alteration.GetAlterations().Any(x => x.State.BaseType == characterStateType);
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
