using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector.Interface
{
    /// <summary>
    /// Component that exposes a per-turn maintainence method
    /// </summary>
    public interface ITurnBasedAlterationCollector
    {
        /// <summary>
        /// Applies appropriate maintainence of alterations at the end of each turn
        /// </summary>
        void ApplyEndOfTurn();
    }
}
