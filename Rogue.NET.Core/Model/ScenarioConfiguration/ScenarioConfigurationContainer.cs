using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration
{
    [Serializable]
    public class ScenarioConfigurationContainer
    {
        public DungeonTemplate DungeonTemplate { get; set; }

        public List<PlayerTemplate> PlayerTemplates { get; set; }
        public List<SkillSetTemplate> SkillTemplates { get; set; }
        public List<EnemyTemplate> EnemyTemplates { get; set; }
        public List<EquipmentTemplate> EquipmentTemplates { get; set; }
        public List<ConsumableTemplate> ConsumableTemplates { get; set; }
        public List<DoodadTemplate> DoodadTemplates { get; set; }
        public List<AttackAttributeTemplate> AttackAttributes { get; set; }
        public List<AlteredCharacterStateTemplate> AlteredCharacterStates { get; set; }

        public ScenarioConfigurationContainer()
        {
            this.DungeonTemplate = new DungeonTemplate();
            this.PlayerTemplates = new List<PlayerTemplate>();
            this.EnemyTemplates = new List<EnemyTemplate>();
            this.EquipmentTemplates = new List<EquipmentTemplate>();
            this.ConsumableTemplates = new List<ConsumableTemplate>();
            this.SkillTemplates = new List<SkillSetTemplate>();
            this.DoodadTemplates = new List<DoodadTemplate>();
            this.AttackAttributes = new List<AttackAttributeTemplate>();
            this.AlteredCharacterStates = new List<AlteredCharacterStateTemplate>();
        }
    }
}
