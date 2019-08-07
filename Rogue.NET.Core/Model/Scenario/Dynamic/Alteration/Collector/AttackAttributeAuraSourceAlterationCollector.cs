using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector.Interface;
using System;
using System.Collections.Generic;


namespace Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector
{
    /// <summary>
    /// Collects alterations that are produced by the character
    /// </summary>
    [Serializable]
    public class AttackAttributeAuraSourceAlterationCollector 
                 : IAlterationAuraSourceCollector<AttackAttributeAuraAlterationEffect>
    {
        // Alteration Effects that are applied to any targets that wander in the effect range
        protected IDictionary<string, AttackAttributeAuraAlterationEffect> TargetEffects { get; set; }
        protected IDictionary<string, AlterationCost> Costs { get; set; }
        protected IDictionary<string, AuraSourceParameters> SourceParameters { get; set; }

        public AttackAttributeAuraSourceAlterationCollector()
        {
            this.Costs = new Dictionary<string, AlterationCost>();
            this.TargetEffects = new Dictionary<string, AttackAttributeAuraAlterationEffect>();
            this.SourceParameters = new Dictionary<string, AuraSourceParameters>();
        }
        public void Apply(string alterationId, AttackAttributeAuraAlterationEffect targetEffect, AuraSourceParameters sourceParameters, AlterationCost cost = null)
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

        public IEnumerable<Tuple<AttackAttributeAuraAlterationEffect, AuraSourceParameters>> GetEffects()
        {
            var result = new List<Tuple<AttackAttributeAuraAlterationEffect, AuraSourceParameters>>();

            foreach (var effect in this.TargetEffects)
                result.Add(new Tuple<AttackAttributeAuraAlterationEffect, 
                                     AuraSourceParameters>(effect.Value, this.SourceParameters[effect.Key]));

            return result;
        }
    }
}
