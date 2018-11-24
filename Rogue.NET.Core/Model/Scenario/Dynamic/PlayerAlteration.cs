using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Rogue.NET.Common.Extension;

namespace Rogue.NET.Core.Model.Scenario.Dynamic
{
    [Serializable]
    public class PlayerAlteration : CharacterAlteration
    {
        /// <summary>
        /// List of all active auras (PLAYER ONLY) - These DO NOT EFFECT THE PLAYER; but DO
        /// effect any Enemies in range.
        /// </summary>
        protected IDictionary<SpellReference, AlterationEffect> ActiveAuras { get; set; }

        public PlayerAlteration() : base()
        {
            this.ActiveAuras = new Dictionary<SpellReference, AlterationEffect>();
        }

        /// <summary>
        /// Returns active aura effects - PLAYER ONLY
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AlterationEffect> GetActiveAuras()
        {
            return this.ActiveAuras.Values;
        }

        /// <summary>
        /// Returns list of all spell id's involved with stackable calculations
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> GetStackableSpellIds()
        {
            return base.GetStackableSpellIds().Union(this.ActiveAuras.Keys.Select(x => x.SpellId));
        }

        // NOTE*** DO NOTHING HERE BECAUSE AURA'S AREN'T SUPPORTED FOR ENEMIES. Aura EFFECTS
        //         are ONLY supported for Enemies because Player won't be affected by any Aura's - but
        //         will be the ONLY GENERATOR of Aura's
        protected override void ApplyTargetAlteration(AlterationContainer alteration)
        {
            base.ApplyTargetAlteration(alteration);
        }

        protected override void ApplySourceAlteration(AlterationContainer alteration)
        {
            base.ApplySourceAlteration(alteration);

            // Only overrides PassiveAura type - no other exception handling
            switch (alteration.Type)
            {
                case AlterationType.PassiveAura:
                    this.ActiveAuras.Add(new SpellReference(alteration), alteration.AuraEffect);
                    break;
            }
        }

        public override void DeactivatePassiveAlteration(string spellId)
        {
            base.DeactivatePassiveAlteration(spellId);

            // Active Auras
            this.ActiveAuras.Filter(x => x.Key.SpellId == spellId);
        }
    }
}
