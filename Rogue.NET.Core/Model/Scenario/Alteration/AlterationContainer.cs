using Rogue.NET.Core.Model.Enums;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration
{
    /// <summary>
    /// Contains all charater altering data - (cost, effect, type, etc...)
    /// </summary>
    [Serializable]
    public class AlterationContainer : RogueBase
    {
        public AlterationCost Cost { get; set; }
        public AlterationEffect Effect { get; set; }
        public AlterationEffect AuraEffect { get; set; }
        public AlterationType Type { get; set; }
        public AlterationMagicEffectType OtherEffectType { get; set; }
        public AlterationAttackAttributeType AttackAttributeType { get; set; }
        public double EffectRange { get; set; }
        public bool IsStackable { get; set; }
        public bool ScaledByIntelligence { get; set; }

        /// <summary>
        /// Spell Id used as a reference for lookup
        /// </summary>
        public string GeneratingSpellId { get; set; }

        /// <summary>
        /// Spell RogueName used as a reference for lookup (Remedies)
        /// </summary>
        public string GeneratingSpellName { get; set; }

        /// <summary>
        /// Enemy created as an effect of the alteration
        /// </summary>
        public string CreateMonsterEnemy { get; set; }

        public AlterationContainer() { }
    }
}
