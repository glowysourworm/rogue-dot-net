using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Design
{
    [Serializable]
    public class LevelDesignTemplate : Template
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
        public List<LayoutGenerationTemplate> Layouts { get; set; }
        public List<EquipmentGenerationTemplate> Equipment { get; set; }
        public List<ConsumableGenerationTemplate> Consumables { get; set; }
        public List<EnemyGenerationTemplate> Enemies { get; set; }
        public List<FriendlyGenerationTemplate> Friendlies { get; set; }
        public List<DoodadGenerationTemplate> Doodads { get; set; }
        public LevelDesignTemplate()
        {
            this.MonsterGenerationPerStep = ModelConstants.Scenario.MonsterGenerationPerStepDefault;

            this.EquipmentGeneration = ModelConstants.Scenario.EquipmentGenerationDefault;
            this.ConsumableGeneration = ModelConstants.Scenario.ConsumableGenerationDefault;
            this.EnemyGeneration = ModelConstants.Scenario.EnemyGenerationDefault;
            this.FriendlyGeneration = ModelConstants.Scenario.FriendlyGenerationDefault;
            this.DoodadGeneration = ModelConstants.Scenario.DoodadGenerationDefault;

            this.Layouts = new List<LayoutGenerationTemplate>();
            this.Equipment = new List<EquipmentGenerationTemplate>();
            this.Consumables = new List<ConsumableGenerationTemplate>();
            this.Enemies = new List<EnemyGenerationTemplate>();
            this.Friendlies = new List<FriendlyGenerationTemplate>();
            this.Doodads = new List<DoodadGenerationTemplate>();
        }
    }
}
