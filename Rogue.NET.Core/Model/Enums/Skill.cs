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
    public enum AlterationBlockType
    {
        Magic,
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
        AttackAttribute
    }
    public enum AlterationAttackAttributeType
    {
        /// <summary>
        /// Permanent alteration to an Equipment item
        /// </summary>
        Imbue,

        /// <summary>
        /// Applied with an equipped item
        /// </summary>
        Passive,

        /// <summary>
        /// Applied with a temporary event to be calulcated as friendly to the source character
        /// </summary>
        TemporaryFriendlySource,

        /// <summary>
        /// Applied with a temporary event to be calulcated as friendly to the target
        /// </summary>
        TemporaryFriendlyTarget,

        /// <summary>
        /// Applied with a temporary event to be calulcated as malign to the source character
        /// </summary>
        TemporaryMalignSource,

        /// <summary>
        /// Applied with a temporary event to be calulcated as malign to the target
        /// </summary>
        TemporaryMalignTarget,

        /// <summary>
        /// Applied as a one-time attack attribute melee to the target
        /// </summary>
        MeleeTarget
    }
}
