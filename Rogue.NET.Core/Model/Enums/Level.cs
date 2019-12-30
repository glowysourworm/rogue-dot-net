using System;
namespace Rogue.NET.Core.Model.Enums
{
    [Flags]
    public enum Compass : int
    {
        Null = 0,
        N = 1,
        S = 2,
        E = 4,
        W = 8,
        NW = 16,
        NE = 32,
        SE = 64,
        SW = 128
    }
    public enum MoveType
    {
        Ordinal = 0,
        Diagonal
    }
    public enum LevelCommandType
    {
        None,
        Move,
        Attack,
        ToggleDoor,
        Search,
        Throw,
        InvokeSkill,
        InvokeDoodad,
        Consume,
        Equip,
        Drop,
        Fire,

        // Only supported for debug configuration
        DebugNext,
        DebugSimulateNext,
        DebugExperience,
        DebugIdentifyAll,
        DebugRevealAll
    }

    /// <summary>
    /// These types of actions are initiated by the UI. (They originate from
    /// some kind of a UI or Dialog interaction)
    /// </summary>
    public enum PlayerCommandType
    {
        AlterationEffect,
        Uncurse,
        Identify,
        ActivateSkillSet,
        CycleSkillSet,
        SelectSkill,
        UnlockSkill,
        PlayerAdvancement
    }

    /// <summary>
    /// These types of actions are initiated by the UI. (They originate from
    /// some kind of a UI or Dialog interaction) They will have multiple items
    /// carried from the UI interaction. (Maybe a multi-item selector or something)
    /// </summary>
    public enum PlayerMultiItemActionType
    {
        AlterationEffect
    }

    /// <summary>
    /// Enumeration meant for working with multiple-UI interations. For example, casting a spell
    /// would require first working with the UI for animation. Also, this could be used for procssing
    /// further actions based on what happened during the request. For example, if an action is cancelled
    /// because the player doesn't meet the requirements, then maybe return DoNothing.
    /// </summary>
    public enum LevelContinuationAction
    {
        DoNothing,

        /// <summary>
        /// Processes Turn to allow Enemies to react. Can occur when Player performs an action that constitutes a turn - meaning they did something: Opened
        /// a door; Searched a wall; etc... They're penalized with no regenration for that turn.
        /// </summary>
        ProcessTurnNoRegeneration,

        /// <summary>
        /// Processes turn to allow enemies to react
        /// </summary>
        ProcessTurn,
    }
    public enum DoodadType
    {
        Normal,
        Magic
    }
    public enum DoodadNormalType
    {
        StairsUp,
        StairsDown,
        SavePoint,
        Transporter
    }

    public enum ContentRandomPlacementType
    {
        /// <summary>
        /// Content will be placed on a random walkable location
        /// </summary>
        Random = 0,

        /// <summary>
        /// Content will be placed inside a random region - WITH DEFAULT TO A RANDOM LEVEL LOCATION
        /// </summary>
        RandomRegion = 1,

        /// <summary>
        /// Content will be place inside a random corridor - WITH DEFAULT TO A RANDOM LEVEL LOCATION
        /// </summary>
        RandomCorridor = 2
    }

    /// <summary>
    /// Specifies how content groups are added to the level
    /// </summary>
    public enum ContentGroupPlacementType
    {
        /// <summary>
        /// Contents are placed in adjacent cells. This should fail if there are no adjacent cells with enough room
        /// for the contents.
        /// </summary>
        Adjacent = 0,

        /// <summary>
        /// This will "randomly maximize" the total distance between the contents in the group. The random number will
        /// be drawn from a Poisson distribution to get a value "close to" the max distance. This draw will occur once
        /// per content object in the group. The first object will be placed somewhere near the edge of a region or
        /// the edge region if there are multiple.
        /// </summary>
        RandomlyDistant = 1,

        /// <summary>
        /// Same as Randomly Distant with the added constraint that they're all located in rooms
        /// </summary>
        RandomlyDistantRoom = 2
    }
}
