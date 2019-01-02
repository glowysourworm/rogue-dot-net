namespace Rogue.NET.Core.Model.Enums
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

        CreateMonster
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
        Physical
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
        Remedy
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
        MeleeTarget
    }
}
