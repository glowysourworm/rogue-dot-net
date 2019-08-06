using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Common
{
    [Serializable]
    public class EquipmentModifyAlterationEffect
        : RogueBase, IConsumableAlterationEffect,
                     IDoodadAlterationEffect,
                     IEnemyAlterationEffect,
                     ISkillAlterationEffect
    {
        public AlterationModifyEquipmentType Type { get; set; }
        public int ClassChange { get; set; }
        public double QualityChange { get; set; }

        public List<AttackAttribute> AttackAttributes { get; set; }

        public EquipmentModifyAlterationEffect()
        {
            this.AttackAttributes = new List<AttackAttribute>();
        }
    }
}
