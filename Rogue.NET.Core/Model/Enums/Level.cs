﻿using System;
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
    public enum LevelAction : int
    {
        Null = 0,
        Move,
        Attack,
        Open,
        Close,
        Search,
        Target,
        Throw,
        InvokeSkill,
        InvokeDoodad,
        Consume,
        Equip,
        Enchant,
        Uncurse,
        Identify,
        Drop,
        Fire,
        EmphasizeSkillUp,
        EmphasizeSkillDown,
        ActivateSkill,
        DeactivateSkill,
        CycleActiveSkill,
        DebugNext,
        DebugExperience,
        DebugIdentifyAll,
        DebugSkillUp,
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
        ProcessTurnNoRegeneration,
        ProcessTurn,
        ProcessTurnAfterAnimation,
        DungeonTickNoReactions
    }
    public enum LayoutType : int
    {
        Normal = 0,
        Maze = 1,
        Teleport = 2,
        TeleportRandom = 3,
        // (DON'T RENUMBER) Shop was removed - had to number these to support saved configuration
        Hall = 5,
        BigRoom = 6,
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
        Character,
        Smiley,
        Image
    }
}