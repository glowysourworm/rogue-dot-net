﻿using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Common.Extension;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector
{
    [Serializable]
    public class PassiveAlterationCollector : IAlterationCollector, IAlterationEffectCollector
    {
        protected SimpleDictionary<string, AlterationContainer> Alterations { get; set; }

        public PassiveAlterationCollector()
        {
            this.Alterations = new SimpleDictionary<string, AlterationContainer>();
        }
        public bool Apply(AlterationContainer alteration)
        {
            if (!this.Alterations.ContainsKey(alteration.RogueName))
                this.Alterations.Add(alteration.RogueName, alteration);

            else
                return false;

            return true;
        }

        public IEnumerable<AlterationContainer> Filter(string alterationName)
        {
            return this.Alterations.Filter(x => x.Key == alterationName).Values.Actualize();
        }

        public bool CanSeeInvisible()
        {
            return this.Alterations
                       .Values
                       .Select(x => x.Effect)
                       .Cast<PassiveAlterationEffect>()
                       .Any(x => x.CanSeeInvisibleCharacters);
        }

        public IEnumerable<KeyValuePair<string, AlterationCost>> GetCosts()
        {
            return this.Alterations
                       .ToSimpleDictionary(x => x.Key, x => x.Value.Cost);
        }

        public IEnumerable<KeyValuePair<string, IAlterationEffect>> GetEffects(bool includeSourceEffects = false)
        {
            return this.Alterations
                       .ToSimpleDictionary(x => x.Key, x => x.Value.Effect);
        }

        public IEnumerable<SymbolEffectTemplate> GetSymbolChanges()
        {
            return this.Alterations
                       .Values
                       .Select(x => x.Effect)
                       .Cast<PassiveAlterationEffect>()
                       .Where(x => x.SymbolAlteration.HasSymbolChange())
                       .Select(x => x.SymbolAlteration)
                       .Actualize();
        }

        public IEnumerable<AlteredCharacterState> GetAlteredStates()
        {
            return new List<AlteredCharacterState>();
        }

        public double GetAttributeAggregate(CharacterAttribute attribute)
        {
            return this.Alterations
                       .Values
                       .Select(x => x.Effect)
                       .Cast<PassiveAlterationEffect>()
                       .Aggregate(0D, (aggregator, effect) => effect.GetAttribute(attribute));
        }
    }
}
