using Rogue.NET.Core.Model.Enums;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Common
{
    [Serializable]
    public class AlterationCost : RogueBase
    {
        public double Experience { get; set; }
        public double Hunger { get; set; }
        public double Hp { get; set; }
        public double Stamina { get; set; }

        public AlterationCost() { }
    }
}
