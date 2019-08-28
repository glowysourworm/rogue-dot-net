using Rogue.NET.Core.Model.Attribute;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    [AlterationBlockable(typeof(IEnemyAlterationEffectTemplate),
                         typeof(IEquipmentAttackAlterationEffectTemplate),
                         typeof(ISkillAlterationEffectTemplate))]
    [AlterationCostSpecifier(AlterationCostType.OneTime,
                             typeof(IConsumableAlterationEffectTemplate),
                             typeof(IEnemyAlterationEffectTemplate),
                             typeof(IEquipmentAttackAlterationEffectTemplate),
                             typeof(ISkillAlterationEffectTemplate))]
    public class AttackAttributeMeleeAlterationEffectTemplate 
        : Template, IConsumableAlterationEffectTemplate, 
                    IConsumableProjectileAlterationEffectTemplate,
                    IDoodadAlterationEffectTemplate,
                    IEnemyAlterationEffectTemplate,
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
