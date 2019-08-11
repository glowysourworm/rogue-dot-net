using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Character.Extension;

namespace Rogue.NET.Core.Model.Scenario.Content.Skill.Extension
{
    public static class SkillExtension
    {
        public static bool AreRequirementsMet(this Skill skill, Player player)
        {
            // Point Requirement
            if ((player.SkillPoints < skill.PointRequirement) &&
                !skill.IsLearned)
                return false;

            // Level Requirement
            if (player.Level < skill.LevelRequirement)
                return false;

            // Attribute Requirement
            if (skill.HasAttributeRequirement &&
                player.GetAttribute(skill.AttributeRequirement) < skill.AttributeLevelRequirement)
                return false;

            // Character Class Requirement
            if (skill.HasCharacterClassRequirement &&
               !player.Alteration.MeetsClassRequirement(skill.CharacterClass))
                return false;

            return true;
        }
    }
}
