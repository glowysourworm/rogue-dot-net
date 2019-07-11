﻿using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Generator.Interface
{
    public interface ISpellGenerator
    {
        Spell GenerateSpell(SpellTemplate spellTemplate, IEnumerable<Religion> religions);
    }
}
