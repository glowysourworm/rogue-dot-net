using ExpressMapper;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            Mapper.Compile();
        }
    }
}
