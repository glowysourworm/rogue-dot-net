using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Equipment;
using System.Collections.ObjectModel;


namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content
{
    public class EquipmentTemplateViewModel : DungeonObjectTemplateViewModel
    {
        public ObservableCollection<AttackAttributeTemplateViewModel> AttackAttributes { get; set; }

        private RangeViewModel<int> _class;
        private RangeViewModel<double> _quality;
        private EquipmentType _type;
        private CharacterBaseAttribute _combatType;
        private EquipmentAttackAlterationTemplateViewModel _equipmentAttackAlteration;
        private EquipmentEquipAlterationTemplateViewModel _equipmentEquipAlteration;
        private EquipmentCurseAlterationTemplateViewModel _equipmentCurseAlteration;
        private ConsumableTemplateViewModel _ammoTemplate;
        private double _weight;
        private int _levelRequired;
        private bool _hasAttackAlteration;
        private bool _hasEquipAlteration;
        private bool _hasCurseAlteration;
        private bool _hasCharacterClassRequirement;
        private string _characterClass;

        public RangeViewModel<int> Class
        {
            get { return _class; }
            set { this.RaiseAndSetIfChanged(ref _class, value); }
        }
        public double Weight
        {
            get { return _weight; }
            set { this.RaiseAndSetIfChanged(ref _weight, value); }
        }
        public int LevelRequired
        {
            get { return _levelRequired; }
            set { this.RaiseAndSetIfChanged(ref _levelRequired, value); }
        }
        public RangeViewModel<double> Quality
        {
            get { return _quality; }
            set { this.RaiseAndSetIfChanged(ref _quality, value); }
        }
        public EquipmentType Type
        {
            get { return _type; }
            set { this.RaiseAndSetIfChanged(ref _type, value); }
        }
        public CharacterBaseAttribute CombatType
        {
            get { return _combatType; }
            set { this.RaiseAndSetIfChanged(ref _combatType, value); }
        }
        public EquipmentAttackAlterationTemplateViewModel EquipmentAttackAlteration
        {
            get { return _equipmentAttackAlteration; }
            set { this.RaiseAndSetIfChanged(ref _equipmentAttackAlteration, value); }
        }
        public EquipmentEquipAlterationTemplateViewModel EquipmentEquipAlteration
        {
            get { return _equipmentEquipAlteration; }
            set { this.RaiseAndSetIfChanged(ref _equipmentEquipAlteration, value); }
        }
        public EquipmentCurseAlterationTemplateViewModel EquipmentCurseAlteration
        {
            get { return _equipmentCurseAlteration; }
            set { this.RaiseAndSetIfChanged(ref _equipmentCurseAlteration, value); }
        }
        public ConsumableTemplateViewModel AmmoTemplate
        {
            get { return _ammoTemplate; }
            set { this.RaiseAndSetIfChanged(ref _ammoTemplate, value); }
        }
        public bool HasAttackAlteration
        {
            get { return _hasAttackAlteration; }
            set { this.RaiseAndSetIfChanged(ref _hasAttackAlteration, value); }
        }
        public bool HasEquipAlteration
        {
            get { return _hasEquipAlteration; }
            set { this.RaiseAndSetIfChanged(ref _hasEquipAlteration, value); }
        }
        public bool HasCurseAlteration
        {
            get { return _hasCurseAlteration; }
            set { this.RaiseAndSetIfChanged(ref _hasCurseAlteration, value); }
        }
        public bool HasCharacterClassRequirement
        {
            get { return _hasCharacterClassRequirement; }
            set { this.RaiseAndSetIfChanged(ref _hasCharacterClassRequirement, value); }
        }
        public string CharacterClass
        {
            get { return _characterClass; }
            set { this.RaiseAndSetIfChanged(ref _characterClass, value); }
        }

        public EquipmentTemplateViewModel()
        {
            this.Class = new RangeViewModel<int>(0, 10);
            this.Type = EquipmentType.Ring;
            this.Quality = new RangeViewModel<double>(0, 0, 100, 100);
            this.AmmoTemplate = new ConsumableTemplateViewModel();
            this.AttackAttributes = new ObservableCollection<AttackAttributeTemplateViewModel>();
            this.HasCharacterClassRequirement = false;
            this.EquipmentAttackAlteration = new EquipmentAttackAlterationTemplateViewModel();
            this.EquipmentCurseAlteration = new EquipmentCurseAlterationTemplateViewModel();
            this.EquipmentEquipAlteration = new EquipmentEquipAlterationTemplateViewModel();
        }
    }
}
