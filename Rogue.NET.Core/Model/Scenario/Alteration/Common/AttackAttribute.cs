using Rogue.NET.Core.Model.Scenario.Content;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Common
{
    [Serializable]
    public class AttackAttribute : ScenarioImage
    {
        public double Attack { get; set; }
        public double Resistance { get; set; }
        public int Weakness { get; set; }

        public AttackAttribute()
        {
        }
        public override string ToString()
        {
            return this.RogueName + " " + this.Attack.ToString() + "|" + this.Resistance.ToString() + "|" + this.Weakness.ToString();
        }
    }
}
