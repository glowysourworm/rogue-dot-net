using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using System.Collections.Generic;

namespace Rogue.NET.Engine.Model.Generator.Interface
{
    public interface ILayoutGenerator
    {
        IEnumerable<Level> CreateDungeonLayouts(ScenarioConfigurationContainer configuration);
    }
}
