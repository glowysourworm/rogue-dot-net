using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector.Interface
{
    /// <summary>
    /// Component that provides common methods for alteration collectors
    /// </summary>
    public interface IAlterationCollector
    {
        /// <summary>
        /// Returns all per-step AlterationCosts (NOTE*** This is the only cost type that
        /// should be collected)
        /// </summary>
        IEnumerable<AlterationCost> GetCosts();

        /// <summary>
        /// Returns all altered states for the collector
        /// </summary>
        IEnumerable<AlteredCharacterState> GetAlteredStates();

        /// <summary>
        /// Gets symbol changes for all alterations that have a symbol change
        /// </summary>
        IEnumerable<SymbolDeltaTemplate> GetSymbolChanges();

        /// <summary>
        /// Gets attribute alteration sum for all alterations
        /// </summary>
        double GetAttributeAggregate(CharacterAttribute attribute);
    }
}
