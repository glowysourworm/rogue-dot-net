using System;
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
        /// <summary>
        /// Attack type is physical combat that is calculated from the character's stats / equipment
        /// </summary>
        PhysicalCombat = 0,

        /// <summary>
        /// Attack type is some kind of alteration
        /// </summary>
        Alteration = 1,

        /// <summary>
        /// No Attack
        /// </summary>
        None = 2
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

    public enum SmileyExpression
    { 
        Happy = 0,
        Blind = 1,
        Dead = 2,
        Disgruntled = 3,
        Emoji = 4,
        FreakedOut = 5,
        Frustrated = 6,
        Insane = 7,
        LeftWink = 8,
        MeanPumpkinFace = 9,
        Mischievous = 10,
        RightWink = 11,
        Sad = 12,
        Scared = 13,
        Shocked = 14,
        Sleeping = 15,
        Sour = 16,
        Unsure = 17,
        WeirdWhistler = 18
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

    /// <summary>
    /// Specifies what side units will fight for (Player / Enemy)
    /// </summary>
    public enum CharacterAlignmentType
    {
        [Display(Name = "Player Aligned",
                 Description = "Character fights for Player")]
        PlayerAligned = 0,

        [Display(Name = "Enemy Aligned",
                 Description = "Character fights for Enemies")]
        EnemyAligned = 1
    }
}
