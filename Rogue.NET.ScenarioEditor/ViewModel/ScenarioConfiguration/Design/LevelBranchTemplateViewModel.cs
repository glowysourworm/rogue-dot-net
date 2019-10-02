using Rogue.NET.Core.Model;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System.Collections.ObjectModel;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Design
{
    public class LevelBranchTemplateViewModel : TemplateViewModel
    {
        private double _monsterGenerationPerStep;

        private RangeViewModel<int> _equipmentGenerationRange;
        private RangeViewModel<int> _consumableGenerationRange;
        private RangeViewModel<int> _enemyGenerationRange;
        private RangeViewModel<int> _friendlyGenerationRange;
        private RangeViewModel<int> _doodadGenerationRange;

        public double MonsterGenerationPerStep
        {
            get { return _monsterGenerationPerStep; }
            set { this.RaiseAndSetIfChanged(ref _monsterGenerationPerStep, value); }
        }
        public RangeViewModel<int> EquipmentGenerationRange
        {
            get { return _equipmentGenerationRange; }
            set { this.RaiseAndSetIfChanged(ref _equipmentGenerationRange, value); }
        }
        public RangeViewModel<int> EnemyGenerationRange
        {
            get { return _enemyGenerationRange; }
            set { this.RaiseAndSetIfChanged(ref _enemyGenerationRange, value); }
        }
        public RangeViewModel<int> ConsumableGenerationRange
        {
            get { return _consumableGenerationRange; }
            set { this.RaiseAndSetIfChanged(ref _consumableGenerationRange, value); }
        }
        public RangeViewModel<int> FriendlyGenerationRange
        {
            get { return _friendlyGenerationRange; }
            set { this.RaiseAndSetIfChanged(ref _friendlyGenerationRange, value); }
        }
        public RangeViewModel<int> DoodadGenerationRange
        {
            get { return _doodadGenerationRange; }
            set { this.RaiseAndSetIfChanged(ref _doodadGenerationRange, value); }
        }
        public ObservableCollection<LayoutGenerationTemplateViewModel> Layouts { get; set; }
        public ObservableCollection<EquipmentGenerationTemplateViewModel> Equipment { get; set; }
        public ObservableCollection<ConsumableGenerationTemplateViewModel> Consumables { get; set; }
        public ObservableCollection<EnemyGenerationTemplateViewModel> Enemies { get; set; }
        public ObservableCollection<FriendlyGenerationTemplateViewModel> Friendlies { get; set; }
        public ObservableCollection<DoodadGenerationTemplateViewModel> Doodads { get; set; }
        public LevelBranchTemplateViewModel()
        {
            this.MonsterGenerationPerStep = ModelConstants.Scenario.MonsterGenerationPerStepDefault;

            this.EquipmentGenerationRange = new RangeViewModel<int>(0, ModelConstants.Scenario.EquipmentGenerationDefault);
            this.ConsumableGenerationRange = new RangeViewModel<int>(0, ModelConstants.Scenario.ConsumableGenerationDefault);
            this.EnemyGenerationRange = new RangeViewModel<int>(0, ModelConstants.Scenario.EnemyGenerationDefault);
            this.FriendlyGenerationRange = new RangeViewModel<int>(0, ModelConstants.Scenario.FriendlyGenerationDefault);
            this.DoodadGenerationRange = new RangeViewModel<int>(0, ModelConstants.Scenario.DoodadGenerationDefault);

            this.Layouts = new ObservableCollection<LayoutGenerationTemplateViewModel>();
            this.Equipment = new ObservableCollection<EquipmentGenerationTemplateViewModel>();
            this.Consumables = new ObservableCollection<ConsumableGenerationTemplateViewModel>();
            this.Enemies = new ObservableCollection<EnemyGenerationTemplateViewModel>();
            this.Friendlies = new ObservableCollection<FriendlyGenerationTemplateViewModel>();
            this.Doodads = new ObservableCollection<DoodadGenerationTemplateViewModel>();
        }
    }
}
