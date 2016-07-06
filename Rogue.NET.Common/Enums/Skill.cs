using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rogue.NET.Common
{
    /// <summary>
    /// Specify effects that require code support after
    /// animation is played
    /// </summary>
    public enum AlterationMagicEffectType
    {
        None,
        ChangeLevelRandomUp,
        ChangeLevelRandomDown,
        Identify,
        Uncurse,    
        EnchantArmor,
        EnchantWeapon,
        
        RevealItems,
        RevealMonsters,
        RevealSavePoint,
        RevealFood,
        RevealLevel,

        RoamingLightSource,

        CreateMonster
    }
    public enum AlterationCostType
    {
        OneTime,
        PerStep
    }
    public enum AlterationBlockType
    {
        Magic,
        Physical
    }
    public enum AlterationType
    {
        PassiveSource,
        PassiveAura,
        TemporarySource,
        TemporaryTarget,
        TemporaryAllTargets,
        PermanentSource,
        PermanentTarget,
        PermanentAllTargets,
        Steal,
        RunAway,
        TeleportSelf,
        TeleportTarget,
        TeleportAllTargets,
        OtherMagicEffect,
        AttackAttribute
    }
    public enum AlterationAttackAttributeType
    {
        Imbue,
        Passive,
        TemporaryFriendlySource,
        TemporaryFriendlyTarget,
        TemporaryMalignSource,
        TemporaryMalignTarget,
        MeleeTarget
    }
}
