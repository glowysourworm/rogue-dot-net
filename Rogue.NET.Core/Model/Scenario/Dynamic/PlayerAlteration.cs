using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Model.Scenario.Dynamic
{
    [Serializable]
    public class PlayerAlteration : CharacterAlteration
    {
        /// <summary>
        /// List of all active auras (PLAYER ONLY) - managed via AlterationContainer.Id
        /// </summary>
        protected IDictionary<string, AlterationEffect> ActiveAuras { get; set; }

        public PlayerAlteration() : base()
        {
            this.ActiveAuras = new Dictionary<string, AlterationEffect>();
        }

        //public PlayerAlteration(SerializationInfo info, StreamingContext context) : base(info, context)
        //{
        //    this.ActiveAuras = (Dictionary<string, AlterationEffect>)info.GetValue("ActiveAuras", typeof(Dictionary<string, AlterationEffect>));
        //}

        //public override void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    base.GetObjectData(info, context);

        //    info.AddValue("ActiveAuras", this.ActiveAuras, typeof(Dictionary<string, AlterationEffect>));
        //}

        /// <summary>
        /// Returns active aura effects - PLAYER ONLY
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AlterationEffect> GetActiveAuras()
        {
            return this.ActiveAuras.Values;
        }

        public override IEnumerable<string> GetStackableSpellIds()
        {
            return base.GetStackableSpellIds().Union(this.ActiveAuras.Keys);
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
                    this.ActiveAuras.Add(alteration.GeneratingSpellId, alteration.AuraEffect);
                    break;
            }
        }

        public override void DeactivatePassiveAlteration(string spellId)
        {
            base.DeactivatePassiveAlteration(spellId);

            if (this.ActiveAuras.ContainsKey(spellId))
                this.ActiveAuras.Remove(spellId);
        }
    }
}
