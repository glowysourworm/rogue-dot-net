namespace Rogue.NET.Core.Model.Enums
{
    /// <summary>
    /// Specify effects that require code support after
    /// animation is played
    /// </summary>
    public enum AlterationMagicEffectType
    {
        None = 0,
        ChangeLevelRandomUp = 1,
        ChangeLevelRandomDown = 2,
        Identify = 3,
        Uncurse = 4,    
        EnchantArmor = 5,
        EnchantWeapon = 6,
        
        RevealItems = 7,
        RevealMonsters = 8,
        RevealSavePoint = 9,
        RevealFood = 10,
        RevealLevel = 11,

        CreateMonster = 12
    }
    public enum AlterationCostType
    {
        OneTime,
        PerStep
    }
    /// <summary>
    /// Defines a way to block (completely negate Effects) Alterations using different stats
    /// </summary>
    public enum AlterationBlockType
    {
        /// <summary>
        /// Blocks Alteration using Base Intelligence ONLY
        /// </summary>
        Mental,

        /// <summary>
        /// Blocks Alteration using Base Agility ONLY
        /// </summary>
        Physical,

        /// <summary>
        /// Alteration can't be blocked
        /// </summary>
        NonBlockable
    }
    public enum AlterationType
    {
        PassiveSource,

        /// <summary>
        /// NOT SUPPORTED FOR ENEMIES
        /// </summary>
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
        AttackAttribute,

        /// <summary>
        /// Remedies spell temporary effects - SOURCE ONLY
        /// </summary>
        Remedy,

        TemporaryAllInRange,
        TemporaryAllInRangeExceptSource,
        PermanentAllInRange,
        PermanentAllInRangeExceptSource,
        TeleportAllInRange,
        TeleportAllInRangeExceptSource
    }
    public enum AlterationAttackAttributeType
    {
        /// <summary>
        /// Permanent alteration to an Armor item
        /// </summary>
        ImbueArmor,

        /// <summary>
        /// Permanent alteration to a Weapon item
        /// </summary>
        ImbueWeapon,

        /// <summary>
        /// Applied with an equipped item
        /// </summary>
        Passive,

        /// <summary>
        /// Applied with a temporary event to be calulcated as friendly to the source character (EXAMPLE WOULD BE DEFENSIVE EVENT)
        /// </summary>
        TemporaryFriendlySource,

        /// <summary>
        /// Applied with a temporary event to be calulcated as friendly to the target (EXAMPLE WOULD BE DEFENSIVE EVENT)
        /// </summary>
        TemporaryFriendlyTarget,

        /// <summary>
        /// Applied with a temporary event to be calulcated as malign to the source character (EXAMPLE WOULD BE POISON)
        /// </summary>
        TemporaryMalignSource,

        /// <summary>
        /// Applied with a temporary event to be calulcated as malign to the target (EXAMPLE WOULD BE POISON)
        /// </summary>
        TemporaryMalignTarget,

        /// <summary>
        /// Applied as a one-time attack attribute melee to the target (EXAMPLE IS FIREBALL)
        /// </summary>
        MeleeTarget,

        /// <summary>
        /// Applied as a one-time attack attribute melee to all characters in range
        /// </summary>
        MeleeAllInRange,

        /// <summary>
        /// Applied as a one-time attack attribute melee to all characters in range except source character
        /// </summary>
        MeleeAllInRangeExceptSource,

        /// <summary>
        /// Applied with a temporary event to be calculated as malign to all in range (EXAMPLE WOULD BE POISON)
        /// </summary>
        TemporaryMalignAllInRange,

        /// <summary>
        /// Applied with a temporary event to be calculated as malign to all in range except source (EXAMPLE WOULD BE POISON)
        /// </summary>
        TemporaryMalignAllInRangeExceptSource
    }
}
