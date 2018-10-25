﻿using Rogue.NET.Core.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Scenario.Alteration
{
    [Serializable]
    public class AlterationCost : RogueBase
    {
        public AlterationCostType Type { get; set; }

        public double Strength { get; set; }
        public double Intelligence { get; set; }
        public virtual double Agility { get; set; }
        public virtual double FoodUsagePerTurn { get; set; }
        public virtual double AuraRadius { get; set; }
        public virtual double Experience { get; set; }
        public virtual double Hunger { get; set; }
        public virtual double Hp { get; set; }
        public virtual double Mp { get; set; }

        public AlterationCost() { }
    }
}