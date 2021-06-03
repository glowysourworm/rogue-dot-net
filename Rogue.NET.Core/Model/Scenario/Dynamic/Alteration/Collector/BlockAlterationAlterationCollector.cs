using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
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
    public class BlockAlterationAlterationCollector : IAlterationCollector, IAlterationEffectCollector
    {
        protected SimpleDictionary<string, AlterationContainer> Alterations { get; set; }

        public BlockAlterationAlterationCollector()
        {
            this.Alterations = new SimpleDictionary<string, AlterationContainer>();
        }

        public bool IsAlterationBlocked(AlterationCategory category)
        {
            return this.Alterations
                       .Values
                       .Select(x => x.Effect as BlockAlterationAlterationEffect)
                       .Any(x => x.AlterationCategory.RogueName == category.RogueName);
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

        public IEnumerable<AlteredCharacterState> GetAlteredStates()
        {
            return new List<AlteredCharacterState>();
        }

        public double GetAttributeAggregate(CharacterAttribute attribute)
        {
            return 0D;
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
                       .Cast<BlockAlterationAlterationEffect>()
                       .Where(x => x.SymbolAlteration.HasSymbolChange())
                       .Select(x => x.SymbolAlteration)
                       .Actualize();
        }
    }
}
