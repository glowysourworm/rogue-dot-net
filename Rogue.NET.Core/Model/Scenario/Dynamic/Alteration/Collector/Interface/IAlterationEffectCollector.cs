using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector.Interface
{
    /// <summary>
    /// Collector that contains IAlterationEffect types for query.
    /// </summary>
    public interface IAlterationEffectCollector
    {
        /// <summary>
        /// Returns list of effects (by Alteration name) associated with the collector 
        /// THAT AFFECT THE CHARACTER ONLY. (This exclues aura source effects because 
        /// they're only applied to the target). To INCLUDE aura source effects just
        /// pass in the includeSourceEffects flag as "true".
        /// </summary>
        IEnumerable<KeyValuePair<string, IAlterationEffect>> GetEffects(bool includeSourceEffects = false);

        /// <summary>
        /// Returns all altered states for the collector
        /// </summary>
        IEnumerable<AlteredCharacterState> GetAlteredStates();

        /// <summary>
        /// Gets symbol changes for all alterations that have a symbol change
        /// </summary>
        IEnumerable<SymbolEffectTemplate> GetSymbolChanges();

        /// <summary>
        /// Gets attribute alteration sum for all alterations
        /// </summary>
        double GetAttributeAggregate(CharacterAttribute attribute);
    }
}
