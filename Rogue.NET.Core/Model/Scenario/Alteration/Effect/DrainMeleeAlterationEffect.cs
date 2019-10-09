using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Effect
{
    [Serializable]
    public class DrainMeleeAlterationEffect
        : RogueBase, IEquipmentAttackAlterationEffect
    {
        public double Hp { get; set; }
        public double Stamina { get; set; }

        public DrainMeleeAlterationEffect()
        {
        }
    }
}
