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
        private bool _canOpenDoors;
        private double _engageRadius;
        private double _disengageRadius;
        private double _criticalRatio;
        private double _counterAttackProbability;

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
        public bool CanOpenDoors
        {
            get { return _canOpenDoors; }
            set { this.RaiseAndSetIfChanged(ref _canOpenDoors, value); }
        }
        public double EngageRadius
        {
            get { return _engageRadius; }
            set { this.RaiseAndSetIfChanged(ref _engageRadius, value); }
        }
        public double DisengageRadius
        {
            get { return _disengageRadius; }
            set { this.RaiseAndSetIfChanged(ref _disengageRadius, value); }
        }
        public double CriticalRatio
        {
            get { return _criticalRatio; }
            set { this.RaiseAndSetIfChanged(ref _criticalRatio, value); }
        }
        public double CounterAttackProbability
        {
            get { return _counterAttackProbability; }
            set { this.RaiseAndSetIfChanged(ref _counterAttackProbability, value); }
        }

        public BehaviorTemplateViewModel()
        {
            this.EnemySpell = new SpellTemplateViewModel();
        }
    }
}
