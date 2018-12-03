using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;

using System.Collections.ObjectModel;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content
{
    public class CharacterTemplateViewModel : DungeonObjectTemplateViewModel
    {
        public ObservableCollection<ProbabilityEquipmentTemplateViewModel> StartingEquipment { get; set; }
        public ObservableCollection<ProbabilityConsumableTemplateViewModel> StartingConsumables { get; set; }

        private RangeViewModel<double> _strength;
        private RangeViewModel<double> _agility;
        private RangeViewModel<double> _intelligence;
        private RangeViewModel<double> _speed;
        private RangeViewModel<double> _hp;
        private RangeViewModel<double> _mp;

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
        public RangeViewModel<double> Mp
        {
            get { return _mp; }
            set { this.RaiseAndSetIfChanged(ref _mp, value); }
        }

        public CharacterTemplateViewModel()
        {
            this.Strength = new RangeViewModel<double>(1, 3, 5, 100);
            this.Agility = new RangeViewModel<double>(1, 4, 5, 100);
            this.Intelligence = new RangeViewModel<double>(1, 2, 3, 100);
            this.Speed = new RangeViewModel<double>(0.1, 0.5, 0.5, 1);
            this.Hp = new RangeViewModel<double>(1, 10, 20, 10000);
            this.Mp = new RangeViewModel<double>(1, 2, 5, 10000);

            this.StartingConsumables = new ObservableCollection<ProbabilityConsumableTemplateViewModel>();
            this.StartingEquipment = new ObservableCollection<ProbabilityEquipmentTemplateViewModel>();
        }
        public CharacterTemplateViewModel(DungeonObjectTemplateViewModel tmp) : base(tmp)
        {
            this.Strength = new RangeViewModel<double>(1, 3, 5, 100);
            this.Agility = new RangeViewModel<double>(1, 4, 5, 100);
            this.Intelligence = new RangeViewModel<double>(1, 2, 3, 100);
            this.Speed = new RangeViewModel<double>(0.1, 0.5, 0.5, 1);
            this.Hp = new RangeViewModel<double>(1, 10, 20, 10000);
            this.Mp = new RangeViewModel<double>(1, 2, 5, 10000);

            this.StartingConsumables = new ObservableCollection<ProbabilityConsumableTemplateViewModel>();
            this.StartingEquipment = new ObservableCollection<ProbabilityEquipmentTemplateViewModel>();
        }
    }
}
