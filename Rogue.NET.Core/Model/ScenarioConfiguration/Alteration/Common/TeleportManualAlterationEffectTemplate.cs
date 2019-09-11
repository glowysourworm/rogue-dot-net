using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    public class TeleportManualAlterationEffectTemplate : Template, ISkillAlterationEffectTemplate
    {
        public TeleportManualAlterationEffectTemplate()
        {

        }
    }
}
