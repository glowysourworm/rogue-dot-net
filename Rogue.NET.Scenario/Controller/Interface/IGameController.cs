using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using System.Threading.Tasks;

namespace Rogue.NET.Scenario.Controller.Interface
{
    public interface IGameController
    {
        void Initialize();

        void New(ScenarioConfigurationContainer config, string rogueName, string religionName, int seed, bool survivorMode);
        void Open(string file);
        void Save();

        void LoadCurrentLevel();
    }
}
