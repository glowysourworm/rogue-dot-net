using System;
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
        Abnormal
    }
    /// <summary>
    /// Specifies character movement
    /// </summary>
    public enum CharacterMovementType
    {
        /// <summary>
        /// Random Walk - Hits if in range
        /// </summary>
        Random,

        /// <summary>
        /// Doesn't think to use doors
        /// </summary>
        HeatSeeker,

        /// <summary>
        /// Figures out a way to the player
        /// </summary>
        PathFinder,

        /// <summary>
        /// Stays just inside its attack range and
        /// Fires range missiles.. or simply being a coward
        /// </summary>
        StandOffIsh,
    }
    public enum CharacterAttackType
    {
        Melee,

        /// <summary>
        /// This should cover Range attacks for enemies
        /// </summary>
        Skill,

        /// <summary>
        /// Skill that can only be used at very close (radius=1) range
        /// </summary>
        SkillCloseRange,

        /// <summary>
        /// no attack
        /// </summary>
        None
    }
    [Flags]
    public enum BehaviorCondition : int
    {
        /// <summary>
        /// Can attack using the prescribed behavior attack method (skill costs met)
        /// </summary>
        AttackConditionsMet = 1,

        /// <summary>
        /// Some behaviors may rely on having low HP
        /// </summary>
        HpLow = 2
    }
    [Flags]
    public enum BehaviorExitCondition : int
    {
        /// <summary>
        /// Behavior has a sub-counter that acts to change selected behavior in the
        /// "state machine" (of behaviors)
        /// </summary>
        BehaviorCounterExpired = 1,

        /// <summary>
        /// Some behaviors may exit on low HP - others may switch on (see BehaviorCondition)
        /// </summary>
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
