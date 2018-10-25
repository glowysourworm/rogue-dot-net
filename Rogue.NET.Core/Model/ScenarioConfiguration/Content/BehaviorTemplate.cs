using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    public class BehaviorTemplate : Template
    {
        private CharacterMovementType _movementType;
        private CharacterAttackType _attackType;
        private SpellTemplate _enemySpell;
        private bool _canOpenDoors;
        private double _engageRadius;
        private double _disengageRadius;
        private double _criticalRatio;
        private double _counterAttackProbability;

        public CharacterMovementType MovementType
        {
            get { return _movementType; }
            set
            {
                if (_movementType != value)
                {
                    _movementType = value;
                    OnPropertyChanged("MovementType");
                }
            }
        }
        public CharacterAttackType AttackType
        {
            get { return _attackType; }
            set
            {
                if (_attackType != value)
                {
                    _attackType = value;
                    OnPropertyChanged("AttackType");
                }
            }
        }
        public SpellTemplate EnemySpell
        {
            get { return _enemySpell; }
            set
            {
                if (_enemySpell != value)
                {
                    _enemySpell = value;
                    OnPropertyChanged("EnemySpell");
                }
            }
        }
        public bool CanOpenDoors
        {
            get { return _canOpenDoors; }
            set
            {
                if (_canOpenDoors != value)
                {
                    _canOpenDoors = value;
                    OnPropertyChanged("CanOpenDoors");
                }
            }
        }
        public double EngageRadius
        {
            get { return _engageRadius; }
            set
            {
                if (_engageRadius != value)
                {
                    _engageRadius = value;
                    OnPropertyChanged("EngageRadius");
                }
            }
        }
        public double DisengageRadius
        {
            get { return _disengageRadius; }
            set
            {
                if (_disengageRadius != value)
                {
                    _disengageRadius = value;
                    OnPropertyChanged("DisengageRadius");
                }
            }
        }
        public double CriticalRatio
        {
            get { return _criticalRatio; }
            set
            {
                if (_criticalRatio != value)
                {
                    _criticalRatio = value;
                    OnPropertyChanged("CriticalRatio");
                }
            }
        }
        public double CounterAttackProbability
        {
            get { return _counterAttackProbability; }
            set
            {
                if (_counterAttackProbability != value)
                {
                    _counterAttackProbability = value;
                    OnPropertyChanged("CounterAttackProbability");
                }
            }
        }

        public BehaviorTemplate()
        {
            this.EnemySpell = new SpellTemplate();
        }
    }
}
