using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Common.Extension;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Content.Item.Extension;
using Rogue.NET.Core.Processing.Model.Static;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Model.Scenario.Character.Extension
{
    public static class CharacterExtension
    {
        public static double GetMentalBlockBase(this Character character)
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
            return character.MpRegenBase + character.Alteration.GetAttribute(CharacterAttribute.MpRegen);
        }
        public static double GetHpRegen(this Character character)
        {
            return character.HpRegenBase + character.Alteration.GetAttribute(CharacterAttribute.HpRegen);
        }
        public static double GetStrength(this Character character)
        {
            var result = character.StrengthBase;

            // Alteration
            result += character.Alteration.GetAttribute(CharacterAttribute.Strength);

            return Math.Max(0.1, result);
        }
        public static double GetAgility(this Character character)
        {
            var result = character.AgilityBase;

            // Alteration
            result += character.Alteration.GetAttribute(CharacterAttribute.Agility);

            return Math.Max(0.1, result);
        }
        public static double GetIntelligence(this Character character)
        {
            var result = character.IntelligenceBase;

            // Alteration
            result += character.Alteration.GetAttribute(CharacterAttribute.Intelligence);

            return Math.Max(0.1, result);
        }
        public static double GetLightRadius(this Character character)
        {
            var result = character.LightRadiusBase;

            // Alteration
            result += character.Alteration.GetAttribute(CharacterAttribute.LightRadius);

            return Math.Max(0.1, result);
        }
        public static double GetMentalBlock(this Character character)
        {
            var result = character.GetMentalBlockBase();

            // Alteration
            result += character.Alteration.GetAttribute(CharacterAttribute.MagicBlock);

            return result.Clip();
        }
        public static double GetDodge(this Character character)
        {
            var result = character.GetDodgeBase();

            // Alteration
            result += character.Alteration.GetAttribute(CharacterAttribute.Dodge);

            return result.Clip();
        }
        public static double GetSpeed(this Character character)
        {
            var speed = character.SpeedBase + character.Alteration.GetAttribute(CharacterAttribute.Speed);

            // 0.1 < speed < 1
            return speed.Clip(0.1, 1);
        }

        public static double GetAttack(this Character character)
        {
            
            var attack = 0D;

            var equippedWeapons = character.Equipment
                                           .Values
                                           .Where(x => x.IsEquipped && x.IsWeaponType());

            // No Weapons Equipped
            if (equippedWeapons.None())
            {
                // Base Attack is Strength with no Equipment Modifier
                attack = MeleeCalculator.GetAttackValue(0D, character.GetStrength());
            }
            else
            {
                foreach (var equipment in equippedWeapons)
                {
                    // Get the character attribute for calculating the attack value
                    var characterAttributeValue = equipment.CombatType == CharacterBaseAttribute.Strength ? character.GetStrength() :
                                                  equipment.CombatType == CharacterBaseAttribute.Agility ? character.GetAgility() :
                                                  character.GetIntelligence();

                    // Calculate strength * equipment base attack value
                    attack += MeleeCalculator.GetAttackValue(equipment.GetAttackValue(), characterAttributeValue);
                }
            }

            // Add on alteration contributions
            var result = attack + character.Alteration.GetAttribute(CharacterAttribute.Attack);

            return Math.Max(0, result);
        }
        public static double GetDefense(this Character character)
        {
            var defense = 0D;

            var equippedArmor = character.Equipment
                                           .Values
                                           .Where(x => x.IsEquipped && x.IsArmorType());

            // No Armor Equipped
            if (equippedArmor.None())
            {
                // Base Defense is Strength with no Equipment Modifier
                defense = MeleeCalculator.GetDefenseValue(0D, character.GetStrength());
            }
            else
            {
                foreach (var equipment in equippedArmor)
                {
                    // Get the character attribute for calculating the attack value
                    var characterAttributeValue = equipment.CombatType == CharacterBaseAttribute.Strength ? character.GetStrength() :
                                                  equipment.CombatType == CharacterBaseAttribute.Agility ? character.GetAgility() :
                                                  character.GetIntelligence();

                    // Calculate strength * equipment base attack value
                    defense += MeleeCalculator.GetDefenseValue(equipment.GetDefenseValue(), characterAttributeValue);
                }
            }

            // Add on alteration contributions
            var result = defense + character.Alteration.GetAttribute(CharacterAttribute.Defense);

            return Math.Max(0, result);
        }
        public static double GetThrowAttack(this Character character, Equipment equipment)
        {
            // Get the character attribute for calculating the attack value
            var characterAttributeValue = equipment.CombatType == CharacterBaseAttribute.Strength ? character.GetStrength() :
                                          equipment.CombatType == CharacterBaseAttribute.Agility ? character.GetAgility() :
                                          character.GetIntelligence();

            return MeleeCalculator.GetAttackValue(equipment.GetThrowValue(), characterAttributeValue);
        }
        public static double GetCriticalHitProbability(this Character character)
        {
            var result = ModelConstants.CriticalHitBase;

            // Alteration
            result += character.Alteration.GetAttribute(CharacterAttribute.CriticalHit);

            return result.Clip();
        }

        /// <summary>
        /// Returns effective attack attributes for use with direct melee calculation
        /// </summary>
        public static IEnumerable<AttackAttribute> GetMeleeAttributes(this Character character)
        {
            // Create base attribute list from the intrinsic character attributes
            var result = character.AttackAttributes
                                  .Values
                                  .Select(x => x.DeepClone())
                                  .ToList();

            // Alteration attack attribute contributions
            foreach (var friendlyAttackAttribute in character.Alteration
                                                             .GetAttackAttributes(AlterationAttackAttributeCombatType.FriendlyAggregate))
            {
                // Get the matching attribute
                var attribute = result.FirstOrDefault(x => x.RogueName == friendlyAttackAttribute.RogueName);

                // Add a clone
                if (attribute == null)
                    result.Add(friendlyAttackAttribute.DeepClone());

                // Add to the result
                else
                    attribute.Add(friendlyAttackAttribute, false);
            }

            //Equipment contributions
            foreach (var equipmentAttackAttribute in character.Equipment.Values.Where(z => z.IsEquipped).SelectMany(x => x.AttackAttributes))
            {
                // Get the matching attribute
                var attribute = result.FirstOrDefault(x => x.RogueName == equipmentAttackAttribute.RogueName);

                // Add a clone
                if (attribute == null)
                    result.Add(equipmentAttackAttribute.DeepClone());

                // Add to the result
                else
                    attribute.Add(equipmentAttackAttribute, false);
            }

            return result;
        }


        /// <summary>
        /// Returns the end-of-turn malign attack attribute contribution used at the end
        /// of each character turn.
        /// </summary>
        public static double GetMalignAttackAttributeHit(this Character character)
        {
            var result = 0D;

            // Get character effective attack attributes
            var attackAttributes = character.GetMeleeAttributes();

            // Malign attack attribute contributions
            foreach (var malignAttribute in character.Alteration
                                                    .GetAttackAttributes(AlterationAttackAttributeCombatType.MalignPerStep))
            {
                var defensiveAttribute = attackAttributes.FirstOrDefault(x => x.RogueName == malignAttribute.RogueName);

                // Calculate the attack attribute hit
                if (defensiveAttribute != null)
                {
                    result += Calculator.CalculateAttackAttributeMelee(
                                    malignAttribute.Attack,
                                    defensiveAttribute.Resistance,
                                    defensiveAttribute.Weakness + malignAttribute.Weakness,
                                    defensiveAttribute.Immune);
                }
                else
                {
                    result += Calculator.CalculateAttackAttributeMelee(
                                    malignAttribute.Attack,
                                    0.0,
                                    malignAttribute.Weakness,
                                    false);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns true if character has an altered state that matches the input altered state.
        /// </summary>
        public static bool Is(this Character character, CharacterStateType characterStateType)
        {
            // Altered States from Character.Alteration
            return character.Alteration
                            .GetStates()
                            .Any(x => x.BaseType == characterStateType);
        }
    }
}
