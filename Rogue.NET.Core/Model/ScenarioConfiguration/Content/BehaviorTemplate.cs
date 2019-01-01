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

        public BehaviorTemplate()
        {
            this.EnemySpell = new SpellTemplate();
        }
    }
}
