using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.Controller.Interface
{
    public interface IScenarioEditorController
    {
        ScenarioConfigurationContainerViewModel CurrentConfig { get; }

        void New();
        void Open(string name, bool builtIn);
        void Save();
    }
}
