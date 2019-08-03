using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    public class AttackAttributeTemporaryAlterationEffectTemplate 
        : Template, IConsumableAlterationEffectTemplate, 
                    IConsumableProjectileAlterationEffectTemplate,
                    IDoodadAlterationEffectTemplate,
                    IEnemyAlterationEffectTemplate,
                    ISkillAlterationEffectTemplate
    {
        AlterationTargetType _targetType;
        AlterationAttackAttributeCombatType _combatType;

        public AlterationTargetType TargetType
        {
            get { return _targetType; }
            set
            {
                if (_targetType != value)
                {
                    _targetType = value;
                    OnPropertyChanged("TargetType");
                }
            }
        }
        public AlterationAttackAttributeCombatType CombatType
        {
            get { return _combatType; }
            set
            {
                if (_combatType != value)
                {
                    _combatType = value;
                    OnPropertyChanged("CombatType");
                }
            }
        }

        public List<AttackAttributeTemplate> AttackAttributes { get; set; }

        public AttackAttributeTemporaryAlterationEffectTemplate()
        {
            this.AttackAttributes = new List<AttackAttributeTemplate>();
        }
    }
}
