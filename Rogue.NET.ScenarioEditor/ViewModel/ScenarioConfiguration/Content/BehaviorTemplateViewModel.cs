using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content
{
    public class BehaviorTemplateViewModel : TemplateViewModel
    {
        private CharacterMovementType _movementType;
        private CharacterAttackType _attackType;
        private SpellTemplateViewModel _enemySpell;

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

        public BehaviorTemplateViewModel()
        {
            this.EnemySpell = new SpellTemplateViewModel();
        }
    }
}
