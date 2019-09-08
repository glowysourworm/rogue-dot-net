using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.ScenarioConfiguration;

namespace Rogue.NET.Core.Processing.Model.Generator.Interface
{
    public interface IScenarioGenerator
    {
        ScenarioContainer CreateScenario(ScenarioConfigurationContainer configuration, string characterClassName, int seed, bool survivorMode);
    }
}
