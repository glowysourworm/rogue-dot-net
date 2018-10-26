using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.ScenarioConfiguration;

namespace Rogue.NET.Core.Model.Generator.Interface
{
    public interface IScenarioGenerator
    {
        ScenarioContainer CreateScenario(ScenarioConfigurationContainer configuration, bool survivorMode);

        ScenarioContainer CreateDebugScenario(ScenarioConfigurationContainer configuration);
    }
}
