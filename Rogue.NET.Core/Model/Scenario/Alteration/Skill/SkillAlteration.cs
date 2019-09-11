using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Skill
{
    [Serializable]
    public class SkillAlteration : Common.AlterationContainer
    {
        public AuraSourceParameters AuraParameters { get; set; }

        public SkillAlteration()
        {
            this.AuraParameters = new AuraSourceParameters();
        }

        public override Type EffectInterfaceType
        {
            get { return typeof(ISkillAlterationEffect); }
        }
    }
}
