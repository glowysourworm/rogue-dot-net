using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector
{
    /// <summary>
    /// Collects attack attribute alterations that are passive application type
    /// </summary>
    [Serializable]
    public class AttackAttributePassiveAlterationCollector 
                    : IAlterationCollector,
                      IAlterationEffectCollector,
                      IAttackAttributeAlterationCollector
    {
        protected IDictionary<string, Scenario.Alteration.Common.AlterationContainer> Alterations { get; set; }

        public AttackAttributePassiveAlterationCollector()
        {
            this.Alterations = new Dictionary<string, Scenario.Alteration.Common.AlterationContainer>();
        }
        public bool Apply(Scenario.Alteration.Common.AlterationContainer alteration)
        {
            if (!this.Alterations.ContainsKey(alteration.RogueName))
                this.Alterations.Add(alteration.RogueName, alteration);

            else
                return false;

            return true;
        }

        public IEnumerable<Scenario.Alteration.Common.AlterationContainer> Filter(string alterationName)
        {
            return this.Alterations.Filter(x => x.Key == alterationName).Values.Actualize();
        }

        public IEnumerable<KeyValuePair<string, AlterationCost>> GetCosts()
        {
            return this.Alterations
                       .ToDictionary(x => x.Key, x => x.Value.Cost);
        }

        public IEnumerable<KeyValuePair<string, IAlterationEffect>> GetEffects(bool includeSourceEffects)
        {
            return this.Alterations
                       .ToDictionary(x => x.Key, x => x.Value.Effect);
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

        public IEnumerable<AttackAttribute> GetAttackAttributes(AlterationAttackAttributeCombatType combatType)
        {
            return this.Alterations
                    .Values
                    .Select(x => x.Effect)
                    .Cast<AttackAttributePassiveAlterationEffect>()
                    .Where(x => x.CombatType == combatType)
                    .Aggregate(new List<AttackAttribute>(), (aggregator, effect) =>
                    {
                        foreach (var attackAttribute in effect.AttackAttributes)
                        {
                            var existingAttackAttribute = aggregator.FirstOrDefault(x => x.RogueName == attackAttribute.RogueName);

                            if (existingAttackAttribute == null)
                                aggregator.Add(attackAttribute);

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

        public IEnumerable<string> GetEffectNames(AlterationAttackAttributeCombatType combatType)
        {
            // TODO:ALTERATION - Have to apply the AlterationContainer.RogueName to all the
            //                   IAlterationEffect instances.
            return this.Alterations
                       .Values
                       .Cast<AttackAttributePassiveAlterationEffect>()
                       .Where(x => x.CombatType == combatType)
                       .Select(x => x.RogueName)
                       .Actualize();
        }
    }
}
