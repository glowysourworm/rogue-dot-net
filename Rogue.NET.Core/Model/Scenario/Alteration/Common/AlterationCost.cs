﻿using Rogue.NET.Core.Model.Enums;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Common
{
    [Serializable]
    public class AlterationCost : RogueBase
    {
        public AlterationCostType Type { get; set; }

        public double Strength { get; set; }
        public double Intelligence { get; set; }
        public virtual double Agility { get; set; }
        public virtual double Speed { get; set; }
        public virtual double FoodUsagePerTurn { get; set; }
        public virtual double LightRadius { get; set; }
        public virtual double Experience { get; set; }
        public virtual double Hunger { get; set; }
        public virtual double Hp { get; set; }
        public virtual double Mp { get; set; }

        public AlterationCost() { }
    }
}