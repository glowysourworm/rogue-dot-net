using Rogue.NET.Core.Model.ScenarioConfiguration;
using System.Threading.Tasks;

namespace Rogue.NET.Scenario.Controller.Interface
{
    public interface IGameController
    {
        void Initialize();

        // scenario loading
        Task New(ScenarioConfigurationContainer config, string rogueName, int seed, bool survivorMode);
        Task Open(string file);
        Task Save();

        void LoadCurrentLevel();
    }
}
