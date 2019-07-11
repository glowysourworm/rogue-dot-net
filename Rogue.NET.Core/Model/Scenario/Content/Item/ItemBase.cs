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
        public Religion Religion { get; set; }

        public ItemBase()
        {
            this.Religion = new Religion();
        }
        public ItemBase(string name, ImageResources icon) : base(name, icon)
        {
            this.Religion = new Religion();
        }
    }
}
