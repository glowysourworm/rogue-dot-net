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
        /// Calculated support for animations based on alteration type
        /// </summary>
        public static bool SupportsAnimations(this Common.AlterationContainer alteration)
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
        /// Calculated requirement for a target (EXCLUDES EQUIPMENT ATTACK ALTERATIONS)
        /// </summary>
        public static bool RequiresTarget(this Common.AlterationContainer alteration)
        {
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
        /// Returns true if the alteration requires a target location has been selected (by the ITargetingService)
        /// </summary>
        public static bool RequiresTargetLocation(this Common.AlterationContainer alteration)
        {
            if (alteration.Effect is TeleportManualAlterationEffect)
                return true;

            else
                return false;
        }

        /// <summary>
        /// Calculated requirement for using an alteration (EXCLUDES EQUIPMENT ATTACK ALTERATIONS)
        /// </summary>
        public static bool RequiresCharacterInRange(this Common.AlterationContainer alteration)
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
        public static bool IsPlayerPassiveOrAura(this Common.AlterationContainer alteration)
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
        public static bool IsPassiveOrAura(this Common.AlterationContainer alteration)
        {
            return alteration.Effect is AttackAttributePassiveAlterationEffect ||
                   alteration.Effect is PassiveAlterationEffect ||
                   alteration.Effect is AttackAttributeAuraAlterationEffect ||
                   alteration.Effect is AuraAlterationEffect;
        }
    }
}
