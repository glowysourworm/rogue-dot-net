using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector
{
    /// <summary>
    /// Collects alterations that are effecting the character
    /// </summary>
    [Serializable]
    public class AttackAttributeAuraTargetAlterationCollector 
               : IAttackAttributeAlterationCollector
    {
        protected IList<AttackAttributeAuraAlterationEffect> TargetEffects { get; set; }

        public AttackAttributeAuraTargetAlterationCollector()
        {
            this.TargetEffects = new List<AttackAttributeAuraAlterationEffect>();
        }

        /// <summary>
        /// Clears / re-applies all specified effects that act ON THE CHARACTER. This should
        /// be done at the end of each turn.
        /// </summary>
        public void Apply(IEnumerable<AttackAttributeAuraAlterationEffect> alterationEffects)
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

        public IEnumerable<AttackAttribute> GetAttackAttributes(AlterationAttackAttributeCombatType combatType)
        {
            return this.TargetEffects
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
    }
}
