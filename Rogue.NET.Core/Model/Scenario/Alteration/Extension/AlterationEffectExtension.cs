using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Extension
{
    public static class AlterationEffectExtension
    {
        #region (public) UI Related Query Methods
        /// <summary>
        /// Calculates "IsStackable" for any IAlterationEffect type (using type inspection)
        /// </summary>
        public static bool IsStackable(this IAlterationEffect effect)
        {
            // ONLY TEMPORARY EFFECTS ARE STACKABLE

            if (effect is AttackAttributeTemporaryAlterationEffect)
                return (effect as AttackAttributeTemporaryAlterationEffect).IsStackable;

            else if (effect is TemporaryAlterationEffect)
                return (effect as TemporaryAlterationEffect).IsStackable;

            else
                return false;
        }

        /// <summary>
        /// Returns the attribute value for the IAlterationEffect using type inspection
        /// </summary>
        public static double GetAttribute(this IAlterationEffect effect, CharacterAttribute attribute)
        {
            // Create a lookup for character attributes for alteration effects that
            // support stat changes:
            //
            // AuraAlterationEffect
            // PassiveAlterationEffect
            // PermanentAlterationEffect
            // TemporaryAlterationEffect

            switch (attribute)
            {
                case CharacterAttribute.Hp:
                    {
                        if (effect is PermanentAlterationEffect)
                            return (effect as PermanentAlterationEffect).Hp;

                        return 0D;
                    }
                case CharacterAttribute.Mp:
                    {
                        if (effect is PermanentAlterationEffect)
                            return (effect as PermanentAlterationEffect).Mp;

                        return 0D;
                    }
                case CharacterAttribute.Strength:
                    {
                        if (effect is AuraAlterationEffect)
                            return (effect as AuraAlterationEffect).Strength;

                        else if (effect is PassiveAlterationEffect)
                            return (effect as PassiveAlterationEffect).Strength;

                        else if (effect is PermanentAlterationEffect)
                            return (effect as PermanentAlterationEffect).Strength;

                        else if (effect is TemporaryAlterationEffect)
                            return (effect as TemporaryAlterationEffect).Strength;

                        return 0D;
                    }
                case CharacterAttribute.Agility:
                    {
                        if (effect is AuraAlterationEffect)
                            return (effect as AuraAlterationEffect).Agility;

                        else if (effect is PassiveAlterationEffect)
                            return (effect as PassiveAlterationEffect).Agility;

                        else if (effect is PermanentAlterationEffect)
                            return (effect as PermanentAlterationEffect).Agility;

                        else if (effect is TemporaryAlterationEffect)
                            return (effect as TemporaryAlterationEffect).Agility;

                        return 0D;
                    }
                case CharacterAttribute.Intelligence:
                    {
                        if (effect is AuraAlterationEffect)
                            return (effect as AuraAlterationEffect).Intelligence;

                        else if (effect is PassiveAlterationEffect)
                            return (effect as PassiveAlterationEffect).Intelligence;

                        else if (effect is PermanentAlterationEffect)
                            return (effect as PermanentAlterationEffect).Intelligence;

                        else if (effect is TemporaryAlterationEffect)
                            return (effect as TemporaryAlterationEffect).Intelligence;

                        return 0D;
                    }
                case CharacterAttribute.Speed:
                    {
                        if (effect is AuraAlterationEffect)
                            return (effect as AuraAlterationEffect).Speed;

                        else if (effect is PassiveAlterationEffect)
                            return (effect as PassiveAlterationEffect).Speed;

                        else if (effect is PermanentAlterationEffect)
                            return (effect as PermanentAlterationEffect).Speed;

                        else if (effect is TemporaryAlterationEffect)
                            return (effect as TemporaryAlterationEffect).Speed;

                        return 0D;
                    }
                case CharacterAttribute.HpRegen:
                    {
                        if (effect is AuraAlterationEffect)
                            return (effect as AuraAlterationEffect).HpPerStep;

                        else if (effect is PassiveAlterationEffect)
                            return (effect as PassiveAlterationEffect).HpPerStep;

                        else if (effect is TemporaryAlterationEffect)
                            return (effect as TemporaryAlterationEffect).HpPerStep;

                        return 0D;
                    }
                case CharacterAttribute.MpRegen:
                    {
                        if (effect is AuraAlterationEffect)
                            return (effect as AuraAlterationEffect).MpPerStep;

                        else if (effect is PassiveAlterationEffect)
                            return (effect as PassiveAlterationEffect).MpPerStep;

                        else if (effect is TemporaryAlterationEffect)
                            return (effect as TemporaryAlterationEffect).MpPerStep;

                        return 0D;
                    }
                case CharacterAttribute.LightRadius:
                    {
                        if (effect is PassiveAlterationEffect)
                            return (effect as PassiveAlterationEffect).LightRadius;

                        else if (effect is PermanentAlterationEffect)
                            return (effect as PermanentAlterationEffect).LightRadius;

                        else if (effect is TemporaryAlterationEffect)
                            return (effect as TemporaryAlterationEffect).LightRadius;

                        return 0D;
                    }
                case CharacterAttribute.Attack:
                    {
                        if (effect is AuraAlterationEffect)
                            return (effect as AuraAlterationEffect).Attack;

                        else if (effect is PassiveAlterationEffect)
                            return (effect as PassiveAlterationEffect).Attack;

                        else if (effect is TemporaryAlterationEffect)
                            return (effect as TemporaryAlterationEffect).Attack;

                        return 0D;
                    }
                case CharacterAttribute.Defense:
                    {
                        if (effect is AuraAlterationEffect)
                            return (effect as AuraAlterationEffect).Defense;

                        else if (effect is PassiveAlterationEffect)
                            return (effect as PassiveAlterationEffect).Defense;

                        else if (effect is TemporaryAlterationEffect)
                            return (effect as TemporaryAlterationEffect).Defense;

                        return 0D;
                    }
                case CharacterAttribute.Dodge:
                    {
                        if (effect is AuraAlterationEffect)
                            return (effect as AuraAlterationEffect).DodgeProbability;

                        else if (effect is PassiveAlterationEffect)
                            return (effect as PassiveAlterationEffect).DodgeProbability;

                        else if (effect is TemporaryAlterationEffect)
                            return (effect as TemporaryAlterationEffect).DodgeProbability;

                        return 0D;
                    }
                case CharacterAttribute.MagicBlock:
                    {
                        if (effect is AuraAlterationEffect)
                            return (effect as AuraAlterationEffect).MagicBlockProbability;

                        else if (effect is PassiveAlterationEffect)
                            return (effect as PassiveAlterationEffect).MagicBlockProbability;

                        else if (effect is TemporaryAlterationEffect)
                            return (effect as TemporaryAlterationEffect).MentalBlockProbability;

                        return 0D;
                    }
                case CharacterAttribute.CriticalHit:
                    {
                        if (effect is PassiveAlterationEffect)
                            return (effect as PassiveAlterationEffect).CriticalHit;

                        else if (effect is TemporaryAlterationEffect)
                            return (effect as TemporaryAlterationEffect).CriticalHit;

                        return 0D;
                    }
                case CharacterAttribute.FoodUsagePerTurn:
                    {
                        if (effect is PassiveAlterationEffect)
                            return (effect as PassiveAlterationEffect).FoodUsagePerTurn;

                        else if (effect is TemporaryAlterationEffect)
                            return (effect as TemporaryAlterationEffect).FoodUsagePerTurn;

                        return 0D;
                    }
                default:
                    throw new Exception("Unhandled CharacterAttribute");
            }
        }

        /// <summary>
        /// Returns set of UI-Formatted attributes to display on the UI.
        /// </summary>
        public static IDictionary<string, double> GetUIAttributes(this IAlterationEffect alterationEffect)
        {
            // Create a lookup of UI-related attributes (just character stats) for any
            // IAlterationEffect type (that supports stat changes):
            //
            // AuraAlterationEffect
            // PassiveAlterationEffect
            // PermanentAlterationEffect
            // TemporaryAlterationEffect

            Dictionary<string, double> result = null;

            if (alterationEffect.GetType() == typeof(AuraAlterationEffect))
            {
                var effect = alterationEffect as AuraAlterationEffect;

                result = new Dictionary<string, double>()
                {
                    { "Strength", effect.Strength },
                    { "Agility", effect.Agility },
                    { "Intelligence", effect.Intelligence },
                    { "Speed", effect.Speed },

                    { "Hp Per Step", effect.HpPerStep },
                    { "Mp Per Step", effect.MpPerStep },

                    { "Attack", effect.Attack },
                    { "Defense", effect.Defense },
                    { "Mental Block", effect.MagicBlockProbability },
                    { "Dodge", effect.DodgeProbability }
                };
            }
            else if (alterationEffect.GetType() == typeof(PassiveAlterationEffect))
            {
                var effect = alterationEffect as PassiveAlterationEffect;

                result = new Dictionary<string, double>()
                {
                    { "Strength", effect.Strength },
                    { "Agility", effect.Agility },
                    { "Intelligence", effect.Intelligence },
                    { "Speed", effect.Speed },
                    { "Light Radius", effect.LightRadius },

                    { "Hp Per Step", effect.HpPerStep },
                    { "Mp Per Step", effect.MpPerStep },
                    { "Food Usage", effect.FoodUsagePerTurn },

                    { "Attack", effect.Attack },
                    { "Defense", effect.Defense },
                    { "Mental Block", effect.MagicBlockProbability },
                    { "Dodge", effect.DodgeProbability },
                    { "Critical Hit", effect.CriticalHit }
                };
            }
            else if (alterationEffect.GetType() == typeof(PermanentAlterationEffect))
            {
                var effect = alterationEffect as PermanentAlterationEffect;

                result = new Dictionary<string, double>()
                {
                    { "Strength", effect.Strength },
                    { "Agility", effect.Agility },
                    { "Intelligence", effect.Intelligence },
                    { "Speed", effect.Speed },
                    { "Light Radius", effect.LightRadius },
                    { "Experience", effect.Experience },
                    { "Hunger", effect.Hunger },
                    { "Hp", effect.Hp },
                    { "Mp", effect.Mp }
                };
            }
            else if (alterationEffect.GetType() == typeof(TemporaryAlterationEffect))
            {
                var effect = alterationEffect as TemporaryAlterationEffect;

                result = new Dictionary<string, double>()
                {
                    { "Hp", effect.HpPerStep },
                    { "Mp", effect.MpPerStep },
                    { "Strength", effect.Strength },
                    { "Agility", effect.Agility },
                    { "Intelligence", effect.Intelligence },
                    { "Speed", effect.Speed },

                    { "Food Usage", effect.FoodUsagePerTurn },
                    { "Light Radius", effect.LightRadius },

                    { "Attack", effect.Attack },
                    { "Defense", effect.Defense },
                    { "Critical Hit", effect.CriticalHit },
                    { "Dodge", effect.DodgeProbability },
                    { "Mental Block", effect.MentalBlockProbability }
                };
            }

            if (result != null)
                result.Filter(x => x.Value != 0D);

            return result ?? new Dictionary<string, double>();
        }

        /// <summary>
        /// Returns collection of attack attributes for the alteration (using type inspection)
        /// </summary>
        public static IEnumerable<AttackAttribute> GetAttackAttributes(this IAlterationEffect alterationEffect)
        {
            IEnumerable<AttackAttribute> result = null;

            if (alterationEffect.GetType() == typeof(AttackAttributeAuraAlterationEffect))
            {
                result = (alterationEffect as AttackAttributeAuraAlterationEffect).AttackAttributes;
            }
            else if (alterationEffect.GetType() == typeof(AttackAttributeMeleeAlterationEffect))
            {
                result = (alterationEffect as AttackAttributeMeleeAlterationEffect).AttackAttributes;
            }
            else if (alterationEffect.GetType() == typeof(AttackAttributePassiveAlterationEffect))
            {
                result = (alterationEffect as AttackAttributePassiveAlterationEffect).AttackAttributes;
            }
            else if (alterationEffect.GetType() == typeof(AttackAttributeTemporaryAlterationEffect))
            {
                result = (alterationEffect as AttackAttributeTemporaryAlterationEffect).AttackAttributes;
            }

            return result ?? new List<AttackAttribute>();
        }

        /// <summary>
        /// Returns altered character state using type inspection
        /// </summary>
        /// <returns>AlteredCharacterState for the IAlterationEffect (or null)</returns>
        public static AlteredCharacterState GetAlteredState(this IAlterationEffect alterationEffect)
        {
            if (alterationEffect is AttackAttributeTemporaryAlterationEffect)
                return (alterationEffect as AttackAttributeTemporaryAlterationEffect).AlteredState;

            else if (alterationEffect is TemporaryAlterationEffect)
                return (alterationEffect as TemporaryAlterationEffect).AlteredState;

            return null;
        }

        /// <summary>
        /// Returns a UI Type descriptor for the IAlterationEffect. This is specific to listing out
        /// types of alteration effects to accompany the GetUIAttributes method above. (This could
        /// be refactored to a UI-specific component; but didn't think it warranted the work right now)
        /// </summary>
        public static string GetUITypeDescription(this IAlterationEffect effect)
        {
            if (effect is AttackAttributeAuraAlterationEffect)
                return string.Format("Aura ({0})", GetUIAttackAttributeCombatType((effect as AttackAttributeAuraAlterationEffect).CombatType));

            else if (effect is AttackAttributePassiveAlterationEffect)
                return string.Format("Passive ({0})", GetUIAttackAttributeCombatType((effect as AttackAttributePassiveAlterationEffect).CombatType));

            else if (effect is AttackAttributeTemporaryAlterationEffect)
                return string.Format("Temporary ({0})", GetUIAttackAttributeCombatType((effect as AttackAttributeTemporaryAlterationEffect).CombatType));

            else if (effect is AuraAlterationEffect)
                return "Aura";

            else if (effect is PassiveAlterationEffect)
                return "Passive";

            else if (effect is TemporaryAlterationEffect)
                return "Temporary";

            else
                throw new Exception("Unhandled IAlterationEffect UI-Type Description");
        }

        /// <summary>
        /// Returns a descriptive string for the corresponding "AlterationOtherEffectType" enum and also 
        /// deals with type inspection
        /// </summary>
        public static string GetUIOtherEffectType(this IAlterationEffect effect)
        {
            if (effect is OtherAlterationEffect)
            {
                switch ((effect as OtherAlterationEffect).Type)
                {
                    case AlterationOtherEffectType.Identify:
                        return "Identify";
                    case AlterationOtherEffectType.Uncurse:
                        return "Uncurse";
                    default:
                        throw new Exception("Unhandled AlterationOtherEffectType");
                }
            }

            return "None";
        }

        /// <summary>
        /// UI Type for the Skill Tree (Consider another way to provide UI structure)
        /// </summary>
        public static string GetUIType(this IAlterationEffect effect)
        {
            if (effect is AttackAttributeAuraAlterationEffect)
                return "Attack Attribute (Aura)";

            else if (effect is AttackAttributeMeleeAlterationEffect)
                return "Attack Attribute (Combat)";

            else if (effect is AttackAttributePassiveAlterationEffect)
                return "Attack Attribute (Passive)";

            else if (effect is AttackAttributeTemporaryAlterationEffect)
                return "Attack Attribute (Temporary)";

            else if (effect is AuraAlterationEffect)
                return "Aura";

            else if (effect is ChangeLevelAlterationEffect)
                return "Change Level";

            else if (effect is CreateMonsterAlterationEffect)
                return "Create Monster";

            else if (effect is EquipmentModifyAlterationEffect)
                return (effect as EquipmentModifyAlterationEffect).Type.ToString();

            else if (effect is OtherAlterationEffect)
                return (effect as OtherAlterationEffect).Type.ToString();

            else if (effect is PassiveAlterationEffect)
                return "Passive";

            else if (effect is PermanentAlterationEffect)
                return "Permanent";

            else if (effect is RemedyAlterationEffect)
                return "Remedy";

            else if (effect is RevealAlterationEffect)
                return "Reveal";

            else if (effect is RunAwayAlterationEffect)
                return "Run Away";

            else if (effect is StealAlterationEffect)
                return "Steal";

            else if (effect is TeleportAlterationEffect)
                return "Teleport";

            else
                throw new Exception("Unhandled IAlterationEffect Type");
        }

        /// <summary>
        /// UI Attack Attribute Type (Consider another way to provide UI structure)
        /// </summary>
        public static string GetUIAttackAttributeType(this IAlterationEffect effect)
        {
            if (effect is AttackAttributeAuraAlterationEffect)
                return GetUIAttackAttributeCombatType((effect as AttackAttributeAuraAlterationEffect).CombatType);

            else if (effect is AttackAttributeMeleeAlterationEffect)
                return "(Combat)";

            else if (effect is AttackAttributePassiveAlterationEffect)
                return GetUIAttackAttributeCombatType((effect as AttackAttributePassiveAlterationEffect).CombatType);

            else if (effect is AttackAttributeTemporaryAlterationEffect)
                return GetUIAttackAttributeCombatType((effect as AttackAttributeTemporaryAlterationEffect).CombatType);

            else
                throw new Exception("Unhandled Attack Attribue IAlterationEffect type");
        }
        #endregion

        private static string GetUIAttackAttributeCombatType(AlterationAttackAttributeCombatType combatType)
        {
            switch (combatType)
            {
                case AlterationAttackAttributeCombatType.FriendlyAggregate:
                    return "Friendly";
                case AlterationAttackAttributeCombatType.MalignPerStep:
                    return "Malign";
                default:
                    throw new Exception("Unhandled AlterationAttackAttributeCombatType");
            }
        }
    }
}
