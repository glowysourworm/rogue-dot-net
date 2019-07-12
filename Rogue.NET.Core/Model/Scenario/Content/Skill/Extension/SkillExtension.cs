using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Character.Extension;

namespace Rogue.NET.Core.Model.Scenario.Content.Skill.Extension
{
    public static class SkillExtension
    {
        public static bool AreRequirementsMet(this Skill skill, Player player)
        {
            // Point Requirement
            if (player.SkillPoints < skill.PointRequirement &&
                !skill.IsLearned)
                return false;

            // Level Requirement
            if (player.Level < skill.LevelRequirement)
                return false;

            // Attribute Requirement
            if (skill.HasAttributeRequirement &&
                player.Get(skill.AttributeRequirement) < skill.AttributeLevelRequirement)
                return false;

            // Religion Requirement
            if (skill.HasReligionRequirement &&
               (!player.ReligiousAlteration.IsAffiliated() ||
                skill.Religion != player.ReligiousAlteration.Religion))
                return false;

            return true;
        }
    }
}
