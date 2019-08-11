using Rogue.NET.Core.Model.Scenario.Content.Skill;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Character
{
    [Serializable]
    public class Player : Character
    {
        public IList<SkillSet> SkillSets { get; set; }
        public int SkillPoints { get; set; }

        public int Level { get; set; }
        public string Class { get; set; }
        public double Experience { get; set; }
        public double Hunger { get; set; }
        public double FoodUsagePerTurnBase { get; set; }

        public Player()
        {
            this.SkillSets = new List<SkillSet>();
            this.IsPhysicallyVisible = true;
        }
    }
}
