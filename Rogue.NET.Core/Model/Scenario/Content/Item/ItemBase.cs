using Rogue.NET.Core.Model.Enums;
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

        public bool HasReligionRequirement { get; set; }
        public string ReligionName { get; set; }

        public ItemBase()
        {
            this.ReligionName = null;
        }
        public ItemBase(string name, ImageResources icon) : base(name, icon)
        {
            this.ReligionName = null;
        }
    }
}
