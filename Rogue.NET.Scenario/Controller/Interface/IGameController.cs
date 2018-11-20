using Rogue.NET.Core.Model.ScenarioConfiguration;

namespace Rogue.NET.Scenario.Controller.Interface
{
    public interface IGameController
    {
        void Initialize();

        // scenario loading
        void New(ScenarioConfigurationContainer config, string rogueName, int seed, bool survivorMode);
        void Open(string file);
        void Save();

        void LoadCurrentLevel();
    }
}
