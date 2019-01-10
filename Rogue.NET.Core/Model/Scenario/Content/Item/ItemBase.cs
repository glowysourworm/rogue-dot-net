using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Religion;
using System;

namespace Rogue.NET.Core.Model.Scenario.Content.Item
{
    [Serializable]
    public class ItemBase : ScenarioObject
    {
        /// <summary>
        /// Identified individual item flag (reveals class / cursed / unique)
        /// </summary>
        public bool IsIdentified { get; set; }
        public double Weight { get; set; }
        public int LevelRequired { get; set; }

        public bool HasReligiousAffiliationRequirement { get; set; }
        public ReligiousAffiliationRequirement ReligiousAffiliationRequirement { get; set; }

        public ItemBase()
        {
            this.ReligiousAffiliationRequirement = new ReligiousAffiliationRequirement();
        }
        public ItemBase(string name, ImageResources icon) : base(name, icon)
        {
            this.ReligiousAffiliationRequirement = new ReligiousAffiliationRequirement();
        }
    }
}
