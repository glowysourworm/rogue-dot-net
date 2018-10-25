using Rogue.NET.Core.Model.Scenario.Alteration;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Dynamic
{
    public class PlayerAlteration : CharacterAlteration
    {
        /// <summary>
        /// List of all active auras (PLAYER ONLY)
        /// </summary>
        public List<AlterationEffect> ActiveAuras { get; set; }
    }
}
