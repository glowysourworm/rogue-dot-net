using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector.Interface
{
    /// <summary>
    /// This component has a dual responsibility:  1) Collect source parameters (cost, aura color, aura range)
    /// and 2) Collect target effects. The target effects are queried to be applied at the end of each turn
    /// </summary>
    public interface IAlterationAuraSourceCollector<TEffect>
    {
        void Apply(string alterationId, TEffect targetEffect, AuraSourceParameters sourceParameters, AlterationCost cost = null);

        /// <summary>
        /// Pass-through method that will remove alteration from collector if it exists there
        /// </summary>
        void Filter(string alterationId);

        IEnumerable<AlterationCost> GetCosts();
        IEnumerable<AuraSourceParameters> GetAuraSourceParameters();
        IEnumerable<Tuple<TEffect, AuraSourceParameters>> GetEffects();
    }
}
