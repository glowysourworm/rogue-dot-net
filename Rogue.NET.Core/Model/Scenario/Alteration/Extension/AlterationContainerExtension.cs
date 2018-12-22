using Rogue.NET.Core.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Extension
{
    public static class AlterationContainerExtension
    {
        public static bool IsPassive(this AlterationContainer alterationContainer)
        {
            return alterationContainer.Type == AlterationType.PassiveAura ||
                   alterationContainer.Type == AlterationType.PassiveSource ||
                  (alterationContainer.Type == AlterationType.AttackAttribute &&
                   alterationContainer.AttackAttributeType == AlterationAttackAttributeType.Passive);
        }
    }
}
