using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Skill GenerateSkill(SkillTemplate skillTemplate)
        {
            return new Skill()
            { 
                Alteration = _spellGenerator.GenerateSpell(skillTemplate.Alteration),
                LevelRequirement = skillTemplate.LevelRequirement,
                SkillPointRequirement = skillTemplate.PointRequirement,
                RequiredAffiliationLevel = skillTemplate.RequiredAffiliationLevel                
            };
        }
    }
}
