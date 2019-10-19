using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Consumable;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Doodad;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Enemy;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Equipment;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Friendly;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Skill;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Extension
{
    /// <summary>
    /// Class that is used for convenience during processing. This provides common properties
    /// that were intentionally left out of the interface design to avoid parameter overlap.
    /// </summary>
    public class AlterationProcessingContainer
    {
        public Template Asset { get; private set; }
        public AlterationTemplate Alteration { get; private set; }

        public AlterationProcessingContainer(Template asset, AlterationTemplate alteration)
        {
            this.Asset = asset;
            this.Alteration = alteration;
        }
    }
}
