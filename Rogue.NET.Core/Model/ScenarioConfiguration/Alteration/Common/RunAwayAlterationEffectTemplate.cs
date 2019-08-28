using Rogue.NET.Core.Model.Attribute;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    [AlterationBlockable]
    [AlterationCostSpecifier(AlterationCostType.None)]
    public class RunAwayAlterationEffectTemplate : Template, IEnemyAlterationEffectTemplate
    {
        public RunAwayAlterationEffectTemplate() { }
    }
}
