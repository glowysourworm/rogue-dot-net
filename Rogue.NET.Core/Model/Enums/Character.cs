﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Rogue.NET.Core.Model.Enums
{
    [Flags]
    public enum CharacterStateType : uint
    {
        /// <summary>
        /// No turn impedement - no abnormal state
        /// </summary>
        Normal = 0,

        /// <summary>
        /// Impedes vision - light radius calculation of 1.
        /// </summary>
        Blind = 1,

        /// <summary>
        /// Impedes character movement
        /// </summary>
        CantMove = 2,

        /// <summary>
        /// Causes character to move randomly
        /// </summary>
        MovesRandomly = 4,

        /// <summary>
        /// Impedes character skill use
        /// </summary>
        CantUseSkills = 8,

        /// <summary>
        /// Character not engaged by enemies - enemies will have a visibility calculation that is different.
        /// </summary>
        Invisible = 16,

        /// <summary>
        /// Considered Abnormal; but does not require any support from the game engine.
        /// </summary>
        Abnormal = 32
    }
    /// <summary>
    /// Specifies character movement
    /// </summary>
    public enum CharacterMovementType
    {
        /// <summary>
        /// Random Walk - Hits if in range
        /// </summary>
        Random = 0,

        /// <summary>
        /// Doesn't think to use doors
        /// </summary>
        HeatSeeker = 1,

        /// <summary>
        /// Figures out a way to the player
        /// </summary>
        PathFinder = 2,

        /// <summary>
        /// Stays just inside its attack range and
        /// Fires range missiles.. or simply being a coward
        /// </summary>
        StandOffIsh = 3
    }
    public enum CharacterAttackType
    {
        Melee = 0,

        /// <summary>
        /// This should cover Range attacks for enemies
        /// </summary>
        Skill = 1,

        /// <summary>
        /// Skill that can only be used at very close (radius=1) range
        /// </summary>
        SkillCloseRange = 2,

        /// <summary>
        /// no attack
        /// </summary>
        None = 3
    }
    [Flags]
    public enum BehaviorCondition : int
    {
        [Display(Name = "Attack Condition Met",
                 Description = "Can attack using the prescribed behavior attack method (skill costs met)")]
        AttackConditionsMet = 1,

        [Display(Name = "Hp Low",
                 Description = "Behavior ONLY available when Hp is below 10%")]
        HpLow = 2
    }
    [Flags]
    public enum BehaviorExitCondition : int
    {
        [Display(Name = "Behavior Counter Expired",
                 Description = "Behavior has a sub-counter that acts to change selected behavior in the state machine (of behaviors)")]
        BehaviorCounterExpired = 1,

        [Display(Name = "Hp Low",
                 Description = "Behavior will not be available when Hp is below 10%")]
        HpLow = 2
    }
    public enum SmileyMoods
    {
        None,
        Happy,
        Indifferent,
        Sad,
        Shocked,
        Angry,
        Drunk,
        Scared,
        Mischievous
    }

    /// <summary>
    /// Represents charater attributes
    /// </summary>
    public enum CharacterAttribute
    {
        Hp,
        Mp,
        Strength,
        Agility,
        Intelligence,
        Speed,
        HpRegen,
        MpRegen,
        LightRadius,
        Attack,
        Defense,
        Dodge,
        // TODO:ALTERATION (Change name to MentalBlock)
        MagicBlock,
        CriticalHit,
        FoodUsagePerTurn
    }

    /// <summary>
    /// Represents Character Base Attributes (non-derived)
    /// </summary>
    public enum CharacterBaseAttribute
    {
        Strength,
        Agility,
        Intelligence
    }
}
