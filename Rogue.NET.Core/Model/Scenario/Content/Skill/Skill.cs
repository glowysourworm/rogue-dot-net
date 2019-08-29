using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Skill;
using System;

namespace Rogue.NET.Core.Model.Scenario.Content.Skill
{
    [Serializable]
    public class Skill : RogueBase
    {
        public SkillAlterationTemplate Alteration { get; set; }

        public int LevelRequirement { get; set; }
        public int SkillPointRequirement { get; set; }
        public int PointRequirement { get; set; }
        public bool HasCharacterClassRequirement { get; set; }
        public bool HasAttributeRequirement { get; set; }
        public double AttributeLevelRequirement { get; set; }
        public CharacterAttribute AttributeRequirement { get; set; }
        public string CharacterClass { get; set; }

        public bool IsLearned { get; set; }
        public bool AreRequirementsMet { get; set; }

        public Skill()
        {
        }
    }
}
