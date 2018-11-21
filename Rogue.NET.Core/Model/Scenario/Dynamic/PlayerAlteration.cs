using Rogue.NET.Core.Model.Scenario.Alteration;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Dynamic
{
    [Serializable]
    public class PlayerAlteration : CharacterAlteration
    {
        /// <summary>
        /// List of all active auras (PLAYER ONLY) - managed via Spell.Id
        /// </summary>
        public IDictionary<string, AlterationEffect> ActiveAuras { get; set; }

        public PlayerAlteration() : base()
        {
            this.ActiveAuras = new Dictionary<string, AlterationEffect>();
        }
    }
}
