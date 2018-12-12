using Rogue.NET.Core.Logic.Static;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Item.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Scenario.Character.Extension
{
    public static class CharacterExtension
    {
        public static double GetMagicBlockBase(this Character character)
        {
            return character.IntelligenceBase / 100;
        }
        public static double GetDodgeBase(this Character character)
        {
            return character.AgilityBase / 100;
        }
        public static double GetHaulMax(this Character character)
        {
            return character.StrengthBase * ModelConstants.HaulMaxStrengthMultiplier;
        }
        public static double GetStrengthBase(this Character character)
        {
            return character.StrengthBase;
        }
        public static double GetHaul(this Character character)
        {
            return character.Equipment.Values.Sum(x => x.Weight) +
                   character.Consumables.Values.Sum(x => x.Weight);
        }
        public static double GetAttackBase(this Character character)
        {
            return character.StrengthBase;
        }
        public static double GetDefenseBase(this Character character)
        {
            return character.GetStrengthBase() / 5.0D;
        }

        public static double GetMpRegen(this Character character)
        {
            return character.MpRegenBase + character.Alteration.GetAlterations().Sum(x => x.MpPerStep);
        }
        public static double GetHpRegen(this Character character, bool regenerate)
        {
            var result = regenerate ? character.HpRegenBase : 0;

            // Normal alteration effects
            if (regenerate)
                result += character.Alteration.GetAlterations().Sum(x => x.HpPerStep);

            // Malign attack attribute contributions
            foreach (var malignEffect in character.Alteration.GetTemporaryAttackAttributeAlterations(false))
            {
                foreach (var malignAttribute in malignEffect.AttackAttributes)
                {
                    double resistance = 0;
                    double weakness = 0;
                    double attack = malignAttribute.Attack;

                    // Friendly attack attribute contributions
                    foreach (var friendlyEffect in character.Alteration.GetTemporaryAttackAttributeAlterations(true))
                    {
                        resistance += friendlyEffect.AttackAttributes.First(z => z.RogueName == malignAttribute.RogueName).Resistance;
                        weakness += friendlyEffect.AttackAttributes.First(z => z.RogueName == malignAttribute.RogueName).Weakness;
                    }

                    // Passive attack attribute contributions
                    foreach (var passiveEffect in character.Alteration.GetPassiveAttackAttributeAlterations())
                    {
                        resistance += passiveEffect.AttackAttributes.First(z => z.RogueName == malignAttribute.RogueName).Resistance;
                        weakness += passiveEffect.AttackAttributes.First(z => z.RogueName == malignAttribute.RogueName).Weakness;
                    }

                    // Equipment contributions
                    foreach (var equipment in character.Equipment.Where(z => z.Value.IsEquipped).Select(x => x.Value))
                    {
                        resistance += equipment.AttackAttributes.First(z => z.RogueName == malignAttribute.RogueName).Resistance;
                        weakness += equipment.AttackAttributes.First(z => z.RogueName == malignAttribute.RogueName).Weakness;
                    }

                    result -= Calculator.CalculateAttackAttributeMelee(attack, resistance, weakness);
                }
            }

            return result;
        }
        public static double GetStrength(this Character character)
        {
            var result = character.StrengthBase;

            result += character.Alteration.GetAlterations().Sum(x => x.Strength);

            return Math.Max(0.1, result);
        }
        public static double GetAgility(this Character character)
        {
            var result = character.AgilityBase;

            result += character.Alteration.GetAlterations().Sum(x => x.Agility);

            return Math.Max(0.1, result);
        }
        public static double GetIntelligence(this Character character)
        {
            var result = character.IntelligenceBase;

            result += character.Alteration.GetAlterations().Sum(x => x.Intelligence);

            return Math.Max(0.1, result);
        }
        public static double GetAuraRadius(this Character character)
        {
            var result = character.AuraRadiusBase;

            result += character.Alteration.GetAlterations().Sum(x => x.AuraRadius);

            return Math.Max(0.1, result);
        }
        public static double GetMagicBlock(this Character character)
        {
            var result = character.GetIntelligence() / 100;

            result += character.Alteration.GetAlterations().Sum(x => x.MagicBlockProbability);

            return Math.Max(Math.Min(1, result), 0);
        }
        public static double GetDodge(this Character character)
        {
            var result = character.GetAgility() / 100;

            result += character.Alteration.GetAlterations().Sum(x => x.DodgeProbability);

            return Math.Max(Math.Min(1, result), 0);
        }
        public static double GetSpeed(this Character character)
        {
            var speed = character.SpeedBase + character.Alteration.GetAlterations().Sum(x => x.Speed);

            // 0.1 < speed < 1
            return Math.Max(Math.Min(1, speed), 0.1);
        }

        public static double GetAttack(this Character character)
        {
            var equipmentAttackValue = character.Equipment
                                             .Values
                                             .Where(x => x.IsEquipped && x.IsWeaponType())
                                             .Sum(x => x.GetAttackValue());

            // Calculate strength * equipment base attack value
            var attackValue = MeleeCalculator.GetAttackValue(equipmentAttackValue, character.GetStrength());

            // Add on alteration contributions
            var result = attackValue + character.Alteration.GetAlterations().Sum(x => x.Attack);

            return Math.Max(0, result);
        }
        public static double GetDefense(this Character character)
        {
            var equipmentDefenseValue = character.Equipment
                                                 .Values
                                                 .Where(x => x.IsEquipped && x.IsArmorType())
                                                 .Sum(x => x.GetDefenseValue());

            // Calculate strength * equipment base defense value
            var defenseValue = MeleeCalculator.GetDefenseValue(equipmentDefenseValue, character.GetStrength());

            // Add on alteration contributions
            var result = defenseValue + character.Alteration.GetAlterations().Sum(x => x.Defense);

            return Math.Max(0, result);
        }
        public static double GetCriticalHitProbability(this Character character)
        {
            var result = ModelConstants.CriticalHitBase;

            result += character.Alteration.GetAlterations().Sum(x => x.CriticalHit);

            return Math.Max(0, result);
        }

        public static bool Is(this Character character, CharacterStateType characterStateType)
        {
            return character.Alteration.GetAlterations().Any(x => x.State.BaseType == characterStateType);
        }
    }
}
