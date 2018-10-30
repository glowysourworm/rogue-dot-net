using System;
namespace Rogue.NET.Core.Model.Enums
{
    [Flags]
    public enum CharacterStateType : uint
    {
        //No turn impedement
        Normal = 0,
        Blind = 1,
        Paralyzed = 2,
        Sleeping = 4,
        Confused = 8,
        Silenced = 16
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
        /// no attack
        /// </summary>
        None
    }
    public enum SecondaryBehaviorInvokeReason
    {
        SecondaryNotInvoked,
        PrimaryInvoked,
        HpLow,
        Random,
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
    public enum AttributeEmphasis
    {
        Strength,
        Agility,
        Intelligence
    }
}
