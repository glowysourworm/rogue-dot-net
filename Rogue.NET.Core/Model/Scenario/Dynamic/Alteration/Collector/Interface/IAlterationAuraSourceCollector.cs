using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector.Interface
{
    /// <summary>
    /// This component has a dual responsibility:  1) Collect source parameters (cost, aura color, aura range)
    /// and 2) Collect target effects. The target effects are queried to be applied at the end of each turn
    /// </summary>
    public interface IAlterationAuraSourceCollector
    {
        /// <summary>
        /// Returns Aura Source Parameters that are affecting the character
        /// </summary>
        /// <returns></returns>
        IEnumerable<AuraSourceParameters> GetAuraSourceParameters();

        /// <summary>
        /// Returns a collection of effects that are applied to characters in range
        /// </summary>
        IEnumerable<Tuple<IAlterationEffect, AuraSourceParameters>> GetAuraEffects();
    }
}
