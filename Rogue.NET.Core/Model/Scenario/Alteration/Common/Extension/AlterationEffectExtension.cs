using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Core.Model.Static;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Common.Extension
{
    public static class AlterationEffectExtension
    {
        #region (public) Query Methods
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
                case CharacterAttribute.Stamina:
                    {
                        if (effect is PermanentAlterationEffect)
                            return (effect as PermanentAlterationEffect).Stamina;

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
                case CharacterAttribute.StaminaRegen:
                    {
                        if (effect is AuraAlterationEffect)
                            return (effect as AuraAlterationEffect).StaminaPerStep;

                        else if (effect is PassiveAlterationEffect)
                            return (effect as PassiveAlterationEffect).StaminaPerStep;

                        else if (effect is TemporaryAlterationEffect)
                            return (effect as TemporaryAlterationEffect).StaminaPerStep;

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
        /// Returns true if the IAlterationEffect supports blocking when used with the supplied
        /// alteration type.
        /// </summary>
        /// <returns>True if the alteration should support blocking</returns>
        public static bool GetSupportsBlocking(this IAlterationEffect effect, AlterationContainer alteration)
        {
            return AlterationSpecificationContainer.GetSupportsBlocking(alteration, effect);
        }

        /// <summary>
        /// Returns true if the IAlterationEffect supports blocking when used with the supplied
        /// alteration type.
        /// </summary>
        /// <returns>True if the alteration should support blocking</returns>
        public static AlterationCostType GetCostType(this IAlterationEffect effect, AlterationContainer alteration)
        {
            return AlterationSpecificationContainer.GetCostType(alteration, effect);
        }
        #endregion
    }
}
