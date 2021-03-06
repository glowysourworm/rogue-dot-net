﻿using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    public class AttackAttributeMeleeAlterationEffectTemplate : Template, IConsumableAlterationEffectTemplate, 
                                                                          IConsumableProjectileAlterationEffectTemplate,
                                                                          IDoodadAlterationEffectTemplate,
                                                                          IEnemyAlterationEffectTemplate,
                                                                          IFriendlyAlterationEffectTemplate,
                                                                          ITemporaryCharacterAlterationEffectTemplate,
                                                                          IEquipmentAttackAlterationEffectTemplate,
                                                                          ISkillAlterationEffectTemplate
    {
        public List<AttackAttributeTemplate> AttackAttributes { get; set; }

        public AttackAttributeMeleeAlterationEffectTemplate()
        {
            this.AttackAttributes = new List<AttackAttributeTemplate>();
        }
    }
}
