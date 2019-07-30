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
            return character.MpRegenBase + character.Alteration.GetAlterations().Sum(x => x.MpPerStep)
                                         + (character.CharacterClassAlteration.HasAttributeEffect ?
                                           character.CharacterClassAlteration.AttributeEffect.MpPerStep : 0);
        }
        public static double GetHpRegen(this Character character)
        {
            return character.HpRegenBase + character.Alteration.GetAlterations().Sum(x => x.HpPerStep)
                                         + (character.CharacterClassAlteration.HasAttributeEffect ?
                                           character.CharacterClassAlteration.AttributeEffect.HpPerStep : 0);
        }
        public static double GetStrength(this Character character)
        {
            var result = character.StrengthBase;

            // Alteration
            result += character.Alteration.GetAlterations().Sum(x => x.Strength);

            // Religious Alteration
            if (character.CharacterClassAlteration.HasAttributeEffect)
                result += character.CharacterClassAlteration.AttributeEffect.Strength;

            return Math.Max(0.1, result);
        }
        public static double GetAgility(this Character character)
        {
            var result = character.AgilityBase;

            // Alteration
            result += character.Alteration.GetAlterations().Sum(x => x.Agility);

            // Religious Alteration
            if (character.CharacterClassAlteration.HasAttributeEffect)
                result += character.CharacterClassAlteration.AttributeEffect.Agility;

            return Math.Max(0.1, result);
        }
        public static double GetIntelligence(this Character character)
        {
            var result = character.IntelligenceBase;

            // Alteration
            result += character.Alteration.GetAlterations().Sum(x => x.Intelligence);

            // Religious Alteration
            if (character.CharacterClassAlteration.HasAttributeEffect)
                result += character.CharacterClassAlteration.AttributeEffect.Intelligence;

            return Math.Max(0.1, result);
        }
        public static double GetAuraRadius(this Character character)
        {
            var result = character.AuraRadiusBase;

            // Alteration
            result += character.Alteration.GetAlterations().Sum(x => x.AuraRadius);

            // Religious Alteration
            if (character.CharacterClassAlteration.HasAttributeEffect)
                result += character.CharacterClassAlteration.AttributeEffect.AuraRadius;

            return Math.Max(0.1, result);
        }
        public static double GetMagicBlock(this Character character)
        {
            var result = character.GetMagicBlockBase();

            // Alteration
            result += character.Alteration.GetAlterations().Sum(x => x.MagicBlockProbability);

            // Religious Alteration
            if (character.CharacterClassAlteration.HasAttributeEffect)
                result += character.CharacterClassAlteration.AttributeEffect.MagicBlockProbability;

            return result.Clip();
        }
        public static double GetDodge(this Character character)
        {
            var result = character.GetDodgeBase();

            // Alteration
            result += character.Alteration.GetAlterations().Sum(x => x.DodgeProbability);

            // Religious Alteration
            if (character.CharacterClassAlteration.HasAttributeEffect)
                result += character.CharacterClassAlteration.AttributeEffect.DodgeProbability;

            return result.Clip();
        }
        public static double GetSpeed(this Character character)
        {
            var speed = character.SpeedBase + character.Alteration.GetAlterations().Sum(x => x.Speed)
                                            + (character.CharacterClassAlteration.HasAttributeEffect ? 
                                               character.CharacterClassAlteration.AttributeEffect.Speed : 0);

            // 0.1 < speed < 1
            return speed.Clip(0.1, 1);
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

            // Add on religious alteration contribution
            if (character.CharacterClassAlteration.HasAttributeEffect)
                result += character.CharacterClassAlteration.AttributeEffect.Attack;

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

            // Add on religious alteration contribution
            if (character.CharacterClassAlteration.HasAttributeEffect)
                result += character.CharacterClassAlteration.AttributeEffect.Defense;

            return Math.Max(0, result);
        }
        public static double GetCriticalHitProbability(this Character character)
        {
            var result = ModelConstants.CriticalHitBase;

            // Alteration
            result += character.Alteration.GetAlterations().Sum(x => x.CriticalHit);

            // Religious Alteration
            if (character.CharacterClassAlteration.HasAttributeEffect)
                result += character.CharacterClassAlteration.AttributeEffect.CriticalHit;

            return result.Clip();
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
                    x.Weakness += y.Weakness;

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
                    attribute.Weakness += passiveAttribute.Weakness;
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
                    attribute.Weakness += passiveAttribute.Weakness;
                }
            }

            // Character Class contributions
            if (character.CharacterClassAlteration.HasAttackAttributeEffect)
            {
                foreach (var attribute in result)
                {
                    var characterClassAttribute = character.CharacterClassAlteration.AttackAttributeEffect.AttackAttributes.First(y => y.RogueName == attribute.RogueName);

                    attribute.Attack += characterClassAttribute.Attack;
                    attribute.Resistance += characterClassAttribute.Resistance;
                    attribute.Weakness += characterClassAttribute.Weakness;
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
                    result += Calculator.CalculateAttackAttributeMelee(
                                malignAttribute.Attack, 
                                defensiveAttribute.Resistance, 
                                defensiveAttribute.Weakness + malignAttribute.Weakness);
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
