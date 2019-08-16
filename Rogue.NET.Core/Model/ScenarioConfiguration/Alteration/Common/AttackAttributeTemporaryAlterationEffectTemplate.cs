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
        AlterationAttackAttributeCombatType _combatType;
        AlteredCharacterStateTemplate _alteredState;
        bool _isStackable;
        Range<int> _eventTime;

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
        public Range<int> EventTime
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
            this.EventTime = new Range<int>();
        }
    }
}
