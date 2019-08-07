using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector
{
    /// <summary>
    /// Collects attack attribute alterations that are passive application type
    /// </summary>
    [Serializable]
    public class AttackAttributePassiveAlterationCollector 
                    : IAlterationCollector,
                      IAlterationEffectCollector<AttackAttributePassiveAlterationEffect>, 
                      IAttackAttributeAlterationCollector
    {
        protected IDictionary<string, AttackAttributePassiveAlterationEffect> PassiveAlterations { get; set; }
        protected IDictionary<string, AlterationCost> Costs { get; set; }

        public AttackAttributePassiveAlterationCollector()
        {
            this.PassiveAlterations = new Dictionary<string, AttackAttributePassiveAlterationEffect>();
            this.Costs = new Dictionary<string, AlterationCost>();
        }

        public bool Apply(string alterationId, AttackAttributePassiveAlterationEffect alterationEffect, AlterationCost cost = null)
        {
            this.PassiveAlterations.Add(alterationId, alterationEffect);

            if (cost != null)
                this.Costs.Add(alterationId, cost);

            return true;
        }

        public void Filter(string alterationId)
        {
            if (this.PassiveAlterations.ContainsKey(alterationId))
                this.PassiveAlterations.Remove(alterationId);

            if (this.Costs.ContainsKey(alterationId))
                this.Costs.Remove(alterationId);
        }

        #region IAlterationCollector
        public IEnumerable<AlterationCost> GetCosts()
        {
            return this.Costs.Values;
        }

        public IEnumerable<AlteredCharacterState> GetAlteredStates()
        {
            return new List<AlteredCharacterState>();
        }

        public IEnumerable<SymbolDeltaTemplate> GetSymbolChanges()
        {
            // Attack Attribute Passive Alteration Effect doesn't support symbol changes
            return new List<SymbolDeltaTemplate>();
        }

        public double GetAttributeAggregate(CharacterAttribute attribute)
        {
            return 0D;
        }
        #endregion

        #region IAttackAttributeAlterationCollector
        public IEnumerable<AttackAttribute> GetAttackAttributes(AlterationAttackAttributeCombatType combatType)
        {
            return this.PassiveAlterations
                    .Values
                    .Where(x => x.CombatType == combatType)
                    .Aggregate(new List<AttackAttribute>(), (aggregator, effect) =>
                    {
                        foreach (var attackAttribute in effect.AttackAttributes)
                        {
                            var existingAttackAttribute = aggregator.FirstOrDefault(x => x.RogueName == attackAttribute.RogueName);

                            if (existingAttackAttribute == null)
                                aggregator.Add(existingAttackAttribute);

                            else
                            {
                                existingAttackAttribute.Attack += attackAttribute.Attack;
                                existingAttackAttribute.Resistance += attackAttribute.Resistance;
                                existingAttackAttribute.Weakness += attackAttribute.Weakness;
                            }
                        }

                        return aggregator;
                    });
        }
        #endregion
    }
}
