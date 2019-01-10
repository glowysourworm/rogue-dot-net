using Rogue.NET.Core.Model.Scenario.Content.Religion;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Generator.Interface
{
    public interface IReligionGenerator
    {
        Religion GenerateReligion(ReligionTemplate template, IEnumerable<SkillSetTemplate> skillSetTemplates);
    }
}
