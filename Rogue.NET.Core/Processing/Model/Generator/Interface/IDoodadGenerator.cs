﻿using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Doodad;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Generator.Interface
{
    public interface IDoodadGenerator
    {
        DoodadMagic GenerateDoodad(DoodadTemplate doodadTemplate);
    }
}