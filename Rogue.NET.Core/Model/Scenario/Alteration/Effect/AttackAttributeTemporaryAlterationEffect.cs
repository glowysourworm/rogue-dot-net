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
    [AlterationBlockable(typeof(IEnemyAlterationEffect),
                         typeof(IFriendlyAlterationEffect),
                         typeof(ITemporaryCharacterAlterationEffect),
                         typeof(ISkillAlterationEffect))]
    [AlterationCostSpecifier(AlterationCostType.OneTime,
                             typeof(IConsumableAlterationEffect),
                             typeof(IEnemyAlterationEffect),
                             typeof(IFriendlyAlterationEffect),
                             typeof(ITemporaryCharacterAlterationEffect),
                             typeof(ISkillAlterationEffect))]
    public class AttackAttributeTemporaryAlterationEffect
        : RogueBase, IConsumableAlterationEffect, 
                     IConsumableProjectileAlterationEffect,
                     IDoodadAlterationEffect,
                     IEnemyAlterationEffect,
                     IFriendlyAlterationEffect,
                     ITemporaryCharacterAlterationEffect,
                     ISkillAlterationEffect
    {
        public AlterationAttackAttributeCombatType CombatType { get; set; }
        public AlteredCharacterState AlteredState { get; set; }
        public bool IsStackable { get; set; }
        public int EventTime { get; set; }
        public bool HasAlteredState { get; set; }

        public SymbolDeltaTemplate SymbolAlteration { get; set; }

        public List<AttackAttribute> AttackAttributes { get; set; }

        public AttackAttributeTemporaryAlterationEffect()
        {
            this.AttackAttributes = new List<AttackAttribute>();
            this.SymbolAlteration = new SymbolDeltaTemplate();
        }
    }
}
