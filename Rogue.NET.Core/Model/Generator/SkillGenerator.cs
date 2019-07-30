using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Model.Generator
{
    [Export(typeof(ISkillGenerator))]
    public class SkillGenerator : ISkillGenerator
    {
        readonly ISpellGenerator _spellGenerator;

        [ImportingConstructor]
        public SkillGenerator(ISpellGenerator spellGenerator)
        {
            _spellGenerator = spellGenerator;
        }

        public Skill GenerateSkill(SkillTemplate skillTemplate, IEnumerable<CharacterClass> religions)
        {
            return new Skill()
            { 
                Alteration = _spellGenerator.GenerateSpell(skillTemplate.Alteration, religions),     
                AttributeLevelRequirement = skillTemplate.AttributeLevelRequirement,
                AttributeRequirement = skillTemplate.AttributeRequirement,
                HasAttributeRequirement = skillTemplate.HasAttributeRequirement,
                HasReligionRequirement = skillTemplate.HasReligionRequirement,                
                LevelRequirement = skillTemplate.LevelRequirement,
                PointRequirement = skillTemplate.PointRequirement,
                // TODO:RELIGION
                //Religion = skillTemplate.HasReligionRequirement ? religions.First(religion => religion.RogueName == skillTemplate.Religion.Name) : new CharacterClass(),
                SkillPointRequirement = skillTemplate.PointRequirement
            };
        }
    }
}
