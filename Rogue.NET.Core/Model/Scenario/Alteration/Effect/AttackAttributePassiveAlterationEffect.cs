using Rogue.NET.Core.Model.Attribute;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Effect
{
    [Serializable]
    [AlterationBlockable]
    [AlterationCostSpecifier(AlterationCostType.PerStep,
                             typeof(IEquipmentEquipAlterationEffect),
                             typeof(ISkillAlterationEffect))]
    public class AttackAttributePassiveAlterationEffect
        : RogueBase, IEquipmentCurseAlterationEffect,
                     IEquipmentEquipAlterationEffect,
                     ISkillAlterationEffect
    {
        public AlterationAttackAttributeCombatType CombatType { get; set; }

        public List<AttackAttribute> AttackAttributes { get; set; }

        public SymbolDeltaTemplate SymbolAlteration { get; set; }

        public AttackAttributePassiveAlterationEffect()
        {
            this.AttackAttributes = new List<AttackAttribute>();
            this.SymbolAlteration = new SymbolDeltaTemplate();
        }
    }
}
