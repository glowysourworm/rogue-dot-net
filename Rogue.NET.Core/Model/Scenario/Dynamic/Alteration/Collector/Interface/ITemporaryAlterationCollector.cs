using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector.Interface
{
    /// <summary>
    /// Component that exposes a per-turn maintainence method
    /// </summary>
    public interface ITemporaryAlterationCollector
    {
        /// <summary>
        /// Applies appropriate maintainence of alterations at the end of each turn. Returns effect names
        /// that have worn off.
        /// </summary>
        IEnumerable<string> Decrement();

        /// <summary>
        /// (Any Temporary Effect supports remedies) Applies the remedy alteration effect to the alterations 
        /// in the collector, removes, and returns the names of those effects.
        /// </summary>
        IEnumerable<AlterationContainer> ApplyRemedy(RemedyAlterationEffect remedyEffect);
    }
}
