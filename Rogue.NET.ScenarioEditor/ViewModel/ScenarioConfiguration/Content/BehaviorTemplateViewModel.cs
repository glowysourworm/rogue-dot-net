using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Enemy;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content
{
    public class BehaviorTemplateViewModel : TemplateViewModel
    {
        private BehaviorCondition _behaviorCondition;
        private BehaviorExitCondition _behaviorExitCondition;

        private int _behaviorTurnCounter;

        private CharacterMovementType _movementType;
        private CharacterAttackType _attackType;
        private SpellTemplateViewModel _enemySpell;
        private EnemyAlterationTemplateViewModel _enemyAlteration;

        public BehaviorCondition BehaviorCondition
        {
            get { return _behaviorCondition; }
            set { this.RaiseAndSetIfChanged(ref _behaviorCondition, value); }
        }
        public BehaviorExitCondition BehaviorExitCondition
        {
            get { return _behaviorExitCondition; }
            set { this.RaiseAndSetIfChanged(ref _behaviorExitCondition, value); }
        }
        public int BehaviorTurnCounter
        {
            get { return _behaviorTurnCounter; }
            set { this.RaiseAndSetIfChanged(ref _behaviorTurnCounter, value); }
        }
        public CharacterMovementType MovementType
        {
            get { return _movementType; }
            set { this.RaiseAndSetIfChanged(ref _movementType, value); }
        }
        public CharacterAttackType AttackType
        {
            get { return _attackType; }
            set { this.RaiseAndSetIfChanged(ref _attackType, value); }
        }
        public SpellTemplateViewModel EnemySpell
        {
            get { return _enemySpell; }
            set { this.RaiseAndSetIfChanged(ref _enemySpell, value); }
        }
        public EnemyAlterationTemplateViewModel EnemyAlteration
        {
            get { return _enemyAlteration; }
            set { this.RaiseAndSetIfChanged(ref _enemyAlteration, value); }
        }

        public BehaviorTemplateViewModel()
        {
            this.EnemySpell = new SpellTemplateViewModel();
            this.EnemyAlteration = new EnemyAlterationTemplateViewModel();
            this.BehaviorCondition = BehaviorCondition.AttackConditionsMet;
            this.BehaviorExitCondition = BehaviorExitCondition.BehaviorCounterExpired;
            this.BehaviorTurnCounter = 1;
        }
    }
}
