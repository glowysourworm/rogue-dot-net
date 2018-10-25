using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.ScenarioConfiguration;

namespace Rogue.NET.Engine.Model.Generator.Interface
{
    public interface IScenarioGenerator
    {
        ScenarioContainer CreateScenario(ScenarioConfigurationContainer configuration, int seed, bool survivorMode);

        ScenarioContainer CreateDebugScenario(ScenarioConfigurationContainer configuration);
    }
}
