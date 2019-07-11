using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Generator.Interface
{
    public interface ISkillSetGenerator
    {
        SkillSet GenerateSkillSet(SkillSetTemplate skillSetTemplate);
    }
}
