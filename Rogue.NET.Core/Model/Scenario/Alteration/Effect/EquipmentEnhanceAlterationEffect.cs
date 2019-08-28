using Rogue.NET.Core.Model.Attribute;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Effect
{
    [Serializable]
    [AlterationBlockable]
    [AlterationCostSpecifier(AlterationCostType.OneTime,
                             typeof(IConsumableAlterationEffect),
                             typeof(IDoodadAlterationEffect),
                             typeof(ISkillAlterationEffect))]
    public class EquipmentEnhanceAlterationEffect
        : RogueBase, IConsumableAlterationEffect,
                     IDoodadAlterationEffect,
                     ISkillAlterationEffect
    {
        public AlterationModifyEquipmentType Type { get; set; }
        public bool UseDialog { get; set; }
        public int ClassChange { get; set; }
        public double QualityChange { get; set; }
        public List<AttackAttribute> AttackAttributes { get; set; }

        public EquipmentEnhanceAlterationEffect()
        {
            this.AttackAttributes = new List<AttackAttribute>();
        }
    }
}
