using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Model.Generator
{
    [Export(typeof(ISkillSetGenerator))]
    public class SkillSetGenerator : ISkillSetGenerator
    {
        private readonly ISpellGenerator _spellGenerator;

        [ImportingConstructor]
        public SkillSetGenerator(ISpellGenerator spellGenerator)
        {
            _spellGenerator = spellGenerator;
        }

        public SkillSet GenerateSkillSet(SkillSetTemplate skillSetTemplate)
        {
            return new SkillSet()
            {
                CharacterColor = skillSetTemplate.SymbolDetails.CharacterColor,
                CharacterSymbol = skillSetTemplate.SymbolDetails.CharacterSymbol,
                Icon = skillSetTemplate.SymbolDetails.Icon,
                LevelLearned = skillSetTemplate.LevelLearned,
                RogueName = skillSetTemplate.Name,
                Skills = skillSetTemplate.Spells.Select(x => _spellGenerator.GenerateSpell(x)).ToList(),
                SmileyAuraColor = skillSetTemplate.SymbolDetails.SmileyAuraColor,
                SmileyBodyColor = skillSetTemplate.SymbolDetails.SmileyBodyColor,
                SmileyLineColor = skillSetTemplate.SymbolDetails.SmileyLineColor,
                SmileyMood = skillSetTemplate.SymbolDetails.SmileyMood
            };
        }
    }
}
