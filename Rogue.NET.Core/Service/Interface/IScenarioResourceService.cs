using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration;

namespace Rogue.NET.Core.Service.Interface
{
    public interface IScenarioResourceService
    {
        ScenarioConfigurationContainer GetEmbeddedScenarioConfiguration(ConfigResources configResource);

        void SaveConfig(string name, ScenarioConfigurationContainer config);
    }
}
