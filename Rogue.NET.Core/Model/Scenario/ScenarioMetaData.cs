using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;

using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario
{
    /// <summary>
    /// Storage for Data about in-game items: Equipment, Consumables, Enemies, Doodads, etc...
    /// </summary>
    [Serializable]
    public class ScenarioMetaData : ScenarioImage
    {
        public string Type { get; set; }
        public string Description { get; set; }
        public string LongDescription { get; set; }
        public bool IsIdentified { get; set; }
        public bool IsObjective { get; set; }
        public bool IsCursed { get; set; }
        public bool IsUnique { get; set; }
        public bool IsCurseIdentified { get; set; }
        public DungeonMetaDataObjectTypes ObjectType { get; set; }
        public IList<AttackAttributeTemplate> AttackAttributes { get; set; }

        public ScenarioMetaData()
        {
            this.RogueName = "";
            this.Type = "";
            this.Description = "";
            this.LongDescription = "";
            this.ObjectType = DungeonMetaDataObjectTypes.Item;
            this.AttackAttributes = new List<AttackAttributeTemplate>();
        }
    }
}
