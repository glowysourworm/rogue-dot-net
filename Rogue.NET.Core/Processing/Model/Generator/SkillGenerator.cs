using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Processing.Model.Generator
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(ISkillGenerator))]
    public class SkillGenerator : ISkillGenerator
    {
        [ImportingConstructor]
        public SkillGenerator()
        {
        }

        public Skill GenerateSkill(SkillTemplate skillTemplate)
        {
            return new Skill()
            { 
                Alteration = skillTemplate.SkillAlteration,     
                AttributeLevelRequirement = skillTemplate.AttributeLevelRequirement,
                AttributeRequirement = skillTemplate.AttributeRequirement,
                HasAttributeRequirement = skillTemplate.HasAttributeRequirement,
                HasCharacterClassRequirement = skillTemplate.HasCharacterClassRequirement,                
                LevelRequirement = skillTemplate.LevelRequirement,
                PointRequirement = skillTemplate.PointRequirement,
                CharacterClass = skillTemplate.CharacterClass,
                SkillPointRequirement = skillTemplate.PointRequirement
            };
        }
    }
}
