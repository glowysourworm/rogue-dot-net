using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector.Interface;
using System;
using System.Linq;
using System.Collections.Generic;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Core.Model.Scenario.Alteration.Common.Extension;
using Rogue.NET.Common.Collection;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector
{
    /// <summary>
    /// Collects attack attribute alterations that are temporary application type
    /// </summary>
    [Serializable]
    public class AttackAttributeTemporaryAlterationCollector 
                    : IAlterationCollector,
                      IAlterationEffectCollector,
                      IAttackAttributeAlterationCollector, 
                      ITemporaryAlterationCollector
    {
        protected IList<Scenario.Alteration.Common.AlterationContainer> Alterations { get; set; }

        public AttackAttributeTemporaryAlterationCollector()
        {
            this.Alterations = new List<Scenario.Alteration.Common.AlterationContainer>();
        }

        public bool Apply(Scenario.Alteration.Common.AlterationContainer alteration)
        {
            if (!alteration.Effect.IsStackable() &&
                this.Alterations.Any(x => x.RogueName == alteration.RogueName))
                return false;

            this.Alterations.Add(alteration);

            return true;
        }

        public IEnumerable<Scenario.Alteration.Common.AlterationContainer> Filter(string alterationName)
        {
            return this.Alterations
                       .Remove(x => x.RogueName == alterationName);
        }

        public IEnumerable<Scenario.Alteration.Common.AlterationContainer> ApplyRemedy(RemedyAlterationEffect remedyEffect)
        {
            var curedAlterations = new List<Scenario.Alteration.Common.AlterationContainer>();

            for (int i = this.Alterations.Count - 1; i >= 0; i--)
            {
                var effect = this.Alterations[i].Effect as AttackAttributeTemporaryAlterationEffect;

                // Compare the altered state name with the remedied state name
                if (effect.AlteredState.RogueName == remedyEffect.RemediedState.RogueName)
                {
                    // Add alteration to result
                    curedAlterations.Add(this.Alterations[i]);

                    // Remove alteration from list
                    this.Alterations.RemoveAt(i);
                }
            }

            return curedAlterations;
        }

        public IEnumerable<KeyValuePair<string, AlterationCost>> GetCosts()
        {
            // Temporary alterations only have a one-time cost
            return new SimpleDictionary<string, AlterationCost>();
        }

        public IEnumerable<KeyValuePair<string, IAlterationEffect>> GetEffects(bool includeSourceEffects = false)
        {
            return this.Alterations
                       .Select(x => new KeyValuePair<string, IAlterationEffect>(x.RogueName, x.Effect))
                       .Actualize();
        }

        public IEnumerable<AlteredCharacterState> GetAlteredStates()
        {
            return this.Alterations
                       .Select(x => x.Effect)
                       .Cast<AttackAttributeTemporaryAlterationEffect>()
                       .Where(x => x.HasAlteredState)
                       .Select(x => x.AlteredState)
                       .Actualize();
        }

        public IEnumerable<SymbolEffectTemplate> GetSymbolChanges()
        {
            // Attack attribute effects don't support symbol changes
            return this.Alterations
                       .Select(x => x.Effect)
                       .Cast<AttackAttributeTemporaryAlterationEffect>()
                       .Where(x => x.SymbolAlteration.HasSymbolChange())
                       .Select(x => x.SymbolAlteration)
                       .Actualize();
        }

        public double GetAttributeAggregate(CharacterAttribute attribute)
        {
            return 0D;
        }

        public IEnumerable<AttackAttribute> GetAttackAttributes(AlterationAttackAttributeCombatType combatType)
        {
            return this.Alterations
                        .Select(x => x.Effect)
                        .Cast<AttackAttributeTemporaryAlterationEffect>()
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
                       .Select(x => x.Effect)
                       .Cast<AttackAttributeTemporaryAlterationEffect>()
                       .Where(x => x.CombatType == combatType)
                       .Select(x => x.RogueName)
                       .Actualize();
        }

        public IEnumerable<string> Decrement()
        {
            // Decrement Event Counter
            this.Alterations
                .ForEach(x => (x.Effect as AttackAttributeTemporaryAlterationEffect).EventTime--);

            // Return effects that have worn off
            return this.Alterations
                       .Remove(x => (x.Effect as AttackAttributeTemporaryAlterationEffect).EventTime <= 0)
                       .Select(x => x.RogueName);
        }
    }
}
