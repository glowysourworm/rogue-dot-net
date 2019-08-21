using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Effect
{
    [Serializable]
    public class EquipmentDamageAlterationEffect
        : RogueBase, IConsumableAlterationEffect,
                     IConsumableProjectileAlterationEffect,
                     IDoodadAlterationEffect,
                     IEnemyAlterationEffect,
                     IEquipmentAttackAlterationEffect,
                     ISkillAlterationEffect
    {
        public AlterationModifyEquipmentType Type { get; set; }

        public int ClassChange { get; set; }
        public double QualityChange { get; set; }
        public List<AttackAttribute> AttackAttributes { get; set; }

        public EquipmentDamageAlterationEffect()
        {
            this.AttackAttributes = new List<AttackAttribute>();
        }
    }
}
