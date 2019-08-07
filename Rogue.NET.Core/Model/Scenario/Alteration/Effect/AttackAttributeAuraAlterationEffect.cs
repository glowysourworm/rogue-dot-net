using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Effect
{
    [Serializable]
    public class AttackAttributeAuraAlterationEffect
        : RogueBase, IEquipmentCurseAlterationEffect,
                     IEquipmentEquipAlterationEffect,
                     ISkillAlterationEffect
    {
        public AlterationAttackAttributeCombatType CombatType { get; set; }

        public List<AttackAttribute> AttackAttributes { get; set; }

        public SymbolDeltaTemplate SymbolAlteration { get; set; }

        public AttackAttributeAuraAlterationEffect()
        {
            this.AttackAttributes = new List<AttackAttribute>();
            this.SymbolAlteration = new SymbolDeltaTemplate();
        }      
    }
}
