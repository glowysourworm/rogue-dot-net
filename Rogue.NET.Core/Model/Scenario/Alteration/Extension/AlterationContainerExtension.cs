using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Consumable;
using Rogue.NET.Core.Model.Scenario.Alteration.Doodad;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Alteration.Enemy;
using Rogue.NET.Core.Model.Scenario.Alteration.Equipment;
using Rogue.NET.Core.Model.Scenario.Alteration.Skill;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Extension
{
    public static class AlterationContainerExtension
    {
        /// <summary>
        /// Calculated cost type based on alteration type
        /// </summary>
        public static AlterationCostType GetCostType(this AlterationContainer alteration)
        {
            // Use once -> pay once
            if (alteration is ConsumableAlteration)
                return AlterationCostType.OneTime;

            // I'm just throwing it!
            else if (alteration is ConsumableProjectileAlteration)
                return AlterationCostType.None;

            // Thought about it.. simplified by saying No.
            else if (alteration is DoodadAlteration)
                return AlterationCostType.None;

            // This one is more involved.. probably one time. Not supporting
            // Aura / Passive types for Enemy skills; but they can be wearing
            // equipment that DOES have those things. So, probably just one-time
            // costs.
            else if (alteration is EnemyAlteration)
                // Went with one-time based on what effects are supported
                return AlterationCostType.OneTime;

            // Pew! Pew! Pew!
            else if (alteration is EquipmentAttackAlteration)
                return AlterationCostType.OneTime;

            // Passive / Aura
            else if (alteration is EquipmentEquipAlteration)
                return AlterationCostType.PerStep;

            // You can't make me pay for a curse.. That's just stupid...
            else if (alteration is EquipmentCurseAlteration)
                return AlterationCostType.None;

            // Anything and Everything a chap can unload...
            else if (alteration is SkillAlteration)
            {
                if (alteration.Effect is AttackAttributeAuraAlterationEffect)
                    return AlterationCostType.PerStep;

                else if (alteration.Effect is AttackAttributeMeleeAlterationEffect)
                    return AlterationCostType.OneTime;

                else if (alteration.Effect is AttackAttributePassiveAlterationEffect)
                    return AlterationCostType.PerStep;

                else if (alteration.Effect is AttackAttributeTemporaryAlterationEffect)
                    return AlterationCostType.OneTime;

                else if (alteration.Effect is AuraAlterationEffect)
                    return AlterationCostType.PerStep;

                else if (alteration.Effect is ChangeLevelAlterationEffect)
                    return AlterationCostType.OneTime;

                else if (alteration.Effect is CreateMonsterAlterationEffect)
                    return AlterationCostType.OneTime;

                else if (alteration.Effect is EquipmentDamageAlterationEffect)
                    return AlterationCostType.OneTime;

                else if (alteration.Effect is EquipmentEnhanceAlterationEffect)
                    return AlterationCostType.OneTime;

                else if (alteration.Effect is OtherAlterationEffect)
                    return AlterationCostType.OneTime;

                else if (alteration.Effect is PassiveAlterationEffect)
                    return AlterationCostType.PerStep;

                else if (alteration.Effect is PermanentAlterationEffect)
                    return AlterationCostType.OneTime;

                else if (alteration.Effect is RemedyAlterationEffect)
                    return AlterationCostType.OneTime;

                else if (alteration.Effect is RevealAlterationEffect)
                    return AlterationCostType.OneTime;

                else if (alteration.Effect is RunAwayAlterationEffect)
                    return AlterationCostType.None;

                else if (alteration.Effect is StealAlterationEffect)
                    return AlterationCostType.OneTime;

                else if (alteration.Effect is TeleportAlterationEffect)
                    return AlterationCostType.OneTime;

                else if (alteration.Effect is TemporaryAlterationEffect)
                    return AlterationCostType.OneTime;

                else
                    throw new Exception("Unhandled Alteration Effect type");
            }

            else
                throw new Exception("Unhandled Alteration type");
        }

        /// <summary>
        /// Calculated support for animations based on alteration type
        /// </summary>
        public static bool SupportsAnimations(this AlterationContainer alteration)
        {
            // If it's supported, then what about un-equipping? Or overlapping
            // animations with cursed equipment? Or un-equipping-but-cursed?
            // probably too much to support with overaps. Plus, the equipment
            // screen makes this an issue.
            if ((alteration is EquipmentCurseAlteration) ||
                (alteration is EquipmentEquipAlteration))
                return false;

            return true;
        }

        /// <summary>
        /// Calculated support for blocking based on alteration / alteration effect type
        /// </summary>
        public static bool SupportsBlocking(this AlterationContainer alteration)
        {
            if (alteration is ConsumableAlteration)
                return false;

            else if (alteration is ConsumableProjectileAlteration)
                return false;

            else if (alteration is DoodadAlteration)
                return false;

            // Temporary, Melee, or Equipment Modify (can block)
            else if (alteration is EnemyAlteration)
                return true;

            // Allow character to block
            else if (alteration is EquipmentAttackAlteration)
                return true;

            else if (alteration is EquipmentEquipAlteration)
                return false;

            else if (alteration is EquipmentCurseAlteration)
                return false;

            else if (alteration is SkillAlteration)
            {
                if (alteration.Effect is AttackAttributeAuraAlterationEffect)
                    return false;

                else if (alteration.Effect is AttackAttributeMeleeAlterationEffect)
                    return true;

                else if (alteration.Effect is AttackAttributePassiveAlterationEffect)
                    return false;

                else if (alteration.Effect is AttackAttributeTemporaryAlterationEffect)
                    return true;

                else if (alteration.Effect is AuraAlterationEffect)
                    return false;

                // Nah...
                else if (alteration.Effect is ChangeLevelAlterationEffect)
                    return false;

                // Could be - but usually no.. better to keep it simple
                else if (alteration.Effect is CreateMonsterAlterationEffect)
                    return false;

                // Yes, because using these on other characters involves some kind of
                // negative modification (like, "acid eats my armor"). So, need a blocking type
                else if (alteration.Effect is EquipmentDamageAlterationEffect)
                    return true;

                // No way!
                else if (alteration.Effect is EquipmentEnhanceAlterationEffect)
                    return true;

                else if (alteration.Effect is OtherAlterationEffect)
                {
                    switch ((alteration.Effect as OtherAlterationEffect).Type)
                    {
                        case AlterationOtherEffectType.Identify:
                        case AlterationOtherEffectType.Uncurse:
                            return false;
                        default:
                            throw new Exception("Unhandled AlterationOtherEffectType");
                    }
                }

                else if (alteration.Effect is PassiveAlterationEffect)
                    return false;

                else if (alteration.Effect is PermanentAlterationEffect)
                    return true;

                else if (alteration.Effect is RemedyAlterationEffect)
                    return false;

                else if (alteration.Effect is RevealAlterationEffect)
                    return false;

                else if (alteration.Effect is RunAwayAlterationEffect)
                    return false;

                else if (alteration.Effect is StealAlterationEffect)
                    return true;

                else if (alteration.Effect is TeleportAlterationEffect)
                    return true;

                else if (alteration.Effect is TemporaryAlterationEffect)
                    return true;

                else
                    throw new Exception("Unhandled Alteration Effect type");
            }

            else
                throw new Exception("Unhandled Alteration Type");
        }

        /// <summary>
        /// Calculated requirement for a target (EXCLUDES EQUIPMENT ATTACK ALTERATIONS)
        /// </summary>
        public static bool RequiresTarget(this AlterationContainer alteration)
        {
            // TODO:ALTERATION - Validate AlterationTargetType for each IAlterationEffect sub-type for
            //                   each alteration container type. This should be done in the editor!
            if (alteration is ConsumableAlteration)
                return (alteration as ConsumableAlteration).AnimationGroup.TargetType == AlterationTargetType.Target;

            else if (alteration is ConsumableProjectileAlteration)
                return true;

            else if (alteration is DoodadAlteration)
                return (alteration as DoodadAlteration).AnimationGroup.TargetType == AlterationTargetType.Target;

            else if (alteration is EnemyAlteration)
                return (alteration as EnemyAlteration).AnimationGroup.TargetType == AlterationTargetType.Target;

            else if (alteration is EquipmentAttackAlteration)
                return false;

            else if (alteration is EquipmentEquipAlteration)
                return false;

            else if (alteration is EquipmentCurseAlteration)
                return false;

            else if (alteration is SkillAlteration)
                return (alteration as SkillAlteration).AnimationGroup.TargetType == AlterationTargetType.Target;

            else
                throw new Exception("Unknwon Alteration Type");
        }

        /// <summary>
        /// Calculated requirement for using an alteration (EXCLUDES EQUIPMENT ATTACK ALTERATIONS)
        /// </summary>
        public static bool RequiresCharacterInRange(this AlterationContainer alteration)
        {
            // TODO:ALTERATION - Validate AlterationTargetType for each IAlterationEffect sub-type for
            //                   each alteration container type. This should be done in the editor!
            if (alteration is ConsumableAlteration)
                return (alteration as ConsumableAlteration).AnimationGroup.TargetType != AlterationTargetType.Source;

            else if (alteration is ConsumableProjectileAlteration)
                return true;

            else if (alteration is DoodadAlteration)
                return (alteration as DoodadAlteration).AnimationGroup.TargetType != AlterationTargetType.Source;

            else if (alteration is EnemyAlteration)
                return (alteration as EnemyAlteration).AnimationGroup.TargetType != AlterationTargetType.Source;

            else if (alteration is EquipmentAttackAlteration)
                return false;

            else if (alteration is EquipmentEquipAlteration)
                return false;

            else if (alteration is EquipmentCurseAlteration)
                return false;

            else if (alteration is SkillAlteration)
                return (alteration as SkillAlteration).AnimationGroup.TargetType != AlterationTargetType.Source;

            else
                throw new Exception("Unknwon Alteration Type");
        }

        /// <summary>
        /// Calculated Player Passive / Aura
        /// </summary>
        public static bool IsPlayerPassiveOrAura(this AlterationContainer alteration)
        {
            if (alteration is SkillAlteration)
            {
                var skillAlteration = alteration as SkillAlteration;

                return skillAlteration.Effect is AttackAttributePassiveAlterationEffect ||
                       skillAlteration.Effect is PassiveAlterationEffect ||
                       skillAlteration.Effect is AttackAttributeAuraAlterationEffect ||
                       skillAlteration.Effect is AuraAlterationEffect;
            }

            return false;
        }

        /// <summary>
        /// Calculated Player Passive / Aura
        /// </summary>
        public static bool IsPassiveOrAura(this AlterationContainer alteration)
        {
            return alteration.Effect is AttackAttributePassiveAlterationEffect ||
                   alteration.Effect is PassiveAlterationEffect ||
                   alteration.Effect is AttackAttributeAuraAlterationEffect ||
                   alteration.Effect is AuraAlterationEffect;
        }
    }
}
