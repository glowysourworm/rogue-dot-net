using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Design
{
    [Serializable]
    public class LevelBranchTemplate : Template
    {
        private double _monsterGenerationPerStep;

        private Range<int> _equipmentGenerationRange;
        private Range<int> _consumableGenerationRange;
        private Range<int> _enemyGenerationRange;
        private Range<int> _friendlyGenerationRange;
        private Range<int> _doodadGenerationRange;

        public double MonsterGenerationPerStep
        {
            get { return _monsterGenerationPerStep; }
            set { this.RaiseAndSetIfChanged(ref _monsterGenerationPerStep, value); }
        }
        public Range<int> EquipmentGenerationRange
        {
            get { return _equipmentGenerationRange; }
            set { this.RaiseAndSetIfChanged(ref _equipmentGenerationRange, value); }
        }
        public Range<int> EnemyGenerationRange
        {
            get { return _enemyGenerationRange; }
            set { this.RaiseAndSetIfChanged(ref _enemyGenerationRange, value); }
        }
        public Range<int> ConsumableGenerationRange
        {
            get { return _consumableGenerationRange; }
            set { this.RaiseAndSetIfChanged(ref _consumableGenerationRange, value); }
        }
        public Range<int> FriendlyGenerationRange
        {
            get { return _friendlyGenerationRange; }
            set { this.RaiseAndSetIfChanged(ref _friendlyGenerationRange, value); }
        }
        public Range<int> DoodadGenerationRange
        {
            get { return _doodadGenerationRange; }
            set { this.RaiseAndSetIfChanged(ref _doodadGenerationRange, value); }
        }
        public List<LayoutGenerationTemplate> Layouts { get; set; }
        public List<EquipmentGenerationTemplate> Equipment { get; set; }
        public List<ConsumableGenerationTemplate> Consumables { get; set; }
        public List<EnemyGenerationTemplate> Enemies { get; set; }
        public List<FriendlyGenerationTemplate> Friendlies { get; set; }
        public List<DoodadGenerationTemplate> Doodads { get; set; }
        public LevelBranchTemplate()
        {
            this.MonsterGenerationPerStep = ModelConstants.Scenario.MonsterGenerationPerStepDefault;

            this.EquipmentGenerationRange = new Range<int>(0, ModelConstants.Scenario.EquipmentGenerationDefault);
            this.ConsumableGenerationRange = new Range<int>(0, ModelConstants.Scenario.ConsumableGenerationDefault);
            this.EnemyGenerationRange = new Range<int>(0, ModelConstants.Scenario.EnemyGenerationDefault);
            this.FriendlyGenerationRange = new Range<int>(0, ModelConstants.Scenario.FriendlyGenerationDefault);
            this.DoodadGenerationRange = new Range<int>(0, ModelConstants.Scenario.DoodadGenerationDefault);

            this.Layouts = new List<LayoutGenerationTemplate>();
            this.Equipment = new List<EquipmentGenerationTemplate>();
            this.Consumables = new List<ConsumableGenerationTemplate>();
            this.Enemies = new List<EnemyGenerationTemplate>();
            this.Friendlies = new List<FriendlyGenerationTemplate>();
            this.Doodads = new List<DoodadGenerationTemplate>();
        }
    }
}
