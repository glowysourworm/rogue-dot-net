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
        private RangeViewModel<double> _hp;
        private RangeViewModel<double> _stamina;
        private RangeViewModel<double> _hpRegen;
        private RangeViewModel<double> _staminaRegen;
        private RangeViewModel<int> _lightRadius;

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
        public RangeViewModel<double> Hp
        {
            get { return _hp; }
            set { this.RaiseAndSetIfChanged(ref _hp, value); }
        }
        public RangeViewModel<double> Stamina
        {
            get { return _stamina; }
            set { this.RaiseAndSetIfChanged(ref _stamina, value); }
        }
        public RangeViewModel<double> HpRegen
        {
            get { return _hpRegen; }
            set { this.RaiseAndSetIfChanged(ref _hpRegen, value); }
        }
        public RangeViewModel<double> StaminaRegen
        {
            get { return _staminaRegen; }
            set { this.RaiseAndSetIfChanged(ref _staminaRegen, value); }
        }
        public RangeViewModel<int> LightRadius
        {
            get { return _lightRadius; }
            set { this.RaiseAndSetIfChanged(ref _lightRadius, value); }
        }

        public CharacterTemplateViewModel()
        {
            this.Strength = new RangeViewModel<double>(3, 5);
            this.Agility = new RangeViewModel<double>(4, 5);
            this.Intelligence = new RangeViewModel<double>(2, 3);
            this.Speed = new RangeViewModel<double>(0.5, 0.5);
            this.Hp = new RangeViewModel<double>(10, 20);
            this.Stamina = new RangeViewModel<double>(2, 5);
            this.HpRegen = new RangeViewModel<double>(0, 0);
            this.StaminaRegen = new RangeViewModel<double>(0, 0);
            this.LightRadius = new RangeViewModel<int>(5, 5);

            this.StartingConsumables = new ObservableCollection<ProbabilityConsumableTemplateViewModel>();
            this.StartingEquipment = new ObservableCollection<ProbabilityEquipmentTemplateViewModel>();

            this.AttackAttributes = new ObservableCollection<AttackAttributeTemplateViewModel>();
        }
    }
}
