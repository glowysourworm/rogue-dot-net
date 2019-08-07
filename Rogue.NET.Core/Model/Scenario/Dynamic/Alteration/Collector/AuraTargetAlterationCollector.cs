using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector
{
    /// <summary>
    /// This component collects aura effects that are effect the target character.
    /// </summary>
    [Serializable]
    public class AuraTargetAlterationCollector
    {
        protected IList<AuraAlterationEffect> TargetEffects { get; set; }

        public AuraTargetAlterationCollector()
        {
            this.TargetEffects = new List<AuraAlterationEffect>();
        }

        /// <summary>
        /// Clears / re-applies all specified effects that act ON THE CHARACTER. This should
        /// be done at the end of each turn.
        /// </summary>
        public void Apply(IEnumerable<AuraAlterationEffect> alterationEffects)
        {
            this.TargetEffects.Clear();
            foreach (var effect in alterationEffects)
                this.TargetEffects.Add(effect);
        }

        public IEnumerable<SymbolDeltaTemplate> GetSymbolChanges()
        {
            return this.TargetEffects
                       .Where(x => x.SymbolAlteration.HasSymbolDelta())
                       .Select(x => x.SymbolAlteration)
                       .Actualize();
        }
    }
}
