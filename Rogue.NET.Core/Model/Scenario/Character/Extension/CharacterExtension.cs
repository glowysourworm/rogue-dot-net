using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Logic.Content.Enum;
using Rogue.NET.Core.Logic.Static;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Content.Item.Extension;
using System;
using System.Collections.Generic;
using System.Linq;

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
            return character.GetStrengthBase() * ModelConstants.Melee.DefenseBaseMultiplier;
        }
        public static double GetMpRegen(this Character character)
        {
            return character.MpRegenBase + character.Alteration.GetAlterations().Sum(x => x.MpPerStep);
        }
        public static double GetHpRegen(this Character character)
        {
            return character.HpRegenBase + character.Alteration.GetAlterations().Sum(x => x.HpPerStep);
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
            var result = character.GetMagicBlockBase();

            result += character.Alteration.GetAlterations().Sum(x => x.MagicBlockProbability);

            return Math.Max(Math.Min(1, result), 0);
        }
        public static double GetDodge(this Character character)
        {
            var result = character.GetDodgeBase();

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

            return Math.Min(Math.Max(0, result), 1);
        }

        /// <summary>
        /// Returns effective attack attributes for use with direct melee calculation
        /// </summary>
        public static IEnumerable<AttackAttribute> GetMeleeAttributes(this Character character, IEnumerable<AttackAttribute> scenarioAttributes)
        {
            // Create base attribute list from the scenario list
            var result = scenarioAttributes.DeepClone();

            // For Enemy characters - add on the intrinsic attributes
            if (character is Enemy)
            {
                var enemy = character as Enemy;

                // Add contributions from enemy list
                result = result.Join(enemy.AttackAttributes.Values, x => x.RogueName, y => y.RogueName, (x, y) =>
                {
                    x.Attack += y.Attack;
                    x.Resistance += y.Resistance;

                    return x;
                });
            }

            // Friendly attack attribute contributions
            foreach (var friendlyAttackAttributes in character.Alteration.GetTemporaryAttackAttributeAlterations(true).Select(x => x.AttackAttributes))
            {
                foreach (var attribute in result)
                {
                    attribute.Resistance += friendlyAttackAttributes.First(y => y.RogueName == attribute.RogueName).Resistance;
                }
            }

            // Passive attack attribute contributions
            foreach (var passiveAttackAttributes in character.Alteration.GetPassiveAttackAttributeAlterations().Select(x => x.AttackAttributes))
            {
                foreach (var attribute in result)
                {
                    var passiveAttribute = passiveAttackAttributes.First(y => y.RogueName == attribute.RogueName);

                    attribute.Attack += passiveAttribute.Attack;
                    attribute.Resistance += passiveAttribute.Resistance;
                }
            }

            //Equipment contributions
            foreach (var equipment in character.Equipment.Values.Where(z => z.IsEquipped))
            {
                foreach (var attribute in result)
                {
                    var passiveAttribute = equipment.AttackAttributes.First(y => y.RogueName == attribute.RogueName);

                    attribute.Attack += passiveAttribute.Attack;
                    attribute.Resistance += passiveAttribute.Resistance;
                }
            }

            // Filter by attributes that apply to strength based combat ONLY.
            return result;
        }


        /// <summary>
        /// Returns the end-of-turn malign attack attribute contribution used at the end
        /// of each character turn.
        /// </summary>
        public static double GetMalignAttackAttributeHit(this Character character, IEnumerable<AttackAttribute> scenarioAttributes)
        {
            var result = 0D;

            // Get character effective attack attributes
            var attackAttributes = character.GetMeleeAttributes(scenarioAttributes);

            // Malign attack attribute contributions
            foreach (var malignEffect in character.Alteration.GetTemporaryAttackAttributeAlterations(false))
            {
                foreach (var malignAttribute in malignEffect.AttackAttributes)
                {
                    var defensiveAttribute = attackAttributes.First(x => x.RogueName == malignAttribute.RogueName);

                    // TODO: Have an issue with AlterationContainer.BlockType not saved to AlterationEffect. 
                    //       ***Needs to be refactored to split up all of the alteration types to properly support
                    //          all features. (Example: Break apart Physical / Mental Alterations entirely)
                    result += Calculator.CalculateAttackAttributeEffect(character, malignAttribute, defensiveAttribute, InteractionType.Mental);
                }
            }

            return result;
        }

        public static bool Is(this Character character, CharacterStateType characterStateType)
        {
            return character.Alteration.GetAlterations().Any(x => x.State.BaseType == characterStateType);
        }
    }
}
