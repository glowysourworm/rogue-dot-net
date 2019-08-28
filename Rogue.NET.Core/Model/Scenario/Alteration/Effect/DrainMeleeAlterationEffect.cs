using Rogue.NET.Core.Model.Attribute;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Effect
{
    [Serializable]
    [AlterationBlockable(typeof(IEquipmentAttackAlterationEffect))]
    [AlterationCostSpecifier(AlterationCostType.OneTime, 
                             typeof(IEquipmentAttackAlterationEffect))]
    public class DrainMeleeAlterationEffect
        : RogueBase, IEquipmentAttackAlterationEffect
    {
        public double Hp { get; set; }
        public double Mp { get; set; }

        public DrainMeleeAlterationEffect()
        {
        }
    }
}
