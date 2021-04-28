using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Processing.Service.Cache;

using System.Threading.Tasks;

namespace Rogue.NET.Scenario.Processing.Controller.Interface
{
    public interface IGameController
    {
        void Initialize();

        void New(ScenarioConfigurationContainer config, string rogueName, string characterClassName, int seed, bool survivorMode);
        void Open(ScenarioInfo scenarioInfo);
        void Save();

        void LoadCurrentLevel();
    }
}
