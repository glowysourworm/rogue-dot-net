﻿using Rogue.NET.Core.Model.Scenario.Content;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration
{
    [Serializable]
    public class AttackAttribute : ScenarioImage
    {
        public double Attack { get; set; }
        public double Resistance { get; set; }

        public bool AppliesToStrengthBasedCombat { get; set; }
        public bool AppliesToIntelligenceBasedCombat { get; set; }

        public bool ScaledByStrength { get; set; }
        public bool ScaledByIntelligence { get; set; }

        public AttackAttribute()
        {
        }
        public override string ToString()
        {
            return this.RogueName + " " + this.Attack.ToString() + "|" + this.Resistance.ToString();
        }
    }
}
