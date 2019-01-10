using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Content.Religion;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Model.Generator
{
    [Export(typeof(ISkillSetGenerator))]
    public class SkillSetGenerator : ISkillSetGenerator
    {
        private readonly ISkillGenerator _skillGenerator;

        [ImportingConstructor]
        public SkillSetGenerator(ISkillGenerator skillGenerator)
        {
            _skillGenerator = skillGenerator;
        }

        public SkillSet GenerateSkillSet(SkillSetTemplate skillSetTemplate)
        {
            return new SkillSet()
            {
                CharacterColor = skillSetTemplate.SymbolDetails.CharacterColor,
                CharacterSymbol = skillSetTemplate.SymbolDetails.CharacterSymbol,
                DisplayIcon = skillSetTemplate.SymbolDetails.DisplayIcon,
                HasReligiousAffiliationRequirement = skillSetTemplate.HasReligiousAffiliationRequirement,
                Icon = skillSetTemplate.SymbolDetails.Icon,
                LevelLearned = skillSetTemplate.LevelLearned,
                ReligiousAffiliationRequirement = new ReligiousAffiliationRequirement()
                {
                    ReligionName = skillSetTemplate.ReligiousAffiliationRequirement.Religion.Name,
                    RequiredAffiliationLevel = skillSetTemplate.ReligiousAffiliationRequirement.RequiredAffiliationLevel
                },
                RogueName = skillSetTemplate.Name,
                Skills = skillSetTemplate.Skills.Select(x => _skillGenerator.GenerateSkill(x)).ToList(),
                SmileyAuraColor = skillSetTemplate.SymbolDetails.SmileyAuraColor,
                SmileyBodyColor = skillSetTemplate.SymbolDetails.SmileyBodyColor,
                SmileyLineColor = skillSetTemplate.SymbolDetails.SmileyLineColor,
                SmileyMood = skillSetTemplate.SymbolDetails.SmileyMood,
                SymbolType = skillSetTemplate.SymbolDetails.Type
            };
        }
    }
}
