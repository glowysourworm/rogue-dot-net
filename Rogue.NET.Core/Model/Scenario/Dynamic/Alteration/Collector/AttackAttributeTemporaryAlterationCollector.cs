using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector.Interface;
using System;
using System.Linq;
using System.Collections.Generic;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector
{
    /// <summary>
    /// Collects attack attribute alterations that are temporary application type
    /// </summary>
    [Serializable]
    public class AttackAttributeTemporaryAlterationCollector 
                    : IAlterationCollector,
                      IAlterationEffectCollector<AttackAttributeTemporaryAlterationEffect>, 
                      IAttackAttributeAlterationCollector, 
                      ITurnBasedAlterationCollector
    {
        protected IDictionary<string, AttackAttributeTemporaryAlterationEffect> TemporaryAlterations { get; set; }

        public AttackAttributeTemporaryAlterationCollector()
        {
            this.TemporaryAlterations = new Dictionary<string, AttackAttributeTemporaryAlterationEffect>();
        }

        public bool Apply(string alterationId, AttackAttributeTemporaryAlterationEffect effect, AlterationCost cost = null)
        {
            if (effect.IsStackable &&
                this.TemporaryAlterations.Any(x => x.Value.RogueName == effect.RogueName))
                return false;

            this.TemporaryAlterations.Add(alterationId, effect);

            return true;
        }

        public void Filter(string alterationId)
        {
            this.TemporaryAlterations.Remove(alterationId);
        }

        public void ApplyRemedy(RemedyAlterationEffect effect)
        {
            this.TemporaryAlterations.Filter(x => x.Value.AlteredState.RogueName == effect.RemediedState.RogueName);
        }

        #region IAlterationCollector
        public IEnumerable<AlterationCost> GetCosts()
        {
            // Temporary alterations only have a one-time cost
            return new List<AlterationCost>();
        }

        public IEnumerable<AlteredCharacterState> GetAlteredStates()
        {
            return new List<AlteredCharacterState>();
        }

        public IEnumerable<SymbolDeltaTemplate> GetSymbolChanges()
        {
            // Attack attribute effects don't support symbol changes
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
            return this.TemporaryAlterations
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

        #region ITurnBasedAlterationCollector
        public void ApplyEndOfTurn()
        {
            for (int i = this.TemporaryAlterations.Count - 1; i >= 0; i--)
            {
                var item = this.TemporaryAlterations.ElementAt(i);

                item.Value.EventTime--;

                if (item.Value.EventTime == 0)
                    this.TemporaryAlterations.Remove(item.Key);
            }
        }
        #endregion
    }
}
