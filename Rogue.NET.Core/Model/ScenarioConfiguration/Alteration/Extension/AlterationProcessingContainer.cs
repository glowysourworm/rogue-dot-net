using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Consumable;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Doodad;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Enemy;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Equipment;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Skill;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Extension
{
    /// <summary>
    /// Class that is used for convenience during processing. This provides common properties
    /// that were intentionally left out of the interface design to avoid parameter overlap.
    /// </summary>
    public class AlterationProcessingContainer
    {
        public string AssetName { get; private set; }
        public string AlterationName { get; private set; }
        public IAlterationEffectTemplate Effect { get; private set; }
        public AnimationSequenceTemplate Animation { get; private set; }
        public bool HasAnimation { get; private set; }

        public AlterationProcessingContainer(string assetName, AlterationTemplate template)
        {
            this.AssetName = assetName;
            this.AlterationName = template.Name;
            this.Effect = template.Effect;
            this.Animation = template.Animation;
            this.HasAnimation = template.Animation.Animations.Count > 0;
        }

        public AlterationProcessingContainer(string assetName, ConsumableAlterationTemplate template)
        {
            this.AssetName = assetName;
            this.AlterationName = template.Name;
            this.Effect = template.Effect;
            this.Animation = template.Animation;
            this.HasAnimation = template.Animation.Animations.Count > 0;
        }

        public AlterationProcessingContainer(string assetName, ConsumableProjectileAlterationTemplate template)
        {
            this.AssetName = assetName;
            this.AlterationName = template.Name;
            this.Effect = template.Effect;
            this.Animation = template.Animation;
            this.HasAnimation = template.Animation.Animations.Count > 0;
        }

        public AlterationProcessingContainer(string assetName, DoodadAlterationTemplate template)
        {
            this.AssetName = assetName;
            this.AlterationName = template.Name;
            this.Effect = template.Effect;
            this.Animation = template.Animation;
            this.HasAnimation = template.Animation.Animations.Count > 0;
        }

        public AlterationProcessingContainer(string assetName, EnemyAlterationTemplate template)
        {
            this.AssetName = assetName;
            this.AlterationName = template.Name;
            this.Effect = template.Effect;
            this.Animation = template.Animation;
            this.HasAnimation = template.Animation.Animations.Count > 0;
        }

        public AlterationProcessingContainer(string assetName, EquipmentAttackAlterationTemplate template)
        {
            this.AssetName = assetName;
            this.AlterationName = template.Name;
            this.Effect = template.Effect;
            this.Animation = template.Animation;
            this.HasAnimation = template.Animation.Animations.Count > 0;
        }

        public AlterationProcessingContainer(string assetName, EquipmentCurseAlterationTemplate template)
        {
            this.AssetName = assetName;
            this.AlterationName = template.Name;
            this.Effect = template.Effect;
            this.Animation = null;
            this.HasAnimation = false;
        }

        public AlterationProcessingContainer(string assetName, EquipmentEquipAlterationTemplate template)
        {
            this.AssetName = assetName;
            this.AlterationName = template.Name;
            this.Effect = template.Effect;
            this.Animation = null;
            this.HasAnimation = false;
        }

        public AlterationProcessingContainer(string assetName, SkillAlterationTemplate template)
        {
            this.AssetName = assetName;
            this.AlterationName = template.Name;
            this.Effect = template.Effect;
            this.Animation = template.Animation;
            this.HasAnimation = template.Animation.Animations.Count > 0;
        }
    }
}
