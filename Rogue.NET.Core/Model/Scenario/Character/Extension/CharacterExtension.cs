﻿using Rogue.NET.Common.Extension;
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
        public static double GetHaulMax(this CharacterBase character)
        {
            return character.StrengthBase * ModelConstants.HaulMaxStrengthMultiplier;
        }
        public static double GetStrengthBase(this CharacterBase character)
        {
            return character.StrengthBase;
        }
        public static double GetHaul(this CharacterBase character)
        {
            return character.Equipment.Values.Sum(x => x.Weight) +
                   character.Consumables.Values.Sum(x => x.Weight);
        }
        public static double GetAttackBase(this CharacterBase character)
        {
            return character.StrengthBase;
        }
        public static double GetDefenseBase(this CharacterBase character)
        {
            return character.GetStrengthBase() * ModelConstants.Melee.DefenseBaseMultiplier;
        }
        public static double GetTotalStaminaRegen(this CharacterBase character)
        {
            return character.StaminaRegenBase + character.Alteration.GetAttribute(CharacterAttribute.StaminaRegen);
        }
        public static double GetTotalHealthRegen(this CharacterBase character)
        {
            return character.HealthRegenBase + character.Alteration.GetAttribute(CharacterAttribute.HealthRegen);
        }
        public static double GetStrength(this CharacterBase character)
        {
            var result = character.StrengthBase;

            // Alteration
            result += character.Alteration.GetAttribute(CharacterAttribute.Strength);

            return System.Math.Max(0.1, result);
        }
        public static double GetAgility(this CharacterBase character)
        {
            var result = character.AgilityBase;

            // Alteration
            result += character.Alteration.GetAttribute(CharacterAttribute.Agility);

            return System.Math.Max(0.1, result);
        }
        public static double GetIntelligence(this CharacterBase character)
        {
            var result = character.IntelligenceBase;

            // Alteration
            result += character.Alteration.GetAttribute(CharacterAttribute.Intelligence);

            return System.Math.Max(0.1, result);
        }
        public static double GetVision(this CharacterBase character)
        {
            var result = character.VisionBase;

            // Alteration
            result += character.Alteration.GetAttribute(CharacterAttribute.Vision);

            return result.Clip(0.1, 1);
        }
        public static double GetSpeed(this CharacterBase character)
        {
            var speed = character.SpeedBase + character.Alteration.GetAttribute(CharacterAttribute.Speed);

            // 0.1 < speed < 1
            return speed.Clip(0.1, 1);
        }

        public static double GetAttack(this CharacterBase character)
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

            return System.Math.Max(0, result);
        }
        public static double GetDefense(this CharacterBase character)
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

            return System.Math.Max(0, result);
        }
        public static double GetThrowAttack(this CharacterBase character, Equipment equipment)
        {
            // Get the character attribute for calculating the attack value
            var characterAttributeValue = equipment.CombatType == CharacterBaseAttribute.Strength ? character.GetStrength() :
                                          equipment.CombatType == CharacterBaseAttribute.Agility ? character.GetAgility() :
                                          character.GetIntelligence();

            return MeleeCalculator.GetAttackValue(equipment.GetThrowValue(), characterAttributeValue);
        }

        /// <summary>
        /// Returns effective attack attributes for use with direct melee calculation
        /// </summary>
        public static IEnumerable<AttackAttribute> GetMeleeAttributes(this CharacterBase character)
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
        public static double GetMalignAttackAttributeHit(this CharacterBase character)
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
                    result += RogueCalculator.CalculateAttackAttributeMelee(
                                    malignAttribute.Attack,
                                    defensiveAttribute.Resistance,
                                    defensiveAttribute.Weakness + malignAttribute.Weakness,
                                    defensiveAttribute.Immune);
                }
                else
                {
                    result += RogueCalculator.CalculateAttackAttributeMelee(
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
        public static bool Is(this CharacterBase character, CharacterStateType characterStateType)
        {
            // Altered States from Character.Alteration
            return character.Alteration
                            .GetStates()
                            .Any(x => x.BaseType == characterStateType);
        }
    }
}
