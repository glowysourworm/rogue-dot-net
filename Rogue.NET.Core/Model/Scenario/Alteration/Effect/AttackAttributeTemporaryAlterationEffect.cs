using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Effect
{
    [Serializable]
    public class AttackAttributeTemporaryAlterationEffect
        : RogueBase, IConsumableAlterationEffect, 
                     IConsumableProjectileAlterationEffect,
                     IDoodadAlterationEffect,
                     IEnemyAlterationEffect,
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
