using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Consumable;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Doodad;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Enemy;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Equipment;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Skill;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration
{
    [Serializable]
    public class ScenarioConfigurationAlterationContainer
    {
        public List<ConsumableAlterationTemplate> ConsumableAlterations { get; set; }
        public List<ConsumableProjectileAlterationTemplate> ConsumableProjectileAlterations { get; set; }
        public List<DoodadAlterationTemplate> DoodadAlterations { get; set; }
        public List<EnemyAlterationTemplate> EnemyAlterations { get; set; }
        public List<EquipmentAttackAlterationTemplate> EquipmentAttackAlterations { get; set; }
        public List<EquipmentCurseAlterationTemplate> EquipmentCurseAlterations { get; set; }
        public List<EquipmentEquipAlterationTemplate> EquipmentEquipAlterations { get; set; }
        public List<SkillAlterationTemplate> SkillAlterations { get; set; }

        public ScenarioConfigurationAlterationContainer()
        {
            this.ConsumableAlterations = new List<ConsumableAlterationTemplate>();
            this.ConsumableProjectileAlterations = new List<ConsumableProjectileAlterationTemplate>();
            this.DoodadAlterations = new List<DoodadAlterationTemplate>();
            this.EnemyAlterations = new List<EnemyAlterationTemplate>();
            this.EquipmentAttackAlterations = new List<EquipmentAttackAlterationTemplate>();
            this.EquipmentCurseAlterations = new List<EquipmentCurseAlterationTemplate>();
            this.EquipmentEquipAlterations = new List<EquipmentEquipAlterationTemplate>();
            this.SkillAlterations = new List<SkillAlterationTemplate>();
        }
    }
}
