using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Core.Model.Scenario.Dynamic;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Character
{
    [Serializable]
    public class Player : Character
    {
        public int Level { get; set; }
        public string Class { get; set; }
        public double Experience { get; set; }
        public double Hunger { get; set; }
        public double FoodUsagePerTurnBase { get; set; }

        public int SkillPoints { get; set; }

        public override CharacterAlteration Alteration { get; set; }        
        public IList<SkillSet> SkillSets { get; set; }

        public Player() : base()
        {
            this.SkillSets = new List<SkillSet>();
            this.IsPhysicallyVisible = true;
            this.Alteration = new PlayerAlteration();
            this.CharacterClassAlteration = new CharacterClassAlteration();
        }
        public Player(string name, SmileyMoods mood, string bodyColor, string lineColor, string auraColor) 
            : base(name, mood, bodyColor, lineColor, auraColor)
        {
            this.SkillSets = new List<SkillSet>();
            this.IsPhysicallyVisible = true;
            this.Alteration = new PlayerAlteration();
            this.CharacterClassAlteration = new CharacterClassAlteration();
        }
    }
}
