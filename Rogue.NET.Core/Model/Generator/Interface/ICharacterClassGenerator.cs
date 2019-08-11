﻿using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Generator.Interface
{
    public interface ICharacterClassGenerator
    {
        CharacterClass GenerateCharacterClass(CharacterClassTemplate template, IEnumerable<SkillSetTemplate> skillSetTemplates);
    }
}