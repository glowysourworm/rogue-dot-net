using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Enemy;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    public class BehaviorTemplate : Template
    {
        private BehaviorCondition _behaviorCondition;
        private BehaviorExitCondition _behaviorExitCondition;

        private int _behaviorTurnCounter;

        private CharacterMovementType _movementType;
        private CharacterAttackType _attackType;
        private EnemyAlterationTemplate _enemyAlteration;

        /// <summary>
        /// Treated as flags (many)
        /// </summary>
        public BehaviorCondition BehaviorCondition
        {
            get { return _behaviorCondition; }
            set
            {
                if (_behaviorCondition != value)
                {
                    _behaviorCondition = value;
                    OnPropertyChanged("BehaviorCondition");
                }
            }
        }

        /// <summary>
        /// Treated as flags (many)
        /// </summary>
        public BehaviorExitCondition BehaviorExitCondition
        {
            get { return _behaviorExitCondition; }
            set
            {
                if (_behaviorExitCondition != value)
                {
                    _behaviorExitCondition = value;
                    OnPropertyChanged("BehaviorExitCondition");
                }
            }
        }

        public int BehaviorTurnCounter
        {
            get { return _behaviorTurnCounter; }
            set
            {
                if (_behaviorTurnCounter != value)
                {
                    _behaviorTurnCounter = value;
                    OnPropertyChanged("BehaviorTurnCounter");
                }
            }
        }

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
        public EnemyAlterationTemplate EnemyAlteration
        {
            get { return _enemyAlteration; }
            set
            {
                if (_enemyAlteration != value)
                {
                    _enemyAlteration = value;
                    OnPropertyChanged("EnemySpell");
                }
            }
        }

        public BehaviorTemplate()
        {
            this.EnemyAlteration = new EnemyAlterationTemplate();
            this.BehaviorCondition = BehaviorCondition.AttackConditionsMet;
            this.BehaviorExitCondition = BehaviorExitCondition.BehaviorCounterExpired;
        }
    }
}
