using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Common
{
    [Serializable]
    public class AttackAttributeAuraAlterationEffect
        : RogueBase, IEquipmentCurseAlterationEffect,
                     IEquipmentEquipAlterationEffect,
                     ISkillAlterationEffect
    {
        public AlterationTargetType TargetType { get; set; }
        public AlterationAttackAttributeCombatType CombatType { get; set; }

        public List<AttackAttribute> AttackAttributes { get; set; }

        // TODO:ALTERATION Add this to the template data
        public SymbolDeltaTemplate SymbolAlteration { get; set; }

        public AttackAttributeAuraAlterationEffect()
        {
            this.AttackAttributes = new List<AttackAttribute>();
            this.SymbolAlteration = new SymbolDeltaTemplate();
        }
    }
}
