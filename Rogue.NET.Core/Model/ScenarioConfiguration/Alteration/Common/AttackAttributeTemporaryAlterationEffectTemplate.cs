using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
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
        AlteredCharacterStateTemplate _alteredState;
        bool _isStackable;
        int _eventTime;

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
        public AlteredCharacterStateTemplate AlteredState

        {
            get { return _alteredState; }
            set
            {
                if (_alteredState != value)
                {
                    _alteredState = value;
                    OnPropertyChanged("AlteredState");
                }
            }
        }
        public bool IsStackable
        {
            get { return _isStackable; }
            set
            {
                if (_isStackable != value)
                {
                    _isStackable = value;
                    OnPropertyChanged("IsStackable");
                }
            }
        }
        public int EventTime
        {
            get { return _eventTime; }
            set
            {
                if (_eventTime != value)
                {
                    _eventTime = value;
                    OnPropertyChanged("EventTime");
                }
            }
        }

        public List<AttackAttributeTemplate> AttackAttributes { get; set; }

        public AttackAttributeTemporaryAlterationEffectTemplate()
        {
            this.AttackAttributes = new List<AttackAttributeTemplate>();
            this.AlteredState = new AlteredCharacterStateTemplate();
        }
    }
}
