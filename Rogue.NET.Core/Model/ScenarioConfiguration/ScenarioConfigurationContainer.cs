using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration.Design;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Model.ScenarioConfiguration
{
    [Serializable]
    public class ScenarioConfigurationContainer
    {
        public ScenarioTemplate ScenarioDesign { get; set; }
        public List<PlayerTemplate> PlayerTemplates { get; set; }
        public List<LayoutTemplate> LayoutTemplates { get; set; }
        public List<SkillSetTemplate> SkillTemplates { get; set; }
        public List<EnemyTemplate> EnemyTemplates { get; set; }
        public List<FriendlyTemplate> FriendlyTemplates { get; set; }
        public List<EquipmentTemplate> EquipmentTemplates { get; set; }
        public List<ConsumableTemplate> ConsumableTemplates { get; set; }
        public List<DoodadTemplate> DoodadTemplates { get; set; }

        public List<AttackAttributeTemplate> AttackAttributes { get; set; }
        public List<AlteredCharacterStateTemplate> AlteredCharacterStates { get; set; }
        public List<SymbolPoolItemTemplate> SymbolPool { get; set; }
        public List<AlterationCategoryTemplate> AlterationCategories { get; set; }
        public List<TerrainLayerTemplate> TerrainLayers { get; set; }

        public ScenarioConfigurationContainer()
        {
            this.ScenarioDesign = new ScenarioTemplate();

            this.LayoutTemplates = new List<LayoutTemplate>();
            this.PlayerTemplates = new List<PlayerTemplate>();
            this.EnemyTemplates = new List<EnemyTemplate>();
            this.FriendlyTemplates = new List<FriendlyTemplate>();
            this.EquipmentTemplates = new List<EquipmentTemplate>();
            this.ConsumableTemplates = new List<ConsumableTemplate>();
            this.SkillTemplates = new List<SkillSetTemplate>();
            this.DoodadTemplates = new List<DoodadTemplate>();

            this.AttackAttributes = new List<AttackAttributeTemplate>();
            this.AlteredCharacterStates = new List<AlteredCharacterStateTemplate>();
            this.SymbolPool = new List<SymbolPoolItemTemplate>();
            this.AlterationCategories = new List<AlterationCategoryTemplate>();
            this.TerrainLayers = new List<TerrainLayerTemplate>();
        }
    }
}
