﻿using Rogue.NET.Core.Model.Attribute;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Effect
{
    /// <summary>
    /// This attack attribute type is used for a one-time melee hit
    /// </summary>
    [Serializable]
    [AlterationBlockable(typeof(IEnemyAlterationEffect),                                  
                         typeof(IFriendlyAlterationEffect),
                         typeof(ITemporaryCharacterAlterationEffect),
                         typeof(IEquipmentAttackAlterationEffect),
                         typeof(ISkillAlterationEffect))]
    [AlterationCostSpecifier(AlterationCostType.OneTime,         
                             typeof(IConsumableAlterationEffect),
                             typeof(IEnemyAlterationEffect),
                             typeof(IFriendlyAlterationEffect),
                             typeof(ITemporaryCharacterAlterationEffect),
                             typeof(IEquipmentAttackAlterationEffect),
                             typeof(ISkillAlterationEffect))]
    public class AttackAttributeMeleeAlterationEffect
        : RogueBase, IConsumableAlterationEffect, 
                     IConsumableProjectileAlterationEffect,
                     IDoodadAlterationEffect,
                     IEnemyAlterationEffect,
                     IFriendlyAlterationEffect,
                     ITemporaryCharacterAlterationEffect,
                     IEquipmentAttackAlterationEffect,
                     ISkillAlterationEffect
    {
        public List<AttackAttribute> AttackAttributes { get; set; }

        public AttackAttributeMeleeAlterationEffect()
        {
            this.AttackAttributes = new List<AttackAttribute>();
        }
    }
}
