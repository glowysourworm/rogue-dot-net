using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector.Interface;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector
{
    [Serializable]
    public class AuraSourceAlterationCollector 
                 : IAlterationAuraSourceCollector<AuraAlterationEffect>
    {
        // Alteration Effects that are applied to any targets that wander in the effect range
        protected IDictionary<string, AuraAlterationEffect> TargetEffects { get; set; }
        protected IDictionary<string, AuraSourceParameters> SourceParameters { get; set; }
        protected IDictionary<string, AlterationCost> Costs { get; set; }

        public AuraSourceAlterationCollector()
        {
            this.TargetEffects = new Dictionary<string, AuraAlterationEffect>();
            this.SourceParameters = new Dictionary<string, AuraSourceParameters>();
            this.Costs = new Dictionary<string, AlterationCost>();
        }

        public void Apply(string alterationId, AuraAlterationEffect targetEffect, AuraSourceParameters sourceParameters, AlterationCost cost = null)
        {            
            this.TargetEffects.Add(alterationId, targetEffect);
            this.SourceParameters.Add(alterationId, sourceParameters);

            if (cost != null)
                this.Costs.Add(alterationId, cost);
        }
        public void Filter(string alterationId)
        {
            this.TargetEffects.Remove(alterationId);
            this.SourceParameters.Remove(alterationId);

            if (this.Costs.ContainsKey(alterationId))
                this.Costs.Remove(alterationId);
        }
        public IEnumerable<AlterationCost> GetCosts()
        {
            return this.Costs.Values;
        }
        public IEnumerable<AuraSourceParameters> GetAuraSourceParameters()
        {
            return this.SourceParameters.Values;
        }

        public IEnumerable<Tuple<AuraAlterationEffect, AuraSourceParameters>> GetEffects()
        {
            var result = new List<Tuple<AuraAlterationEffect, AuraSourceParameters>>();

            foreach (var effect in this.TargetEffects)
                result.Add(new Tuple<AuraAlterationEffect,
                                     AuraSourceParameters>(effect.Value, this.SourceParameters[effect.Key]));

            return result;
        }
    }
}
