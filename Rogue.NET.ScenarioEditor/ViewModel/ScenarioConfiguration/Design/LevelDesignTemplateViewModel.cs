using Rogue.NET.Core.Model;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System;
using System.Collections.ObjectModel;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Design
{
    public class LevelDesignTemplateViewModel : TemplateViewModel
    {
        private double _monsterGenerationPerStep;
        private int _equipmentGeneration;
        private int _consumableGeneration;
        private int _enemyGeneration;
        private int _friendlyGeneration;
        private int _doodadGeneration;
        public double MonsterGenerationPerStep
        {
            get { return _monsterGenerationPerStep; }
            set { this.RaiseAndSetIfChanged(ref _monsterGenerationPerStep, value); }
        }
        public int EquipmentGeneration
        {
            get { return _equipmentGeneration; }
            set { this.RaiseAndSetIfChanged(ref _equipmentGeneration, value); }
        }
        public int ConsumableGeneration
        {
            get { return _consumableGeneration; }
            set { this.RaiseAndSetIfChanged(ref _consumableGeneration, value); }
        }
        public int EnemyGeneration
        {
            get { return _enemyGeneration; }
            set { this.RaiseAndSetIfChanged(ref _enemyGeneration, value); }
        }
        public int FriendlyGeneration
        {
            get { return _friendlyGeneration; }
            set { this.RaiseAndSetIfChanged(ref _friendlyGeneration, value); }
        }
        public int DoodadGeneration
        {
            get { return _doodadGeneration; }
            set { this.RaiseAndSetIfChanged(ref _doodadGeneration, value); }
        }
        public ObservableCollection<LayoutGenerationTemplateViewModel> Layouts { get; set; }
        public ObservableCollection<EquipmentGenerationTemplateViewModel> Equipment { get; set; }
        public ObservableCollection<ConsumableGenerationTemplateViewModel> Consumables { get; set; }
        public ObservableCollection<EnemyGenerationTemplateViewModel> Enemies { get; set; }
        public ObservableCollection<FriendlyGenerationTemplateViewModel> Friendlies { get; set; }
        public ObservableCollection<DoodadGenerationTemplateViewModel> Doodads { get; set; }
        public LevelDesignTemplateViewModel()
        {
            this.MonsterGenerationPerStep = ModelConstants.Scenario.MonsterGenerationPerStepDefault;

            this.EquipmentGeneration = ModelConstants.Scenario.EquipmentGenerationDefault;
            this.ConsumableGeneration = ModelConstants.Scenario.ConsumableGenerationDefault;
            this.EnemyGeneration = ModelConstants.Scenario.EnemyGenerationDefault;
            this.FriendlyGeneration = ModelConstants.Scenario.FriendlyGenerationDefault;
            this.DoodadGeneration = ModelConstants.Scenario.DoodadGenerationDefault;

            this.Layouts = new ObservableCollection<LayoutGenerationTemplateViewModel>();
            this.Equipment = new ObservableCollection<EquipmentGenerationTemplateViewModel>();
            this.Consumables = new ObservableCollection<ConsumableGenerationTemplateViewModel>();
            this.Enemies = new ObservableCollection<EnemyGenerationTemplateViewModel>();
            this.Friendlies = new ObservableCollection<FriendlyGenerationTemplateViewModel>();
            this.Doodads = new ObservableCollection<DoodadGenerationTemplateViewModel>();
        }
    }
}
