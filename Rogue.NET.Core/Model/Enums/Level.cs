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
    public enum LevelActionType
    {
        None,
        Move,
        Attack,
        ToggleDoor,
        Search,
        Target,
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
    public enum PlayerActionType
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
    public enum LayoutType : int
    {
        // (DON'T RENUMBER) Numbers prevent loss of data - so enums can be refactored
        Maze = 1,
        ConnectedRectangularRooms = 7,
        ConnectedCellularAutomata = 8      
    }
    public enum LayoutCellularAutomataType : int
    {
        Open = 0,
        Filled = 1
    }
    public enum LayoutConnectionType : int
    {
        Corridor = 0,
        CorridorWithDoors = 1,
        Teleporter = 2,
        TeleporterRandom = 3
    }
    public enum LayoutConnectionGeometryType : int
    {
        /// <summary>
        /// Available for RectangularGrid room placement type
        /// </summary>
        Rectilinear = 0,

        /// <summary>
        /// Uses Minimum Spanning Tree algorithm to generate room connections
        /// </summary>
        MinimumSpanningTree = 1
    }
    public enum LayoutCorridorGeometryType : int
    {
        /// <summary>
        /// Straight line connecting cells from two rooms
        /// </summary>
        Linear = 0,
    }
    public enum LayoutRoomPlacementType : int
    {
        /// <summary>
        /// Rectangular grid of rooms
        /// </summary>
        RectangularGrid = 0,

        /// <summary>
        /// Random placement of rectangular rooms
        /// </summary>
        Random = 1
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
        Teleport1,
        Teleport2,
        TeleportRandom,
    }
    public enum DoodadPairType
    {
        TeleportPair
    }
    public enum GenerationRate
    {
        Low = 0,
        Medium,
        High,
    }
    public enum SymbolTypes
    {
        /// <summary>
        /// UTF-8 character set symbols
        /// </summary>
        Character,

        /// <summary>
        /// Smiley type wpf control
        /// </summary>
        Smiley,

        /// <summary>
        /// Level grid cell sized image
        /// </summary>
        Image,

        /// <summary>
        /// Larger (30x30) image used for other UI purposes
        /// </summary>
        DisplayImage
    }
}
