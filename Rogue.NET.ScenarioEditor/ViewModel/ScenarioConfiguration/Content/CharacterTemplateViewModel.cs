using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;

using System.Collections.ObjectModel;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content
{
    public class CharacterTemplateViewModel : DungeonObjectTemplateViewModel
    {
        public ObservableCollection<ProbabilityEquipmentTemplateViewModel> StartingEquipment { get; set; }
        public ObservableCollection<ProbabilityConsumableTemplateViewModel> StartingConsumables { get; set; }

        public ObservableCollection<AttackAttributeTemplateViewModel> AttackAttributes { get; set; }

        private RangeViewModel<double> _strength;
        private RangeViewModel<double> _agility;
        private RangeViewModel<double> _intelligence;
        private RangeViewModel<double> _speed;
        private RangeViewModel<double> _health;
        private RangeViewModel<double> _stamina;
        private RangeViewModel<double> _healthRegen;
        private RangeViewModel<double> _staminaRegen;

        private double _vision;

        public RangeViewModel<double> Strength
        {
            get { return _strength; }
            set { this.RaiseAndSetIfChanged(ref _strength, value); }
        }
        public RangeViewModel<double> Agility
        {
            get { return _agility; }
            set { this.RaiseAndSetIfChanged(ref _agility, value); }
        }
        public RangeViewModel<double> Intelligence
        {
            get { return _intelligence; }
            set { this.RaiseAndSetIfChanged(ref _intelligence, value); }
        }
        public RangeViewModel<double> Speed
        {
            get { return _speed; }
            set { this.RaiseAndSetIfChanged(ref _speed, value); }
        }
        public RangeViewModel<double> Health
        {
            get { return _health; }
            set { this.RaiseAndSetIfChanged(ref _health, value); }
        }
        public RangeViewModel<double> Stamina
        {
            get { return _stamina; }
            set { this.RaiseAndSetIfChanged(ref _stamina, value); }
        }
        public RangeViewModel<double> HealthRegen
        {
            get { return _healthRegen; }
            set { this.RaiseAndSetIfChanged(ref _healthRegen, value); }
        }
        public RangeViewModel<double> StaminaRegen
        {
            get { return _staminaRegen; }
            set { this.RaiseAndSetIfChanged(ref _staminaRegen, value); }
        }

        public double Vision
        {
            get { return _vision; }
            set { this.RaiseAndSetIfChanged(ref _vision, value); }
        }


        public CharacterTemplateViewModel()
        {
            this.Strength = new RangeViewModel<double>(3, 5);
            this.Agility = new RangeViewModel<double>(4, 5);
            this.Intelligence = new RangeViewModel<double>(2, 3);
            this.Speed = new RangeViewModel<double>(0.5, 0.5);
            this.Health = new RangeViewModel<double>(10, 20);
            this.Stamina = new RangeViewModel<double>(2, 5);
            this.HealthRegen = new RangeViewModel<double>(0.1, 0.2);
            this.StaminaRegen = new RangeViewModel<double>(0.1, 0.2);

            this.Vision = 0.8;

            this.StartingConsumables = new ObservableCollection<ProbabilityConsumableTemplateViewModel>();
            this.StartingEquipment = new ObservableCollection<ProbabilityEquipmentTemplateViewModel>();

            this.AttackAttributes = new ObservableCollection<AttackAttributeTemplateViewModel>();
        }
    }
}
