﻿using ExpressMapper;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;

namespace Rogue.NET.Core.Utility
{
    /// <summary>
    /// Required per project to initialize mappings statically
    /// </summary>
    public static class MapperInit
    {
        static MapperInit()
        {
            Mapper.Register<Enemy, ScenarioImage>();
            Mapper.Register<Player, ScenarioImage>();
            Mapper.Register<SymbolDetailsTemplate, ScenarioImage>();
        }
    }
}
