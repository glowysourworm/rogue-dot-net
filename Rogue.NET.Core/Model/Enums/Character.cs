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
    public enum CharacterRestBehaviorType
    {
        [Display(Name = "Home Location",
                 Description = "Character rests at its initial location")]
        HomeLocation = 0,

        [Display(Name = "Current Location",
                 Description = "Character rests at current location after it performs its search sweep")]
        CurrentLocation = 1
    }
    
    [Flags]
    public enum BehaviorCondition : int
    {
        [Display(Name = "Attack Condition Met",
                 Description = "Can attack using the prescribed behavior attack method (skill costs met)")]
        AttackConditionsMet = 0,

        [Display(Name = "Health Low",
                 Description = "Behavior ONLY available when Health is below 10%")]
        HealthLow = 1
    }
    [Flags]
    public enum BehaviorExitCondition : int
    {
        [Display(Name = "Behavior Counter Expired",
                 Description = "Behavior has a sub-counter that acts to change selected behavior in the state machine (of behaviors)")]
        BehaviorCounterExpired = 0,

        [Display(Name = "Health Low",
                 Description = "Behavior will not be available when Health is below 10%")]
        HealthLow = 1
    }

    public enum SmileyExpression
    {
        [Display(Name = "Happy",
                 Description = "Happy smiley face expression")]
        Happy = 0,

        [Display(Name = "Blind",
                 Description = "Blind smiley face expression")]
        Blind = 1,

        [Display(Name = "Dead",
                 Description = "Dead smiley face expression")]
        Dead = 2,

        [Display(Name = "Disgruntled",
                 Description = "Unhappy smiley face expression")]
        Disgruntled = 3,

        [Display(Name = "Emoji",
                 Description = "Very happy smiley face expression")]
        Emoji = 4,

        [Display(Name = "Freaked Out",
                 Description = "Scared smiley face expression")]
        FreakedOut = 5,

        [Display(Name = "Frustrated",
                 Description = "Frustrated or exasporated smiley face expression")]
        Frustrated = 6,

        [Display(Name = "Insane",
                 Description = "Insane smiley face expression")]
        Insane = 7,

        [Display(Name = "Left Wink",
                 Description = "Winking smiley face expression")]
        LeftWink = 8,

        [Display(Name = "Mean",
                 Description = "Mean smiley face expression")]
        MeanPumpkinFace = 9,

        [Display(Name = "Mischievous",
                 Description = "Mischievous smiley face expression")]
        Mischievous = 10,

        [Display(Name = "Right Wink",
                 Description = "Winking smiley face expression")]
        RightWink = 11,

        [Display(Name = "Sad",
                 Description = "Sad smiley face expression")]
        Sad = 12,

        [Display(Name = "Scared",
                 Description = "Scared smiley face expression")]
        Scared = 13,

        [Display(Name = "Shocked",
                 Description = "Shocked smiley face expression")]
        Shocked = 14,

        [Display(Name = "Sleeping",
                 Description = "Sleeping smiley face expression")]
        Sleeping = 15,

        [Display(Name = "Sour",
                 Description = "Angry smiley face expression")]
        Sour = 16,

        [Display(Name = "Unsure",
                 Description = "Unsure smiley face expression")]
        Unsure = 17,

        [Display(Name = "Weird",
                 Description = "Strange smiley face expression")]
        WeirdWhistler = 18
    }

    /// <summary>
    /// Represents charater attributes
    /// </summary>
    public enum CharacterAttribute
    {
        [Display(Name = "Health",
                 Description = "Health character attribute")]
        Health,

        [Display(Name = "Stamina",
                 Description = "Stamina character attribute")]
        Stamina,

        [Display(Name = "Strength",
                 Description = "Strength character attribute")]
        Strength,

        [Display(Name = "Agility",
                 Description = "Agility character attribute")]
        Agility,

        [Display(Name = "Intelligence",
                 Description = "Intelligence character attribute")]
        Intelligence,

        [Display(Name = "Speed",
                 Description = "Speed character attribute")]
        Speed,

        [Display(Name = "Health Regen",
                 Description = "Health regeneration character attribute")]
        HealthRegen,

        [Display(Name = "Stamina Regen",
                 Description = "Stamina regeneration character attribute")]
        StaminaRegen,

        [Display(Name = "Vision",
                 Description = "Vision character attribute")]
        Vision,

        [Display(Name = "Attack",
                 Description = "Attack character attribute")]
        Attack,

        [Display(Name = "Defense",
                 Description = "Defense character attribute")]
        Defense,

        [Display(Name = "Food Usage",
                 Description = "Food usage (per turn) character attribute")]
        FoodUsagePerTurn
    }

    /// <summary>
    /// Represents Character Base Attributes (non-derived)
    /// </summary>
    public enum CharacterBaseAttribute
    {
        [Display(Name = "Strength",
                 Description = "Strength character base attribute")]
        Strength,

        [Display(Name = "Agility",
                 Description = "Agility character base attribute")]
        Agility,

        [Display(Name = "Intelligence",
                 Description = "Intelligence character base attribute")]
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
        EnemyAligned = 1,

        [Display(Name = "None",
                 Description = "(This setting is not allowed - TODO)")]
        None = 2
    }
}
