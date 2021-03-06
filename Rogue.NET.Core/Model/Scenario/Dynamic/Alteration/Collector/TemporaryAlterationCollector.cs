﻿using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector.Interface;
using System;
using System.Linq;
using System.Collections.Generic;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Core.Model.Scenario.Alteration.Common.Extension;
using Rogue.NET.Common.Collection;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector
{
    [Serializable]
    public class TemporaryAlterationCollector 
                    : IAlterationCollector,
                      IAlterationEffectCollector,
                      ITemporaryAlterationCollector
    {
        protected IList<AlterationContainer> Alterations { get; set; }

        public TemporaryAlterationCollector()
        {
            this.Alterations = new List<AlterationContainer>();
        }

        public bool Apply(AlterationContainer alteration)
        {
            if (!alteration.Effect.IsStackable() &&
                this.Alterations.Any(x => x.RogueName == alteration.RogueName))
                return false;

            this.Alterations.Add(alteration);

            return true;
        }

        public IEnumerable<Scenario.Alteration.Common.AlterationContainer> Filter(string alterationName)
        {
            return this.Alterations.Where(x => x.RogueName == alterationName).Actualize();
        }

        public IEnumerable<Scenario.Alteration.Common.AlterationContainer> ApplyRemedy(RemedyAlterationEffect remedyEffect)
        {
            var curedAlterations = new List<Scenario.Alteration.Common.AlterationContainer>();

            for (int i = this.Alterations.Count - 1; i >= 0; i--)
            {
                var effect = this.Alterations[i].Effect as TemporaryAlterationEffect;

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
                       .Cast<TemporaryAlterationEffect>()
                       .Where(x => x.HasAlteredState)
                       .Select(x => x.AlteredState)
                       .Actualize();
        }

        public IEnumerable<SymbolEffectTemplate> GetSymbolChanges()
        {
            return this.Alterations
                       .Select(x => x.Effect)
                       .Cast<TemporaryAlterationEffect>()
                       .Where(x => x.SymbolAlteration.HasSymbolChange())
                       .Select(x => x.SymbolAlteration)
                       .Actualize();
        }

        public bool CanSeeInvisible()
        {
            return this.Alterations
                       .Select(x => x.Effect)
                       .Cast<TemporaryAlterationEffect>()
                       .Any(x => x.CanSeeInvisibleCharacters);
        }

        public IEnumerable<string> Decrement()
        {
            // Decrement Event Counter
            this.Alterations
                .ForEach(x => (x.Effect as TemporaryAlterationEffect).EventTime--);

            // Return effects that have worn off
            return this.Alterations
                       .Remove(x => (x.Effect as TemporaryAlterationEffect).EventTime <= 0)
                       .Select(x => x.RogueName);
        }
        public double GetAttributeAggregate(CharacterAttribute attribute)
        {
            return this.Alterations
                       .Select(x => x.Effect)
                       .Cast<TemporaryAlterationEffect>()
                       .Aggregate(0D, (aggregator, effect) => effect.GetAttribute(attribute));
        }
    }
}
