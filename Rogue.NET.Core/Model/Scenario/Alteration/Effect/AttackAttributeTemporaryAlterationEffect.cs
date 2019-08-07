using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
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
        public AlterationTargetType TargetType { get; set; }
        public AlterationAttackAttributeCombatType CombatType { get; set; }

        public List<AttackAttribute> AttackAttributes { get; set; }

        // TODO:ALTERATION fill this part of the template in
        public AlteredCharacterState AlteredState { get; set; }

        // TODO:ALTERATION fill this part of the template in
        public bool IsStackable { get; set; }

        // TODO:ALTERATION fill this part of the template in
        public int EventTime { get; set; }

        public AttackAttributeTemporaryAlterationEffect()
        {
            this.AttackAttributes = new List<AttackAttribute>();
        }
    }
}
