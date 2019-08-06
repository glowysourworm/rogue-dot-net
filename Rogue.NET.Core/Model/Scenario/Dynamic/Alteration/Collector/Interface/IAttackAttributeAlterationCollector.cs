using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector.Interface
{
    public interface IAttackAttributeAlterationCollector
    {
        /// <summary>
        /// Returns an aggregate set of attack attributes for the specified combat type
        /// </summary>
        IEnumerable<AttackAttribute> GetAttackAttributes(AlterationAttackAttributeCombatType combatType);
    }
}
