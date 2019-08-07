﻿using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Skill
{
    [Serializable]
    public class SkillAlteration : AlterationBase
    {
        // TODO:ALTERATION
        public AuraSourceParameters AuraParameters { get; set; }

        // TODO:ALTERATION
        public AlterationTargetType TargetType { get; set; }

        public SkillAlteration()
        {
            this.AuraParameters = new AuraSourceParameters();
        }

        protected override bool ValidateEffectInterfaceType()
        {
            return this.Effect is ISkillAlterationEffect;
        }
    }
}
