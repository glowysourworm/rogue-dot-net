using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Generator
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(ISkillSetGenerator))]
    public class SkillSetGenerator : ISkillSetGenerator
    {
        private readonly ISymbolDetailsGenerator _symbolDetailsGenerator;
        private readonly ISkillGenerator _skillGenerator;

        [ImportingConstructor]
        public SkillSetGenerator(ISkillGenerator skillGenerator, ISymbolDetailsGenerator symbolDetailsGenerator)
        {
            _skillGenerator = skillGenerator;
            _symbolDetailsGenerator = symbolDetailsGenerator;
        }

        public SkillSet GenerateSkillSet(SkillSetTemplate skillSetTemplate)
        {
            var result = new SkillSet();

            result.RogueName = skillSetTemplate.Name;
            result.Skills = skillSetTemplate.Skills.Select(x => _skillGenerator.GenerateSkill(x)).ToList();

            _symbolDetailsGenerator.MapSymbolDetails(skillSetTemplate.SymbolDetails, result);

            return result;
        }
    }
}
