using ProtoBuf;
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
    [ProtoContract(AsReferenceDefault = true)]
    public class ScenarioConfigurationContainer
    {
        [ProtoMember(1)]
        public DungeonTemplate DungeonTemplate { get; set; }
        [ProtoMember(2)]
        public PlayerTemplate PlayerTemplate { get; set; }

        [ProtoMember(3, AsReference = true)]
        public List<SkillSetTemplate> SkillTemplates { get; set; }
        [ProtoMember(4, AsReference = true)]
        public List<BrushTemplate> BrushTemplates { get; set; }
        [ProtoMember(5, AsReference = true)]
        public List<PenTemplate> PenTemplates { get; set; }
        [ProtoMember(6, AsReference = true)]
        public List<EnemyTemplate> EnemyTemplates { get; set; }
        [ProtoMember(7, AsReference = true)]
        public List<AnimationTemplate> AnimationTemplates { get; set; }
        [ProtoMember(8, AsReference = true)]
        public List<EquipmentTemplate> EquipmentTemplates { get; set; }
        [ProtoMember(9, AsReference = true)]
        public List<ConsumableTemplate> ConsumableTemplates { get; set; }
        [ProtoMember(10, AsReference = true)]
        public List<SpellTemplate> MagicSpells { get; set; }
        [ProtoMember(11, AsReference = true)]
        public List<DoodadTemplate> DoodadTemplates { get; set; }
        [ProtoMember(12, AsReference = true)]
        public List<DungeonObjectTemplate> AttackAttributes { get; set; }
        [ProtoMember(13, AsReference = true)]
        public List<CombatAttributeTemplate> CombatAttributes { get; set; }
        [ProtoMember(14, AsReference = true)]
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
