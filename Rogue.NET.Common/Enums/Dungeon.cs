using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rogue.NET.Common
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
        Normal,
        Maze,
        Teleport,
        TeleportRandom,
        Hall,
        BigRoom,
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
