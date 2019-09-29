using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector.Interface;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Extension;

using System;
using System.Linq;
using System.Collections.Generic;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector
{
    /// <summary>
    /// This component collects aura effects that are effect the target character.
    /// </summary>
    [Serializable]
    public class AuraTargetAlterationCollector : IAlterationEffectCollector
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

        public IEnumerable<KeyValuePair<string, IAlterationEffect>> GetEffects(bool includeSourceEffects = false)
        {
            return this.TargetEffects
                       .ToDictionary(x => x.RogueName, x => (IAlterationEffect)x);
        }

        public IEnumerable<SymbolEffectTemplate> GetSymbolChanges()
        {
            return this.TargetEffects
                       .Where(x => x.SymbolAlteration.HasSymbolChange())
                       .Select(x => x.SymbolAlteration)
                       .Actualize();
        }

        public IEnumerable<AlteredCharacterState> GetAlteredStates()
        {
            // Auras don't support altered states
            return new List<AlteredCharacterState>();
        }

        public double GetAttributeAggregate(CharacterAttribute attribute)
        {
            return this.TargetEffects
                       .Aggregate(0D, (aggregator, effect) => effect.GetAttribute(attribute));
        }
    }
}
