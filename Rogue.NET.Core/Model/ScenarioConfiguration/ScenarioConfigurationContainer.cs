using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
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
        public PlayerTemplate PlayerTemplate { get; set; }

        public List<SkillSetTemplate> SkillTemplates { get; set; }
        public List<BrushTemplate> BrushTemplates { get; set; }
        public List<PenTemplate> PenTemplates { get; set; }
        public List<EnemyTemplate> EnemyTemplates { get; set; }
        public List<AnimationTemplate> AnimationTemplates { get; set; }
        public List<EquipmentTemplate> EquipmentTemplates { get; set; }
        public List<ConsumableTemplate> ConsumableTemplates { get; set; }
        public List<SpellTemplate> MagicSpells { get; set; }
        public List<DoodadTemplate> DoodadTemplates { get; set; }
        public List<DungeonObjectTemplate> AttackAttributes { get; set; }
        public List<CombatAttributeTemplate> CombatAttributes { get; set; }
        public List<AlteredCharacterStateTemplate> AlteredCharacterStates { get; set; }

        public ScenarioConfigurationContainer()
        {
            this.DungeonTemplate = new DungeonTemplate();
            this.MagicSpells = new List<SpellTemplate>();
            this.EnemyTemplates = new List<EnemyTemplate>();
            this.BrushTemplates = new List<BrushTemplate>();
            this.EquipmentTemplates = new List<EquipmentTemplate>();
            this.AnimationTemplates = new List<AnimationTemplate>();
            this.ConsumableTemplates = new List<ConsumableTemplate>();
            this.SkillTemplates = new List<SkillSetTemplate>();
            this.PlayerTemplate = new PlayerTemplate();
            this.DoodadTemplates = new List<DoodadTemplate>();
            this.PenTemplates = new List<PenTemplate>();
            this.AttackAttributes = new List<DungeonObjectTemplate>();
            this.CombatAttributes = new List<CombatAttributeTemplate>();
            this.AlteredCharacterStates = new List<AlteredCharacterStateTemplate>();
        }
    }
}
