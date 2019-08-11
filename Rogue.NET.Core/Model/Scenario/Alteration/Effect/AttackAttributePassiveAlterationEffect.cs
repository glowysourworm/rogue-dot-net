using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Effect
{
    [Serializable]
    public class AttackAttributePassiveAlterationEffect
        : RogueBase, IEquipmentCurseAlterationEffect,
                     IEquipmentEquipAlterationEffect,
                     ISkillAlterationEffect
    {
        public AlterationAttackAttributeCombatType CombatType { get; set; }

        public List<AttackAttribute> AttackAttributes { get; set; }

        public AttackAttributePassiveAlterationEffect()
        {
            this.AttackAttributes = new List<AttackAttribute>();
        }
    }
}
